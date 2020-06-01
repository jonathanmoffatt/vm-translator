using System;
namespace VMTranslator
{
    public class Translator
    {
        private const string push = "// {0}\n@{1}\nD=M\n@{2}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushConstant = "// {0}\n@{2}\nD=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushPointer0 = "// {0}\n@THIS\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushPointer1 = "// {0}\n@THAT\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pop = "// {0}\n@{1}\nD=M\n@{2}\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";
        private const string popPointer0 = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@THIS\nM=D\n";
        private const string popPointer1 = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@THAT\nM=D\n";

        public string Translate(LineOfCode parsedLine)
        {
            string asm;
            if (parsedLine.Command == Command.Push)
            {
                asm = parsedLine.Segment switch
                {
                    Segment.Constant => pushConstant,
                    Segment.Pointer => parsedLine.Value == 0 ? pushPointer0 : pushPointer1,
                    _ => push
                };
            }
            else
            {
                asm = parsedLine.Segment switch
                {
                    Segment.Pointer =>  parsedLine.Value == 0 ? popPointer0 : popPointer1,
                    _ => pop
                };
            }
            return string.Format(asm, parsedLine.VmCode, GetRamForSegment(parsedLine.Segment), parsedLine.Value);
        }

        private string GetRamForSegment(Segment? segment)
        {
            return segment switch
            {
                Segment.Argument => "ARG",
                Segment.Local => "LCL",
                Segment.This => "THIS",
                Segment.That => "THAT",
                Segment.Temp => "5",
                _ => null,
            };
        }
    }
}
