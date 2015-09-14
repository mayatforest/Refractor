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
    public class AssemblyButterflyGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            // Add the central node, the body of the butterfly.
            AddCentralNode();

            AddOutNodes();
            AddInNodes();
        }

        private void AddCentralNode()
        {
            string id = _activeItem.GetShortID();
            AddNode(id, _activeItem.Name, _sharedOptions.AssemblyColor, _activeItem);
        }

        private void AddOutNodes()
        {
            string activeId = _activeItem.GetShortID();

            foreach (NamespaceItem nsItem in (_activeItem as AssemblyItem).NameSpaces)
            {
                if (CheckWorker()) break;

                foreach (TypeItem typeItem in nsItem.Types)
                {
                    if (CheckWorker()) break;

                    foreach (MethodItem methodItem in typeItem.Methods)
                    {
                        SDILReader.MethodBodyReader mr = new MethodBodyReader(methodItem.MethodInfo);
                        if (mr == null) continue;

                        // Abstract..external, etc
                        if (mr.instructions == null) continue;

                        string id = string.Empty;
                        string methodId = methodItem.GetShortID();

                        foreach (ILInstruction instruction in mr.instructions)
                        {
                            if (CheckWorker()) break;
                            if (instruction.Code.OperandType != OperandType.InlineMethod) continue;

                            // common code :/ todo
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
                                _logView.LogStr("Assembly Butterfly failed to recognise call instruction due to generics.");
                            }
                            else
                            {
                                _logView.LogStr("Assembly Butterfly failed to recognise call instruction :" + instruction.Operand.GetType().ToString());
                            }

                            // Find the method baseItem we're calling.
                            // The declaring type of the method we're calling.
                            BaseItem item = _projectBrowser.Lookup(id);

                            // If the method is not present in our project, then forget about it.
                            if (item == null) continue;

                            // Find the assembly the method is defined in.
                            BaseItem asmFrom = item;
                            while (!(asmFrom is AssemblyItem))
                            {
                                asmFrom = asmFrom.Parent;
                                if (asmFrom == null) break;
                            }

                            string asmFromId = asmFrom.GetShortID();

                            // Only add methods once.
                            if (this.AddedNodes.ContainsKey(asmFromId)) continue; //todo, needed now?
                            //

                            AddNode(asmFromId, asmFrom.Name, _sharedOptions.AssemblyColor, asmFrom);
                            AddEdge(activeId, asmFromId, EdgeStyle.NormalArrow);
                        }
                    }
                }
            }
        }


        private void AddInNodes()
        {
            string activeId = _activeItem.GetShortID();

            string activeAssemblyname = (_activeItem as AssemblyItem).PureName();

            foreach (KeyValuePair<string, BaseItem> pair in _projectBrowser.Files)
            {
                AssemblyItem assemblyItem = (AssemblyItem)pair.Value;

                if (assemblyItem.ReferencedAssemblies.Contains(activeAssemblyname))
                {
                    if (CheckWorker()) return;

                    string shortId = assemblyItem.GetShortID();
                    AddNode(shortId, assemblyItem.PureName(), _sharedOptions.AssemblyColor, assemblyItem);
                    AddEdge(shortId, activeId, EdgeStyle.NormalArrow);
                }
            }
        }


    }
}

