using System.Numerics;

namespace xmas
{
    public record Monkey
    {
        public int Number { get; set; }
        public IEnumerable<BigInteger> Items { get; set; }
        public string Operation { get; set; }
        public int Test { get; set; }
        public int IfTrue { get; set; }
        public int IfFalse { get; set; }
        public BigInteger NumberOfItemsInspected { get; set; }
    }



    public class Day11
    {
        public static BigInteger CalculateNewWorry(BigInteger item, string test, int mod) =>
            test.Split(" ")
                .Bind(x => (Operation: x[0], Value: x[1]))
                .Bind(x => (valA: item, valB: x.Value == "old" ? item : BigInteger.Parse(x.Value), x.Operation))
                .Bind(x => x.Operation == "*" ? x.valA * x.valB : x.valA + x.valB)
                .Bind(x => x % mod);

        public static IEnumerable<(BigInteger Item, int Monkey)> CalculateNewItemLocations(Monkey m, bool superAnxious, int mod) =>
        m.Items.Select(i =>
            CalculateNewWorry(i, m.Operation, mod)
                .Bind(x => x / (superAnxious ? 1 : 3))
                .Bind(x => (x, x % m.Test == 0 ? m.IfTrue : m.IfFalse))
            );

        public static IEnumerable<Monkey> DoTurn(IEnumerable<Monkey> monkeys, Monkey m, bool superAnxious)
        {
            var monkeysA = monkeys.ToArray();
            var mod = monkeysA.Aggregate(1, (agg, x) => agg * x.Test);
            var updates = CalculateNewItemLocations(m, superAnxious, mod)
                .GroupBy(x => x.Monkey)
                .ToDictionary(x => x.Key, x => x);

            var returnValue = monkeysA.Select(x =>
                x with
                {
                    Items = x.Number switch
                    {
                        var n when x.Number == m.Number => Enumerable.Empty<BigInteger>(),
                        var n when updates.ContainsKey(n) => x.Items.Concat(updates[x.Number].Select(y => y.Item)),
                        _ => x.Items
                    },
                    NumberOfItemsInspected = x.Number == m.Number ? x.NumberOfItemsInspected + m.Items.Count() : x.NumberOfItemsInspected
                });

            return returnValue;
        }




        public static IEnumerable<Monkey> DoRound(IEnumerable<Monkey> m, bool superAnxious) =>
            m.ToArray().Bind(x =>
                x.Aggregate(x, (agg, y) => DoTurn(agg, agg.Single(z => z.Number == y.Number), superAnxious).ToArray())
            );

        public static IEnumerable<Monkey> DoRounds(IEnumerable<Monkey> m, int times, bool superAnxious = false) =>
            Enumerable.Range(0, times)
                .Aggregate(m, (agg, _) => DoRound(agg, superAnxious).ToArray()).ToArray();

        public static BigInteger GetBusiestMonkeys(string input, bool superAnxious = false, int numberOfRounds = 20) =>
            input.Bind(ParseMonkeys)
                .Bind(x => DoRounds(x, numberOfRounds, superAnxious))
                .OrderByDescending(x => x.NumberOfItemsInspected)
                .Take(2)
                .Aggregate(new BigInteger(1), (agg, x) => agg * x.NumberOfItemsInspected);

        public static Monkey ParseMonkey(string input) =>
            input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray()
                .Bind(x => new Monkey
                {
                    Number = x[0].Replace("Monkey ", "").Replace(":", "").Bind(int.Parse),
                    Items = x[1].Replace("Starting items: ", "").Split(",").Select(BigInteger.Parse),
                    Operation = x[2].Replace("Operation: new = old ", "").Trim(),
                    Test = x[3].Replace("Test: divisible by ", "").Bind(int.Parse),
                    IfTrue = x[4].Replace("    If true: throw to monkey ", "").Bind(int.Parse),
                    IfFalse = x[5].Replace("    If false: throw to monkey ", "").Bind(int.Parse)
                });
                

        public static IEnumerable<Monkey> ParseMonkeys(string input) =>
            input.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseMonkey).ToArray();


        [Fact]
        public void Test01()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var answer = ParseMonkeys(input);
            answer.Should().BeEquivalentTo(new[]
            {
                new Monkey
                {
                    Number = 0,
                    Items = new BigInteger[] { 79, 98 },
                    Operation = "* 19",
                    Test = 23,
                    IfTrue = 2,
                    IfFalse = 3
                },
                new Monkey
                {
                    Number = 1,
                    Items = new BigInteger[] { 54, 65, 75, 74 },
                    Operation = "+ 6",
                    Test = 19,
                    IfTrue = 2,
                    IfFalse = 0
                },
                new Monkey
                {
                    Number = 2,
                    Items = new BigInteger[] { 79, 60, 97 },
                    Operation = "* old",
                    Test = 13,
                    IfTrue = 1,
                    IfFalse = 3
                },
                new Monkey
                {
                    Number = 3,
                    Items = new BigInteger[] { 74 },
                    Operation = "+ 3",
                    Test = 17,
                    IfTrue = 0,
                    IfFalse = 1
                }
            });

        }

        [Fact]
        public void Test02()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 1).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 20, 23, 27, 26 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 2080, 25, 167, 207, 401, 1046 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test03()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 2).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 695, 10, 71, 135, 350 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 43, 49, 58, 55, 362 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }


        [Fact]
        public void Test04()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 3).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 16, 18, 21, 20, 122 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 1468, 22, 150, 286, 739 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test05()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 4).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 491, 9, 52, 97, 248, 34 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 39, 45, 43, 258 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test06()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 5).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 15, 17, 16, 88, 1037 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 20, 110, 205, 524, 72 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test07()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 6).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 8, 70, 176, 26, 34 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 481, 32, 36, 186, 2190 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test08()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 7).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 162, 12, 14, 64, 732, 17 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 148, 372, 55, 72 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test09()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 8).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 51, 126, 20, 26, 136 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 343, 26, 30, 1546, 36 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test10()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 9).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 116, 10, 12, 517, 14 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 108, 267, 43, 55, 288 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test11()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 10).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 91, 16, 20, 98 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 481, 245, 22, 26, 1092, 30 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test12()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 15).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 83, 44, 8, 184, 9, 20, 26, 102 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 110, 36 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test13()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 20).ToArray();

            afterRound.Single(x => x.Number == 0).Items.Should().BeEquivalentTo(new BigInteger[] { 10, 12, 14, 26, 34 });
            afterRound.Single(x => x.Number == 1).Items.Should().BeEquivalentTo(new BigInteger[] { 245, 93, 53, 199, 115 });
            afterRound.Single(x => x.Number == 2).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
            afterRound.Single(x => x.Number == 3).Items.Should().BeEquivalentTo(Enumerable.Empty<BigInteger>());
        }

        [Fact]
        public void Test14()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();
            var afterRound = DoRounds(monkeys, 20).ToArray();

            afterRound[0].NumberOfItemsInspected.Should().Be(101);
            afterRound[1].NumberOfItemsInspected.Should().Be(95);
            afterRound[2].NumberOfItemsInspected.Should().Be(7);
            afterRound[3].NumberOfItemsInspected.Should().Be(105);
        }

        [Fact]
        public void Test15()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var answer = GetBusiestMonkeys(input);
            answer.Should().Be(10605);
        }

        [Fact]
        public void Day11_PartA()
        {
            const string input = "Monkey 0:\r\n  Starting items: 54, 98, 50, 94, 69, 62, 53, 85\r\n  Operation: new = old * 13\r\n  Test: divisible by 3\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 1\r\n\r\nMonkey 1:\r\n  Starting items: 71, 55, 82\r\n  Operation: new = old + 2\r\n  Test: divisible by 13\r\n    If true: throw to monkey 7\r\n    If false: throw to monkey 2\r\n\r\nMonkey 2:\r\n  Starting items: 77, 73, 86, 72, 87\r\n  Operation: new = old + 8\r\n  Test: divisible by 19\r\n    If true: throw to monkey 4\r\n    If false: throw to monkey 7\r\n\r\nMonkey 3:\r\n  Starting items: 97, 91\r\n  Operation: new = old + 1\r\n  Test: divisible by 17\r\n    If true: throw to monkey 6\r\n    If false: throw to monkey 5\r\n\r\nMonkey 4:\r\n  Starting items: 78, 97, 51, 85, 66, 63, 62\r\n  Operation: new = old * 17\r\n  Test: divisible by 5\r\n    If true: throw to monkey 6\r\n    If false: throw to monkey 3\r\n\r\nMonkey 5:\r\n  Starting items: 88\r\n  Operation: new = old + 3\r\n  Test: divisible by 7\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 0\r\n\r\nMonkey 6:\r\n  Starting items: 87, 57, 63, 86, 87, 53\r\n  Operation: new = old * old\r\n  Test: divisible by 11\r\n    If true: throw to monkey 5\r\n    If false: throw to monkey 0\r\n\r\nMonkey 7:\r\n  Starting items: 73, 59, 82, 65\r\n  Operation: new = old + 6\r\n  Test: divisible by 2\r\n    If true: throw to monkey 4\r\n    If false: throw to monkey 3";
            var answer = GetBusiestMonkeys(input);
            answer.Should().Be(112221);
        }

        [Fact]
        public void Test16()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 1, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(2);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(4);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(3);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(6);

        }

        [Fact]
        public void Test17()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 20, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(99);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(97);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(8);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(103);

        }

        [Fact]
        public void Test18()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 1000, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(5204);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(4792);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(199);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(5192);

        }

        [Fact]
        public void Test19()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 2000, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(10419);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(9577);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(392);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(10391);

        }

        [Fact]
        public void Test20()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 3000, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(15638);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(14358);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(587);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(15593);

        }

        [Fact]
        public void Test21()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 4000, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(20858);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(19138);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(780);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(20797 ) ;

        }

        [Fact]
        public void Test22()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var monkeys = ParseMonkeys(input).ToArray();

            var afterRound = DoRounds(monkeys, 10000, true).ToArray();

            afterRound.Single(x => x.Number == 0).NumberOfItemsInspected.Should().Be(52166);
            afterRound.Single(x => x.Number == 1).NumberOfItemsInspected.Should().Be(47830);
            afterRound.Single(x => x.Number == 2).NumberOfItemsInspected.Should().Be(1938);
            afterRound.Single(x => x.Number == 3).NumberOfItemsInspected.Should().Be(52013);

        }

        [Fact]
        public void Test23()
        {
            const string input = "Monkey 0:\r\n  Starting items: 79, 98\r\n  Operation: new = old * 19\r\n  Test: divisible by 23\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 3\r\n\r\nMonkey 1:\r\n  Starting items: 54, 65, 75, 74\r\n  Operation: new = old + 6\r\n  Test: divisible by 19\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 0\r\n\r\nMonkey 2:\r\n  Starting items: 79, 60, 97\r\n  Operation: new = old * old\r\n  Test: divisible by 13\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 3\r\n\r\nMonkey 3:\r\n  Starting items: 74\r\n  Operation: new = old + 3\r\n  Test: divisible by 17\r\n    If true: throw to monkey 0\r\n    If false: throw to monkey 1";
            var answer = GetBusiestMonkeys(input, true, 10000);
            answer.Should().Be(2713310158);

        }


        [Fact]
        public void Day11_PartB()
        {
            const string input = "Monkey 0:\r\n  Starting items: 54, 98, 50, 94, 69, 62, 53, 85\r\n  Operation: new = old * 13\r\n  Test: divisible by 3\r\n    If true: throw to monkey 2\r\n    If false: throw to monkey 1\r\n\r\nMonkey 1:\r\n  Starting items: 71, 55, 82\r\n  Operation: new = old + 2\r\n  Test: divisible by 13\r\n    If true: throw to monkey 7\r\n    If false: throw to monkey 2\r\n\r\nMonkey 2:\r\n  Starting items: 77, 73, 86, 72, 87\r\n  Operation: new = old + 8\r\n  Test: divisible by 19\r\n    If true: throw to monkey 4\r\n    If false: throw to monkey 7\r\n\r\nMonkey 3:\r\n  Starting items: 97, 91\r\n  Operation: new = old + 1\r\n  Test: divisible by 17\r\n    If true: throw to monkey 6\r\n    If false: throw to monkey 5\r\n\r\nMonkey 4:\r\n  Starting items: 78, 97, 51, 85, 66, 63, 62\r\n  Operation: new = old * 17\r\n  Test: divisible by 5\r\n    If true: throw to monkey 6\r\n    If false: throw to monkey 3\r\n\r\nMonkey 5:\r\n  Starting items: 88\r\n  Operation: new = old + 3\r\n  Test: divisible by 7\r\n    If true: throw to monkey 1\r\n    If false: throw to monkey 0\r\n\r\nMonkey 6:\r\n  Starting items: 87, 57, 63, 86, 87, 53\r\n  Operation: new = old * old\r\n  Test: divisible by 11\r\n    If true: throw to monkey 5\r\n    If false: throw to monkey 0\r\n\r\nMonkey 7:\r\n  Starting items: 73, 59, 82, 65\r\n  Operation: new = old + 6\r\n  Test: divisible by 2\r\n    If true: throw to monkey 4\r\n    If false: throw to monkey 3";
            var answer = GetBusiestMonkeys(input, true, 10000);
            answer.Should().Be(25272176808);
        }
    }
}
