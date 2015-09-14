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
    public class MethodButterflyGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            // Add the central node, the body of the butterfly.
            AddCentralNode();

            // Add the easy out links.
            AddOutNodes();

            // Add the harder in links.
            AddInNodesExternal();
        }

        private Type _methodType;

        private void AddCentralNode()
        {
            string methodId = _activeItem.GetShortID();
            string caption = _activeItem.Name;
            
            if ((_activeItem as MethodItem).MethodInfo.IsAbstract)
            {
                caption += "\n(abstract)";
            }

            AddNode(methodId, caption, _sharedOptions.MethodColor, _activeItem);

            _methodType = (_activeItem as MethodItem).MethodInfo.DeclaringType;
        }

        private void AddNullBodyOutNodes()
        {
            if ((_activeItem as MethodItem).MethodInfo.IsAbstract) //todo
            {
                // we want all the subclasses of this class,
                // or all the classes that implement the interface

                //_activeItem.MethodInfo.DeclaringType.IsAbstract

                //_windowManager.ProjectBrowser.

                //(_activeItem.Parent as TypeItem).Set
            }
        }

        private void AddOutNodes()
        {
            SDILReader.MethodBodyReader mr = new MethodBodyReader((_activeItem as MethodItem).MethodInfo);
            if (mr == null) return;

            // Abstract..external, etc
            if (mr.instructions == null)
            {
                AddNullBodyOutNodes();
                return;
            }

            string id = string.Empty;
            string methodId = _activeItem.GetShortID();

            foreach (ILInstruction instruction in mr.instructions)
            {                
                if (CheckWorker()) break;

                if (instruction.Code.OperandType != OperandType.InlineMethod) continue;

                Type type = null;
                if (instruction.Operand is MethodInfo)
                {
                    id = string.Format("{0} {1}",
                        (instruction.Operand as MethodInfo).DeclaringType.ToString(),
                        RefHelp.GetNameWithParameterList(instruction.Operand as MethodInfo));

                    type = (instruction.Operand as MethodInfo).DeclaringType;
                }
                else if (instruction.Operand is ConstructorInfo)
                {
                    id = string.Format("{0} {1}",
                        (instruction.Operand as ConstructorInfo).DeclaringType.ToString(),
                        RefHelp.GetNameWithParameterList(instruction.Operand as ConstructorInfo));

                    type = (instruction.Operand as ConstructorInfo).DeclaringType;
                }
                else
                {
                    _logView.LogStr("Butterfly failed to recognise call instruction :" + instruction.Operand.GetType().ToString());
                }

                // Find the method baseItem we're calling.
                BaseItem item = _projectBrowser.Lookup(id);

                // If the method is not present in our project, then forget about it.
                if (item == null) continue;

                string caption = item.Name;
                if (_methodType != type)
                {
                    caption = type.Name + "\n" + caption;
                }
                AddNode(id, caption, _sharedOptions.MethodColor, item);
                AddEdge(methodId, id, EdgeStyle.NormalArrow);
            }
        }

        private void AddInNodesExternal()
        {
            string methodId = _activeItem.GetShortID();

            BaseItem item = _activeItem;
            while (!(item is AssemblyItem)) item = item.Parent;

            string currentAssemblyname = (item as AssemblyItem).PureName();

            foreach (KeyValuePair<string, BaseItem> pair in _projectBrowser.Files)
            {
                if (CheckWorker()) break;

                AssemblyItem assemblyItem = (AssemblyItem)pair.Value;


                if ((assemblyItem.PureName() == currentAssemblyname) ||
                    assemblyItem.ReferencedAssemblies.Contains(currentAssemblyname))
                {

                    foreach (NamespaceItem nsItem in assemblyItem.NameSpaces)
                    {
                        if (CheckWorker()) break;
                        foreach (TypeItem typeItem in nsItem.Types)
                        {
                            if (CheckWorker()) break;
                            AddLinksFromMethods(typeItem, methodId);
                        }
                    }
                }
            }
        }

        private void AddLinksFromMethods(TypeItem fromTypeItem, string toMethodId)
        {
            string fromMethodId, id;

            foreach (MethodItem methodItem in fromTypeItem.Methods)
            {
                if (CheckWorker()) break;
                fromMethodId = methodItem.GetShortID();

                SDILReader.MethodBodyReader mr = new MethodBodyReader(methodItem.MethodInfo);
                if (mr == null) continue;

                // Abstract..external, etc
                if (mr.instructions == null) continue;

                foreach (ILInstruction instruction in mr.instructions)
                {
                    if (CheckWorker()) break;
                    id = string.Empty;

                    if (instruction.Code.OperandType != OperandType.InlineMethod) continue;

                    if (instruction.Operand is MethodInfo)
                    {
                        id = string.Format("{0} {1}",
                            (instruction.Operand as MethodInfo).DeclaringType.ToString(),
                            RefHelp.GetNameWithParameterList(instruction.Operand as MethodInfo));
                    }
                    else if (instruction.Operand is ConstructorInfo)
                    {
                        id = string.Format("{0} {1}",
                            (instruction.Operand as ConstructorInfo).DeclaringType.ToString(),
                            RefHelp.GetNameWithParameterList(instruction.Operand as ConstructorInfo));
                    }
                    else if (instruction.Operand == null)
                    {
                        _logView.LogStr("Butterfly failed with null instruction.operand :" + instruction.ToString());
                    }
                    else
                    {
                        _logView.LogStr("Butterfly failed to recognise call instruction (in) :" + instruction.Operand.GetType().ToString());
                    }

                    // We're looking for hits on one specific method.
                    if (id != toMethodId) continue;

                    // Only add nodes once.
                    //if (_addedNodes.ContainsKey(fromMethodId)) continue;

                    string caption = fromTypeItem + "\n" + methodItem.Name;

                    AddNode(fromMethodId, caption, _sharedOptions.MethodColor, methodItem);
                    AddEdge(fromMethodId, toMethodId, EdgeStyle.NormalArrow);
                }
            }
        }

    }
}
