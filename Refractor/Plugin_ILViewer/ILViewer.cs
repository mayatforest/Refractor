using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;
using SDILReader;


namespace Refractor.Plugins
{
    /// <summary>
    /// Show a list of IL dissassembly for a method.
    /// </summary>
    public partial class ILViewer : DockContent, IActiveItemViewer, IWindowPlugin
    {
        public ILViewer()
        {
            InitializeComponent();

            _updateGrid = UpdateGrid;
            _panelRefresh = PanelRefresh;

            _topOffset = _grid.ColumnHeadersHeight;
            _rowHeight = _grid.RowTemplate.Height;
            _colWidth = 10;

            this.TabText = "IL Viewer";
        }

        public string GetID()
        {
            return "IL Viewer";
        }

        public List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(MethodItem) };
        }

        public WindowPluginKind GetKind()
        {
            return WindowPluginKind.ProjectItem;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _logView = (ILogView)serviceProvider.GetService(typeof(ILogView)); 
        }

        public void SetRefresh(BaseItem item)
        {
        }

        public void SetActiveItem(BaseItem item)
        {
            if (item is MethodItem)
            {
                try
                {
                    _activeItem = item as MethodItem;

                    MethodInfo mi = (item as MethodItem).MethodInfo;
                    SDILReader.MethodBodyReader mr = new MethodBodyReader(mi);

                    if (mr != null && mr.instructions != null)
                    {
                        _position = 0;
                        _lb.Build(mr);
                        BuildBitmap();
                    }
                    
                    _grid.Invoke(_updateGrid, new object[] { mi });

                    panel1.Invoke(_panelRefresh); 

                }
                catch (Exception exc)
                {
                    _logView.LogExc(exc);
                }
            }
        }

        public BaseItem GetActiveItem()
        {
            return _activeItem;
        }

        public PluginOptions GetOptions()
        {
            return new ILViewerOptions(this.GetID()) as PluginOptions;
        }

        public void SetOptions(PluginOptions optionss)
        {
            // todo
        }


        private LineBuilder _lb = new LineBuilder();
        private ILogView _logView;
        private MethodItem _activeItem;
        private DSingleObjectDelegate _updateGrid;
        private DNoArgsDelegate _panelRefresh;
        private Bitmap _bitmap;
        private int _topOffset;
        private int _rowHeight;
        private int _colWidth;
        private int _position = 0;

        private void UpdateGrid(object o)
        {
            _grid.Rows.Clear();

            if (o == null) return;

            MethodInfo mi = (MethodInfo)o;


            SDILReader.MethodBodyReader mr = new MethodBodyReader(mi);
            if (mr.instructions == null) return;

            foreach (SDILReader.ILInstruction i in mr.instructions)
            {
                string str = i.GetCode();
                int idx = str.IndexOf(':');
                if (idx < 0) continue;
                string address = str.Substring(0, idx);
                string code = str.Substring(idx + 1);
                
                _grid.Rows.Add(new object[] { address, code });
                _grid.CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
            } 
        }

        private void PanelRefresh()
        {
            panel1.Refresh();
        }
        
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (_bitmap == null) return;

            Panel panel = sender as Panel;
            e.Graphics.DrawImage(_bitmap,
                    new Rectangle(
                        panel1.Width - _bitmap.Width,
                        0, 
                        _bitmap.Width, 
                        _bitmap.Height),
                    0,
                    _position * _rowHeight,
                    _bitmap.Width, 
                    _bitmap.Height, 
                    GraphicsUnit.Pixel);

        }

        private void BuildBitmap()
        {
            int depth = _lb.GetDepth();
            int maxHeight = 0;
            int maxWidth = 0;
            foreach (LineGroup group in _lb.Groups)
            {
                foreach (LineJoin j in group)
                {
                    maxWidth = Math.Max(maxWidth, depth * _colWidth);
                    maxHeight = Math.Max(maxHeight, _topOffset + j.End * _rowHeight);
                }
                depth--;
            }
            _bitmap = new Bitmap(maxWidth + 5, maxHeight + 5);
            Graphics g = Graphics.FromImage(_bitmap);
            
            using (Pen pen1 = new Pen(Color.Green))
            using (Pen pen2 = new Pen(Color.Blue))
            {
                depth = _lb.GetDepth();
                foreach (LineGroup group in _lb.Groups)
                {
                    foreach (LineJoin j in group)
                    {
                        int left = maxWidth - depth * _colWidth;
                        int right = maxWidth;
                        int start = _topOffset + j.Start * _rowHeight;
                        int end = _topOffset + j.End * _rowHeight;

                        Pen pen = start < end ? pen2 : pen1;

                        g.DrawLine(pen, left, start, right, start);
                        g.DrawLine(pen, left, start, left, end);
                        g.DrawLine(pen, left, end, right, end);                    
                                                
                        g.DrawLine(pen, right - 5, end - 5, right, end);
                        g.DrawLine(pen, right - 5, end + 5, right, end);
                    }
                    depth--;
                }


            }

        }

        private void _grid_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                _position = e.NewValue;

                panel1.Invalidate();
            }

        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            panel1.Invalidate();
        }

    }

    public class LineJoin
    {
        public int Start;
        public int End;
    }

    public class LineGroup : List<LineJoin>
    {
    }

    public class LineBuilder
    {
        public List<LineGroup> Groups = new List<LineGroup>();

        private Dictionary<int, int> _instr = new Dictionary<int, int>();

        public int GetDepth()
        {
            return _depth;
        }

        public void Build(MethodBodyReader mr)
        {
            Groups.Clear();
            _instr.Clear();
            _depth = 0;

            int count = 0;
            foreach (ILInstruction i in mr.instructions)
            {
                _instr.Add(i.Offset, count);
                count++;
            }
            
            foreach (ILInstruction i in mr.instructions)
            {
                if (i.Code.OperandType == OperandType.ShortInlineBrTarget ||
                    i.Code.OperandType == OperandType.InlineBrTarget)
                {
                    int pos1 = _instr[i.Offset];
                    int pos2 = _instr[(int)i.Operand];

                    AddLine(pos1, pos2);
                }            
            }

        }

        private int _depth = 0;

        private void AddLine(int fromPos, int toPos)
        {
            LineGroup lg = new LineGroup();
            _depth++;

            LineJoin item1 = new LineJoin();
            item1.Start = fromPos;
            item1.End = toPos;

            lg.Add(item1);

            Groups.Insert(0, lg);
        }
    }

    [Serializable]
    public class ILViewerOptions : PluginOptions
    {
        public ILViewerOptions(string id) : base(id) { }
        public ILViewerOptions() { }
    }


}
