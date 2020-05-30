namespace VMTranslator
{
    public class LineOfCode
    {
        public Command Command { get; set; }
        public Segment? Segment { get; set; }
        public int? Value { get; set; }
        public string Error { get; set; }
    }
}
