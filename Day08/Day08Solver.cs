namespace AdventOfCode.Day08;

public class Day08Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day08Solver().ExecuteExample1("14");

    [Fact]
    public void Step2WithExample() => new Day08Solver().ExecuteExample2("34");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day08Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day08Solver().ExecutePuzzle2());
}

public class Day08Solver : SolverBase
{
    private Dictionary<char, HashSet<(int x, int y)>> _antennasByFrequency = new();
    private HashSet<(int x, int y)> _antinode = new();
    private (int x, int y) _max;

    protected override void Parse(List<string> data)
    {
        _max = (data[0].Length, data.Count);
        for (var y = 0; y < _max.y; y++)
        for (var x = 0; x < _max.x; x++)
        {
            var c = data[y][x];
            if (c == '.')
                continue;

            if (!_antennasByFrequency.TryGetValue(c, out var positions))
                _antennasByFrequency.Add(c, positions = new());
            positions.Add((x, y));
        }
    }

    private bool AddAntinode(int x, int y)
    {
        if (x < 0 || x >= _max.x || y < 0 || y >= _max.y)
            return false;

        _antinode.Add((x, y));
        return true;
    }

    private object AntinodeCount(Action<(int x, int y), (int x, int y)> addAntinodes)
    {
        _antinode.Clear();
        foreach (var antennaList in _antennasByFrequency.Select(antennas => antennas.Value.ToList()))
        {
            for (var i = 0; i < antennaList.Count; i++)
            for (var j = i + 1; j < antennaList.Count; j++)
                addAntinodes(antennaList[i], antennaList[j]);
        }

        return _antinode.Count;
    }

    private void AddAntinodes1((int x, int y) a, (int x, int y) b)
    {
        var distance = (x: b.x - a.x, y: b.y - a.y);
        AddAntinode(a.x - distance.x, a.y - distance.y);
        AddAntinode(b.x + distance.x, b.y + distance.y);
    }

    private void AddAntinodes2((int x, int y) a, (int x, int y) b)
    {
        var distance = (x: b.x - a.x, y: b.y - a.y);
        while (AddAntinode(a.x, a.y))
            a = (a.x - distance.x, a.y - distance.y);

        while (AddAntinode(b.x, b.y))
            b = (b.x + distance.x, b.y + distance.y);
    }

    protected override object Solve1() => AntinodeCount(AddAntinodes1);

    protected override object Solve2() => AntinodeCount(AddAntinodes2);
}