using System;
using System.IO;


namespace MipsToBin
{
    public class FileControl
    {
        /// <summary>
        /// Le um arquivo, verifica se existe e em seguida tenta realizar a conversão
        /// </summary>
        /// <param name="pathToFile">O caminho para o arquivo</param>
        public void ProcessFile(string pathToFile) {
            if (File.Exists(pathToFile))
            {
                ConvertFromMips( File.ReadAllLines(pathToFile), pathToFile );
            }
            else {
                Log.WriteLog(Log.Status.Fail, $"O arquivo '{pathToFile}' não foi encontrado.");
            }
        }

        /// <summary>
        /// Converte uma sequencia de codigo mips(por linha) em codigo binário. 
        /// </summary>
        /// <param name="lines">As linhas MIPS</param>
        /// <param name="path">O local do arquivo</param>
        public void ConvertFromMips(string[] lines, string path) {
            string finalStr = "";
            bool error = false;
            foreach (var line in lines) {
                var result = InstructionConverter.FromString(line);
                if (result == null)
                {
                    error = true;
                    break;
                }
                else {
                    finalStr += result.InstructionString + Environment.NewLine;
                }
            }

            if (error)
            {
                Log.WriteLog(Log.Status.Fail, "Falha ao converter.");
            }
            else {
                
                path = path.Replace("txt", "bin");
                File.WriteAllText( path, finalStr );
                Log.WriteLog(Log.Status.Success, $"Arquivo salvo como {path} ");
            }
        }
    }
}
