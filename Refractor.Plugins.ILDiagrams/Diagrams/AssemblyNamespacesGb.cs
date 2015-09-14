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
    public class AssemblyNamespacesGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            FirstPass();
            SecondPass();
        }

        //private Dictionary<string, Type> _types = new Dictionary<string, Type>();

        private Dictionary<NamespaceItem, bool> _namespaces = new Dictionary<NamespaceItem, bool>();

        private void FirstPass()
        {
            AssemblyItem assemblyItem = _activeItem as AssemblyItem;
            foreach (NamespaceItem nsitem in assemblyItem.NameSpaces)
            {
                if (CheckWorker()) break;

                // Get a short id.
                string id = nsitem.Name;

                // Add the graph node with a link back to the item.
                this.AddNode(id, id, _sharedOptions.NamespaceColor, nsitem);

                _namespaces.Add(nsitem, true);

                //_logView.LogStr("Added:" + id);
            }
        }


        private void SecondPass()
        {
            AssemblyItem assemblyItem = _activeItem as AssemblyItem;

            foreach (NamespaceItem nsitem in assemblyItem.NameSpaces)
            {
                if (CheckWorker()) break;

                string fromId = nsitem.Name;
                foreach (TypeItem typeitem in nsitem.Types)
                {
                    if (CheckWorker()) break;

                    Type t = typeitem.TypeRef;

                    foreach (MethodItem m in typeitem.Methods)
                    {
                        ParseInstructions(m.MethodInfo, fromId);
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

                // Find the namespace the method is defined in.
                BaseItem nsTo = item;
                while (!(nsTo is NamespaceItem))
                {
                    nsTo = nsTo.Parent;
                    if (nsTo == null) break;
                }

                if (!_namespaces.ContainsKey(nsTo as NamespaceItem)) continue;

                string nsToId = nsTo.Name;

                AddEdge(fromId, nsToId, EdgeStyle.NormalArrow);
            }
        }
    
    
    }
}
