namespace AdventOfCode.Day04;

public class Day04Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day04Solver().ExecuteExample1("18");

    [Fact]
    public void Step2WithExample() => new Day04Solver().ExecuteExample2("9");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day04Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day04Solver().ExecutePuzzle2());
}

public class Day04Solver : SolverBase
{
    private char[,] _data;
    private int _maxX;
    private int _maxY;

    protected override void Parse(List<string> data)
    {
        _maxX = data[0].Length;
        _maxY = data.Count;

        _data = new char[_maxX, _maxY];
        for (var x = 0; x < _maxX; x++)
        for (var y = 0; y < _maxY; y++)
            _data[x, y] = data[y][x];
    }

    private static char[] XMAS = ['X', 'M', 'A', 'S'];

    private int FindXMAS(int x, int y, int deltaX, int deltaY)
    {
        if (deltaX == 0 && deltaY == 0)
            return 0;

        for (var i = 0; i < XMAS.Length; i++)
        {
            var x2 = x + deltaX * i;
            var y2 = y + deltaY * i;
            if (x2 < 0 || x2 >= _maxX || y2 < 0 || y2 >= _maxY)
                return 0;
            if (_data[x2, y2] != XMAS[i])
                return 0;
        }

        return 1;
    }

    private int FindXMAS(int x, int y)
    {
        var result = 0;
        for (var deltaX = -1; deltaX <= 1; deltaX++)
        for (var deltaY = -1; deltaY <= 1; deltaY++)
            result += FindXMAS(x, y, deltaX, deltaY);

        return result;
    }

    private int FindX_MAS(int x, int y, char topLeft, char topRight, char bottomLeft, char bottomRight)
    {
        if (_data[x - 1, y - 1] == topLeft
            && _data[x + 1, y - 1] == topRight
            && _data[x - 1, y + 1] == bottomLeft
            && _data[x + 1, y + 1] == bottomRight)
            return 1;
        
        return 0;
    }

    private int FindX_MAS(int x, int y)
    {
        if (x < 1 || x >= _maxX - 1 || y < 1 || y >= _maxY - 1 || _data[x, y] != 'A')
            return 0;

        return FindX_MAS(x, y, 'M', 'M', 'S', 'S')
               + FindX_MAS(x, y, 'M', 'S', 'M', 'S')
               + FindX_MAS(x, y, 'S', 'S', 'M', 'M')
               + FindX_MAS(x, y, 'S', 'M', 'S', 'M');
    }

    protected override object Solve1()
    {
        var result = 0;
        for (var x = 0; x < _maxX; x++)
        for (var y = 0; y < _maxX; y++)
            result += FindXMAS(x, y);
        return result;
    }

    protected override object Solve2()
    {
        var result = 0;
        for (var x = 0; x < _maxX; x++)
        for (var y = 0; y < _maxX; y++)
            result += FindX_MAS(x, y);
        return result;
    }
}