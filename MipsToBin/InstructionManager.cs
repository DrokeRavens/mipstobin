using System;
using System.Linq;

namespace MipsToBin
{

    public enum FieldType : byte { 
        Default = 0,
        Immediate = 1,
        Funct = 2,
        Addr = 3,
        Op = 4
    }
    
    public enum InstructionType :  byte
    {
        // 0 = R, 1 = I, 2 = J
        LB =  1,
        LH =  1,
        LWL = 1,
        LW =  1,
        LBU = 1,
        LHU = 1,
        LWR = 1,
        SB =  1,
        SH =  1,
        SWL = 1,
        SW =  1,
        SWR = 1,
        ADD  = 0,
        ADDU = 0,
        SUB  = 0,
        SUBU = 0,
        AND  = 0,
        OR   = 0,
        XOR  = 0,
        NOR  = 0,
        SLT  = 0,
        SLTU = 0,
        ADDI = 1,
        ADDIU= 1,
        SLTI = 1,
        SLTIU= 1,
        ANDI = 1,
        ORI  = 1,
        XORI = 1,
        LUI = 1,
        SLL = 0,
        SRL = 0,
        SRA = 0,
        SLLV = 0,
        SRLV = 0,
        SRAV = 0,
        MFHI = 0,
        MTHI = 0,
        MFLO = 0,
        MTLO = 0,
        MULT = 0,
        MULTU = 0,
        DIV = 0,
        DIVU = 0,
        JR = 0,
        JALR = 0,
        BLTZ = 1,
        BGEZ = 1,
        BLTZAL = 1,
        BGEZAL = 1,
        J = 2,
        JAL = 2,
        BEQ    = 1,
        BNE    = 1,
        BLEZ   = 1,
        BGTZ   = 1



    }

    public enum InstructionOpCode : byte {
        LB  = 32,
        LH  = 33,
        LWL = 34,
        LW  = 35,
        LBU = 36,
        LHU = 37,
        LWR = 38,
        SB  = 40,
        SH  = 41,
        SWL = 42,
        SW  = 43,
        SWR = 46,
        ADD  = 0, 
        ADDU = 0, 
        SUB  = 0, 
        SUBU = 0, 
        AND  = 0, 
        OR   = 0, 
        XOR  = 0, 
        NOR  = 0, 
        SLT  = 0, 
        SLTU = 0, 
        ADDI = 8, 
        ADDIU= 9, 
        SLTI = 10,
        SLTIU= 11,
        ANDI = 12,
        ORI  = 13,
        XORI = 14,
        LUI = 15,
        SLL  = 0,
        SRL  = 0,
        SRA  = 0,
        SLLV = 0,
        SRLV = 0,
        SRAV = 0,
        MFHI = 0,
        MTHI = 0,
        MFLO = 0,
        MTLO = 0,
        MULT = 0,
        MULTU= 0,
        DIV  = 0,
        DIVU = 0,
        JR = 0,
        JALR = 0,
        BLTZ   = 1,
        BGEZ   = 1,
        BLTZAL = 1,
        BGEZAL = 1,
        J      = 2,
        JAL    = 3,
        BEQ    = 4,
        BNE    = 5,
        BLEZ   = 6,
        BGTZ   = 7
    }

    public enum InstructionR_FuncId : byte {
        ADD   = 32,
        ADDU  = 33,
        SUB   = 34,
        SUBU  = 35,
        AND   = 36,
        OR    = 37,
        XOR   = 38,
        NOR   = 39,
        SLT   = 42,
        SLTU  = 43,
        SLL   = 0,
        SRL   = 2,
        SRA   = 3,
        SLLV  = 4,
        SRLV  = 6,
        SRAV  = 7,
        MFHI  = 16,
        MTHI  = 17,
        MFLO  = 18,
        MTLO  = 19,
        MULT  = 24,
        MULTU = 25,
        DIV   = 26,
        DIVU  = 27,
        JR    = 8,
        JALR = 9
          
    }
    /// <summary>
    /// Classe de Instrução. Contem metodos para a conversao do MIPS -> Binário
    /// </summary>
    public class Instruction {
        public string InstructionString { get; private set; }

        private byte GetIndex(string cmd) {
            
            byte index;
            if (RegNames.Names.Contains(cmd))
                index = (byte)Array.IndexOf(RegNames.Names, cmd);
            else if (char.ToUpper(cmd[0]) == 'r' || char.ToUpper(cmd[0]) == '$')
                index = byte.Parse(cmd.Substring(1));
            else
                throw new Exception("Argumento desconhecido: " + cmd);

            return index;
        }

        private int GetFieldSize(FieldType fieldType) {
            switch (fieldType)
            {

                case FieldType.Op:
                    return 6;
                case FieldType.Default:
                    return 5;
                case FieldType.Funct:
                    return 6;
                case FieldType.Immediate:
                    return 16;
                case FieldType.Addr:
                    return 26;
                default:
                    throw new Exception("Unkown field");
            }
        }

        /// <summary>
        /// Le um comando separado com base nos delimitadores, e faz as operações necessárias para que o codigo seja convertido em binário, após a conversão o buffer é salvo no InstructionString.
        /// </summary>
        /// <param name="type">O tipo da instrução. R, I ou J.</param>
        /// <param name="cmd">A linha de comando já separada.</param>
        public Instruction(InstructionType type, string[] cmd) {
            if (type == 0)
            {
                /* R
                 0 = OP,
                 1 = RS,
                 2 = RT,
                 3 = RD, 
                 4 = SHAMT
                 6 = func
                */
                InstructionR_FuncId funcId;
                if (!Enum.TryParse(cmd[0], true, out funcId))
                {
                    throw new Exception("Unknown command : " + cmd[0]);
                }
                else {
                    byte funcIdByte = (byte)funcId;
                    if ((funcIdByte >= 32 && funcIdByte <= 43) || (funcIdByte >= 4 && funcIdByte <= 7))
                    {
                        if (cmd.Length == 4)
                        {
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Op), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[3]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(funcIdByte, 2).PadLeft(GetFieldSize(FieldType.Funct), '0');
                        }
                    }
                    else if (funcIdByte >= 0 && funcIdByte <= 3) {
                        if (cmd.Length == 4)
                        {
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Op), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(byte.Parse(cmd[3]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(funcIdByte, 2).PadLeft(GetFieldSize(FieldType.Funct), '0');
                        }
                    }
                    else if ((funcIdByte >= 24 && funcIdByte <= 27) || (funcIdByte == 17 || funcIdByte == 19 || funcIdByte == 8))
                    {
                        if (cmd.Length == 4)
                        {

                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Op), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(funcIdByte, 2).PadLeft(GetFieldSize(FieldType.Funct), '0');
                        }
                    }
                    else if (funcIdByte == 16 || funcIdByte == 18)
                    {
                        if (cmd.Length == 4)
                        {
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Op), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0');
                            InstructionString += Convert.ToString(funcIdByte, 2).PadLeft(GetFieldSize(FieldType.Funct), '0');
                        }
                    }

                }

                #region info
                /*
                Possibilidades para R Func de 32 a 43 inclusive / 4 a 7 inclusive:
                        OP = 0,
                        RS = From Register Bank
                        RT = From Register Bank
                        RD = From Register Bank
                        SHAMT = 0

                Possibilidades para R Func de 0 a 3 inclusive:
                        OP = 0,
                        RS = 0
                        RT = From Register Bank
                        RD = From Register Bank
                        SHAMT = ShiftAmount

                 Possibilidades para R Func de 24 a 27 inclusive:
                        OP = 0,
                        RS = From Register Bank
                        RT = 0
                        RD = 0
                        SHAMT = 0

                Possibilidades para R Func de 8 inclusive:
                        OP = 0,
                        RS = From Register Bank
                        RT = 0
                        RD = 0
                        SHAMT = 0

                Possibilidades para R Func de 9 inclusive:
                        OP = 0,
                        RS = From Register Bank
                        RT = 0
                        RD = From Register Bank
                        SHAMT = 0

                Possibilidades para R Func de 16 inclusive:
                        OP = 0,
                        RS = 0
                        RT = 0
                        RD = From Register Bank
                        SHAMT = 0
                Possibilidades para R Func de 17 inclusive:
                        OP = 0,
                        RS = From Register Bank
                        RT = 0
                        RD = 0
                        SHAMT = 0
                Possibilidades para R Func de 18 inclusive:
                        OP = 0,
                        RS = 0
                        RT = 0
                        RD = From Register Bank
                        SHAMT = 0
                Possibilidades para R Func de 19 inclusive:
                        OP = 0,
                        RS = From Register Bank
                        RT = 0
                        RD = 0
                        SHAMT = 0
             */
                #endregion
            }
            else if ((byte)type == 1) //I
            {
                InstructionOpCode opCode;
                if (!Enum.TryParse(cmd[0], true, out opCode))
                {
                    throw new Exception("Unknown command : " + cmd[0]);
                }
                else
                {
                    byte opCodeByte = (byte)opCode;
                    if (opCodeByte >= 32 && opCodeByte <= 46) {
                        if (!cmd[2].Contains("(") && !cmd[2].Contains(")"))
                            throw new Exception("Unknown pattern. " + cmd[3]);

                        string[] fixStr = cmd[2].Split('(');

                        InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0');  //OP
                        InstructionString += Convert.ToString(GetIndex(fixStr[1].Replace(")", "")), 2).PadLeft(GetFieldSize(FieldType.Default), '0');  // RS
                        InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT

                        short immValue = short.Parse(fixStr[0]);
                        if (immValue > Math.Pow(2, GetFieldSize(FieldType.Immediate)))
                            throw new Exception("Stack overflow");
                        InstructionString += Convert.ToString(immValue, 2).PadLeft(GetFieldSize(FieldType.Immediate), '0');  // Immediate
                        
                    }
                    else if ((opCodeByte >= 8 && opCodeByte <= 15) || (opCodeByte >= 4 && opCodeByte <= 5)){
                        InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                        InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RS
                        InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT


                        short immValue = short.Parse(cmd[3]);
                        if (immValue > Math.Pow(2, GetFieldSize(FieldType.Immediate)))
                            throw new Exception("Stack overflow");
                        InstructionString += Convert.ToString(immValue, 2).PadLeft(GetFieldSize(FieldType.Immediate), '0'); // Immediate
                    }
                    else if (opCodeByte == 1) {
                        if (cmd[0].Equals("BLTZ", StringComparison.OrdinalIgnoreCase))
                        {
                            InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RS
                            InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT
                            InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Immediate), '0'); // Immediate
                        }
                        else if (cmd[0].Equals("BGEZ", StringComparison.OrdinalIgnoreCase))
                        {
                            InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RS
                            InstructionString += Convert.ToString(1, 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT
                            InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Immediate), '0'); // Immediate
                        }
                        else if (cmd[0].Equals("BLTZAL", StringComparison.OrdinalIgnoreCase))
                        {
                            InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RS
                            InstructionString += Convert.ToString(16, 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT
                            InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Immediate), '0'); // Immediate
                        }
                        else if (cmd[0].Equals("BGEZAL", StringComparison.OrdinalIgnoreCase))
                        {
                            InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                            InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RS
                            InstructionString += Convert.ToString(17, 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT
                            InstructionString += Convert.ToString(GetIndex(cmd[2]), 2).PadLeft(GetFieldSize(FieldType.Immediate), '0'); // Immediate
                        }
                        else {
                            throw new Exception("MIPS code is invalid");
                        }
                    }
                    else if (opCodeByte >= 6 || opCodeByte <= 7) {
                        InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                        InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RS
                        InstructionString += Convert.ToString(0, 2).PadLeft(GetFieldSize(FieldType.Default), '0'); // RT
                        short immValue = short.Parse(cmd[3]);
                        if (immValue > Math.Pow(2, GetFieldSize(FieldType.Immediate)))
                            throw new Exception("Stack overflow");

                        InstructionString += Convert.ToString(immValue, 2).PadLeft(GetFieldSize(FieldType.Immediate), '0'); // Immediate
                    }
                    else
                    {
                        throw new Exception("MIPS code is invalid");
                    }
                }
                /*
                Possibilidades para I OP de 32 a 46 inclusive / OP de 8 a 15 inclusive / OP de 4 a 5 inclusive:
                    RS = From Register Bank
                    RT = From Register Bank
                    IMME = Offset / Immediate

                Possibilidades para RT 0 ou 1 e OP 1:
                    RS = From Register Bank
                    RT = From Register Bank
                    IMME = Offset / Immediate

                Possibilidades para OP de 6 a 7:
                    RS = From Register Bank
                    RT = 0
                    IMME = Offset / Immediate

             */

            }
            else { //Must be J
                /*
                  Possibilidades para J op 2 e 3 inclusive:
                    ADDR = Value
                 */
                InstructionOpCode opCode;
                if (!Enum.TryParse(cmd[0], true, out opCode))
                {
                    throw new Exception("Unknown command : " + cmd[0]);
                }
                else
                {
                    byte opCodeByte = (byte)opCode;
                    if (opCodeByte >= 2 || opCodeByte <= 3) {
                        InstructionString += Convert.ToString(opCodeByte, 2).PadLeft(GetFieldSize(FieldType.Op), '0'); //OP
                        int addr = int.Parse(cmd[1]);
                        if (addr > Math.Pow(2, GetFieldSize(FieldType.Addr)))
                            throw new Exception("Stack overflow");
                        InstructionString += Convert.ToString(GetIndex(cmd[1]), 2).PadLeft(GetFieldSize(FieldType.Addr), '0'); // Offset
                    }

                }
            }
        }



    }

}
