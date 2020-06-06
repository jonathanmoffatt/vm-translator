namespace VMTranslator
{
    public enum Command
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
        Label
    }

    public enum CommandCategory
    {
        Stack,
        Branching,
        Function,
        Arithmetic,
        Logical
    }

}
