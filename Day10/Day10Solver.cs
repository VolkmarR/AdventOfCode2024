namespace AdventOfCode.Day10;

public class Day10Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day10Solver().ExecuteExample1("36");

    [Fact]
    public void Step2WithExample() => new Day10Solver().ExecuteExample2("81");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day10Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day10Solver().ExecutePuzzle2());
}

public class Day10Solver : SolverBase
{
    int[,] _data;
    int _maxX;
    int _maxY;

    protected override void Parse(List<string> data)
    {
        _maxX = data[0].Length;
        _maxY = data.Count;
        _data = new int[_maxX, _maxY];
        for (var y = 0; y < _maxY; y++)
        {
            var line = data[y];
            for (var x = 0; x < _maxX; x++)
                _data[x, y] = int.Parse(line[x].ToString());
        }
    }

    private void Move(int lastHeight, int x, int y, HashSet<(int x, int y)> visited,
        HashSet<(int x, int y)> visitedNine)
    {
        if (x < 0 || x >= _maxX || y < 0 || y >= _maxY || _data[x, y] != lastHeight + 1 || visited.Contains((x, y)))
            return;

        var position = (x, y);
        var current = _data[x, y];
        if (current == 9)
        {
            visitedNine.Add(position);
            return;
        }

        visited.Add(position);
        Move(current, x + 1, y, visited, visitedNine);
        Move(current, x - 1, y, visited, visitedNine);
        Move(current, x, y + 1, visited, visitedNine);
        Move(current, x, y - 1, visited, visitedNine);
    }

    protected override object Solve1()
    {
        var dummy = 0;
        var count = 0;
        var visited = new HashSet<(int x, int y)>();
        var visitedNine = new HashSet<(int x, int y)>();
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            if (_data[x, y] != 0)
                continue;
            
            visited.Clear();
            visitedNine.Clear();
            Move(-1, x, y, visited, visitedNine);
            count += visitedNine.Count;
        }   
        
        return count;
    }

    protected override object Solve2()
    {
        var dummy = 0;
        var count = 0;
        var visited = new HashSet<(int x, int y)>();
        var visitedNine = new HashSet<(int x, int y)>();
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            if (_data[x, y] != 0)
                continue;
            
            visited.Clear();
            visitedNine.Clear();
            Move(-1, x, y, visited, visitedNine);
            count += dummy;
        }   
        
        return count;
    }
}