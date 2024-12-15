namespace AdventOfCode.Day15;

public class Day15Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day15Solver().ExecuteExample1("10092");

    [Fact]
    public void Step2WithExample() => new Day15Solver().ExecuteExample2("??");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day15Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day15Solver().ExecutePuzzle2());
}

public class Day15Solver : SolverBase
{
    private int _maxX;
    private int _maxY;
    private char[,] _map;
    private char[] _moves;
    private int _robotX;
    private int _robotY;

    protected override void Parse(List<string> data)
    {
        var split = data.IndexOf("");
        _map = new char[data[0].Length, split];
        _maxX = data[0].Length;
        _maxY = split;

        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            var item = data[y][x];
            if (item == '@')
            {
                (_robotX, _robotY) = (x, y);
                item = ' ';
            }
            else if (!(item is '#' or 'O'))
                item = ' ';

            _map[x, y] = item;
        }

        var moves = new List<char>();
        for (var i = split + 1; i < data.Count; i++)
            moves.AddRange(data[i]);
        _moves = moves.ToArray();
    }

    private (int dx, int dy) GetMove(char move) => move switch
    {
        '>' => (1, 0),
        '<' => (-1, 0),
        '^' => (0, -1),
        'v' => (0, 1),
        _ => throw new Exception("Move error"),
    };

    private void Move(char move)
    {
        var (dx, dy) = GetMove(move);
        var (x, y) = (_robotX + dx, _robotY + dy);

        if (_map[x, y] == '#')
            return;

        if (_map[x, y] != 'O')
        {
            (_robotX, _robotY) = (x, y);
            return;
        }

        var (boxX, boxY) = (x, y);
        while (_map[boxX, boxY] == 'O')
            (boxX, boxY) = (boxX + dx, boxY + dy);

        if (_map[boxX, boxY] != ' ')
            return;

        _map[boxX, boxY] = 'O';
        _map[x, y] = ' ';
        (_robotX, _robotY) = (x, y);
    }

    private static long GpsResult(int x, int y)
        => y * 100 + x;

    private long CalcGpsResult()
    {
        var result = 0L;
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            if (_map[x, y] == 'O')
                result += GpsResult(x, y);
        }

        return result;
    }

    private void MoveAll()
    {
        foreach (var move in _moves)
            Move(move);
    }

    private void TransformMap()
    {
        var map = _map;
        _map = new char[_maxX * 2, _maxY];
        
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            var newX = x * 2;
            if (map[x, y] == '#')
            {
                _map[newX, y] = '#';
                _map[newX + 1, y] = '#';
            }
            else if (map[x, y] == 'O')
            {
                _map[newX, y] = '[';
                _map[newX + 1, y] = ']';
            }
            else
            {
                _map[newX, y] = ' ';
                _map[newX + 1, y] = ' ';
            }
        }
        
        _maxX *= 2;
        _robotX *= 2;
    }

    protected override object Solve1()
    {
        MoveAll();
        return CalcGpsResult();
    }

    
    protected override object Solve2()
    {
        throw new Exception("Solver error");
    }
}