using System.Text.RegularExpressions;

namespace AdventOfCode.Day24;

public class Day24Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day24Solver().ExecuteExample1("2024");

    [Fact]
    public void Step2WithExample() => new Day24Solver().ExecuteExample2("??");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day24Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day24Solver().ExecutePuzzle2());
}

public class Day24Solver : SolverBase
{
    Dictionary<string, bool> _values;
    List<(string input1, string input2, string operation, string output)> _operations;

    protected override void Parse(List<string> data)
    {
        var opRegEx = new Regex(@"(?'input1'\S+) (?'operation'OR|AND|XOR) (?'input2'\S+) -> (?'output'\S+)");

        _values = data
            .TakeWhile(q => q != "")
            .Select(q => q.Split(": "))
            .ToDictionary(q => q[0], q => q[1].ToInt() == 1);
        _operations = data
            .SkipWhile(q => q != "")
            .Skip(1)
            .Select(ParseOp).ToList();

        (string input1, string input2, string operation, string output) ParseOp(string input)
        {
            var match = opRegEx.Match(input);
            if (!match.Success)
                throw new Exception("Invalid operation");
            return (match.Groups["input1"].Value, match.Groups["input2"].Value,
                match.Groups["operation"].Value, match.Groups["output"].Value);
        }
    }

    void Execute()
    {
        bool changed;
        do
        {
            changed = false;
            foreach (var (input1, input2, operation, output) in _operations
                         .Where(q => !_values.ContainsKey(q.output)
                                     && _values.ContainsKey(q.input1)
                                     && _values.ContainsKey(q.input2)).ToArray())
            {
                changed = true;
                var value1 = _values[input1];
                var value2 = _values[input2];
                if (operation == "OR")
                    _values[output] = value1 || value2;
                else if (operation == "AND")
                    _values[output] = value1 && value2;
                else if (operation == "XOR")
                    _values[output] = value1 ^ value2;
            }
        } while (changed);
    }

    long GetZValue()
    {
        var resultString = string.Join("", _values
            .Where(q => q.Key.StartsWith("z"))
            .OrderByDescending(q => q.Key)
            .Select(q => q.Value ? "1" : "0"));
        return Convert.ToInt64(resultString, 2);
    }

    protected override object Solve1()
    {
        Execute();
        return GetZValue();
    }

    protected override object Solve2()
    {
        throw new Exception("Solver error");
    }
}