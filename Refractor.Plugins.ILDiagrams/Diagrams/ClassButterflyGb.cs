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
    public class ClassButterflyGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            // Add the central node, the body of the butterfly.
            AddCentralNode();

            // Add the easy out links.
            AddOutNodes();

            AddOutImplementsNodes();
            AddOutInheritsFromNode();

            // Add the harder in links.
            AddInNodes();
        }

        private void AddCentralNode()
        {
            string typeId = _activeItem.GetShortID();
            AddNode(typeId, _activeItem.Name, _sharedOptions.TypeColor, _activeItem);

            //if (_activeAssembly.MethodInfo.IsAbstract)
            //{
            //    child.Attr.Label += "\n(abstract)";
            //}
        }

        private void AddOutNodes()
        {
            string activeId = _activeItem.GetShortID();

            foreach (MethodItem methodItem in (_activeItem as TypeItem).Methods)
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

                    AddNode(id, item.Name, _sharedOptions.TypeColor, item);
                    AddEdge(activeId, id, EdgeStyle.NormalArrow);
                }
            }
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

                AddNode(id, baseItem.Name, _sharedOptions.TypeColor, baseItem);
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

            // Only add methods once.
            if (_addedNodes.ContainsKey(id))
            {
                object baby = _addedNodes[id];
                UpdateNodeColor(baby, _sharedOptions.TypeColor);
            }
            else
            {
                AddNode(id, baseItem.Name, _sharedOptions.TypeColor, baseItem);
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

                    AddNode(fromTypeId, fromTypeItem.Name, _sharedOptions.TypeColor, fromTypeItem);
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

                AddNode(fromTypeId, fromTypeItem.Name, _sharedOptions.TypeColor, fromTypeItem);
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
                AddNode(fromTypeId, baseItem.Name, _sharedOptions.TypeColor, baseItem);
            //}

            AddEdge(fromTypeId, toTypeId, EdgeStyle.NormalArrow);
        }

    }
}
