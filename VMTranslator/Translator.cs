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
        private const string eq = "// eq\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@EQ_{0}\nD;JEQ\nD=-1\n(EQ_{0})\nD=!D\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string lt = "// lt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_{0}\nD;JLT\nD=0\n@DONE_{0}\n0;JMP\n(YES_{0})\nD=-1\n(DONE_{0})\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string gt = "// gt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_{0}\nD;JGT\nD=0\n@DONE_{0}\n0;JMP\n(YES_{0})\nD=-1\n(DONE_{0})\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string label = "// {0}\n({1}${2})\n";
        private const string goTo = "// {0}\n@{1}${2}\n0;JMP\n";
        private const string ifGoto = "// {0}\n@SP\nM=M-1\nA=M\nD=M\n@{1}${2}\nD;JNE\n";

        private readonly string filename;

        public Translator(string filename)
        {
            this.filename = filename;
        }

        public string Translate(LineOfCode loc)
        {
            switch (loc.Instruction)
            {
                case InstructionType.Push:
                    string push = GetPushAssembly(loc);
                    return string.Format(push, loc.VmCode, GetRamForSegment(loc), loc.Value);
                case InstructionType.Pop:
                    string pop = GetPopAssembly(loc);
                    return string.Format(pop, loc.VmCode, GetRamForSegment(loc), loc.Value);
                case InstructionType.Add:
                    return add;
                case InstructionType.Sub:
                    return sub;
                case InstructionType.And:
                    return and;
                case InstructionType.Or:
                    return or;
                case InstructionType.Neg:
                    return neg;
                case InstructionType.Not:
                    return not;
                case InstructionType.Eq:
                    return string.Format(eq, loc.LineNumber);
                case InstructionType.Lt:
                    return string.Format(lt, loc.LineNumber);
                case InstructionType.Gt:
                    return string.Format(gt, loc.LineNumber);
                case InstructionType.Label:
                    return string.Format(label, loc.VmCode, filename, loc.Label);
                case InstructionType.Goto:
                    return string.Format(goTo, loc.VmCode, filename, loc.Label);
                case InstructionType.IfGoto:
                    return string.Format(ifGoto, loc.VmCode, filename, loc.Label);
                default:
                    return null;
            }
        }

        private static string GetPopAssembly(LineOfCode loc)
        {
            switch (loc.Segment)
            {
                case Segment.Pointer: return loc.Value == 0 ? popToPointer0 : popToPointer1;
                case Segment.Static: return popToStatic;
                case Segment.Temp: return popToTemp;
                default: return popToSegment;
            }
        }

        private static string GetPushAssembly(LineOfCode loc)
        {
            switch (loc.Segment)
            {
                case Segment.Constant: return pushConstant;
                case Segment.Pointer: return loc.Value == 0 ? pushFromPointer0 : pushFromPointer1;
                case Segment.Static: return pushFromStatic;
                case Segment.Temp: return pushFromTemp;
                default: return pushFromSegment;
            }
        }

        private string GetRamForSegment(LineOfCode loc)
        {
            switch (loc.Segment)
            {
                case Segment.Argument: return "ARG";
                case Segment.Local: return "LCL";
                case Segment.This: return "THIS";
                case Segment.That: return "THAT";
                case Segment.Static: return $"{filename}";
                default: return null;
            }
        }
    }
}
