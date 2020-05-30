using System;
namespace VMTranslator
{
    public class Parser
    {
        public LineOfCode Parse(string line)
        {
            line = line.Trim();
            if (line == "" || line.StartsWith("//"))
                return null;

            string[] fragments = line.Split(' ');
            if (!Enum.TryParse(fragments[0], true, out Command c))
                return new LineOfCode { Error = $"Command '{fragments[0]}' not recognised" };

            LineOfCode result = new LineOfCode { Command = c };
            if (fragments.Length > 1)
            {
                if (!Enum.TryParse(fragments[1], true, out Segment s))
                    return new LineOfCode { Error = $"Segment '{fragments[1]}' not recognised" };
                result.Segment = s;
            }
            if (fragments.Length > 2)
            {
                if (!int.TryParse(fragments[2], out int v))
                    return new LineOfCode { Error = $"Value '{fragments[2]}' is not a valid integer" };
                result.Value = int.Parse(fragments[2]);
            }
            bool arithmetic = result.Command != Command.Pop && result.Command != Command.Push;
            if (arithmetic)
            {
                if (result.Segment != null)
                    result.Error = "Arithmetic operation cannot include any additional arguments";
            }
            else
            {
                if (result.Segment == null)
                    result.Error = "Stack operations must include a segment";
                else if (result.Value == null)
                    result.Error = "Stack operations must include a value";
                else if (result.Command == Command.Pop && result.Segment == Segment.Constant)
                    result.Error = "pop is not a valid operation on a constant";
            }
            return result;
        }
    }
}
