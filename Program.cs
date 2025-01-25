using System;

namespace autoPrint
{
    class Program
    {
        static void ShowUsage()
        {
            Console.WriteLine("""
usage:
    AutoPrint [--reset]
    AutoPrint [--list]
    AutoPrint [--help]|[-h]|[--?]|[-?]

parameters:
    --reset     Reset the state file

    --list      List available printers

    --help      Show this help message.
    -h
    --?
    -?
""");
        }

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                switch (args[0].ToLower())
                {
                    case "--reset":
                        PrintingState.Clear();
                        break;

                    case "--list":
                        PrintingState.ListPrinters();
                        return;

                    case "--?":
                    case "-?":
                    case "--help":
                    case "-h":
                        ShowUsage();
                        return;
                }
            }

            Console.Write("Printing... ");
            var state = PrintingState.Load();
            var engine = new PrintingEngine(state);
            engine.Print();
            PrintingState.Save(state);
            Console.WriteLine("Done.");
        }
    }
}
