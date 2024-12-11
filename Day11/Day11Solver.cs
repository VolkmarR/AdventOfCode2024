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

    protected override void Parse(List<string> data)
    {
        _data = data[0].Split(" ").Select(long.Parse).ToArray();
    }

    private bool Split(long value, out long left, out long right)
    {
        left = 0;
        right = 0;

        var work = value;
        var length = 0;
        while (work > 0)
        {
            length++;
            work /= 10;
        }

        if (length % 2 != 0)
            return false;

        var asString = value.ToString();
        left = long.Parse(asString[..(asString.Length / 2)]);
        right = long.Parse(asString[(asString.Length / 2)..]);
        return true;
    }

    private void Blink(LinkedList<long> start)
    {
        var current = start.First;
        while (current != null)
        {
            if (current.Value == 0)
                current.Value = 1;
            else if (Split(current.Value, out var left, out var right))
            {
                start.AddBefore(current, new LinkedListNode<long>(left));
                current.Value = right;
            }
            else
                current.Value *= 2024;

            current = current.Next;
        }
    }

    private long BlinkFor(int times)
    {
        var count = 0;
        var itemAsList = new LinkedList<long>();
        foreach (var item in _data)
        {
            itemAsList.Clear();
            itemAsList.AddLast(new LinkedListNode<long>(item));
            for (var i = 0; i < times; i++)
                Blink(itemAsList);
            count += itemAsList.Count;
        }

        return count;
    }

    protected override object Solve1()
    {
        return BlinkFor(25);
    }


    protected override object Solve2()
    {
        return BlinkFor(75);
    }
}