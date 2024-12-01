namespace AdventOfCode.Day01;

public class Day01Tests
{
    private readonly ITestOutputHelper _output;
    public Day01Tests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Step1WithExample() => new Day01Solver().ExecuteExample1("11");

    [Fact]
    public void Step2WithExample() => new Day01Solver().ExecuteExample2("31");

    [Fact]
    public void Step1WithPuzzleInput() => _output.WriteLine(new Day01Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => _output.WriteLine(new Day01Solver().ExecutePuzzle2());
}

public class Day01Solver : SolverBase
{
    List<(int Left, int Right)> _data;

    protected override void Parse(List<string> data)
    {
        _data = data.Select(x => x.Split("   ")).Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToList();
    }

    int Distance(int left, int right)
    {
        if (left < right)
            return right - left;
        return left - right;
    }

    protected override object Solve1()
    {
        var left = _data.Select(x => x).OrderBy(x => x.Left).ToList();
        var right = _data.Select(x => x).OrderBy(x => x.Right).ToList();

        return left.Zip(right, (l, r) => Distance(l.Left, r.Right)).Sum();
    }

    protected override object Solve2()
    {
        var countByNumber = _data.GroupBy(x => x.Right).ToDictionary(x => x.Key, x => x.Count());
        return _data.Sum(x => countByNumber.GetValueOrDefault(x.Left, 0) * x.Left);
    }
}