

namespace xmas.DayTwelveC
{
    public record Grid(IEnumerable<(int x, int y, char height, int weight)> Nodes)
    {
        public (int x, int y, char height, int weight) this[int x, int y] => this.Nodes.SingleOrDefault(n => n.x == x && n.y == y);
    }

    public record Queue
    {
        public IEnumerable<(int x, int y, char height, int weight)> CurrentQueue { get; set; }
        public IEnumerable<(int x, int y, char height, int weight)> AlreadySeen { get; set; }
    }

    public class Day12b
    {
        public static int ClimbingValue(char c) => c switch
        {
            'S' => 0,
            'E' => 26,
            _ => c - 96
        };

        public static bool IsClimbable(char from, char to) =>
            ClimbingValue(from) - ClimbingValue(to) <= 1;

        public static Grid ParseGrid(string input) =>
            input.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany((y, yI) =>
                    y.Select((x, xI) => (xI, yI, x, x == 'S' ? 1 : -1))
                ).Bind(x => new Grid(x));

        public static int CalculateStepsToHighestPoint(string input)
        {
            var grid = ParseGrid(input).Nodes.ToArray();
            var start = grid.Single(x => x.height == 'E');
            var initialQueue = new Queue
            {
                CurrentQueue = new[] { (start.x, start.y, start.height, 0) },
                AlreadySeen = new[] { (start.x, start.y, start.height, 0) }
            };

            var finalQueue = initialQueue.IterateUntil(x => x.AlreadySeen.Any(y => y.height == 'S') || !x.CurrentQueue.Any(),
                q =>
                {

                    var currentItem = q.CurrentQueue.FirstOrDefault();
                    var neighbours = new[]
                    {
                        grid.SingleOrDefault(x => x.x == currentItem.x + 1 && x.y == currentItem.y),
                        grid.SingleOrDefault(x => x.x == currentItem.x - 1 && x.y == currentItem.y),
                        grid.SingleOrDefault(x => x.x == currentItem.x && x.y == currentItem.y + 1),
                        grid.SingleOrDefault(x => x.x == currentItem.x && x.y == currentItem.y - 1),
                    }.ToArray();

                    var neighboursToAdd = neighbours.Where(n =>
                            n != default && IsClimbable(currentItem.height, n.height) &&
                            !q.AlreadySeen.Any(z => z.x == n.x && z.y == n.y))
                        .Select(x => (x.x, x.y, x.height, currentItem.weight + 1))
                        .ToArray();

                    var newQ = new Queue
                    {
                        CurrentQueue = q.CurrentQueue.Skip(1).Concat(neighboursToAdd).ToArray(),
                        AlreadySeen = q.AlreadySeen.Concat(neighboursToAdd).ToArray()
                    };

                    return newQ;

                });

            return finalQueue.AlreadySeen.Single(x => x.height == 'S').weight;
        }




        [Fact]
        public void Test01()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var result = CalculateStepsToHighestPoint(input);
            result.Should().Be(31);
        }

        [Fact]
        public void Day12_PartA()
        {
            var input = File.ReadAllText("./Day12.txt");
            var result = CalculateStepsToHighestPoint(input);
            result.Should().Be(425);
        }
    }
}