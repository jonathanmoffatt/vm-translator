using System;
namespace VMTranslator
{
    public class Parser
    {
        private int lineNumber;

        public Parser()
        {
            lineNumber = 0;
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
                case InstructionCategory.NotRecognised:
                    return ParseUnrecognisedInstruction(loc, fragments);
                case InstructionCategory.Branching:
                    return ParseBranchingInstruction(loc, fragments);
                case InstructionCategory.Stack:
                    return ParseStackInstruction(loc, fragments);
                case InstructionCategory.Arithmetic:
                case InstructionCategory.Logical:
                    return ParseArithmeticOrLogicalInstruction(loc, fragments);
            }
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
                loc.Error = "Arithmetic operation cannot include any additional arguments";
            return loc;
        }

        private static LineOfCode ParseStackInstruction(LineOfCode loc, string[] fragments)
        {
            if (fragments.Length < 2)
                loc.Error = "Stack operations must include a segment";
            else if (fragments.Length < 3)
                loc.Error = "Stack operations must include a value";
            else
            {
                if (!Enum.TryParse(fragments[1], true, out Segment s))
                    loc.Error = $"Segment '{fragments[1]}' not recognised";
                else
                {
                    loc.Segment = s;
                    if (!int.TryParse(fragments[2], out int v))
                        loc.Error = $"Value '{fragments[2]}' is not a valid integer";
                    else
                    {
                        loc.Value = v;
                        if (loc.Instruction == InstructionType.Pop && loc.Segment == Segment.Constant)
                            loc.Error = "pop is not a valid operation on a constant";
                        else if (loc.Segment == Segment.Pointer && loc.Value > 1)
                            loc.Error = "pointer value can only be 0 or 1";
                    }
                }
            }
            return loc;
        }

        private static LineOfCode ParseBranchingInstruction(LineOfCode loc, string[] fragments)
        {
            if (fragments.Length < 2)
                loc.Error = "Branching commands must have a label";
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
            LineOfCode loc = new LineOfCode { VmCode = line, LineNumber = lineNumber };
            if (Enum.TryParse(fragments[0].Replace("-", ""), true, out InstructionType c))
                loc.Instruction = c;
            return loc;
        }
    }
}
