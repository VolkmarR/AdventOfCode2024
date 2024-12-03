using System.Text.RegularExpressions;

namespace AdventOfCode.Day03;

public class Day03Tests
{
    private readonly ITestOutputHelper _output;
    public Day03Tests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Step1WithExample() => new Day03Solver().ExecuteExample1("161");

    [Fact]
    public void Step2WithExample() => new Day03Solver().ExecuteExample2("48");

    [Fact]
    public void Step1WithPuzzleInput() => _output.WriteLine(new Day03Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => _output.WriteLine(new Day03Solver().ExecutePuzzle2());
}

public class Day03Solver : SolverBase
{
    List<string> _data;

    protected override void Parse(List<string> data)
    {
        _data = data;
    }

    protected override object Solve1()
    {
        var regex = new Regex(@"mul\((?'x'\d{1,3}),(?'y'\d{1,3})\)");
        var result = 0l;
        foreach (var data in _data)
        {
            var matches = regex.Matches(data);
            result += matches.Sum(match => long.Parse(match.Groups["x"].Value) * long.Parse(match.Groups["y"].Value));
        }

        return result;
    }

    protected override object Solve2()
    {
        var regex = new Regex(@"mul\((?'x'\d{1,3}),(?'y'\d{1,3})\)|do\(\)|don't\(\)");
        var result = 0L;
        var enabled = true;
        foreach (var data in _data)
        {
            var matches = regex.Matches(data);
            foreach (Match match in matches)
            {
                if (match.Value == "do()")
                    enabled = true;
                else if (match.Value == "don't()")
                    enabled = false;
                else if (enabled)
                    result += long.Parse(match.Groups["x"].Value) * long.Parse(match.Groups["y"].Value);
            }
        }

        return result;
    }
}