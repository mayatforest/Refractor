using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Refractor.Common;
using SDILReader;

namespace Refractor.Plugins.ILDiagrams
{
    public class ClassMethodsGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            FirstPass();
            SecondPass();
        }

        private Dictionary<string, MethodInfo> _methodInfos = new Dictionary<string, MethodInfo>();
        private List<string> _names = new List<string>();
        private List<string> _overloads = new List<string>();

        private void FirstPass()
        {
            foreach (MethodItem methodItem in (_activeItem as TypeItem).Methods)
            {
                if (CheckWorker()) break;

                MethodInfo mi = methodItem.MethodInfo;

                string id = mi.DeclaringType.FullName + " " + RefHelp.GetNameWithParameterList(mi);

                // Add the graph node with a link back to the method.
                if (AddNode(id, mi.Name, _sharedOptions.MethodColor, methodItem) != null)
                {
                    // Hang onto the method and the corresponding graph node.
                    _methodInfos.Add(id, mi);

                    // Make a note of overloaded methods.
                    if (_names.Contains(mi.Name)) _overloads.Add(mi.Name);
                    else _names.Add(mi.Name);
                }
            }
        }

        private void SecondPass()
        {
            // For each method in the class in turn.
            foreach (KeyValuePair<string, MethodInfo> pair in _methodInfos)
            {
                if (CheckWorker()) break;

                object methodNode = _addedNodes[pair.Key];

                // Append caption for overloaded methods.
                if (_overloads.Contains(pair.Value.Name))
                {
                    AppendNodeCaption(methodNode, "\n(overloaded)");
                }

                // Make a note of public methods.
                if (pair.Value.IsPublic)
                {
                    AppendNodeCaption(methodNode, "\n(public)");
                    UpdateNodeColor(methodNode, Color.LightGoldenrodYellow); //todo
                }

                //// The method may have no body, e.g. abstract methods, external methods.
                //if (pair.Value.Body == null)
                //{
                //    methodNode.Attr.Label += "\n(abstract)";
                //    continue;
                //}

                ParseInstructions(pair.Key, pair.Value);
            }

        }

        private void ParseInstructions(string methodNodeId, MethodInfo mi)
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

                if (declType != null)
                {
                    if (declType == (_activeItem as TypeItem).TypeRef)
                    {
                        // Only link to nodes we've added. This may or may not include
                        // property get set methods, depending on the setting.
                        if (_addedNodes.ContainsKey(calledId))
                        {
                            AddEdge(methodNodeId, calledId, EdgeStyle.NormalArrow);
                        }
                    }
                }
            }
        }

    }
}
