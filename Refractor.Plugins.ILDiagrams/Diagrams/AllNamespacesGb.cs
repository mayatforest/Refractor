using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

using Refractor.Common;
using SDILReader;

namespace Refractor.Plugins.ILDiagrams
{
    public class AllNamespacesGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            _nsOptions = (this._options as NamespacesOptions);

            FirstPass();
            SecondPass();
        }

        private Dictionary<string, List<NamespaceItem>> _namespaces = new Dictionary<string, List<NamespaceItem>>();
        private NamespacesOptions _nsOptions;
        private void FirstPass()
        {
            RootItem root = _activeItem as RootItem;
            foreach (BaseItem item in root.Files)
            {
                if (item as AssemblyItem == null) continue;

                AssemblyItem assemblyItem = (AssemblyItem)item;

                foreach (NamespaceItem nsItem in assemblyItem.NameSpaces)
                {
                    // Use the allow duplicates version of AddNode, which builds a list
                    // of items for a single node. We don't add more than one node per id.
                    this.AddNode(nsItem.Name, nsItem.Name,
                        _sharedOptions.NamespaceColor, nsItem, true);

                    // Hang onto our namespaces, and keep a list of duplicates.
                    if (_namespaces.ContainsKey(nsItem.GetShortID()))
                    {
                        List<NamespaceItem> list = _namespaces[nsItem.GetShortID()];
                        list.Add(nsItem);
                    }
                    else
                    {
                        List<NamespaceItem> list = new List<NamespaceItem>();
                        list.Add(nsItem);
                        _namespaces.Add(nsItem.GetShortID(), list);
                    }
                }
            }
        }
                
        private void SecondPass()
        {
            RootItem root = _activeItem as RootItem;
            foreach (BaseItem item in root.Files)
            {
                if (CheckWorker()) break;

                if (item as AssemblyItem == null) continue;
                AssemblyItem assemblyItem = (AssemblyItem)item;

                foreach (NamespaceItem nsItem in assemblyItem.NameSpaces)
                {
                    if (CheckWorker()) break;
                    
                    foreach (TypeItem typeItem in nsItem.Types)
                    {
                        if (CheckWorker()) break;

                        foreach (MethodItem methodItem in typeItem.Methods)
                        {                            
                            ParseInstructions(methodItem.MethodInfo, nsItem.GetShortID());
                        }
                    }
                }
            }

        }

        private void ParseInstructions(MethodInfo mi, string fromId)
        {
            MethodBodyReader mr = GetMethodBodyReader(mi);
            if (mr == null) return;
            if (mr.instructions == null) return;

            foreach (ILInstruction instruction in mr.instructions)
            {
                if (CheckWorker()) break;
                if (instruction.Operand == null) continue;
                if (instruction.Code.OperandType != OperandType.InlineMethod) continue;

                Type declType;
                string name, calledId;
                GetInstrDetails(instruction, out name, out declType, out calledId);
                if (name == null) continue;

                // Find the method baseItem we're calling.
                BaseItem item = _projectBrowser.Lookup(calledId);

                // If the method is not present in our project, then forget about it.
                if (item == null) continue;

                // Find the namespace the method that's being called is defined in.
            //    NamespaceItem nsTo = GetParentOfType(item, typeof(NamespaceItem)) as NamespaceItem;
            //    if (nsTo == null) continue;

            //protected BaseItem GetParentOfType(BaseItem item, Type type)
            //{
            //}
                BaseItem nsTo = item;
                while (!(nsTo is NamespaceItem))
                {
                    nsTo = nsTo.Parent;
                    if (nsTo == null) break;
                }
                if (nsTo == null) continue;

                // Ignore any calls coming from outside our list of namespaces.
                if (!_namespaces.ContainsKey((nsTo as NamespaceItem).GetShortID())) continue;

                string nsToId = nsTo.Name;

                AddEdge(fromId, nsToId, EdgeStyle.NormalArrow);
            }
        }

    
    
    }
}
