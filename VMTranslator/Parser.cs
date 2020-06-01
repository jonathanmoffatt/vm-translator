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

            LineOfCode result = new LineOfCode { VmCode = line };

            string[] fragments = line.Split(' ');
            if (!Enum.TryParse(fragments[0], true, out Command c))
            {
                result.Error = $"Command '{fragments[0]}' not recognised";
                return result;
            }
            result.Command = c;

            if (fragments.Length > 1)
            {
                if (!Enum.TryParse(fragments[1], true, out Segment s))
                {
                    result.Error = $"Segment '{fragments[1]}' not recognised";
                    return result;
                }
                result.Segment = s;
            }
            if (fragments.Length > 2)
            {
                if (!int.TryParse(fragments[2], out int v))
                {
                    result.Error = $"Value '{fragments[2]}' is not a valid integer";
                    return result;
                }
                result.Value = v;
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
                else if (result.Segment == Segment.Pointer && result.Value > 1)
                    result.Error = "pointer value can only be 0 or 1";
            }
            return result;
        }
    }
}
