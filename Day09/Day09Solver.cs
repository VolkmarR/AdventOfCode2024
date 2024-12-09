namespace AdventOfCode.Day09;

public class Day09Tests(ITestOutputHelper output)
{
    [Fact]
    public void Step1WithExample() => new Day09Solver().ExecuteExample1("1928");

    [Fact]
    public void Step2WithExample() => new Day09Solver().ExecuteExample2("2858");

    [Fact]
    public void Step1WithPuzzleInput() => output.WriteLine(new Day09Solver().ExecutePuzzle1());

    [Fact]
    public void Step2WithPuzzleInput() => output.WriteLine(new Day09Solver().ExecutePuzzle2());
}

public class Day09Solver : SolverBase
{
    private const int FreeBlock = -1;

    class Block
    {
        public int Id { get; init; }
        public int Length { get; init; }

        public static Block File(int id, int lenght)
            => new Block { Id = id, Length = lenght };

        public static Block Free(int lenght)
            => new Block { Id = FreeBlock, Length = lenght };
    }

    List<Block> _data;
    private int[] _disk;

    protected override void Parse(List<string> data)
    {
        _data = new();
        var isFile = true;
        var id = 0;
        foreach (int length in data[0].Select(lengthChar => int.Parse(lengthChar.ToString())))
        {
            if (isFile)
                _data.Add(Block.File(id++, length));
            else
                _data.Add(Block.Free(length));
            isFile = !isFile;
        }

        var diskSize = _data.Sum(q => q.Length);
        _disk = new int[diskSize];
        var index = 0;
        foreach (var block in _data)
        {
            for (var i = 0; i < block.Length; i++)
                _disk[index++] = block.Id;
        }
    }

    private void DefragmentBlock()
    {
        var indexFreeBlock = 0;
        var indexFile = _disk.Length - 1;
        FindNextFreeBlock();
        FindNextFile();

        while (indexFreeBlock < indexFile)
        {
            while (indexFreeBlock < _disk.Length && indexFile >= 0 && _disk[indexFreeBlock] == FreeBlock &&
                   _disk[indexFile] != FreeBlock)
            {
                _disk[indexFreeBlock] = _disk[indexFile];
                _disk[indexFile] = FreeBlock;
                indexFile--;
                indexFreeBlock++;
            }

            FindNextFreeBlock();
            FindNextFile();
        }

        return;

        void FindNextFreeBlock()
        {
            while (indexFreeBlock < _disk.Length && _disk[indexFreeBlock] != FreeBlock)
                indexFreeBlock++;
        }

        void FindNextFile()
        {
            indexFile = _disk.Length - 1;
            while (indexFile >= 0 && _disk[indexFile] == FreeBlock)
                indexFile--;
        }
    }

    private void DefragmentFile()
    {
        var touched = new HashSet<int>();
        var indexFile = _disk.Length - 1;
        var fileLenght = 0;

        do
        {
            FindNextFile();
            MoveToFirstFreeBlock();
        } while (indexFile > 0);

        return;

        void FindNextFile()
        {
            while (indexFile >= 0 && (_disk[indexFile] == FreeBlock || touched.Contains(_disk[indexFile])))
                indexFile--;

            if (indexFile < 1)
                return;

            var fileId = _disk[indexFile];
            touched.Add(fileId);
            fileLenght = 0;
            while (indexFile >= 0 && _disk[indexFile] == fileId)
            {
                fileLenght++;
                indexFile--;
            }

            indexFile++;
        }

        void MoveToFirstFreeBlock()
        {
            var index = 0;
            while (index < indexFile)
            {
                while (index < indexFile && _disk[index] != FreeBlock)
                    index++;

                var indexFreeBlock = index;
                var lengthFreeBlock = 0;
                while (index < indexFile && lengthFreeBlock < fileLenght && _disk[index] == FreeBlock)
                {
                    index++;
                    lengthFreeBlock++;
                }

                if (lengthFreeBlock >= fileLenght)
                {
                    for (var i = 0; i < fileLenght; i++)
                    {
                        _disk[indexFreeBlock + i] = _disk[indexFile + i];
                        _disk[indexFile + i] = FreeBlock;
                    }
                    return;
                }
            }
        }
    }

    private long Checksum()
    {
        var checksum = 0L;
        for (var i = 0; i < _disk.Length; i++)
            if (_disk[i] != FreeBlock)
                checksum += (long)_disk[i] * i;
        return checksum;
    }

    protected override object Solve1()
    {
        DefragmentBlock();
        return Checksum();
    }

    protected override object Solve2()
    {
        DefragmentFile();
        return Checksum();
    }
}