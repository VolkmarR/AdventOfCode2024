namespace AdventOfCode.Day11;

public class Day11Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day11Solver().ExecuteExample1("55312");

    [Fact]
    public void Step2WithExample() => new Day11Solver().ExecuteExample2("??");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day11Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day11Solver().ExecutePuzzle2());
}

public class Day11Solver : SolverBase
{
    long[] _data;
    Dictionary<int, Dictionary<int, long>> _cache = new();

    protected override void Parse(List<string> data)
    {
        _data = data[0].Split(" ").Select(long.Parse).ToArray();
        for (var i = 0; i <= 9; i++)
            _cache.Add(i, new());
    }

    private static bool Split(long value, out long left, out long right)
    {
        left = 0;
        right = 0;

        if (value <= 9)
            return false;

        var work = value;
        var length = 0;
        while (work > 0)
        {
            length++;
            work /= 10;
        }

        if (length % 2 != 0)
            return false;

        var splitter = 1;
        for (var i = 0; i < length / 2; i++)
            splitter *= 10;

        left = value / splitter;
        right = value % splitter;
        return true;
    }

    private long BlinkFor(long item, int times)
    {
        if (times == 0)
            return 1;

        times--;
        if (item < 10)
        {
            var cache = _cache[(int)item];
            if (cache.TryGetValue(times, out var value))
                return value;

            var count = BlinkFor(item == 0 ? 1 : item * 2024, times);

            cache.Add(times, count);
            return count;
        }

        if (Split(item, out var left, out var right))
            return BlinkFor(left, times) + BlinkFor(right, times);

        return BlinkFor(item * 2024, times);
    }

    private long BlinkFor(int times)
    {
        var sum = 0L;
        foreach (var item in _data)
            sum += BlinkFor(item, times);

        return sum;
    }

    protected override object Solve1()
        => BlinkFor(25);

    protected override object Solve2()
        => BlinkFor(75);
}