namespace AdventOfCode.Day07;

public class Day07Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day07Solver().ExecuteExample1("3749");

    [Fact]
    public void Step2WithExample() => new Day07Solver().ExecuteExample2("11387");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day07Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day07Solver().ExecutePuzzle2());
}

public class Day07Solver : SolverBase
{
    private record Equation(long Result, long[] Values);

    private List<Equation> _data;

    protected override void Parse(List<string> data)
    {
        _data = data.Select(ParseEquation).ToList();

        Equation ParseEquation(string line)
        {
            var parts = line.Split(": ");
            var values = parts[1].Split(' ').Select(long.Parse).ToArray();
            return new(long.Parse(parts[0]), values);
        }
    }

    private long Calculate(long value1, long value2, char operation) => operation switch
    {
        '+' => value1 + value2,
        '*' => value1 * value2,
        '|' => long.Parse(string.Concat(value1, value2)),
        _ => throw new Exception("Can't calculate")
    };

    private IEnumerable<char[]> GetOperations(Equation equations, char[] operations)
    {
        var operationCount = equations.Values.Length - 1;
        var permutations = (long)Math.Pow(3, operationCount);
        var result = new char[operationCount];
        var operationIndexes = new int[operationCount];

        for (var i = 0; i < permutations; i++)
        {
            for (var j = 0; j < operationCount; j++)
            {
                operationIndexes[j]++;
                if (operationIndexes[j] == operations.Length)
                    operationIndexes[j] = 0;
                else
                    break;
            }

            for (var j = 0; j < operationCount; j++)
                result[j] = operations[operationIndexes[j]];
            yield return result;
        }
    }

    private bool IsValidPart(Equation equation, char[] availableOperations)
    {
        foreach (var operations in GetOperations(equation, availableOperations))
        {
            var result = equation.Values[0];
            for (var i = 0; i < operations.Length; i++)
            {
                result = Calculate(result, equation.Values[i + 1], operations[i]);
                if (result > equation.Result)
                    break;
            }

            if (result == equation.Result)
                return true;
        }

        return false;
    }

    private long Solve(char[] availabelOperations) 
        => _data.Where(q => IsValidPart(q, availabelOperations)).Sum(q => q.Result);

    protected override object Solve1() => Solve(['+', '*']);

    protected override object Solve2() => Solve(['+', '*', '|']);
}