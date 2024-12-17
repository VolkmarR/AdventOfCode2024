namespace AdventOfCode.Day16;

public class Day16Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day16Solver().ExecuteExample1("7036");

    [Fact]
    public void Step2WithExample() => new Day16Solver().ExecuteExample2("??");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day16Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day16Solver().ExecutePuzzle2());
}

public class Day16Solver : SolverBase
{
    private class Reindeer
    {
        public long Points { get; set; }
        public (int x, int y) Position { get; set; }
        public (int dx, int dy) Direction { get; set; }
        public HashSet<(int x, int y)> Path { get; set; } = [];
        public char[,] Map { get; init; }

        public (int dx, int dy) CalcRotateClockwise()
            => Direction switch
            {
                (-1, 0) => (0, -1), // < -> ^
                (0, -1) => (1, 0), // ^ -> >
                (1, 0) => (0, 1), // > -> v
                (0, 1) => (-1, 0), // v -> <
                _ => throw new Exception("Rotate error"),
            };

        public Reindeer RotateClockwise()
        {
            Direction = CalcRotateClockwise();
            Points += 1000;
            return this;
        }

        public (int dx, int dy) CalcRotateCounterClockwise()
            => Direction switch
            {
                (-1, 0) => (0, 1), // < -> v
                (0, 1) => (1, 0), // v -> >
                (1, 0) => (0, -1), // > -> ^
                (0, -1) => (-1, 0), // ^ -> <
                _ => throw new Exception("Rotate error"),
            };

        public Reindeer RotateCounterClockwise()
        {
            Direction = CalcRotateCounterClockwise();
            Points += 1000;
            return this;
        }

        private bool CanMove((int x, int y) newPosition)
            => Map[newPosition.x, newPosition.y] != '#' && !Path.Contains(newPosition);

        public Reindeer Move()
        {
            Position = (x: Position.x + Direction.dx, y: Position.y + Direction.dy);
            Points++;
            Path.Add(Position);
            return this;
        }

        public bool CanMoveForward()
        {
            var newPosition = (x: Position.x + Direction.dx, y: Position.y + Direction.dy);
            return CanMove(newPosition);
        }

        public bool CanMoveClockwise()
        {
            var newDirection = CalcRotateClockwise();
            var newPosition = (x: Position.x + newDirection.dx, y: Position.y + newDirection.dy);
            return CanMove(newPosition);
        }

        public bool CanMoveCounterClockwise()
        {
            var newDirection = CalcRotateCounterClockwise();
            var newPosition = (x: Position.x + newDirection.dx, y: Position.y + newDirection.dy);
            return CanMove(newPosition);
        }

        public Reindeer Clone() =>
            new()
            {
                Points = Points,
                Position = Position,
                Direction = Direction,
                Path = [..Path],
                Map = Map
            };
    }

    char[,] _map;
    int _maxX;
    int _maxY;
    Reindeer _start;
    (int x, int y) _end;

    protected override void Parse(List<string> data)
    {
        _maxX = data[0].Length;
        _maxY = data.Count;
        _map = new char[_maxX, _maxY];
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            if (data[y][x] == 'S')
                _start = new Reindeer { Position = (x, y), Direction = (1, 0), Map = _map };
            else if (data[y][x] == 'E')
                _end = (x, y);
            _map[x, y] = data[y][x];
        }
    }

    private long _bestPoints = long.MaxValue;

    private void FindPath(Reindeer reindeer)
    {
        if (reindeer.Position == _end)
        {
            if (reindeer.Points < _bestPoints)
                _bestPoints = reindeer.Points;
            return;
        }

        if (reindeer.Points > _bestPoints)
            return;

        if (reindeer.CanMoveForward())
            FindPath(reindeer.Clone().Move());
        
        if (reindeer.CanMoveCounterClockwise())
            FindPath(reindeer.Clone().RotateCounterClockwise().Move());

        if (reindeer.CanMoveClockwise())
            FindPath(reindeer.Clone().RotateClockwise().Move());
    }

    protected override object Solve1()
    {
        FindPath(_start);
        return _bestPoints;
    }

    protected override object Solve2()
    {
        throw new Exception("Solver error");
    }
}