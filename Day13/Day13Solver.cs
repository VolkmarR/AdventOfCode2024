using System.Text.RegularExpressions;

namespace AdventOfCode.Day13;

public class Day13Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day13Solver().ExecuteExample1("480");

    [Fact]
    public void Step2WithExample() => new Day13Solver().ExecuteExample2("??");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day13Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day13Solver().ExecutePuzzle2());
}

public class Day13Solver : SolverBase
{
    class Machine
    {
        public (decimal X, decimal Y) ButtonA { get; set; }
        public (decimal X, decimal Y) ButtonB { get; set; }
        public (decimal X, decimal Y) Prize { get; set; }

        public (bool valid, decimal a, decimal b) CalcPushes()
        {
            // (py*ax - px*ay) / (by*ax - bx*ay)
            var divident = Prize.Y * ButtonA.X - Prize.X * ButtonA.Y;
            var divisor = ButtonB.Y * ButtonA.X - ButtonB.X * ButtonA.Y;
            
            var b = divident / divisor;
            var br = divident % divisor;
            if (b != Math.Floor(b))
                return (false, 0, 0);
            
            // px/ax - b*bx/ax
            var a = (Prize.X - b * ButtonB.X) / ButtonA.X;
            if (a != Math.Floor(a))
                return (false, 0, 0);
            
            return (true, a, b);
        }
    }

    List<Machine> _data;

    protected override void Parse(List<string> data)
    {
        _data = new();
        var regex = new Regex(@"(Button (A|B)|Prize): X(\+|=)(?'X'\d+), Y(\+|=)(?'Y'\d+)");
        for (var i = 0; i < data.Count; i += 4)
        {
            var matchA = regex.Match(data[i]);
            var matchB = regex.Match(data[i + 1]);
            var matchPrize = regex.Match(data[i + 2]);
            _data.Add(new Machine
            {
                ButtonA = (int.Parse(matchA.Groups["X"].Value), int.Parse(matchA.Groups["Y"].Value)),
                ButtonB = (int.Parse(matchB.Groups["X"].Value), int.Parse(matchB.Groups["Y"].Value)),
                Prize = (int.Parse(matchPrize.Groups["X"].Value), int.Parse(matchPrize.Groups["Y"].Value))
            });
        }
    }

    protected override object Solve1()
    {
        return CalcMinimumCost();
    }

    private decimal CalcMinimumCost()
    {
        decimal result = 0;
        foreach (var machine in _data)
        {
            var (valid, a, b) = machine.CalcPushes();
            if (valid)
                result += 3*a + b;
        }

        return result;
    }

    protected override object Solve2()
    {
        const decimal offset = 10000000000000;
        foreach (var machine in _data)
            machine.Prize = (machine.Prize.X + offset, machine.Prize.Y + offset);
        
        return CalcMinimumCost();
    }
}