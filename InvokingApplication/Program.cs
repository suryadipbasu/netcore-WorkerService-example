using System;
using System.IO;

namespace InvokingApplication
{
    class Program
    {
        private static string _folderPathToDumpFiles = @"C:\temp\PerformArithmaticOperations\DumpingFolder";
        static void Main(string[] args)
        {
            bool flag = true;

            while (flag)
            {
                #region User I/O Operations
                string num1 = "0", num2 = "0", operation = "0";
                Console.WriteLine("Pleae choose operation type:");
                Console.WriteLine("===============================");
                Console.WriteLine("Enter '1' to ADD two numbers");
                Console.WriteLine("Enter '2' to SUBTRACT two numbers");
                Console.WriteLine("Enter '3' to DIVIDE two numbers");
                Console.WriteLine("Enter '4' to MULTIPLY two numbers");
                Console.WriteLine("Enter '5' to perform API call to weather API");
                Console.WriteLine();
                Console.WriteLine("Enter operation number: ");
                operation = Console.ReadLine();
                if (operation != "5")
                {
                    Console.WriteLine("Enter Number 1: ");
                    num1 = Console.ReadLine();
                    Console.WriteLine("Enter Number 2: ");
                    num2 = Console.ReadLine();
                }
                #endregion User I/O Operations

                string stub2Dump = string.Join(",", operation, num1, num2);
                string fileName = $"{ ((Operations)int.Parse(operation)) }_{ num1 }_{ num2 }_" +
                                  $"{ DateTime.Now.ToLongTimeString().Replace(':','_') }.txt";
                string filePath = Path.Combine(_folderPathToDumpFiles, fileName);

                File.WriteAllText(filePath, stub2Dump);

                Console.WriteLine($"Requested operation queued for processing. File can be located at: {filePath}");
                Console.WriteLine();

                Console.WriteLine("Do you want to do another operation ? [Yes/No]");
                flag = Console.ReadLine().ToLower() == "no" ? false : true;
            }
        }
    }
}
