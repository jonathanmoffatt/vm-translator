namespace VMTranslator
{
    public class LineOfCode
    {
        public string VmCode { get; set; }
        public int LineNumber { get; set; }
        public Command Command { get; set; }
        public Segment? Segment { get; set; }
        public int? Value { get; set; }
        public string Label { get; set; }
        public string Error { get; set; }

        public CommandCategory Category
        {
            get
            {
                switch(Command)
                {
                    case Command.Push:
                    case Command.Pop:
                        return CommandCategory.Stack;
                    case Command.Goto:
                    case Command.IfGoto:
                    case Command.Label:
                        return CommandCategory.Branching;
                    case Command.Eq:
                    case Command.Gt:
                    case Command.Lt:
                        return CommandCategory.Logical;
                    default:
                        return CommandCategory.Arithmetic;
                }
            }
        }

    }
}
