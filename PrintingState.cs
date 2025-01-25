using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;
using System.Xml.Serialization;

namespace autoPrint
{
    [Serializable]
    public class PrintingState
    {
        private static String FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThomasEWillson", "AutoPrint", "printingState.xml");

        private static String FolderPath => Path.GetDirectoryName(FilePath);

        public Point CurrentLocation { get; set; } = new Point(0, 0);

        public int CurrentLineHeight { get; set; } = 0;

        public string PrinterName { get; set; } = string.Empty;

        static int PromptInt(string prompt, int min, int max)
        {
            do
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out int value) && min <= value && value <= max)
                    return value;
                Console.WriteLine("Invalid input.");
            } while (true);
        }

        public static void Clear()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        static IEnumerable<string> GetPrinterNames()
        {
            var BadPrefixes = new string[] { "Fax", "OneNote", "Microsoft" };
            return PrinterSettings.InstalledPrinters
                                  .Cast<string>()
                                  .Where(name => !BadPrefixes.Any(prefix => name.StartsWith(prefix)));
        }

        public static void ListPrinters(IList<string> names)
        {
            Console.WriteLine("Available printers:");
            for (int i = 0; i < names.Count; i++)
                Console.WriteLine($"  {i + 1,2}: {names[i]}");
        }

        public static void ListPrinters()
        {
            ListPrinters(GetPrinterNames().ToList());
        }

        public static PrintingState Load()
        {
            if (!File.Exists(FilePath))
            {
                var printingState = new PrintingState();

                var names = GetPrinterNames().ToList();
                int choice = 0;
                if (names.Count > 1)
                {
                    ListPrinters(names);
                    choice = PromptInt("Choose a printer:", 1, names.Count) - 1;
                }
                printingState.PrinterName = names[choice];
                return printingState;
            }

            using(var file = File.OpenRead(FilePath))
            {
                var serializer = new XmlSerializer(typeof(PrintingState));
                return (PrintingState)serializer.Deserialize(file);
            }
        }

        public static void Save(PrintingState printingState)
        {
            Directory.CreateDirectory(FolderPath);

            using(var writer = new StreamWriter(FilePath, false, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(PrintingState));
                serializer.Serialize(writer, printingState);
            }
        }
    }

}