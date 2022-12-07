namespace xmas
{
    public class Day04
    {
        private static (IEnumerable<int>, IEnumerable<int>) ParseAssignedSections(string input) =>
            input.Split(",")
                .Select(x => x.Split("-").ToArray())
                .Select(x => (Start: int.Parse(x[0]), End: int.Parse(x[1])))
                .Select(x => Enumerable.Range(x.Start, x.End - x.Start + 1)).ToArray()
                .Bind(x => (x[0], x[1]));

        private static bool HasFullyOverlappingAssignments((IEnumerable<int>, IEnumerable<int>) input) =>
            input.Item1.All(x => input.Item2.Contains(x)) ||
            input.Item2.All(x => input.Item1.Contains(x));

        private static bool HasAnyOverlappingAssignments((IEnumerable<int>, IEnumerable<int>) input) =>
            input.Item1.Any(x => input.Item2.Contains(x)) ||
            input.Item2.Any(x => input.Item1.Contains(x));

        private static int CountFullyOverlappingAssignments(string input) =>
            input.Split("\r\n")
                .Select(ParseAssignedSections)
                .Count(HasFullyOverlappingAssignments);

        private static int CountAllOverlappingAssignments(string input) =>
            input.Split("\r\n")
                .Select(ParseAssignedSections)
                .Count(HasAnyOverlappingAssignments);


        [Theory]
        [InlineData("2-4,6-8", new[] { 2, 3, 4 }, new[] { 6, 7, 8 })]
        [InlineData("2-3,4-5", new[] { 2, 3 }, new[] { 4, 5 })]
        [InlineData("5-7,7-9", new[] { 5, 6, 7 }, new[] { 7, 8, 9 })]
        [InlineData("2-8,3-7", new[] { 2, 3, 4, 5, 6, 7, 8 }, new[] { 3, 4, 5, 6, 7 })]
        [InlineData("6-6,4-6", new[] { 6 }, new[] { 4, 5, 6 })]
        [InlineData("2-6,4-8", new[] { 2, 3, 4, 5, 6 }, new[] { 4, 5, 6, 7, 8 })]
        public void Test01(string input, int[] expectedOutput1, int[] expectedOutput2)
        {
            var answer = ParseAssignedSections(input);
            answer.Should().BeEquivalentTo((expectedOutput1, expectedOutput2 ) );
        }

        [Theory]
        [InlineData("2-4,6-8", false)]
        [InlineData("2-3,4-5", false)]
        [InlineData("5-7,7-9", false)]
        [InlineData("2-8,3-7", true)]
        [InlineData("6-6,4-6", true)]
        [InlineData("2-6,4-8", false)]
        public void Test2(string input, bool expectedAnswer)
        {
            var sections = ParseAssignedSections(input);
            var answer = HasFullyOverlappingAssignments(sections);
            answer.Should().Be(expectedAnswer);
        }

        [Fact]
        public void Day04_PartA()
        {
            var input = File.ReadAllText("./Day04.txt");
            var answer = CountFullyOverlappingAssignments(input);
            answer.Should().Be(602);
        }

        [Theory]
        [InlineData("2-4,6-8", false)]
        [InlineData("2-3,4-5", false)]
        [InlineData("5-7,7-9", true)]
        [InlineData("2-8,3-7", true)]
        [InlineData("6-6,4-6", true)]
        [InlineData("2-6,4-8", true)]
        public void Test3(string input, bool expectedAnswer)
        {
            var sections = ParseAssignedSections(input);
            var answer = HasAnyOverlappingAssignments(sections);
            answer.Should().Be(expectedAnswer);
        }

        [Fact]
        public void Day04_PartB()
        {
            var input = File.ReadAllText("./Day04.txt");
            var answer = CountAllOverlappingAssignments(input);
            answer.Should().Be(891);
        }
    }
}
