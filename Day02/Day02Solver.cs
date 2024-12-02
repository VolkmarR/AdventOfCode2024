namespace AdventOfCode.Day02;

public class Day02Tests
{
    private readonly ITestOutputHelper _output;
    public Day02Tests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Step1WithExample() => new Day02Solver().ExecuteExample1("2");

    [Fact]
    public void Step2WithExample() => new Day02Solver().ExecuteExample2("4");

    [Fact]
    public void Step1WithPuzzleInput() => _output.WriteLine(new Day02Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => _output.WriteLine(new Day02Solver().ExecutePuzzle2());
}

public class Day02Solver : SolverBase
{
    List<int[]> _data;

    protected override void Parse(List<string> data)
    {
        _data = new();
        foreach (var line in data)
        {
            var levels = line.Split(" ");
            _data.Add(levels.Select(int.Parse).ToArray());
        }
    }

    private bool IsIncrease(int lever1, int lever2) => lever1 < lever2;

    private bool IsSave(int[] levels)
    {
        var last = levels[0];
        var isIncreaseFirst = IsIncrease(last, levels[1]);

        foreach (var level in levels.Skip(1))
        {
            var diff = Math.Abs(level - last);
            if (isIncreaseFirst != IsIncrease(last, level) || diff < 1 || diff > 3)
                return false;
            last = level;
        }

        return true;
    }

    private bool IsSaveWithRemove(int[] levels)
    {
        if (IsSave(levels))
            return true;

        for (var i = 0; i < levels.Length; i++)
        {
            var newLevels = levels.Take(i).Concat(levels.Skip(i + 1)).ToArray();
            if (IsSave(newLevels))
                return true;
        }

        return false;
    }


    protected override object Solve1()
    {
        return _data.Count(IsSave);
    }

    protected override object Solve2()
    {
        return _data.Count(IsSaveWithRemove);
    }
}