using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VMTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayIntro();
            if (!CheckSourceFileOk(args))
                return;
            string sourceFile = args[0];

            LineOfCode[] parsedLines = Parse(sourceFile);
            if (!CheckForParsingErrors(parsedLines))
                return;

            string[] results = Translate(Path.GetFileNameWithoutExtension(sourceFile), parsedLines);
            WriteToOutput(args, sourceFile, results);
        }

        private static void DisplayIntro()
        {
            Console.WriteLine("VM Translator");
            Console.WriteLine("-------------");
            Console.WriteLine("Usage: dotnet ./VMTranslator.dll [source-file] [--console-only]");
            Console.WriteLine();
            Console.WriteLine("source-file:");
            Console.WriteLine("    Path to file containing VM code code (must have an .vm file extension)");
            Console.WriteLine("--console-only:");
            Console.WriteLine("    If this option is specified, results will be written to the console instead of to an .asm file.");
            Console.WriteLine("");
            Console.WriteLine("Results will be written to a file named after the source file, but with a .asm file extension. Any existing file with this name will be overwritten.");
            Console.WriteLine("");
        }

        private static bool CheckSourceFileOk(string[] args)
        {
            string error = null;
            if (args.Length == 0)
                error = "No source file specified.";
            else
            {
                string sourceFile = args[0];
                if (!File.Exists(sourceFile))
                    error = $"Source file {sourceFile} does not exist.";
                else if (Path.GetExtension(sourceFile) != ".vm")
                    error = $"Source file {sourceFile} does not have a .vm file extension.";
            }
            if (error != null)
                Console.WriteLine(error);
            return error == null;
        }

        private static LineOfCode[] Parse(string sourceFile)
        {
            var parser = new Parser();
            var parsedLines = new List<LineOfCode>();
            using (var stream = File.OpenText(sourceFile))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    parsedLines.Add(parser.Parse(line));
                }
            }
            return parsedLines.ToArray();
        }

        private static string[] Translate(string filename, LineOfCode[] parsedLines)
        {
            var translator = new Translator(filename);
            var results = new List<string>();
            foreach (LineOfCode parsedLine in parsedLines)
            {
                // stuff
                if (parsedLine != null)
                    results.Add(translator.Translate(parsedLine));
            }
            return results.ToArray();
        }

        private static bool CheckForParsingErrors(LineOfCode[] parsedLines)
        {
            bool valid = true;
            for(var i = 0; i < parsedLines.Length; i++)
            {
                if (parsedLines[i].Error != null)
                {
                    Console.WriteLine($"Line {i+1}: {parsedLines[i].Error}");
                    valid = false;
                }
            }
            return valid;
        }

        private static bool IsConsoleOnly(string[] args)
        {
            return args.Length > 1 && args[1] == "--console-only";
        }

        private static void WriteToOutput(string[] args, string sourceFile, string[] results)
        {
            if (IsConsoleOnly(args))
            {
                results.ToList().ForEach(r => Console.WriteLine(r));
            }
            else
            {
                string outputFile = $"{Path.GetDirectoryName(sourceFile)}/{Path.GetFileNameWithoutExtension(sourceFile)}.asm";
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                File.WriteAllLines(outputFile, results);
                Console.WriteLine($"Results written to {outputFile}");
            }
        }

    }
}
