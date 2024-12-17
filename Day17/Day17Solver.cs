namespace AdventOfCode.Day17;

public class Day17Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day17Solver().ExecuteExample1("4,6,3,5,6,3,5,2,1,0");

    [Fact]
    public void Step2WithExample() => new Day17Solver().ExecuteExample2("117440");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day17Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day17Solver().ExecutePuzzle2());
}

public class Day17Solver : SolverBase
{
    private long _registerA;
    private long _registerB;
    private long _registerC;
    private long _registerACopy;
    private long _registerBCopy;
    private long _registerCCopy;

    private int[] _data;
    private int _instructionPointer;
    private List<int> _output;
    private bool _outputCheck;

    protected override void Parse(List<string> data)
    {
        _registerACopy = data[0].Split(": ")[1].ToInt();
        _registerBCopy = data[1].Split(": ")[1].ToInt();
        _registerCCopy = data[2].Split(": ")[1].ToInt();
        _data = data[4][8..].Split(",").Select(q => q.ToInt()).ToArray();
        _output = new List<int>(_data.Length);
    }

    (bool valid, long result) InternalAdv(int operand)
    {
        var (valid, result) = GetCombo(operand);
        if (!valid)
            return (false, 0);
        return (true, _registerA / 2L.Pow(result));
    }

    bool Adv0(int operand)
    {
        (var valid, _registerA) = InternalAdv(operand);
        return valid;
    }

    void Bxl1(int operand) => _registerB = _registerB ^ operand;

    bool Bst2(int operand)
    {
        (var valid, var result) = GetCombo(operand);
        _registerB = result % 8;
        return valid;
    }

    void Jnz3(int operand)
    {
        if (_registerA > 0)
            _instructionPointer = operand - 2;
    }

    void Bxc4() => _registerB = _registerB ^ _registerC;

    bool Out5(int operand)
    {
        (var valid, var result) = GetCombo(operand);
        _output.Add((int)(result % 8));

        if (valid && _outputCheck)
        {
            var outputIndex = _output.Count - 1;
            valid = _output.Count <= _data.Length && _output[outputIndex] == _data[outputIndex];
        }

        return valid;
    }

    bool Bdv6(int operand)
    {
        (var valid, _registerB) = InternalAdv(operand);
        return valid;
    }

    bool Cdv7(int operand)
    {
        (var valid, _registerC) = InternalAdv(operand);
        return valid;
    }

    (bool valid, long result) GetCombo(int operand)
    {
        if (operand < 4)
            return (true, operand);
        if (operand == 4)
            return (true, _registerA);
        if (operand == 5)
            return (true, _registerB);
        if (operand == 6)
            return (true, _registerC);

        return (false, 0);
    }

    private bool ExecuteOperation(int opcode, int operand)
    {
        var result = true;
        if (opcode == 0)
            result = Adv0(operand);
        else if (opcode == 1)
            Bxl1(operand);
        else if (opcode == 2)
            result = Bst2(operand);
        else if (opcode == 3)
            Jnz3(operand);
        else if (opcode == 4)
            Bxc4();
        else if (opcode == 5)
            result = Out5(operand);
        else if (opcode == 6)
            result = Bdv6(operand);
        else if (opcode == 7)
            result = Cdv7(operand);

        _instructionPointer += 2;
        return result;
    }

    private bool ExecuteProgram(long valueRegisterA)
    {
        _output.Clear();
        _instructionPointer = 0;
        _registerA = valueRegisterA;
        _registerB = _registerBCopy;
        _registerC = _registerCCopy;
        while (_instructionPointer + 1 < _data.Length)
        {
            if (!ExecuteOperation(_data[_instructionPointer], _data[_instructionPointer + 1]))
                return false;
        }

        return !_outputCheck || _output.Count == _data.Length;
    }

    protected override object Solve1()
    {
        _outputCheck = false;
        ExecuteProgram(_registerACopy);
        return _output.Select(q => q.ToString()).Join(",");
    }

    protected override object Solve2()
    {
        _outputCheck = true;
        for (var i = 0L; i < long.MaxValue; i++)
        {
            if (ExecuteProgram(i))
                return i;
        }

        return -1;
    }
}