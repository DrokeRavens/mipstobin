using System;
using System.Collections.Generic;
using System.Text;

namespace MipsToBin
{
    public class InstructionConverter
    {
        private static char[] DELIMITERS = new char[] {' ', ',' };

        /// <summary>
        /// Converte uma linha de codigo MIPS para Objeto de Instrução
        /// </summary>
        /// <param name="mipsCommand">A linha mips</param>
        /// <returns>Instrução resultante, ou null caso ocorra erro.</returns>
        public static Instruction FromString(string mipsCommand) {
            mipsCommand = Helper.RemoveExtraSpaces(mipsCommand);
            string[] splitedCommand = mipsCommand.Split(DELIMITERS);
            InstructionType type;
            if (!InstructionType.TryParse(splitedCommand[0], true, out type))
            {
                return null;
            }

            return new Instruction(type, splitedCommand);
        }
    }
}
