namespace AdventOfCode.Day12;

public class Day12Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day12Solver().ExecuteExample1("1930");

    [Fact]
    public void Step2WithExample() => new Day12Solver().ExecuteExample2("1206");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day12Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day12Solver().ExecutePuzzle2());
}

public class Day12Solver : SolverBase
{
    char[,] _data;
    int _maxX;
    int _maxY;
    Perimeter _perimeter = new();
    HashSet<(int x, int y)> _area = [];

    private class Perimeter
    {
        public HashSet<(int x, int y)> Left { get; set; } = [];
        public HashSet<(int x, int y)> Right { get; set; } = [];
        public HashSet<(int x, int y)> Top { get; set; } = [];
        public HashSet<(int x, int y)> Bottom { get; set; } = [];
        public int Count => Left.Count + Right.Count + Top.Count + Bottom.Count;

        public void Clear()
        {
            Left.Clear();
            Right.Clear();
            Top.Clear();
            Bottom.Clear();
        }

        private void FloodFill((int x, int y) position, (int x, int y) delta, HashSet<(int x, int y)> perimeter)
        {
            if (!perimeter.Contains(position))
                return;

            perimeter.Remove(position);
            FloodFill((position.x + delta.x, position.y + delta.y), delta, perimeter);
        }

        private int FloodFillLeftRight(HashSet<(int x, int y)> perimeter)
        {
            var result = 0;
            var orderedPerimeter = perimeter.OrderBy(q => q.x).ThenBy(q => q.y).ToArray();
            foreach (var position in orderedPerimeter)
            {
                if (!perimeter.Contains(position))
                    continue;

                result++;
                FloodFill(position, (0, 1), perimeter);
            }

            return result;
        }

        private int FloodFillTopBottom(HashSet<(int x, int y)> perimeter)
        {
            var result = 0;
            var orderedPerimeter = perimeter.OrderBy(q => q.y).ThenBy(q => q.x).ToArray();
            foreach (var position in orderedPerimeter)
            {
                if (!perimeter.Contains(position))
                    continue;

                result++;
                FloodFill(position, (1, 0), perimeter);
            }

            return result;
        }

        public int CountAfterFloodFill()
        {
            return FloodFillLeftRight(Left) + FloodFillLeftRight(Right) + FloodFillTopBottom(Top) +
                   FloodFillTopBottom(Bottom);
        }
    }

    protected override void Parse(List<string> data)
    {
        _maxX = data[0].Length + 2;
        _maxY = data.Count + 2;
        _data = new char[_maxX, _maxY];
        for (var y = 1; y < _maxY - 1; y++)
        for (var x = 1; x < _maxX - 1; x++)
            _data[y, x] = data[y - 1][x - 1];
    }

    void FloodFill(int x, int y, char value, HashSet<(int x, int y)>? perimeterDirection)
    {
        var position = (x, y);
        if (_area.Contains(position))
            return;

        if (_data[y, x] != value)
        {
            (perimeterDirection ?? throw new Exception("Perimeter direction is null")).Add(position);
            return;
        }

        _area.Add(position);

        FloodFill(x - 1, y, value, _perimeter.Left);
        FloodFill(x + 1, y, value, _perimeter.Right);
        FloodFill(x, y - 1, value, _perimeter.Top);
        FloodFill(x, y + 1, value, _perimeter.Bottom);
    }

    void Clear()
    {
        foreach (var (x, y) in _area)
            _data[y, x] = ' ';
    }

    void StartFloodFill(int x, int y)
    {
        _area.Clear();
        _perimeter.Clear();

        if (!char.IsLetter(_data[y, x]))
            return;

        FloodFill(x, y, _data[y, x], null);
        Clear();
    }

    protected override object Solve1()
    {
        var result = 0L;
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            StartFloodFill(x, y);
            result += _area.Count * _perimeter.Count;
        }

        return result;
    }

    protected override object Solve2()
    {
        var result = 0L;

        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            var item = _data[y, x];
            StartFloodFill(x, y);
            if (_area.Count > 0)
            {
                var countAfterFloodFill = _perimeter.CountAfterFloodFill();
                result += _area.Count * countAfterFloodFill;
            }
        }

        return result;
    }
}