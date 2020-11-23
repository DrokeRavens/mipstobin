using System;

namespace MipsToBin
{
    class Program
    {
        static void Main(string[] args)
        {

            FileControl fileControl = new FileControl();

            Log.WriteLog(Log.Status.Warn, "Digite sair a qualquer momento para sair.");

            while (true) {
                Console.WriteLine("Digite o nome do arquivo a ser convertido: ");
                var line = Console.ReadLine();

                if (line.Equals("sair", StringComparison.OrdinalIgnoreCase))
                    break;

                fileControl.ProcessFile(line);
            }
        }


    }
}
