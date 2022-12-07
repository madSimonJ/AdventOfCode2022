namespace xmas
{
    public class Day06
    {
        private static int FindPacketMarker(string input, int stringSize = 4) =>
            Enumerable.Range(0, input.Length - stringSize)
                .Select(x => (index: x, a: input.Skip(x).Take(stringSize)))
                .First(x => x.a.Distinct().Count() == stringSize)
                .index + stringSize;

        [Theory]
        [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7)]
        [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 5)]
        [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 6)]
        [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10)]
        [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11)]
        public void Test01(string input, int expectedAnswer)
        {
            var result = FindPacketMarker(input);
            result.Should().Be(expectedAnswer);
        }

        [Fact]
        public void Day06_PartA()
        {
            var input = File.ReadAllText("./Day06.txt");
            var result = FindPacketMarker(input);
            result.Should().Be(1929);
        }

        [Theory]
        [InlineData("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19)]
        [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 23)]
        [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 23)]
        [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29)]
        [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26)]
        public void Test02(string input, int expectedResult)
        {
            var result = FindPacketMarker(input, 14);
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Day06_PartB()
        {
            var input = File.ReadAllText("./Day06.txt");
            var result = FindPacketMarker(input, 14);
            result.Should().Be(3298);
        }
    }
}
