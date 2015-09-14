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
    public class AssembyClassesGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            FirstPass();
            SecondPass();
        }

        private Dictionary<string, TypeItem> _types = new Dictionary<string, TypeItem>();

        private void FirstPass()
        {
            AssemblyItem assemblyItem = _activeItem as AssemblyItem;

            foreach (NamespaceItem nsitem in assemblyItem.NameSpaces)
            {
                if (CheckWorker()) break;
                foreach (TypeItem typeitem in nsitem.Types)
                {
                    if (CheckWorker()) break;

                    Type t = typeitem.TypeRef;

                    // Get the full name of the type. (shortID)
                    string id = t.FullName;

                    // Add the graph node with a link back to the type.
                    object node = this.AddNode(id, t.Name, _sharedOptions.TypeColor, typeitem);

                    if (node != null)
                    {
                        _types.Add(id, typeitem);
                    }
                }
            }
        }
                
        private void SecondPass()
        {
            foreach (KeyValuePair<string, TypeItem> pair in _types)
            {
                if (CheckWorker()) break;

                foreach (MethodItem m in pair.Value.Methods)
                {
                    ParseInstructions(m.MethodInfo, pair.Key);
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


                calledId = declType.FullName;
                BaseItem calledItem = _projectBrowser.Lookup(calledId);

                if (calledItem == null) continue;
                // Or external types - optional?

                if (_addedNodes.ContainsKey(calledId))
                {
                    AddEdge(fromId, calledId, EdgeStyle.NormalArrow);
                }
            }
        }
    
    
    }
}
