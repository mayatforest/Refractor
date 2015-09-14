using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Refractor.Common;
using SDILReader;
using System.Windows.Forms;

namespace Refractor.Plugins.ILDiagrams
{
    public class ClassButterflyGb : BaseILGraphBuilder
    {
        ClassButterflyOptions _localOptions;

        public override void Translate()
        {
            _localOptions = _windowManager.GetPluginOptions("Class Call Usage Hierarchy") as ClassButterflyOptions;

            // Add the central node, the body of the butterfly.
            AddCentralNode();

            // Add the easy out links.
            AddOutNodes();

            AddOutImplementsNodes();
            AddOutInheritsFromNode();

            // Add the harder in links.
            AddInNodes();
        }

        private byte Colorup(double col)
        {
            if (col <= 0) return 0;
            if (col >= 255) return 255;
            return (byte)Math.Round(col);
        }
        private Color ChangeBrightnessColor(Color inColor,double brightness)
        {
            Color colorout= Color.FromArgb(Colorup(inColor.R * brightness), Colorup(inColor.G * brightness), Colorup(inColor.B * brightness));
            return colorout;
        }
        private String GetTextWithParent(String FullPath)
        {
            String outs = "";
            String[] arr= FullPath.Split(new char[] { '.' });
            if (arr.Length> 1)
            {
                string tabs="";
                for (int i = 0; i < arr.Length; i++)
                {
                    outs += tabs + arr[i] + "\r\n";
                    tabs+="  ";
                }
                return outs;
                //return arr[arr.Length - 2] + "." + arr[arr.Length - 1];
            }
            return FullPath;
        }
        private String GetNameForItem(BaseItem item)
        {
            if (item == null) return "";
            if (item is TypeItem)
            {
                TypeItem ti=(TypeItem)item;
                return ti.GetParentAssemblyID() + "\r\n" + item.Name + "\r\n\r\n" + GetTextWithParent(ti.FullName);
            }
                return item.Name;
        }

        private Color GetColorForNode(BaseItem _item, Color _defcolor)
        {
            Color color=_defcolor;
            TypeItem ti = _item as TypeItem;
            if (ti != null)
            {
                String sname = ti.GetParentAssemblyID();
                uint crc32 = CRC32Class.obj.GetCrc24ForString(sname);
                color = Color.FromArgb((int)crc32);
                color = Color.FromArgb(color.R % 200 + 50, color.G % 200 + 50, color.B % 200 + 50);
            }

            return color;
        }

        private void AddCentralNode()
        {            

            string typeId = _activeItem.GetShortID();

            Color color = GetColorForNode(_activeItem,ChangeBrightnessColor(_sharedOptions.TypeColor, 1));
            color = ChangeBrightnessColor(color, 0.8);
            AddNode(typeId, "*"+GetNameForItem(_activeItem)+"", color, _activeItem);

        }


        private bool AddOutNodes()
        {
            curlevel = 0;
            bool res=AddOutNodes(_activeItem);
            if (res != true)
            {
                MessageBox.Show("Max deep level reached");
            }
            return true;
        }
        int curlevel = 0;
        private bool AddOutNodes(BaseItem initem)
        {
            string activeId = initem.GetShortID();
            curlevel++;
            if (curlevel > _localOptions.OuterScanLevel)
            {
                //MessageBox.Show("Max deep level reached");
                return false;
            }

            foreach (MethodItem methodItem in (initem as TypeItem).Methods)
            {
                SDILReader.MethodBodyReader mr = new MethodBodyReader(methodItem.MethodInfo);
                if (mr == null) continue;

                // Abstract..external, etc
                if (mr.instructions == null) continue;

                string id = string.Empty;
                string methodId = methodItem.GetShortID();

                foreach (ILInstruction instruction in mr.instructions)
                {
                    if (instruction.Code.OperandType != OperandType.InlineMethod) continue;

                    string name = "";
                    if (instruction.Operand is MethodInfo)
                    {
                        id = (instruction.Operand as MethodInfo).DeclaringType.ToString();
                        name = (instruction.Operand as MethodInfo).Name;
                    }
                    else if (instruction.Operand is ConstructorInfo)
                    {
                        id = (instruction.Operand as ConstructorInfo).DeclaringType.ToString();
                        name = (instruction.Operand as ConstructorInfo).Name;
                    }
                    else if (instruction.Operand == null)
                    {
                        _logView.LogStr("Class Butterfly failed to recognise call instruction due to generics.");
                    }
                    else
                    {
                        _logView.LogStr("Class Butterfly failed to recognise call instruction :" + instruction.Operand.GetType().ToString());
                    }

                    // Find the method baseItem we're calling.
                    // The declaring type of the method we're calling.
                    BaseItem item = _projectBrowser.Lookup(id);

                    // If the method is not present in our project, then forget about it.
                    if (item == null) continue;

                    // Only add methods once.
                    if (_addedNodes.ContainsKey(id)) continue;

                    //
                    /*
                    Color color = ChangeBrightnessColor(_sharedOptions.TypeColor, 1.2);
                    TypeItem ti = item as TypeItem;
                    if (ti != null)
                    {
                        color=Color.FromArgb(ti.GetParentAssemblyID().GetHashCode());
                    }
                     */
                    Color color = GetColorForNode(item,ChangeBrightnessColor(_sharedOptions.TypeColor, 1.2));
                    color = ChangeBrightnessColor(color, (1.0 + curlevel / 10.0));

                    AddNode(id, GetNameForItem(item), color, item);
                    AddEdge(activeId, id, EdgeStyle.NormalArrow);

                    AddOutNodes(item);
                }
            }
            curlevel--;
            return true;
        }

        private void AddOutImplementsNodes()
        {
            string activeId = _activeItem.GetShortID();

            foreach (string id in (_activeItem as TypeItem).Implements)
            {
                BaseItem baseItem = _projectBrowser.Lookup(id);

                // If the interface is not present in our project, then forget about it.
                if (baseItem == null) continue;

                // Only add methods once.
                if (_addedNodes.ContainsKey(id)) continue;

                Color color = GetColorForNode(baseItem,ChangeBrightnessColor(_sharedOptions.TypeColor, 1));

                AddNode(id,GetNameForItem(baseItem), color, baseItem);
                AddEdge(activeId, id, EdgeStyle.NormalArrow, Color.DarkSeaGreen); //todo
            }
        }

        private void AddOutInheritsFromNode()
        {
            if ((_activeItem as TypeItem).InheritsFrom == null) return;

            string activeId = _activeItem.GetShortID();
            string id = (_activeItem as TypeItem).InheritsFrom;
            BaseItem baseItem = _projectBrowser.Lookup(id);

            // If the type is not present in our project, then forget about it.
            if (baseItem == null) return;

            Color color = GetColorForNode(baseItem, ChangeBrightnessColor(_sharedOptions.TypeColor, 1));


            // Only add methods once.
            if (_addedNodes.ContainsKey(id))
            {
                object baby = _addedNodes[id];
                UpdateNodeColor(baby, color);
            }
            else
            {
                AddNode(id, GetNameForItem(baseItem), color, baseItem);
            }

            AddEdge(activeId, id, EdgeStyle.NormalArrow, Color.DarkSeaGreen);//todo
        }

        private void AddInNodes()
        {
            string typeId = _activeItem.GetShortID();

            BaseItem item = _activeItem;
            while (!(item is AssemblyItem)) item = item.Parent;

            string currentAssemblyname = (item as AssemblyItem).PureName();

            foreach (KeyValuePair<string, BaseItem> pair in _projectBrowser.Files)
            {
                AssemblyItem assemblyItem = (AssemblyItem)pair.Value;

                if ((assemblyItem.PureName() == currentAssemblyname) ||
                    assemblyItem.ReferencedAssemblies.Contains(currentAssemblyname))
                {
                    foreach (NamespaceItem nsItem in assemblyItem.NameSpaces)
                    {

                        foreach (TypeItem typeItem in nsItem.Types)
                        {
                            AddLinksFromMethods(typeItem, typeId);
                            AddLinksFromClasses(typeItem, typeId);
                            AddLinkFromInherited(typeItem, typeId);
                            if (CheckWorker()) return;
                        }
                    }
                }
            }
        }


        private void AddLinksFromMethods(TypeItem fromTypeItem, string toTypeId)
        {
            foreach (MethodItem methodItem in fromTypeItem.Methods)
            {
                string fromTypeId = fromTypeItem.GetShortID();

                MethodBodyReader mr = new MethodBodyReader(methodItem.MethodInfo);
                if (mr == null) continue;

                // Abstract..external, etc
                if (mr.instructions == null) continue;

                foreach (ILInstruction instruction in mr.instructions)
                {
                    string id = string.Empty;

                    if (instruction.Code.OperandType != OperandType.InlineMethod) continue;

                    // TODO dup code
                    if (instruction.Operand is MethodInfo)
                    {
                        id = (instruction.Operand as MethodInfo).DeclaringType.ToString();
                    }
                    else if (instruction.Operand is ConstructorInfo)
                    {
                        id = (instruction.Operand as ConstructorInfo).DeclaringType.ToString();
                    }
                    else if (instruction.Operand == null)
                    {
                        _logView.LogStr("Class Butterfly failed to recognise call instruction due to generics.(in)");
                    }
                    else
                    {
                        _logView.LogStr("Class Butterfly failed to recognise call instruction (in) :" + instruction.Operand.GetType().ToString());
                    }

                    // We're looking for hits on one specific method.
                    if (id != toTypeId) continue;

                    // Only add nodes once.
                    //if (_addedNodes.ContainsKey(fromTypeId)) continue;

                    //AddNode(fromTypeId, GetNameForItem(fromTypeItem), ChangeBrightnessColor(_sharedOptions.TypeColor,0.8), fromTypeItem);
                    Color color = GetColorForNode(fromTypeItem, ChangeBrightnessColor(_sharedOptions.TypeColor, 1));

                    AddNode(fromTypeId, GetNameForItem(fromTypeItem), color, fromTypeItem);
                    AddEdge(fromTypeId, toTypeId, EdgeStyle.NormalArrow);
                }
            }
        }

        private void AddLinksFromClasses(TypeItem fromTypeItem, string toTypeId)
        {
            if (fromTypeItem.Implements == null) return;

            string fromTypeId = fromTypeItem.GetShortID();

            foreach (string id in fromTypeItem.Implements)
            {
                // We're looking for hits on one specific method.
                if (id != toTypeId) continue;

                BaseItem baseItem = _projectBrowser.Lookup(id);

                // If the interface is not present in our project, then forget about it.
                if (baseItem == null) continue;

                // Only add types once.
                if (_addedNodes.ContainsKey(fromTypeId)) continue;

                //AddNode(fromTypeId, GetNameForItem(fromTypeItem), _sharedOptions.TypeColor, fromTypeItem);
                Color color = GetColorForNode(fromTypeItem, ChangeBrightnessColor(_sharedOptions.TypeColor, 1));

                AddNode(fromTypeId, GetNameForItem(fromTypeItem), color, fromTypeItem);
                AddEdge(fromTypeId, toTypeId, EdgeStyle.NormalArrow, Color.DarkSeaGreen); //todo
            }
        }

        private void AddLinkFromInherited(TypeItem fromTypeItem, string toTypeId)
        {
            string id = fromTypeItem.InheritsFrom;

            // We're looking for hits on one specific method.
            // Everything that inherits from the toType.
            if (id != toTypeId) return;

            string fromTypeId = fromTypeItem.GetShortID();

            BaseItem baseItem = _projectBrowser.Lookup(id);

            // If the interface is not present in our project, then forget about it.
            if (baseItem == null) return;

            // Only add nodes once.
            //if (_addedNodes.ContainsKey(fromTypeId))
            //{
            //    object baby = _addedNodes[fromTypeId];
            //    UpdateNodeColor(baby, _sharedOptions.TypeColor);
            //}
            //else
            //{
            Color color = GetColorForNode(baseItem, ChangeBrightnessColor(_sharedOptions.TypeColor, 1));    
            
            AddNode(fromTypeId, GetNameForItem(baseItem), color, baseItem);
            //}

            AddEdge(fromTypeId, toTypeId, EdgeStyle.NormalArrow);
        }

    }
}
