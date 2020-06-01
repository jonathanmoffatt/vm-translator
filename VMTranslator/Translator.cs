using System;
namespace VMTranslator
{
    public class Translator
    {
        private const string push =
@"// {0}
@{1}
D=M
@{2}
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1
";
        private const string pushConstant =
@"// {0}
@{2}
D=A
@SP
A=M
M=D
@SP
M=M+1
";
        private const string pushPointer0 =
@"// {0}
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1
";
        private const string pushPointer1 =
@"// {0}
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1
";
        private const string popPointer0 =
@"// {0}
@SP
M=M-1
A=M
D=M
@THIS
M=D";

        public string Translate(LineOfCode parsedLine)
        {
            string asm = parsedLine.Segment switch
            {
                Segment.Constant => pushConstant,
                Segment.Pointer => parsedLine.Value == 0 ? pushPointer0 : pushPointer1,
                _ => push
            };
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
