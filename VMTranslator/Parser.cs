using System;
namespace VMTranslator
{
    public class Parser
    {
        private int lineNumber;
        private string functionName;
        private string fileName;

        public Parser(string fileName)
        {
            lineNumber = 0;
            functionName = null;
            this.fileName = fileName;
        }

        public LineOfCode Parse(string line)
        {
            lineNumber++;
            line = line.Trim();
            if (IsCommentOrWhitespace(line))
                return null;

            string[] fragments = line.Split(' ');
            LineOfCode loc = ParseInstruction(fragments, line);

            switch (loc.Category)
            {
                case InstructionCategory.Branching:
                    return ParseBranchingInstruction(loc, fragments);
                case InstructionCategory.Stack:
                    return ParseStackInstruction(loc, fragments);
                case InstructionCategory.Function:
                    return ParseFunctionInstruction(loc, fragments);
                case InstructionCategory.Arithmetic:
                case InstructionCategory.Logical:
                    return ParseArithmeticOrLogicalInstruction(loc, fragments);
                default:
                    return ParseUnrecognisedInstruction(loc, fragments);
            }
        }

        private LineOfCode ParseFunctionInstruction(LineOfCode loc, string[] fragments)
        {
            if (loc.Instruction == InstructionType.Function || loc.Instruction == InstructionType.Call)
            {
                if (fragments.Length < 2)
                    loc.Error = "Function must have a name";
                else
                {
                    loc.FunctionName = fragments[1];
                    if (fragments.Length < 3)
                        loc.Error = loc.Instruction == InstructionType.Function ? "Function must specify the number of local variables" : "Call must specify the number of arguments";
                    else
                        SetValue(loc, fragments[2]);
                }
            }
            if (loc.Instruction == InstructionType.Return && fragments.Length > 1)
                loc.Error = "Return cannot have a name";
            if (loc.Instruction == InstructionType.Function)
                functionName = loc.FunctionName;
            if (loc.Instruction == InstructionType.Return)
                loc.FunctionName = functionName;
            return loc;
        }

        private static LineOfCode ParseUnrecognisedInstruction(LineOfCode loc, string[] fragments)
        {
            loc.Error = $"Command '{fragments[0]}' not recognised";
            return loc;
        }

        private static LineOfCode ParseArithmeticOrLogicalInstruction(LineOfCode loc, string[] fragments)
        {
            if (fragments.Length > 1)
                loc.Error = "Arithmetic instructions cannot include any additional arguments";
            return loc;
        }

        private static LineOfCode ParseStackInstruction(LineOfCode loc, string[] fragments)
        {
            if (fragments.Length < 2)
                loc.Error = "Stack instructions must include a segment";
            else if (fragments.Length < 3)
                loc.Error = "Stack instructions must include a value";
            else
            {
                if (!Enum.TryParse(fragments[1], true, out Segment s))
                    loc.Error = $"Segment '{fragments[1]}' not recognised";
                else
                {
                    loc.Segment = s;
                    if (SetValue(loc, fragments[2]))
                    {
                        if (loc.Instruction == InstructionType.Pop && loc.Segment == Segment.Constant)
                            loc.Error = "pop cannot be performed on a constant";
                        else if (loc.Segment == Segment.Pointer && loc.Value > 1)
                            loc.Error = "pointer value can only be 0 or 1";
                    }
                }
            }
            return loc;
        }

        private static bool SetValue(LineOfCode loc, string s)
        {
            if (!int.TryParse(s, out int v))
            {
                loc.Error = $"Value '{s}' is not a valid integer";
                return false;
            }
            loc.Value = v;
            return true;
        }

        private static LineOfCode ParseBranchingInstruction(LineOfCode loc, string[] fragments)
        {
            if (fragments.Length < 2)
                loc.Error = "Branching instructions must have a label";
            else
                loc.Label = fragments[1];
            return loc;
        }

        private static bool IsCommentOrWhitespace(string line)
        {
            return line == "" || line.StartsWith("//");
        }

        private LineOfCode ParseInstruction(string[] fragments, string line)
        {
            LineOfCode loc = new LineOfCode { VmCode = line, LineNumber = lineNumber, FileName = fileName };
            if (Enum.TryParse(fragments[0].Replace("-", ""), true, out InstructionType c))
                loc.Instruction = c;
            return loc;
        }
    }
}
