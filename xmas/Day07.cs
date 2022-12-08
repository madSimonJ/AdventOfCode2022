using static xmas.Day07;

namespace xmas
{
    public static class ExtDay07
    {
        private static KeyValuePair<string, (int, IEnumerable<string>)> MakeNewEntry(string dir, int size, IEnumerable<string> dirs) =>
            new(dir, (size, dirs));

        private static IEnumerable<string> GetSubDirs(IEnumerable<DirectoryContents> dc) =>
            dc.Where(x => x is DirectorySubdirectory).Select(x => (x as DirectorySubdirectory).SubdirectoryName);


        public static IDictionary<string, (int, IEnumerable<string>)> UpdateDirectoryDictionary(this IDictionary<string, (int, IEnumerable<string>)> @this, string currDir, IEnumerable<DirectoryContents> dc) =>
    (@this.ContainsKey(currDir)
        ? @this.Select(x => x.Key == currDir ? MakeNewEntry(currDir, CalculateDirSize(dc), GetSubDirs(dc) ) : x)
        : @this.Append(MakeNewEntry(currDir, CalculateDirSize(dc), GetSubDirs(dc) )))
        .ToDictionary(x => x.Key, x => x.Value);


    }

    public class Day07
    {
        public static DirectoryFile MakeDirectoryFile(string fileName, int size) => new() {  FileName = fileName, Size = size };
        public static DirectorySubdirectory MakeSubDirectory(string dirName) => new() { SubdirectoryName = dirName };

        public static CdCommand MakeCdCommand(string dir) => new() { Directory = dir };

        public static LsCommand MakeLsCommand(params DirectoryContents[] contents) => new() { Contents = contents };
        public abstract class DirectoryContents
        {

        }

        public class DirectoryFile : DirectoryContents
        {
            public string FileName { get; set; }
            public int Size { get; set; }
        }

        public class DirectorySubdirectory : DirectoryContents
        {
            public string SubdirectoryName { get; set; }
        }

        public abstract class Command
        {

        }

        public class CdCommand : Command
        {
            public string Directory { get; set; }
        }

        public class LsCommand : Command
        {
            public IEnumerable<DirectoryContents> Contents { get; set; }
        }

        public record FileSystem
        {
            public IEnumerable<string> DirectoryStack { get; init; }
            public IDictionary<string, (int, IEnumerable<string>)> Directories { get; set; }
        }

        private static LsCommand ParseLsCommand(string input) => input.Split("\n")
                                                                        .Where(x => !string.IsNullOrWhiteSpace(x))
                                                                        .Skip(1)
                                                                        .Select(x => 
                                                                            x.StartsWith("dir ")
                                                                                ? MakeSubDirectory(x.Split(" ").Last()) as DirectoryContents
                                                                                : x.Split(" ").ToArray().Bind(y => MakeDirectoryFile(y[1], int.Parse(y[0]))) as DirectoryContents
                                                                        )
                                                                        .Bind(x => MakeLsCommand(x.ToArray()));
        private static CdCommand ParseCdCommand(string input) => MakeCdCommand(input);

        public static int CalculateDirSize(IEnumerable<DirectoryContents> dc) =>
            dc.Sum(x => x is DirectoryFile df ? df.Size : 0);

        private static IEnumerable<Command> ParseCommands(string input) =>
                input.Split("$ ")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => {
                    return x[..3].Trim() switch
                    {
                        "ls" => ParseLsCommand(x) as Command,
                        "cd" => ParseCdCommand(x[3..].Trim()) as Command,
                        _ => throw new NotImplementedException()
                    };
                });
        private static FileSystem ScanFileSystem(IEnumerable<Command> commands) =>
            commands.Aggregate(
                    new FileSystem
                    {
                        Directories = new Dictionary<string, (int, IEnumerable<string>)>(),
                        DirectoryStack = Enumerable.Empty<string>()
                    },
                    (agg, x) => (comm: x, Curr: string.Join("/", agg.DirectoryStack)) switch
                    {
                        { comm: CdCommand { Directory: ".." } } => agg with { DirectoryStack = agg.DirectoryStack.Take(agg.DirectoryStack.Count() - 1)},
                        { comm: CdCommand down } => agg with { DirectoryStack = agg.DirectoryStack.Append(down.Directory) },
                        { comm: LsCommand ls, Curr: var c } => agg with { Directories = agg.Directories.UpdateDirectoryDictionary(c, ls.Contents) }
                    }
                );

        public static int GetDirectorySize(string dirName, IDictionary<string, (int, IEnumerable<string>)> fileSystem) =>
            fileSystem[dirName].Item1 + fileSystem[dirName].Item2.Sum(x => GetDirectorySize(  dirName + "/" + x, fileSystem));


        private static IEnumerable<(string, int)> GetSmallDirectories(IDictionary<string, (int, IEnumerable<string>)> fileSystem) =>
            fileSystem.Where(x => x.Key != "/").Select(x => (x.Key, GetDirectorySize(x.Key, fileSystem)))
                        .Where(x => x.Key != "/")
                        .Where(x => x.Item2 <= 100000);

        private static IEnumerable<(string, int)> GetLargerDirectories(IDictionary<string, (int, IEnumerable<string>)> fileSystem) =>
            fileSystem.Where(x => x.Key != "/").Select(x => (x.Key, GetDirectorySize(x.Key, fileSystem)))
                .Where(x => x.Key != "/")
                .Where(x => x.Item2 >= 8381165);

        private static int GetSumOfSmallerDirectories(string input) =>
            input.Bind(ParseCommands)
                .Bind(ScanFileSystem)
                .Bind(x => GetSmallDirectories(x.Directories))
                .Sum(x => x.Item2);

        private static int GetSmallestDirectoryToDelete(string input) =>
            input.Bind(ParseCommands)
                .Bind(ScanFileSystem)
                .Bind(x => GetLargerDirectories(x.Directories))
                .Min(x => x.Item2);

        [Fact]
        public void Test01()
        {
            const string input = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";

            var result = ParseCommands(input).ToArray();

            result[0].Should().BeEquivalentTo(MakeCdCommand("/"));
            result[1].Should().BeEquivalentTo(MakeLsCommand(
                        MakeSubDirectory("a"),
                        MakeDirectoryFile("b.txt", 14848514),
                        MakeDirectoryFile("c.dat", 8504156),
                        MakeSubDirectory("d")
                    ));
            result[2].Should().BeEquivalentTo(MakeCdCommand("a"));
            result[3].Should().BeEquivalentTo(MakeLsCommand(
                                MakeSubDirectory("e"),
                                MakeDirectoryFile("f", 29116),
                                MakeDirectoryFile("g", 2557),
                                MakeDirectoryFile("h.lst", 62596)
                                ));
            result[4].Should().BeEquivalentTo(MakeCdCommand("e"));
            result[5].Should().BeEquivalentTo(MakeLsCommand(
                        MakeDirectoryFile("i", 584)
                        ));
            result[6].Should().BeEquivalentTo(MakeCdCommand(".."));
            result[7].Should().BeEquivalentTo(MakeCdCommand(".."));
            result[8].Should().BeEquivalentTo(MakeCdCommand("d"));
            result[9].Should().BeEquivalentTo(MakeLsCommand(
                        MakeDirectoryFile("j", 4060174),
                        MakeDirectoryFile("d.log", 8033020),
                        MakeDirectoryFile("d.ext", 5626152),
                        MakeDirectoryFile("k", 7214296)));

        }

        [Fact]
        public void Test02()
        {
            const string input = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";

            var parsedCommands = ParseCommands(input);
            var fileSystem = ScanFileSystem(parsedCommands);
            var ld = GetSmallDirectories(fileSystem.Directories);
            ld.Sum(x => x.Item2).Should().Be(95437);
        }

        [Fact]
        public void Day07_PartA()
        {
            var input = File.ReadAllText("./Day07.txt");
            var result = GetSumOfSmallerDirectories(input);
            result.Should().Be(1667443);
        }

        [Fact]
        public void Test03()
        {
            const string input = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";

            var result = GetSmallestDirectoryToDelete(input);
            result.Should().Be(8998590);
        }


        [Fact]
        public void Day07_PartB()
        {
            var input = File.ReadAllText("./Day07.txt");
            var result = GetSmallestDirectoryToDelete(input);
            result.Should().Be(1667443);
        }

    }
}
