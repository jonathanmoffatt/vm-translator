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
            WriteToOutput(sourceFile, results);
        }

        private static void DisplayIntro()
        {
            Console.WriteLine("VM Translator");
            Console.WriteLine("-------------");
            Console.WriteLine("Usage: dotnet ./VMTranslator.dll [source-file] [--console-only]");
            Console.WriteLine();
            Console.WriteLine("source-file:");
            Console.WriteLine("    Path to file containing VM code code (must have an .vm file extension)");
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
            return parsedLines.Where(p => p != null).ToArray();
        }

        private static string[] Translate(string filename, LineOfCode[] parsedLines)
        {
            var translator = new Translator(filename);
            return parsedLines.Select(p => translator.Translate(p)).ToArray();
        }

        private static bool CheckForParsingErrors(LineOfCode[] parsedLines)
        {
            foreach (LineOfCode parsedLine in parsedLines.Where(p => p.Error != null))
            {
                Console.WriteLine($"Line {parsedLine.LineNumber}: {parsedLine.Error}");
            }
            return parsedLines.All(p => p.Error == null);
        }

        private static void WriteToOutput(string sourceFile, string[] results)
        {
            string outputFile = $"{Path.GetDirectoryName(sourceFile)}/{Path.GetFileNameWithoutExtension(sourceFile)}.asm";
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            File.WriteAllLines(outputFile, results);
            Console.WriteLine($"Results written to {outputFile}");
        }

    }
}
