namespace AdventOfCode.Day15;

public class Day15Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day15Solver().ExecuteExample1("10092");

    [Fact]
    public void Step2WithExample() => new Day15Solver().ExecuteExample2("9021");

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

    private bool IsBox(int x, int y) => _map[x, y] is '[' or ']' or 'O';

    private (int dx, int dy) GetMove(char move) => move switch
    {
        '>' => (1, 0),
        '<' => (-1, 0),
        '^' => (0, -1),
        'v' => (0, 1),
        _ => throw new Exception("Move error"),
    };

    private void Move(char move, Func<char, bool> moveBlock)
    {
        var (dx, dy) = GetMove(move);
        var (x, y) = (_robotX + dx, _robotY + dy);

        if (_map[x, y] == '#')
            return;

        if (!IsBox(x, y))
        {
            (_robotX, _robotY) = (x, y);
            return;
        }

        DumpMap(move);
        if (moveBlock(move))
            (_robotX, _robotY) = (x, y);
        DumpMap('R');
    }

    private bool MoveBlock1(char move)
    {
        var (dx, dy) = GetMove(move);
        var (x, y) = (_robotX + dx, _robotY + dy);

        var (boxX, boxY) = (x, y);
        while (IsBox(boxX, boxY))
            (boxX, boxY) = (boxX + dx, boxY + dy);

        if (_map[boxX, boxY] != ' ')
            return false;

        _map[boxX, boxY] = _map[x, y];
        _map[x, y] = ' ';
        return true;
    }

    private (int x1, int x2) GetBoxX1AndX2(int x, int y)
        => _map[x, y] switch
        {
            '[' => (x, x + 1),
            ']' => (x - 1, x),
            _ => throw new Exception("GetOtherBoxPartX error"),
        };

    private bool MoveBlock2(char move)
    {
        var (dx, dy) = GetMove(move);
        var (x, y) = (_robotX + dx, _robotY + dy);
        if (dy == 0)
            return MoveBlock2OnX(x, y, dx);

        var (x1, x2) = GetBoxX1AndX2(x, y);
        if (!CanMoveBlock2OnY(x1, x2, y, dy))
            return false;

        MoveBlock2OnY(x1, x2, y, dy);
        return true;
    }

    private bool MoveBlock2OnX(int x, int y, int dx)
    {
        var boxX = x;
        while (IsBox(boxX, y))
            boxX += dx;

        if (_map[boxX, y] != ' ')
            return false;

        while (boxX != x)
        {
            _map[boxX, y] = _map[boxX - dx, y];
            boxX -= dx;
        }

        _map[x, y] = ' ';
        return true;
    }

    private bool CanMoveBlock2OnY(int x1, int x2, int y, int dy)
    {
        y += dy;

        var valid1 = false;
        var valid2 = false;

        if (IsBox(x1, y))
        {
            var box = GetBoxX1AndX2(x1, y);
            valid1 = CanMoveBlock2OnY(box.x1, box.x2, y, dy);
        }
        else
            valid1 = _map[x1, y] == ' ';

        if (IsBox(x2, y))
        {
            var box = GetBoxX1AndX2(x2, y);
            valid2 = CanMoveBlock2OnY(box.x1, box.x2, y, dy);
        }
        else
            valid2 = _map[x2, y] == ' ';

        return valid1 && valid2;
    }

    private void MoveBlock2OnY(int x1, int x2, int y, int dy)
    {
        y += dy;
        (int x1, int x2) box1 = (-1, -1);
        if (IsBox(x1, y))
        {
            box1 = GetBoxX1AndX2(x1, y);
            MoveBlock2OnY(box1.x1, box1.x2, y, dy);
        }

        if (IsBox(x2, y))
        {
            var box2 = GetBoxX1AndX2(x2, y);
            if (box1.x1 != box2.x1)
                MoveBlock2OnY(box2.x1, box2.x2, y, dy);
        }

        var prevY = y - dy;
        _map[x1, y] = _map[x1, prevY];
        _map[x2, y] = _map[x2, prevY];
        _map[x1, prevY] = ' ';
        _map[x2, prevY] = ' ';
    }

    private static long GpsResult(int x, int y)
        => y * 100 + x;

    private long CalcGpsResult(char box)
    {
        var result = 0L;
        for (var y = 0; y < _maxY; y++)
        for (var x = 0; x < _maxX; x++)
        {
            if (_map[x, y] == box)
                result += GpsResult(x, y);
        }

        return result;
    }

    private void MoveAll(Func<char, bool> moveBlock)
    {
        foreach (var move in _moves)
        {
            Move(move, moveBlock);
        }
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

    private void DumpMap(char move)
    {
        var sb = new StringBuilder();
        for (var y = 0; y < _maxY; y++)
        {
            for (var x = 0; x < _maxX; x++)
                if (x == _robotX && y == _robotY)
                    sb.Append(move);
                else
                    sb.Append(_map[x, y]);
            sb.AppendLine();
        }

        var screen = sb.ToString().Replace(' ', '.');
        Debug.WriteLine(screen);
        Debug.WriteLine("----");
    }

    protected override object Solve1()
    {
        MoveAll(MoveBlock1);
        return CalcGpsResult('O');
    }


    protected override object Solve2()
    {
        TransformMap();
        MoveAll(MoveBlock2);
        return CalcGpsResult('[');
    }
}