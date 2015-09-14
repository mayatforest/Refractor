using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace SDILReader
{
    public class MethodBodyReader
    {
        public List<SDILReader.ILInstruction> instructions = null;
        protected byte[] il = null;
        private MethodBase mi = null;

        #region il read methods
        private int ReadInt16(byte[] _il, ref int position)
        {
            return ((il[position++] | (il[position++] << 8)));
        }
        private ushort ReadUInt16(byte[] _il, ref int position)
        {
            return (ushort)((il[position++] | (il[position++] << 8)));
        }
        private int ReadInt32(byte[] _il, ref int position)
        {
            return (((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18));
        }
        private ulong ReadInt64(byte[] _il, ref int position)
        {
            return (ulong)(((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18) | (il[position++] << 0x20) | (il[position++] << 0x28) | (il[position++] << 0x30) | (il[position++] << 0x38));
        }
        private double ReadDouble(byte[] _il, ref int position)
        {
            return (((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18) | (il[position++] << 0x20) | (il[position++] << 0x28) | (il[position++] << 0x30) | (il[position++] << 0x38));
        }
        private sbyte ReadSByte(byte[] _il, ref int position)
        {
            return (sbyte)il[position++];
        }
        private byte ReadByte(byte[] _il, ref int position)
        {
            return (byte)il[position++];
        }
        private Single ReadSingle(byte[] _il, ref int position)
        {
            return (Single)(((il[position++] | (il[position++] << 8)) | (il[position++] << 0x10)) | (il[position++] << 0x18));
        }
        #endregion

        /// <summary>
        /// Constructs the array of ILInstructions according to the IL byte code.
        /// </summary>
        /// <param name="module"></param>
        private void ConstructInstructions(Module module)
        {
            byte[] il = this.il;
            int position = 0;
            instructions = new List<ILInstruction>();
            while (position < il.Length)
            {
                ILInstruction instruction = new ILInstruction();

                // get the operation code of the current instruction
                OpCode code = OpCodes.Nop;
                ushort value = il[position++];
                if (value != 0xfe)
                {
                    code = Globals.singleByteOpCodes[(int)value];
                }
                else
                {
                    value = il[position++];
                    code = Globals.multiByteOpCodes[(int)value];
                    value = (ushort)(value | 0xfe00);
                }
                instruction.Code = code;
                instruction.Offset = position - 1;
                int metadataToken = 0;
                // get the operand of the current operation
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                        metadataToken = ReadInt32(il, ref position);
                        metadataToken += position;
                        instruction.Operand = metadataToken;
                        break;
                    
                    // patched from comments on CP -hwd
                    case OperandType.InlineField:
                        metadataToken = ReadInt32(il, ref position);
                        if (mi is ConstructorInfo)
                        {
                            instruction.Operand = module.ResolveField(metadataToken,
                                mi.DeclaringType.GetGenericArguments(), null);
                        }
                        else
                        {
                            instruction.Operand = module.ResolveField(metadataToken,
                                mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                        }
                        break;

                    // patched from comments on CP -hwd
                    case OperandType.InlineMethod:
                        metadataToken = ReadInt32(il, ref position);
                        try
                        {
                            if (mi is ConstructorInfo)
                            {
                                instruction.Operand = module.ResolveMethod(metadataToken,
                                    mi.DeclaringType.GetGenericArguments(), null);
                            }
                            else
                            {
                                instruction.Operand = module.ResolveMethod(metadataToken,
                                    mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                            }
                        }
                        catch
                        {
                            if (mi is ConstructorInfo)
                            {
                                instruction.Operand = module.ResolveMember(metadataToken,
                                    mi.DeclaringType.GetGenericArguments(), null);
                            }
                            else
                            {
                                instruction.Operand = module.ResolveMember(metadataToken,
                                    mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                            }
                        }
                        break;
                    
                    case OperandType.InlineSig:
                        metadataToken = ReadInt32(il, ref position);
                        instruction.Operand = module.ResolveSignature(metadataToken);
                        break;

                    // patched from comments on CP -hwd
                    case OperandType.InlineTok:
                        metadataToken = ReadInt32(il, ref position);
                        //try
                        //{
                            if (mi is ConstructorInfo)
                            {
                                instruction.Operand = module.ResolveType(metadataToken,
                                    mi.DeclaringType.GetGenericArguments(), null);
                            }
                            else
                            {
                                instruction.Operand = module.ResolveType(metadataToken,
                                    mi.DeclaringType.GetGenericArguments(), mi.GetGenericArguments());
                            }
                        //}
                        //catch
                        //{
                        //    int i = 1;

                        //}
                        break;

                    // patched from comments on CP -hwd
                    case OperandType.InlineType:
                        metadataToken = ReadInt32(il, ref position);
                        if (this.mi is MethodInfo)
                        {
                            instruction.Operand = module.ResolveType(metadataToken,
                            this.mi.DeclaringType.GetGenericArguments(), this.mi.GetGenericArguments());
                        }
                        else if (mi is ConstructorInfo)
                        {
                            instruction.Operand = module.ResolveType(metadataToken,
                               this.mi.DeclaringType.GetGenericArguments(), null);
                        }
                        else
                        {
                            instruction.Operand = module.ResolveType(metadataToken);
                        }
                        break;

                    case OperandType.InlineI:
                        {
                            instruction.Operand = ReadInt32(il, ref position);
                            break;
                        }
                    case OperandType.InlineI8:
                        {
                            instruction.Operand = ReadInt64(il, ref position);
                            break;
                        }
                    case OperandType.InlineNone:
                        {
                            instruction.Operand = null;
                            break;
                        }
                    case OperandType.InlineR:
                        {
                            instruction.Operand = ReadDouble(il, ref position);
                            break;
                        }
                    case OperandType.InlineString:
                        {
                            metadataToken = ReadInt32(il, ref position);
                            instruction.Operand = module.ResolveString(metadataToken);
                            break;
                        }
                    case OperandType.InlineSwitch:
                        {
                            int count = ReadInt32(il, ref position);
                            int[] casesAddresses = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                casesAddresses[i] = ReadInt32(il, ref position);
                            }
                            int[] cases = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                cases[i] = position + casesAddresses[i];
                            }
                            break;
                        }
                    case OperandType.InlineVar:
                        {
                            instruction.Operand = ReadUInt16(il, ref position);
                            break;
                        }
                    case OperandType.ShortInlineBrTarget:
                        {
                            instruction.Operand = ReadSByte(il, ref position) + position;
                            break;
                        }
                    case OperandType.ShortInlineI:
                        {
                            instruction.Operand = ReadSByte(il, ref position);
                            break;
                        }
                    case OperandType.ShortInlineR:
                        {
                            instruction.Operand = ReadSingle(il, ref position);
                            break;
                        }
                    case OperandType.ShortInlineVar:
                        {
                            instruction.Operand = ReadByte(il, ref position);
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown operand type.");
                        }
                }
                instructions.Add(instruction);
            }
        }

        public object GetRefferencedOperand(Module module, int metadataToken)
        {
            AssemblyName[] assemblyNames = module.Assembly.GetReferencedAssemblies();
            for (int i=0; i<assemblyNames.Length; i++)
            {
                Module[] modules = Assembly.Load(assemblyNames[i]).GetModules();
                for (int j=0; j<modules.Length; j++)
                {
                    try
                    {
                        Type t = modules[j].ResolveType(metadataToken);
                        return t;
                    }
                    catch
                    {

                    }

                }
            }
            return null;
        //System.Reflection.Assembly.Load(module.Assembly.GetReferencedAssemblies()[3]).GetModules()[0].ResolveType(metadataToken)

        }
        /// <summary>
        /// Gets the IL code of the method
        /// </summary>
        /// <returns></returns>
        public string GetBodyCode()
        {
            string result = "";
            if (instructions != null)
            {
                for (int i = 0; i < instructions.Count; i++)
                {
                    result += instructions[i].GetCode() + "\n\r";
                }
            }
            return result;

        }

        /// <summary>
        /// MethodBodyReader constructor
        /// </summary>
        /// <param name="mi">
        /// The System.Reflection defined MethodInfo
        /// </param>
        public MethodBodyReader(MethodInfo mi)
        {
            this.mi = mi;
            if (mi.GetMethodBody() != null)
            {
                il = mi.GetMethodBody().GetILAsByteArray();
                ConstructInstructions(mi.Module);
            }
        }
    }
}
