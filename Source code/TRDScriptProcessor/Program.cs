using System;
using System.Collections.Generic;
using System.IO;
using TRDScriptLib;

namespace TRDScriptProcessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TRD Script processor - by The_Real_Dee__\n");

            if (args.Length == 0)
            {
                Console.WriteLine("Runs a TRD script file(.tscr)");
                Console.WriteLine("TRD Script Processor.exe [SCRIPT]\n");
                Console.WriteLine("    -usage                        | Displays this message");
            }
            else if (args.Length != 1)
            {
                Console.WriteLine("Error: Incorrect syntax");
                Console.WriteLine("       Use -usage for help");
            }
            else
            {
                if (args[0] == "-usage")
                {
                    Console.WriteLine("Runs a TRD script file(.tscr)");
                    Console.WriteLine("TRD Script Processor.exe [SCRIPT]\n");
                    Console.WriteLine("    -usage                        | Displays this message");
                }
                else
                {

                    TRDScriptLib.TRDScriptProcessor processor = new TRDScriptLib.TRDScriptProcessor();

                    if (File.Exists(args[0].Replace("/", "\\")))
                    {
                        bool ran = processor.Process(args[0].Replace("/", "\\"));
                        string runstate = "Failed";

                        Console.Write("Script runstate: ");

                        if (ran)
                        {
                            runstate = "Success";
                            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(runstate);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(runstate);
                        }
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.WriteLine("Error: File not found.");
                    }
                }
            }
        }
    }
}