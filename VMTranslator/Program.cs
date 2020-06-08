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

            if (!ValidateArguments(args))
                return;
            string sourceFileOrDirectory = args[0];
            bool isSingleFile = File.Exists(sourceFileOrDirectory);

            string[] sourceFiles = GetSourceFiles(sourceFileOrDirectory, isSingleFile);
            LineOfCode[] parsedLines = Parse(sourceFiles);
            if (!ValidateParsing(parsedLines))
                return;

            string[] results = Translate(parsedLines, isSingleFile);
            WriteToOutput(sourceFileOrDirectory, results);
        }

        private static void DisplayIntro()
        {
            Console.WriteLine("VM Translator");
            Console.WriteLine("-------------");
            Console.WriteLine("Usage (simple):      dotnet ./VMTranslator.dll [source-file]");
            Console.WriteLine("Usage (complex):     dotnet ./VMTranslator.dll [source-directory]");
            Console.WriteLine();
            Console.WriteLine("source-file:");
            Console.WriteLine("    Path to file containing VM code (must have an .vm file extension).");
            Console.WriteLine("    Results will be written to a file named after the source file, but with a .asm file extension. Any existing file with this name will be overwritten.");
            Console.WriteLine("");
            Console.WriteLine("source-directory:");
            Console.WriteLine("    Path to directory containing .vm files.");
            Console.WriteLine("    Results will be written to a file named after the directory, but with a .asm file extension. Any existing file with this name will be overwritten.");
            Console.WriteLine("    On startup, the assembly code will initialise SP and will call the function Sys.init.");
        }

        private static bool ValidateArguments(string[] args)
        {
            string error = null;
            if (args.Length == 0)
                error = "No source file or directory specified.";
            else
            {
                string sourceFileOrDirectory = args[0];
                bool isDirectory = Directory.Exists(sourceFileOrDirectory);
                bool isFile = File.Exists(sourceFileOrDirectory);
                if (!isDirectory && !isFile)
                    error = $"Path {sourceFileOrDirectory} does not exist.";
                if (isFile && Path.GetExtension(sourceFileOrDirectory) != ".vm")
                    error = $"Source file {sourceFileOrDirectory} does not have a .vm file extension.";
                if (isDirectory && !Directory.GetFiles(sourceFileOrDirectory).Any(f => Path.GetExtension(f) == ".vm"))
                    error = $"Source directory {sourceFileOrDirectory} does not contain any .vm files.";
            }
            if (error != null)
                Console.WriteLine(error);
            return error == null;
        }

        private static string[] GetSourceFiles(string sourceFileOrDirectory, bool isSingleFile)
        {
            if (isSingleFile)
                return new[] { sourceFileOrDirectory };
            return Directory.GetFiles(sourceFileOrDirectory).Where(f => Path.GetExtension(f) == ".vm").ToArray();
        }

        private static LineOfCode[] Parse(string[] sourceFiles)
        {
            var parsedLines = new List<LineOfCode>();
            foreach (string sourceFile in sourceFiles)
            {
                using (var stream = File.OpenText(sourceFile))
                {
                    var parser = new Parser(Path.GetFileNameWithoutExtension(sourceFile));
                    string line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        parsedLines.Add(parser.Parse(line));
                    }
                }
            }
            return parsedLines.Where(p => p != null).ToArray();
        }

        private static string[] Translate(LineOfCode[] parsedLines, bool isSingleFile)
        {
            var translator = new Translator();
            var parser = new Parser("xxx");
            var results = new List<string>();
            if (!isSingleFile)
            {
                results.Add(Translator.SysInit);
                results.Add(translator.Translate(parser.Parse("call Sys.init 0")));
            }
            results.AddRange(parsedLines.Select(p => translator.Translate(p)));
            return results.ToArray();
        }

        private static bool ValidateParsing(LineOfCode[] parsedLines)
        {
            foreach (LineOfCode parsedLine in parsedLines.Where(p => p.Error != null))
            {
                Console.WriteLine($"Line {parsedLine.LineNumber}: {parsedLine.Error}");
            }
            return parsedLines.All(p => p.Error == null);
        }

        private static void WriteToOutput(string sourceFileOrDirectory, string[] results)
        {
            string outputFile = GetOutputFileName(sourceFileOrDirectory);
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            File.WriteAllLines(outputFile, results);
            Console.WriteLine($"Results written to {outputFile}");
        }

        private static string GetOutputFileName(string sourceFileOrDirectory)
        {
            string dir = Path.GetDirectoryName(sourceFileOrDirectory);
            string fn = Path.GetFileNameWithoutExtension(sourceFileOrDirectory);
            char sep = Path.DirectorySeparatorChar;
            return File.Exists(sourceFileOrDirectory) ? $"{dir}{sep}{fn}.asm" : $"{dir}{sep}{fn}{sep}{fn}.asm";
        }

    }
}
