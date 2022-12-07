namespace xmas
{
    public class Day01
    {
        private static int MostCalories(string input) =>
            input.Split("\r\n\r\n")
                .Select(x => x.Split("\r\n"))
                .Select(x => x.Select(int.Parse))
                .Max(x => x.Sum());

        private static int TopThreeCalories(string input) =>
            input.Split("\r\n\r\n")
                .Select(x => x.Split("\r\n"))
                .Select(x => x.Select(int.Parse))
                .Select(x => x.Sum())
                .OrderByDescending(x => x)
                .Take(3)
                .Sum();


        [Fact]
        public void Test1()
        {
            const string input = @"1000                              
2000
3000

5000
6000

7000
8000
9000

10000";

            var mostCalories = MostCalories(input);
            mostCalories.Should().Be(24000);

        }

        [Fact]
        public void Test2()
        {
            const string input = @"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000";

            var topThree = TopThreeCalories(input);
            topThree.Should().Be(45000);
        }

        [Fact]
        public void Day01_partA()
        {
            var input = File.ReadAllText("./Day01.txt");
            var answer = MostCalories(input);
            answer.Should().Be(64929);
        }

        [Fact]
        public void Day01_partB()
        {
            var input = File.ReadAllText("./Day01.txt");
            var answer = TopThreeCalories(input);
            answer.Should().Be(193697);
        }
    }
}