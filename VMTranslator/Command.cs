namespace VMTranslator
{
    public enum InstructionType
    {
        Add,
        Neg,
        Eq,
        Or,
        Sub,
        Gt,
        Lt,
        And,
        Not,
        Push,
        Pop,
        Goto,
        IfGoto,
        Label,
        NotRecognised
    }

    public enum InstructionCategory
    {
        Stack,
        Branching,
        Function,
        Arithmetic,
        Logical,
        NotRecognised
    }

}
