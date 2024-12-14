using System.Text.RegularExpressions;

namespace AdventOfCode.Day14;

public class Day14Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day14Solver().ExecuteExample1("12");

    [Fact]
    public void Step2WithExample() => new Day14Solver().ExecuteExample2("??");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day14Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day14Solver().ExecutePuzzle2());
}

public class Day14Solver : SolverBase
{
    private class Robot(int px, int py, int vx, int vy)
    {
        public int Px { get; set; } = px;
        public int Py { get; set; } = py;
        public int Vx { get; set; } = vx;
        public int Vy { get; set; } = vy;
    }

    private List<Robot> _data;
    int _maxX;
    int _maxY;

    protected override void Parse(List<string> data)
    {
        var regex = new Regex(@"p=(?'px'\d+),(?'py'\d+) v=(?'vx'-?\d+),(?'vy'-?\d+)");
        _data = new();
        foreach (var match in data.Select(line => regex.Match(line)).Where(match => match.Success))
        {
            var (px, py) = (int.Parse(match.Groups["px"].Value), int.Parse(match.Groups["py"].Value));
            var (vx, vy) = (int.Parse(match.Groups["vx"].Value), int.Parse(match.Groups["vy"].Value));
            _data.Add(new(px, py, vx, vy));
        }

        if (_data.All(q => q.Px < 11))
            (_maxX, _maxY) = (11, 7);
        else
            (_maxX, _maxY) = (101, 103);
    }

    private void Move(Robot robot)
    {
        robot.Px += robot.Vx;
        robot.Py += robot.Vy;
        if (robot.Px < 0)
            robot.Px += _maxX;
        else if (robot.Px >= _maxX)
            robot.Px -= _maxX;
        if (robot.Py < 0)
            robot.Py += _maxY;
        else if (robot.Py >= _maxY)
            robot.Py -= _maxY;
    }

    private void MoveAll(int times)
    {
        foreach (var robot in _data)
            for (var i = 0; i < times; i++)
                Move(robot);
    }

    private void Dump()
    {
        var sb = new StringBuilder();
        for (var y = 0; y < _maxY; y++)
        {
            for (var x = 0; x < _maxX; x++)
                sb.Append(_data.Any(q => q.Px == x && q.Py == y) ? "*" : ".");
            sb.AppendLine();
        }

        var screen = sb.ToString();
        Debug.WriteLine(screen);
        Debug.WriteLine("----");
    }

    long CalcQuadResult()
    {
        var splitX = _maxX / 2;
        var splitY = _maxY / 2;
        long[] quads = [0, 0, 0, 0];
        foreach (var robot in _data)
        {
            if (robot.Px == splitX || robot.Py == splitY)
                continue;

            var poxX = robot.Px < splitX ? 0 : 1;
            var poxy = robot.Py < splitY ? 0 : 2;
            quads[poxX + poxy]++;
        }

        return quads[0] * quads[1] * quads[2] * quads[3];
    }

    protected override object Solve1()
    {
        MoveAll(100);
        return CalcQuadResult();
    }

    protected override object Solve2()
    {
        var result = 0L;
        while (true)
        {
            foreach (var robot in _data)
                Move(robot);

            result++;
            
            var map = _data.Select(q => (x: q.Px, y: q.Py)).ToHashSet();
            foreach (var pos in map)
            {
                var found = true;
                for (var i = 1; i < 10 && found; i++)
                {
                    found = (map.Contains((pos.x - i, pos.y + i))
                             && map.Contains((pos.x + i, pos.y + i))
                        );
                }

                if (found)
                {
                    Dump();
                    return result;
                }
            }
        }

        throw new Exception("Solver error");
    }
}