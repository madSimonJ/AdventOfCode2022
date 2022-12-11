using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace xmas
{
    public class Day08
    {
        public class TreeGrid
        {
            private readonly int[][] Trees;

            public TreeGrid(string input)
            {
                this.Trees = input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(y => y.Select(x => int.Parse(x.ToString())).ToArray()).ToArray();
            }

            public int GetTree(int x, int y) => this.Trees[y][x];

            public IEnumerable<int> GetAllNorth(int x, int y) => Enumerable.Range(0, y).Select(y1 => GetTree(x, y1)).Reverse();
            public IEnumerable<int> GetAllSouth(int x, int y) => Enumerable.Range(y + 1, this.Trees.Length - (y+1)).Select(y1 => GetTree(x, y1));

            public IEnumerable<int> GetAllEast(int x, int y) => Enumerable.Range(x + 1, this.Trees.Length - (x + 1)).Select(x1 => GetTree(x1, y));

            public IEnumerable<int> GetAllWest(int x, int y) => Enumerable.Range(0, x).Select(x1 => GetTree(x1, y)).Reverse();

            public bool IsPerimeter(int x, int y) => x == 0 || y == 0 || x == this.Trees.Length - 1 || y == this.Trees.Length - 1;

            public int GetSize() => this.Trees.Length;

        }

        public static bool IsTreeVisible(TreeGrid tg, int x, int y) =>
            tg.IsPerimeter(x, y) ||
            tg.GetTree(x, y)
                .Bind(t1 =>
                    new[]
                    {
                        tg.GetAllNorth(x, y),
                        tg.GetAllEast(x, y),
                        tg.GetAllSouth(x, y),
                        tg.GetAllWest(x, y)
                    }.Any(t2 => t2.All(t22 => t1 > t22)));

        public static IEnumerable<(int, int)> GetAllTrees(TreeGrid tg) =>
            Enumerable.Range(0, tg.GetSize())
                .SelectMany(ty => Enumerable.Range(0, tg.GetSize()).Select(tx => (tx, ty)));

        public static int CountVisibleTrees(string input) =>
            new TreeGrid(input)
                .Bind(tg => GetAllTrees(tg)
                    .Count(t => IsTreeVisible(tg, t.Item1, t.Item2)));



        public static int HowManyTreesBeforeBlockage(int currentTreeHeight, IEnumerable<int> trees) =>
            trees.ToArray()
                .Bind(t =>
                    t.Select((x, i) => (Tree: x, Count: i + 1))
                        .FirstOrDefault(x => x.Tree >= currentTreeHeight).Count
                        .Bind(x => x == 0 ? t.Length : x)
                    );

        public static int CalculateScenicScore(TreeGrid tg, int x, int y) =>
            new[]
                {
                    tg.GetAllNorth(x, y),
                    tg.GetAllEast(x, y),
                    tg.GetAllSouth(x, y),
                    tg.GetAllWest(x, y)
                }.Select(z => HowManyTreesBeforeBlockage(tg.GetTree(x, y), z))
                .Aggregate(1, (agg, x) => x * agg);


        public static int FindHighestScenicScore(string input) =>
            new TreeGrid(input)
                .Bind(tg =>
                    GetAllTrees(tg)
                        .Max(x => CalculateScenicScore(tg, x.Item1, x.Item2)));
            



                [Theory]
        [InlineData(0, 0, 3)]
        [InlineData(4, 0, 3)]
        [InlineData(0, 4, 3)]
        [InlineData(4, 4, 0)]
        [InlineData(4, 3, 9)]
        public void Test01(int x, int y, int expectedValue)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var result = parsedGrid.GetTree(x, y);
            result.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(1, 1, new[] { 0 })]
        [InlineData(1, 4, new[] { 0, 5, 5, 3 })]
        public void Test02(int x, int y, IEnumerable<int> expectedTrees)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var trees = parsedGrid.GetAllNorth(x, y);
            trees.Should().BeEquivalentTo(expectedTrees);
        }

        [Theory]
        [InlineData(1, 1, new[] { 5, 3, 5 })]
        [InlineData(1, 3, new[] { 5 })]
        [InlineData(3, 0, new[] { 1, 3, 4, 9 })]
        public void Test03(int x, int y, IEnumerable<int> expectedTrees)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var trees = parsedGrid.GetAllSouth(x, y);
            trees.Should().BeEquivalentTo(expectedTrees);
        }

        [Theory]
        [InlineData(0, 1, new[] { 5, 5, 1, 2 })]
        [InlineData(3, 3, new[] { 9 })]
        public void Test04(int x, int y, IEnumerable<int> expectedTrees)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var trees = parsedGrid.GetAllEast(x, y);
            trees.Should().BeEquivalentTo(expectedTrees);
        }

        [Theory]
        [InlineData(0, 1, new int[0])]
        [InlineData(3, 3, new[] { 3, 3, 5 })]
        public void Test05(int x, int y, IEnumerable<int> expectedTrees)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var trees = parsedGrid.GetAllWest(x, y);
            trees.Should().BeEquivalentTo(expectedTrees);
        }

        [Theory]
        [InlineData(1, 1, true)]
        [InlineData(2, 1, true)]
        [InlineData(3, 1, false)]
        [InlineData(1, 2, true)]
        [InlineData(2, 2, false)]
        [InlineData(3, 2, true)]
        [InlineData(1, 3, false)]
        [InlineData(2, 3, true)]
        [InlineData(3, 3, false)]
        public void Test06(int x, int y, bool expectedVisibility)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var result = IsTreeVisible(parsedGrid, x, y);
            result.Should().Be(expectedVisibility);
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0, 1, true)]
        [InlineData(0, 2, true)]
        [InlineData(0, 3, true)]
        [InlineData(0, 4, true)]
        [InlineData(1, 0, true)]
        [InlineData(1, 1, false)]
        [InlineData(1, 2, false)]
        [InlineData(1, 3, false)]
        [InlineData(1, 4, true)]
        public void Test07(int x, int y, bool expectedVisibility)
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var result = parsedGrid.IsPerimeter(x, y);
            result.Should().Be(expectedVisibility);
        }

        [Fact]
        public void Test08()
        {
            const string input = @"30373
25512
65332
33549
35390";

            var visibleTrees = CountVisibleTrees(input);
            visibleTrees.Should().Be(21);
        }

        [Fact]
        public void Test09()
        {
            const string input = @"30373
25512
65332
33549
35390";

            var parsedGrid = new TreeGrid(input);
            var allTrees = GetAllTrees(parsedGrid);
            allTrees.Distinct().ToArray().Length.Should().Be(25);
        }

        [Fact]
        public void Day08_PartA()
        {
            var input = File.ReadAllText("./Day08.txt");
            var result = CountVisibleTrees(input);
            result.Should().Be(1719);
        }

        [Fact]
        public void Test10()
        {
            const string input = @"30373
25512
65332
33549
35390";
            var parsedGrid = new TreeGrid(input);
            var result = CalculateScenicScore(parsedGrid, 2, 1);
            result.Should().Be(4);
        }

        [Theory]
        [InlineData(new[] { 3 }, 1)]
        [InlineData(new[] { 5, 2 }, 1)]
        [InlineData(new[] { 1, 2 }, 2)]
        [InlineData(new[] { 3, 5, 3 }, 2)]
        public void Test11(IEnumerable<int> input, int expectedResult)
        {
            var scenicScore = HowManyTreesBeforeBlockage(5, input);
            scenicScore.Should().Be(expectedResult);
        }


        [Fact]
        public void Day08_PartB()
        {
            var input = File.ReadAllText("./Day08.txt");
            var result = FindHighestScenicScore(input);
            result.Should().Be(590824);
        }

    }
}
