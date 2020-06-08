using System;

namespace VMTranslator
{
    public class Translator
    {
        private const string pushFromSegment = "// {vmcode}\n@{segment}\nD=M\n@{value}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromTemp = "// {vmcode}\n@5\nD=A\n@{value}\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushConstant = "// {vmcode}\n@{value}\nD=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromPointer0 = "// {vmcode}\n@THIS\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromPointer1 = "// {vmcode}\n@THAT\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string pushFromStatic = "// {vmcode}\n@{segment}.{value}\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string popToSegment = "// {vmcode}\n@{segment}\nD=M\n@{value}\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";
        private const string popToTemp = "// {vmcode}\n@5\nD=A\n@{value}\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n";
        private const string popToPointer0 = "// {vmcode}\n@SP\nM=M-1\nA=M\nD=M\n@THIS\nM=D\n";
        private const string popToPointer1 = "// {vmcode}\n@SP\nM=M-1\nA=M\nD=M\n@THAT\nM=D\n";
        private const string popToStatic = "// {vmcode}\n@SP\nM=M-1\nA=M\nD=M\n@{segment}.{value}\nM=D\n";
        private const string add = "// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D+M\n@SP\nM=M+1\n";
        private const string sub = "// sub\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=M-D\n@SP\nM=M+1\n";
        private const string and = "// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D&M\n@SP\nM=M+1\n";
        private const string or = "// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D|M\n@SP\nM=M+1\n";
        private const string neg = "// neg\n@SP\nM=M-1\nA=M\nD=M\nM=-D\n@SP\nM=M+1\n";
        private const string not = "// not\n@SP\nM=M-1\nA=M\nD=M\nM=!D\n@SP\nM=M+1\n";
        private const string eq = "// eq\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@EQ_{linenumber}\nD;JEQ\nD=-1\n(EQ_{linenumber})\nD=!D\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string lt = "// lt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_{linenumber}\nD;JLT\nD=0\n@DONE_{linenumber}\n0;JMP\n(YES_{linenumber})\nD=-1\n(DONE_{linenumber})\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string gt = "// gt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_{linenumber}\nD;JGT\nD=0\n@DONE_{linenumber}\n0;JMP\n(YES_{linenumber})\nD=-1\n(DONE_{linenumber})\n@SP\nA=M\nM=D\n@SP\nM=M+1\n";
        private const string label = "// {vmcode}\n({filename}${label})\n";
        private const string goTo = "// {vmcode}\n@{filename}${label}\n0;JMP\n";
        private const string ifGoto = "// {vmcode}\n@SP\nM=M-1\nA=M\nD=M\n@{filename}${label}\nD;JNE\n";
        private const string functionNoArgs = "// {vmcode}\n({functionname})\n";
        private const string function1Arg = "// {vmcode}\n({functionname})\n@SP\nA=M\nM=0\n@SP\nM=M+1\n";
        private const string functionMultipleArgs = "// {vmcode}\n({functionname})\n@{value}\nD=A\n({functionname}.init)\n@SP\nA=M\nM=0\n@SP\nM=M+1\nD=D-1\n@{functionname}.init\nD;JNE\n";
        private const string functionReturn = "// {vmcode}\n@LCL\nD=M\n@{functionname}.endFrame\nM=D \n@5\nD=A\n@{functionname}.endFrame\nA=M\nA=A-D\nD=M\n@{functionname}.retAddr\nM=D\n@SP\nM=M-1\n@SP\nA=M\nD=M\n@ARG\nA=M\nM=D\n@ARG\nD=M+1\n@SP\nM=D\n@{functionname}.endFrame\nA=M-1\nD=M\n@THAT\nM=D\n@2\nD=A\n@{functionname}.endFrame\nA=M-D\nD=M\n@THIS\nM=D\n@3\nD=A\n@{functionname}.endFrame\nA=M-D\nD=M\n@ARG\nM=D\n@4\nD=A\n@{functionname}.endFrame\nA=M-D\nD=M\n@LCL\nM=D\n@{functionname}.retAddr\nA=M\n0;JMP\n";
        private const string call = "// {vmcode}\n@{functionname}.return.{linenumber}\nD=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n@LCL\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n@ARG\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n@THIS\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n@LCL\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n@5\nD=A\n@SP\nA=M\nD=A-D\n@{value}\nD=D-A\n@ARG\nM=D\n@SP\nD=M\n@LCL\nM=D\n@{functionname}\n0;JMP\n({functionname}.return.{linenumber})\n";

        public const string SysInit = "@256\nD=A\n@SP\nM=D\n@Sys.init\n0;JMP\n";

        public string Translate(LineOfCode loc)
        {
            switch (loc.Instruction)
            {
                case InstructionType.Push:
                    string push = GetPushTemplate(loc);
                    return Merge(push, loc);
                case InstructionType.Pop:
                    string pop = GetPopTemplate(loc);
                    return Merge(pop, loc);
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
                    return Merge(eq, loc);
                case InstructionType.Lt:
                    return Merge(lt, loc);
                case InstructionType.Gt:
                    return Merge(gt, loc);
                case InstructionType.Label:
                    return Merge(label, loc);
                case InstructionType.Goto:
                    return Merge(goTo, loc);
                case InstructionType.IfGoto:
                    return Merge(ifGoto, loc);
                case InstructionType.Function:
                    if (loc.Value == 0)
                        return Merge(functionNoArgs, loc);
                    if (loc.Value == 1)
                        return Merge(function1Arg, loc);
                    return Merge(functionMultipleArgs, loc);
                case InstructionType.Return:
                    return Merge(functionReturn, loc);
                case InstructionType.Call:
                    return Merge(call, loc);
                default:
                    return null;
            }
        }

        public string GenerateSysInit()
        {
            return null;
        }

        private string Merge(string template, LineOfCode loc)
        {
            return template
                .Replace("{vmcode}", loc.VmCode, StringComparison.CurrentCultureIgnoreCase)
                .Replace("{functionname}", loc.FunctionName, StringComparison.CurrentCultureIgnoreCase)
                .Replace("{label}", loc.Label, StringComparison.CurrentCultureIgnoreCase)
                .Replace("{linenumber}", loc.LineNumber.ToString())
                .Replace("{value}", loc.Value.ToString(), StringComparison.CurrentCultureIgnoreCase)
                .Replace("{segment}", GetRamForSegment(loc), StringComparison.CurrentCultureIgnoreCase)
                .Replace("{filename}", loc.FileName);
        }

        private static string GetPopTemplate(LineOfCode loc)
        {
            switch (loc.Segment)
            {
                case Segment.Pointer: return loc.Value == 0 ? popToPointer0 : popToPointer1;
                case Segment.Static: return popToStatic;
                case Segment.Temp: return popToTemp;
                default: return popToSegment;
            }
        }

        private static string GetPushTemplate(LineOfCode loc)
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
                case Segment.Static: return $"{loc.FileName}";
                default: return null;
            }
        }
    }
}
