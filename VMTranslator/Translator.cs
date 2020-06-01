using System;
namespace VMTranslator
{
    public class Translator
    {
        private const string push = "// {0}\n@{1}\nD=M\n@{2}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushConstant = "// {0}\n@{2}\nD=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushPointer0 = "// {0}\n@THIS\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushPointer1 = "// {0}\n@THAT\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushStatic = "// {0}\n@{1}\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pop = "// {0}\n@{1}\nD=M\n@{2}\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";
        private const string popPointer0 = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@THIS\nM=D\n";
        private const string popPointer1 = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@THAT\nM=D\n";
        private const string popStatic = "// {0}\n@{1}\nD=M\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";

        private readonly string filename;

        public Translator(string filename)
        {
            this.filename = filename;
        }

        public string Translate(LineOfCode lineOfCode)
        {
            string asm;
            if (lineOfCode.Command == Command.Push)
            {
                asm = lineOfCode.Segment switch
                {
                    Segment.Constant => pushConstant,
                    Segment.Pointer => lineOfCode.Value == 0 ? pushPointer0 : pushPointer1,
                    Segment.Static => pushStatic,
                    _ => push
                };
            }
            else
            {
                asm = lineOfCode.Segment switch
                {
                    Segment.Pointer =>  lineOfCode.Value == 0 ? popPointer0 : popPointer1,
                    Segment.Static => popStatic,
                    _ => pop
                };
            }
            return string.Format(asm, lineOfCode.VmCode, GetRamForSegment(lineOfCode), lineOfCode.Value);
        }

        private string GetRamForSegment(LineOfCode lineOfCode)
        {
            return lineOfCode.Segment switch
            {
                Segment.Argument => "ARG",
                Segment.Local => "LCL",
                Segment.This => "THIS",
                Segment.That => "THAT",
                Segment.Temp => "5",
                Segment.Static => $"{filename}.{lineOfCode.Value}",
                _ => null,
            };
        }
    }
}
