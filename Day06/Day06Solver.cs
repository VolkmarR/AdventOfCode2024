namespace AdventOfCode.Day06;

public class Day06Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day06Solver().ExecuteExample1("41");

    [Fact]
    public void Step2WithExample() => new Day06Solver().ExecuteExample2("6");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day06Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day06Solver().ExecutePuzzle2());
}

public class Day06Solver : SolverBase
{
    private HashSet<(int x, int y)> _path = new();
    private (int X, int Y) _position;
    private (int X, int Y) _delta;

    HashSet<(int x, int y)> _obstacles;
    (int x, int y) _startPosition;
    int _maxX;
    int _maxY;

    protected override void Parse(List<string> data)
    {
        _maxX = data[0].Length;
        _maxY = data.Count;

        _obstacles = new();
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            if (data[y][x] == '#')
                _obstacles.Add((x, y));
            if (data[y][x] == '^')
                _startPosition = (x, y);
        }
    }

    private bool IsOutsideOfMap()
        => _position.X < 0 || _position.X >= _maxX || _position.Y < 0 || _position.Y >= _maxY;

    private void Move()
    {
        _path.Add(_position);
        var newPosition = (_position.X + _delta.X, _position.Y + _delta.Y);
        if (!_obstacles.Contains(newPosition))
        {
            _position = newPosition;
            return;
        }

        // ^ (0, -1)
        // > (1, 0)
        // v (0, 1)
        // < (-1, 0)

        _delta = _delta switch
        {
            (0, -1) => (1, 0),
            (1, 0) => (0, 1),
            (0, 1) => (-1, 0),
            (-1, 0) => (0, -1),
            _ => throw new Exception("Can't move")
        };
    }

    private void InitGuard()
    {
        _path.Clear();
        _position = _startPosition;
        _delta = (0, -1);
    }
    
    private void MoveUntilOutsideOfMap()
    {
        InitGuard();
        while (!IsOutsideOfMap())
            Move();
    }

    private bool IsLoop()
    {
        InitGuard();
        for (var i = 0; i < _maxX * _maxY; i++)
        {
            if (IsOutsideOfMap())
                return false;
            Move();
        }

        return true;
    }

    protected override object Solve1()
    {
        MoveUntilOutsideOfMap();
        return _path.Count;
    }

    protected override object Solve2()
    {
        var result = 0;
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            var newObstacles = (x, y);
            if (_obstacles.Contains(newObstacles) || _startPosition == newObstacles)
                continue;

            _obstacles.Add(newObstacles);
            if (IsLoop())
                result++;
            _obstacles.Remove(newObstacles);
        }

        return result;
    }
}