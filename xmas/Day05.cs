namespace xmas
{
    public class Day05
    {
        public record Stacks
        {
            public IEnumerable<char> this[int s] => s switch
            {
                1 => Stack1,
                2 => Stack2,
                3 => Stack3,
                4 => Stack4,
                5 => Stack5,
                6 => Stack6,
                7 => Stack7,
                8 => Stack8,
                9 => Stack9,
                _ => throw new IndexOutOfRangeException("not possible in AoC")
            };

            public IEnumerable<char> Stack1 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack2 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack3 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack4 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack5 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack6 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack7 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack8 = Enumerable.Empty<char>();
            public IEnumerable<char> Stack9 = Enumerable.Empty<char>();
        }

        public static Stacks ParseStacks(IEnumerable<string> input) =>
            input.Select(s => s.Chunk(4))
                .Select(x => x.Select(y => y[1]))
                .SelectMany(x => x.Select((y, i) => (Crane: i + 1, Crate: y)))
                .GroupBy(x => x.Crane)
                .ToDictionary(x => x.Key, x => x.Where(y => y.Crate != ' ')
                                                                                                    .Select(y => y.Crate))
                .Bind(x => new Stacks
                {
                    Stack1 = x.ContainsKey(1) ? x[1] : Enumerable.Empty<char>(),
                    Stack2 = x.ContainsKey(2) ? x[2] : Enumerable.Empty<char>(),
                    Stack3 = x.ContainsKey(3) ? x[3] : Enumerable.Empty<char>(),
                    Stack4 = x.ContainsKey(4) ? x[4] : Enumerable.Empty<char>(),
                    Stack5 = x.ContainsKey(5) ? x[5] : Enumerable.Empty<char>(),
                    Stack6 = x.ContainsKey(6) ? x[6] : Enumerable.Empty<char>(),
                    Stack7 = x.ContainsKey(7) ? x[7] : Enumerable.Empty<char>(),
                    Stack8 = x.ContainsKey(8) ? x[8] : Enumerable.Empty<char>(),
                    Stack9 = x.ContainsKey(9) ? x[9] : Enumerable.Empty<char>()
                });

        public static IEnumerable<(int quantity, int from, int to)> ParseCommands(IEnumerable<string> input) =>
            input.Select(x => x.Split(" ").ToArray())
                .Select(x => (int.Parse(x[1]), int.Parse(x[3]), int.Parse(x[5])));


        public static (Stacks stacks, IEnumerable<(int quantity, int from, int to)> commands) ParseInputString(string input) =>
            input.Split("\r\n\r\n").ToArray()
                .Bind(x => (Stacks: x[0].Split("\r\n")[..^1], Commands: x[1]))
                .Bind(x => (
                    ParseStacks(x.Stacks),
                    ParseCommands(x.Commands.Split("\r\n"))
                ));


        private static IEnumerable<char> TakeCratesFromStack(int quantity, IEnumerable<char> from, IEnumerable<char> to,
            bool isUpgradedCrane) =>
            isUpgradedCrane
                ? from.Take(quantity).Concat(to)
                : from.Take(quantity).Reverse().Concat(to);


        public static Stacks ImplementCommand(Stacks stacks, (int q, int from, int to) command, bool isUpgradedCrane = false) =>
            stacks with
            {
                Stack1 = (command.from == 1 ? stacks.Stack1.Skip(command.q) : command.to == 1 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack1).ToArray(),
                Stack2 = (command.from == 2 ? stacks.Stack2.Skip(command.q) : command.to == 2 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack2).ToArray(),
                Stack3 = (command.from == 3 ? stacks.Stack3.Skip(command.q) : command.to == 3 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack3).ToArray(),
                Stack4 = (command.from == 4 ? stacks.Stack4.Skip(command.q) : command.to == 4 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack4).ToArray(),
                Stack5 = (command.from == 5 ? stacks.Stack5.Skip(command.q) : command.to == 5 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack5).ToArray(),
                Stack6 = (command.from == 6 ? stacks.Stack6.Skip(command.q) : command.to == 6 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack6).ToArray(),
                Stack7 = (command.from == 7 ? stacks.Stack7.Skip(command.q) : command.to == 7 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack7).ToArray(),
                Stack8 = (command.from == 8 ? stacks.Stack8.Skip(command.q) : command.to == 8 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack8).ToArray(),
                Stack9 = (command.from == 9 ? stacks.Stack9.Skip(command.q) : command.to == 9 ? TakeCratesFromStack(command.q, stacks[command.from], stacks[command.to], isUpgradedCrane) : stacks.Stack9).ToArray(),
            };

        public static string GetMessage(string input, bool isUpgradedCrane = false) =>
            ParseInputString(input)
                .Bind(x =>
                    x.commands.Aggregate(
                        x.stacks,
                        (acc, y) => ImplementCommand(acc, y, isUpgradedCrane)
                    )
                ).Bind(x => new string(new[] {
                    x.Stack1.Any() ? x.Stack1.First() : ' ',
                    x.Stack2.Any() ? x.Stack2.First() : ' ',
                    x.Stack3.Any() ? x.Stack3.First() : ' ',
                    x.Stack4.Any() ? x.Stack4.First() : ' ',
                    x.Stack5.Any() ? x.Stack5.First() : ' ',
                    x.Stack6.Any() ? x.Stack6.First() : ' ',
                    x.Stack7.Any() ? x.Stack7.First() : ' ',
                    x.Stack8.Any() ? x.Stack8.First() : ' ',
                    x.Stack9.Any() ? x.Stack9.First() : ' ',


                })
                ).Trim();



        [Fact]
        public void Test01()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";

            var stackInput = input.Split("\r\n\r\n").First().Split("\r\n");


            var result = ParseStacks(stackInput.Take(stackInput.Length - 1));

            result.Stack1.Should().BeEquivalentTo(new[] { 'N', 'Z' });
            result.Stack2.Should().BeEquivalentTo(new[] { 'D', 'C', 'M' });
            result.Stack3.Should().BeEquivalentTo(new[] { 'P' });
        }

        [Fact]
        public void Test02()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var commandInput = input.Split("\r\n\r\n").Last().Split("\r\n");
            var commands = ParseCommands(commandInput);
            commands.Should().BeEquivalentTo(new[]
            {
                (1, 2, 1),
                (3, 1, 3),
                (2, 2, 1),
                (1, 1, 2)
            });
        }

        [Fact]
        public void Test03()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var data = ParseInputString(input);
            var update1 = ImplementCommand(data.stacks, data.commands.First());
            update1.Stack1.Should().BeEquivalentTo(new[] { 'D', 'N', 'Z' });
            update1.Stack2.Should().BeEquivalentTo(new[] { 'C', 'M' });
            update1.Stack3.Should().BeEquivalentTo(new[] { 'P' });
        }

        [Fact]
        public void Test04()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var data = ParseInputString(input);
            var commands = data.commands.ToArray();
            var update1 = ImplementCommand(data.stacks, data.commands.First());
            var update2 = ImplementCommand(update1, commands[1]);
            update2.Stack1.Should().BeEquivalentTo(Enumerable.Empty<char>());
            update2.Stack2.Should().BeEquivalentTo(new[] { 'C', 'M' });
            update2.Stack3.Should().BeEquivalentTo(new[] { 'Z', 'N', 'D', 'P' });
        }

        [Fact]
        public void Test05()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var data = ParseInputString(input);
            var commands = data.commands.ToArray();
            var update1 = ImplementCommand(data.stacks, data.commands.First());
            var update2 = ImplementCommand(update1, commands[1]);
            var update3 = ImplementCommand(update2, commands[2]);
            update3.Stack1.Should().BeEquivalentTo(new[] { 'M', 'C' });
            update3.Stack2.Should().BeEquivalentTo(Enumerable.Empty<char>());
            update3.Stack3.Should().BeEquivalentTo(new [] { 'Z', 'N', 'D', 'P'});
        }

        [Fact]
        public void Test06()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var data = ParseInputString(input);
            var commands = data.commands.ToArray();
            var update1 = ImplementCommand(data.stacks, data.commands.First());
            var update2 = ImplementCommand(update1, commands[1]);
            var update3 = ImplementCommand(update2, commands[2]);
            var update4 = ImplementCommand(update3, commands[3]);
            update4.Stack1.Should().BeEquivalentTo(new[] { 'C' });
            update4.Stack2.Should().BeEquivalentTo(new[] { 'M' });
            update4.Stack3.Should().BeEquivalentTo(new[] { 'Z', 'N', 'D', 'P' });
        }

        [Fact]
        public void Test07()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var finalMessage = GetMessage(input);
            finalMessage.Should().Be("CMZ");
        }

        [Fact]
        public void Day05_PartA()
        {
            var input = File.ReadAllText("./Day05.txt");
            var finalMessage = GetMessage(input);
            finalMessage.Should().Be("WSFTMRHPP");
        }

        [Fact]
        public void Test08()
        {
            const string input = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";
            var finalMessage = GetMessage(input, true);
            finalMessage.Should().Be("MCD");
        }

        [Fact]
        public void Day05_PartB()
        {
            var input = File.ReadAllText("./Day05.txt");
            var finalMessage = GetMessage(input, true);
            finalMessage.Should().Be("GSLCMFBRP");
        }

    }


}

