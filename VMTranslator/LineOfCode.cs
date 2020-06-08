namespace VMTranslator
{
    public class LineOfCode
    {
        public string VmCode { get; set; }
        public int LineNumber { get; set; }
        public InstructionType? Instruction { get; set; }
        public Segment? Segment { get; set; }
        public int? Value { get; set; }
        public string Label { get; set; }
        public string FunctionName { get; set; }
        public string FileName { get; set; }
        public string Error { get; set; }

        public InstructionCategory Category
        {
            get
            {
                switch(Instruction)
                {
                    case null:
                        return InstructionCategory.NotRecognised;
                    case InstructionType.Push:
                    case InstructionType.Pop:
                        return InstructionCategory.Stack;
                    case InstructionType.Goto:
                    case InstructionType.IfGoto:
                    case InstructionType.Label:
                        return InstructionCategory.Branching;
                    case InstructionType.Eq:
                    case InstructionType.Gt:
                    case InstructionType.Lt:
                        return InstructionCategory.Logical;
                    case InstructionType.Function:
                    case InstructionType.Call:
                    case InstructionType.Return:
                        return InstructionCategory.Function;
                    default:
                        return InstructionCategory.Arithmetic;
                }
            }
        }

        public override string ToString()
        {
            return VmCode;
        }

    }
}
