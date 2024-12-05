namespace AdventOfCode.Day05;

public class Day05Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day05Solver().ExecuteExample1("143");

    [Fact]
    public void Step2WithExample() => new Day05Solver().ExecuteExample2("123");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day05Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day05Solver().ExecutePuzzle2());
}

public class Day05Solver : SolverBase
{
    List<(int X, int Y)> _rules;
    List<int[]> _data;

    protected override void Parse(List<string> data)
    {
        var separator = data.IndexOf("");
        _rules = data
            .Take(separator)
            .Select(x => x.Split("|"))
            .Select(x => (int.Parse(x[0]), int.Parse(x[1]))).ToList();

        _data = data
            .Skip(separator + 1)
            .Select(x => x.Split(",").Select(int.Parse).ToArray()).ToList();
    }

    private bool IsValidOrder(int[] data)
    {
        var numberIndex = data.Index().ToDictionary(x => x.Item, x => x.Index);
        foreach (var rule in _rules)
        {
            if (numberIndex.TryGetValue(rule.X, out var xPos)
                && numberIndex.TryGetValue(rule.Y, out var yPos)
                && xPos > yPos)
                return false;
        }

        return true;
    }

    private int MiddlePageNumber(int[] data)
        => data.Skip(data.Length / 2).First();

    private bool IsFindValidNumber(List<int> data, int index)
    {
        int[] pair = [data[index], -1];
        for (var j = 0; j < data.Count; j++)
        {
            if (j == index)
                continue;

            pair[1] = data[j];
            if (!IsValidOrder(pair))
                return false;
        }

        return true;
    }

    private int FindValidNumber(List<int> data)
    {
        for (var i = 0; i < data.Count; i++)
        {
            if (IsFindValidNumber(data, i))
                return data[i];
        }

        throw new Exception("Can't find valid number");
    }

    private int[] FixOrder(int[] data)
    {
        var result = new int[data.Length];
        var unsorted = data.ToList();

        for (var i = 0; i < data.Length; i++)
        {
            result[i] = FindValidNumber(unsorted);
            unsorted.Remove(result[i]);
        }

        return result;
    }

    protected override object Solve1()
    {
        return _data
            .Where(IsValidOrder)
            .Sum(MiddlePageNumber);
    }

    protected override object Solve2()
    {
        return _data
            .Where(x => !IsValidOrder(x))
            .Select(FixOrder)
            .Sum(MiddlePageNumber);
    }
}