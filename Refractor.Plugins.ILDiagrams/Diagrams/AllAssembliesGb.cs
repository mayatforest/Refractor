using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;
using SDILReader;

namespace Refractor.Plugins.ILDiagrams
{
    public class AllAssembliesGb : BaseILGraphBuilder
    {
        public override void Translate()
        {
            FirstPass();
            SecondPass();
        }

        private void FirstPass()
        {
            RootItem root = _activeItem as RootItem;
            foreach (BaseItem item in root.Files)
            {
                if (item as AssemblyItem == null) continue;

                AssemblyItem assemblyItem = (AssemblyItem)item;

                this.AddNode(assemblyItem.PureName(), assemblyItem.PureName(),
                    _sharedOptions.AssemblyColor, assemblyItem);
            }
        }
                
        private void SecondPass()
        {
            RootItem root = _activeItem as RootItem;
            foreach (BaseItem item in root.Files)
            {
                if (item as AssemblyItem == null) continue;

                AssemblyItem assemblyItem = (AssemblyItem)item;
                string idFrom = assemblyItem.PureName();

                // We can use referenced assemblies as a shortcut, rather than 
                // running over all the method calls in all loaded files.
                foreach (string asmName in assemblyItem.ReferencedAssemblies)
                {
                    string idTo = asmName;

                    if (this.AddedNodes.ContainsKey(idTo))
                    {
                        this.AddEdge(idFrom, idTo, EdgeStyle.NormalArrow);
                    }
                }
            }
        }
    }
}
