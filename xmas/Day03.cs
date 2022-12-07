namespace xmas
{
    public class Day03
    {
        public static int GetAllBackpackPriorities(string input) =>
            input.Split("\r\n")
                .Sum(GetBackpackPriority);

        public static int GetBackpackPriority(string input) =>
            SplitBackpack(input)
                .Bind(GetCommonItems)
                .Bind(CalculateItemPriority);


        public static int CalculateItemPriority(char input) =>
            (int)input - (char.IsLower(input) ? 96 : 38);

        public static int CalculateItemPriority(IEnumerable<char> input) =>
            input.Sum(CalculateItemPriority);
        
        public static (string, string) SplitBackpack(string input) => (
            input[..(input.Length / 2)],
            input[(input.Length/2)..]
        );


        public static int GetCommonItemsFromGroups(string input) =>
                SplitElvesIntoGroups(input)
                    .Select(FindCommonItemInGroup)
                    .Sum(CalculateItemPriority);

        public static IEnumerable<(string elf1, string elf2, string elf3)> SplitElvesIntoGroups(string input) =>
            input.Split("\r\n")
                .Chunk(3)
                .Select(x => (x[0], x[1], x[2]));

        public static char FindCommonItemInGroup((string elf1, string elf2, string elf3) elves) =>
            elves.elf1.Intersect(elves.elf2).Intersect(elves.elf3).First();


        public static IEnumerable<char> GetCommonItems((string Section1, string Section2) backpack) =>
            backpack.Section1.Intersect(backpack.Section2);


        [Theory]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp", "vJrwpWtwJgWr", "hcsFMMfFFhFp")]
        [InlineData("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", "jqHRNqRjqzjGDLGL", "rsFMfFZSrLrFZsSL")]
        [InlineData("PmmdzqPrVvPwwTWBwg", "PmmdzqPrV", "vPwwTWBwg")]
        public void Test01(string input, string expected1, string expected2)
        {
            var (r1, r2) = SplitBackpack(input);
            r1.Should().Be(expected1);
            r2.Should().Be(expected2);
        }

        [Theory]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp", new [] {'p'})]
        [InlineData("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", new [] {'L'})]
        [InlineData("PmmdzqPrVvPwwTWBwg", new [] {'P'})]
        [InlineData("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", new [] {'v'})]
        [InlineData("ttgJtRGJQctTZtZT", new [] {'t'})]
        [InlineData("CrZsJsPPZsGzwwsLwLmpwMDw", new [] {'s'})]
        public void Test02(string input, IEnumerable<char> expectedOutput)
        {
            var backpack = SplitBackpack(input);
            var commonItems = GetCommonItems(backpack);
            commonItems.Should().BeEquivalentTo(expectedOutput);
        }

        [Theory]
        [InlineData("vJrwpWtwJgWrhcsFMMfFFhFp", 16)]
        [InlineData("jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", 38)]
        [InlineData("PmmdzqPrVvPwwTWBwg", 42)]
        [InlineData("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", 22)]
        [InlineData("ttgJtRGJQctTZtZT", 20)]
        [InlineData("CrZsJsPPZsGzwwsLwLmpwMDw", 19)]
        public void Test03(string input, int expectedAnswer)
        {
            var answer = GetBackpackPriority(input);
            answer.Should().Be(expectedAnswer);
        }

        [Fact]
        public void Day03_PartA()
        {
            var input = File.ReadAllText("./Day03.txt");
            var answer = GetAllBackpackPriorities(input);
            answer.Should().Be(7831);
        }

        [Fact]
        public void Test04()
        {
            var input = @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw";

            var groupedElves = SplitElvesIntoGroups(input);
            groupedElves.Should().BeEquivalentTo(new[]
            {
                ("vJrwpWtwJgWrhcsFMMfFFhFp", "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", "PmmdzqPrVvPwwTWBwg"),
                ("wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", "ttgJtRGJQctTZtZT", "CrZsJsPPZsGzwwsLwLmpwMDw")
            });
        }

        [Fact]
        public void Test05()
        {
            var input = @"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw";

            var groupedElves = SplitElvesIntoGroups(input).ToArray();
            var commonItem1 = FindCommonItemInGroup(groupedElves[0]);
            var commonItem2 = FindCommonItemInGroup(groupedElves[1]);

            commonItem1.Should().Be('r');
            commonItem2.Should().Be('Z');
        }


        [Fact]
        public void Day03_PartB()
        {
            var input = File.ReadAllText("./Day03.txt");
            var answer = GetCommonItemsFromGroups(input);
            answer.Should().Be(2683);
        }

    }
}
