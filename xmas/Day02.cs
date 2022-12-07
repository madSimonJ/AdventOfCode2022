namespace xmas
{
    public class Day02
    {
        public enum ScissorPaperStone
        {
            Scissor = 3,
            Paper = 2,
            Stone = 1
        }

        public enum GameResult
        {
            Win = 6,
            Lose = 0,
            Draw = 3
        }

        public static ScissorPaperStone CalculateMyMove((ScissorPaperStone him, string me) x) =>
            x switch
            {
                (var him, "Y") => him,
                var (him, me) when (me == "X" && him == ScissorPaperStone.Paper) ||
                                   (me == "Z" && him == ScissorPaperStone.Scissor)
                    => ScissorPaperStone.Stone,
                var (him, me) when (me == "X" && him == ScissorPaperStone.Scissor) ||
                                   (me == "Z" && him == ScissorPaperStone.Stone)
                    => ScissorPaperStone.Paper,
                var (him, me) when (me == "X" && him == ScissorPaperStone.Stone) ||
                                   (me == "Z" && him == ScissorPaperStone.Paper)
                    => ScissorPaperStone.Scissor,
                _ => throw new NotImplementedException(),
            };

        public static ScissorPaperStone ParseMyMove((ScissorPaperStone him, string me) x) =>
            x.me switch
            {
                "X" => ScissorPaperStone.Stone,
                "Y" => ScissorPaperStone.Paper,
                "Z" => ScissorPaperStone.Scissor,
                _ => throw new ArgumentOutOfRangeException()
            };

        public static IEnumerable<(ScissorPaperStone, ScissorPaperStone)> ParseTurns(string input, bool calculateMyMove = false) =>
            input.Split("\r\n")
                .Select(x => x.Split(" ").ToArray())
                .Select(x => (
                    x[0] switch
                    {
                        "A" => ScissorPaperStone.Stone,
                        "B" => ScissorPaperStone.Paper,
                        "C" => ScissorPaperStone.Scissor,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    x[1]))
                    
                .Select(x => (
                    x.Item1,
                    calculateMyMove ? CalculateMyMove(x) : ParseMyMove(x)
                ));

        public static GameResult CalculateGameResult((ScissorPaperStone opponentTurn, ScissorPaperStone myTurn) turn) =>
            turn switch
            {
                var (him, me) when me == him => GameResult.Draw,
                var (him, me) when (him == ScissorPaperStone.Stone && me == ScissorPaperStone.Scissor) ||
                                           (him == ScissorPaperStone.Paper && me == ScissorPaperStone.Stone) ||
                                           (him == ScissorPaperStone.Scissor && me == ScissorPaperStone.Paper) => GameResult.Lose,
                _ => GameResult.Win
            };

        public static IEnumerable<int> ScoreRounds(
            IEnumerable<(ScissorPaperStone opponentTurn, ScissorPaperStone myTurn)> turns) =>
            turns.Select(x => (
                him: x.opponentTurn,
                me: x.myTurn,
                result: CalculateGameResult(x)
            )).Select(x => (int)x.me + (int)x.result);


        public static int ScoreGame(string input, bool calculateMyMove = false) =>
            ScoreRounds(ParseTurns(input, calculateMyMove)).Sum();
                

            [Fact]
        public void Test01()
        {
            const string input = @"A Y
B X
C Z";
            var output = ParseTurns(input);
            output.Should().BeEquivalentTo(new[]
            {
                (ScissorPaperStone.Stone, ScissorPaperStone.Paper),
                (ScissorPaperStone.Paper, ScissorPaperStone.Stone),
                (ScissorPaperStone.Scissor, ScissorPaperStone.Scissor)
            });
        }

        [Fact]
        public void Test02()
        {
            const string input = @"A Y
B X
C Z";
            var turns = ParseTurns(input);
            var results = turns.Select(CalculateGameResult);
            results.Should().BeEquivalentTo(new[]
            {
                GameResult.Win,
                GameResult.Lose,
                GameResult.Draw
            });

        }

        [Fact]
        public void Test03()
        {
            const string input = @"A Y
B X
C Z";
            var turns = ParseTurns(input);
            var scores = ScoreRounds(turns);
            scores.Should().BeEquivalentTo(new[]
            {
                8,
                1,
                6
            });
        }

        [Fact]
        public void Test04()
        {
            const string input = @"A Y
B X
C Z";
            var scores = ScoreGame(input);
            scores.Should().Be(15);
        }

        [Fact]
        public void Day02_PartA()
        {
            var input = File.ReadAllText("./Day02.txt");
            var score = ScoreGame(input);
            score.Should().Be(10595);
        }

        [Fact]
        public void Test05()
        {
            const string input = @"A Y
B X
C Z";
            var turns = ParseTurns(input, true);
            turns.Should().BeEquivalentTo(
                new[]
                {
                    (ScissorPaperStone.Stone, ScissorPaperStone.Stone),
                    (ScissorPaperStone.Paper, ScissorPaperStone.Stone),
                    (ScissorPaperStone.Scissor, ScissorPaperStone.Stone)
                }
            );
        }

        [Fact]
        public void Test06()
        {
            const string input = @"A Y
B X
C Z";
            var turns = ParseTurns(input, true);
            var scores = ScoreRounds(turns);
            scores.Should().BeEquivalentTo(new[]
            {
                4,
                1,
                7
            });
        }

        [Fact]
        public void Test07()
        {
            const string input = @"A Y
B X
C Z";
            var scores = ScoreGame(input, true);
            scores.Should().Be(12);
        }

        [Fact]
        public void Day02_PartB()
        {
            var input = File.ReadAllText("./Day02.txt");
            var score = ScoreGame(input, true);
            score.Should().Be(9541);
        }
    }
}
