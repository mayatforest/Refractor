using System;
using System.Text;
using System.Reflection;
//using System.Reflection.Emit;

using Refractor.Common;
using SDILReader;

namespace Refractor.Plugins.ILDiagrams
{
    public class BaseILGraphBuilder : BaseGraphBuilder
    {
        public override void BeforeTranslate()
        {
            // This saves us casting everywhere.
            _sharedOptions = _windowManager.GetPluginOptions("IL Parser Plugin") as CSParserOptions;
        }

        protected CSParserOptions _sharedOptions;

        protected MethodBodyReader GetMethodBodyReader(MethodInfo mi)
        {
            SDILReader.MethodBodyReader mr = null;
            try
            {
                mr = new MethodBodyReader(mi);
            }
            catch (System.IO.FileNotFoundException)
            {
                // We will already have been warned about missing files during the load.
            }
            return mr;
        }

        protected void GetInstrDetails(ILInstruction instruction, 
            out string name, out Type declType, out string calledId)
        {
            name = null;
            declType = null;
            calledId = null;
            
            if (instruction.Operand is MethodInfo)
            {
                MethodInfo mOperand = (MethodInfo)instruction.Operand;
                declType = mOperand.DeclaringType;
                name = mOperand.Name;
                calledId = declType.FullName + " " + RefHelp.GetNameWithParameterList(mOperand);
            }
            else if (instruction.Operand is ConstructorInfo)
            {
                ConstructorInfo mOperand = (ConstructorInfo)instruction.Operand;
                declType = mOperand.DeclaringType;
                name = mOperand.Name;
                calledId = declType.FullName + " " + RefHelp.GetNameWithParameterList(mOperand);
            }
        }


    }
}
