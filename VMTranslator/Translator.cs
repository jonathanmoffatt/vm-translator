using System;
namespace VMTranslator
{
    public class Translator
    {
        private const string pushFromSegment = "// {0}\n@{1}\nD=M\n@{2}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromTemp = "// {0}\n@5\nD=A\n@{2}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushConstant = "// {0}\n@{2}\nD=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromPointer0 = "// {0}\n@THIS\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromPointer1 = "// {0}\n@THAT\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromStatic = "// {0}\n@{1}.{2}\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string popToSegment = "// {0}\n@{1}\nD=M\n@{2}\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";
        private const string popToTemp = "// {0}\n@5\nD=A\n@{2}\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";
        private const string popToPointer0 = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@THIS\nM=D\n";
        private const string popToPointer1 = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@THAT\nM=D\n";
        private const string popToStatic = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@{1}.{2}\nM=D\n";
        private const string add = "// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D+M\n@SP\nM=M+1\n";
        private const string sub = "// sub\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=M-D\n@SP\nM=M+1\n";
        private const string and = "// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D&M\n@SP\nM=M+1\n";
        private const string or = "// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D|M\n@SP\nM=M+1\n";
        private const string neg = "// neg\n@SP\nM=M-1\nA=M\nD=M\nM=-D\n@SP\nM=M+1\n";
        private const string not = "// not\n@SP\nM=M-1\nA=M\nD=M\nM=!D\n@SP\nM=M+1\n";
        private const string eq = "// eq\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@SETRESULT\nD;JEQ\nD=-1\n(SETRESULT)\nD=!D\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string lt = "// lt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES\nD;JLT\nD=0\n@RETURN\n0;JMP\n(YES)\nD=-1\n(RETURN)\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string gt = "// gt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES\nD;JGT\nD=0\n@RETURN\n0;JMP\n(YES)\nD=-1\n(RETURN)\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";

        private readonly string filename;

        public Translator(string filename)
        {
            this.filename = filename;
        }

        public string Translate(LineOfCode lineOfCode)
        {
            switch (lineOfCode.Command)
            {
                case Command.Push:
                    string push = lineOfCode.Segment switch
                    {
                        Segment.Constant => pushConstant,
                        Segment.Pointer => lineOfCode.Value == 0 ? pushFromPointer0 : pushFromPointer1,
                        Segment.Static => pushFromStatic,
                        Segment.Temp => pushFromTemp,
                        _ => pushFromSegment
                    };
                    return string.Format(push, lineOfCode.VmCode, GetRamForSegment(lineOfCode), lineOfCode.Value);
                case Command.Pop:
                    string pop = lineOfCode.Segment switch
                    {
                        Segment.Pointer => lineOfCode.Value == 0 ? popToPointer0 : popToPointer1,
                        Segment.Static => popToStatic,
                        Segment.Temp => popToTemp,
                        _ => popToSegment
                    };
                    return string.Format(pop, lineOfCode.VmCode, GetRamForSegment(lineOfCode), lineOfCode.Value);
                case Command.Add:
                    return add;
                case Command.Sub:
                    return sub;
                case Command.And:
                    return and;
                case Command.Or:
                    return or;
                case Command.Neg:
                    return neg;
                case Command.Not:
                    return not;
                case Command.Eq:
                    return eq;
                case Command.Lt:
                    return lt;
                case Command.Gt:
                    return gt;
                default:
                    return null;
            }
        }

        private string GetRamForSegment(LineOfCode lineOfCode)
        {
            return lineOfCode.Segment switch
            {
                Segment.Argument => "ARG",
                Segment.Local => "LCL",
                Segment.This => "THIS",
                Segment.That => "THAT",
                Segment.Static => $"{filename}",
                _ => null,
            };
        }
    }
}
