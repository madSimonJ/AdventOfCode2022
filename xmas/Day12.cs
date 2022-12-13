using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace xmas
{
    public class Day12
    {
        public record Grid(IEnumerable<(int x, int y, char height, int weight)> Nodes, bool Finished)
        {
            public (int x, int y, char height, int weight) this[int x, int y] => this.Nodes.SingleOrDefault(n => n.x == x && n.y == y);
        }

        public static int ClimbingValue(char c) => c switch
        {
            'S' => 0,
            'E' => 27,
            _ => c - 96
        };

        public static bool IsClimbable(char from, char to) =>
            ClimbingValue(to) - ClimbingValue(from) <= 1;

        public static Grid DijkstraPass(Grid g)
        {
            var maxWeight = g.Nodes.Max(x => x.weight);
            var edgeNodes = g.Nodes.Where(x => x.weight == maxWeight).ToArray();

            var newNodesF = edgeNodes.SelectMany(x => new[]
            {
                g[x.x + 1, x.y],
                g[x.x - 1, x.y],
                g[x.x, x.y + 1],
                g[x.x, x.y - 1]
            });

            var newNodes = edgeNodes.SelectMany(x => new []{
                g[x.x + 1, x.y],
                    g[x.x - 1, x.y],
                    g[x.x, x.y + 1],
                    g[x.x, x.y - 1]
            }.Where(y => y != default && y.weight == -1 && !edgeNodes.Any(z => y.x == z.x && y.y == z.y) &&  IsClimbable(x.height, y.height))).Distinct();

            var updatedNewNodes = newNodes.Select(nn => (
                nn.x,
                nn.y,
                nn.height,
                maxWeight + 1
                )).ToArray();

            var updatedGrid = new Grid(
                g.Nodes.Select(x =>
                    updatedNewNodes.Any(y => x.x == y.x && x.y == y.y) ? updatedNewNodes.Single(y => x.x == y.x && x.y == y.y) : x
                    ).ToArray(), !updatedNewNodes.Any()
            );

            return updatedGrid;

        }

        public static Grid DijkstraPasses(Grid g, int num) =>
            Enumerable.Repeat(0, num).Aggregate(g, (agg, x) => DijkstraPass(agg));

        public static Grid ParseGrid(string input) =>
            input.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany((y, yI) =>
                    y.Select((x, xI) => (xI, yI, x, x == 'S' ? 1 : -1))
                ).Bind(x => new Grid(x, false));

        public static Grid FindPaths(Grid g) => g.IterateUntil(x => x.Finished, DijkstraPass);

        public static int CalculateStepsToHighestPoint(string input) =>
            input.Bind(ParseGrid)
                .Bind(FindPaths)
                .Nodes.Single(x => x.height == 'E').weight - 1;

        [Fact]
        public void Test01()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var result = ParseGrid(input);
            result[0, 0].Should().Be((0, 0, 'S', 1));
            result[7, 4].Should().Be((7, 4, 'i', -1));
            result[5, 2].Should().Be((5, 2, 'E', -1));
        }

        [Fact]
        public void Test02()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPass(grid);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0,1].weight.Should().Be(2);
        }

        [Fact]
        public void Test03()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPass(grid);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);
        }

        [Theory]
        [InlineData('a', 1)]
        [InlineData('z', 26)]
        [InlineData('S', 0)]
        [InlineData('E', 27)]
        public void Test04(char input, int expectedResult) =>
            ClimbingValue(input).Should().Be(expectedResult);

        [Theory]
        [InlineData('a', 'b', true)]
        [InlineData('a', 'c', false)]
        [InlineData('b', 'a', true)]
        [InlineData('z', 'A', true)]
        public void Test05(char from, char to, bool expectedResult) =>
            IsClimbable(from, to).Should().Be(expectedResult);

        [Fact]
        public void Test06()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 2);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);

            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);
        }

        [Fact]
        public void Test07()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 3);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);

            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);

            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
        }

        [Fact]
        public void Test08()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 4);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);

            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);

            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(-1);

            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);

        }

        [Fact]
        public void Test09()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 5);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);

            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);

            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(-1);

            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);


            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);
        }

        [Fact]
        public void Test10()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 6);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);

            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);

            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(-1);

            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);


            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);

            result[2, 4].weight.Should().Be(7);
        }

        [Fact]
        public void Test11()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 7);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);

            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);

            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(-1);

            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);


            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);

            result[2, 4].weight.Should().Be(7);
            
            result[3, 4].weight.Should().Be(8);
        }

        [Fact]
        public void Test12()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 8);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);
            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);
            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(-1);
            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);
            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);
            result[2, 4].weight.Should().Be(7);
            result[3, 4].weight.Should().Be(8);

            result[4, 4].weight.Should().Be(9);
        }

        [Fact]
        public void Test13()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 9);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);
            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);
            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(-1);
            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);
            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);
            result[2, 4].weight.Should().Be(7);
            result[3, 4].weight.Should().Be(8);
            result[4, 4].weight.Should().Be(9);
            
            result[5, 4].weight.Should().Be(10);
        }


        [Fact]
        public void Test14()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 32);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);
            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);
            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(20);
            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);
            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);
            result[2, 4].weight.Should().Be(7);
            result[3, 4].weight.Should().Be(8);
            result[4, 4].weight.Should().Be(9);

            result[5, 4].weight.Should().Be(10);
            result[4, 1].weight.Should().Be(30);
            result[4, 2].weight.Should().Be(31);
        }

        [Fact]
        public void Test15()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = DijkstraPasses(grid, 34);

            result[0, 0].weight.Should().Be(1);
            result[1, 0].weight.Should().Be(2);
            result[0, 1].weight.Should().Be(2);
            result[0, 2].weight.Should().Be(3);
            result[1, 1].weight.Should().Be(3);
            result[2, 0].weight.Should().Be(3);
            result[0, 3].weight.Should().Be(4);
            result[2, 1].weight.Should().Be(4);
            result[1, 2].weight.Should().Be(4);
            result[0, 3].weight.Should().Be(4);
            result[3, 0].weight.Should().Be(20);
            result[0, 4].weight.Should().Be(5);
            result[1, 3].weight.Should().Be(5);
            result[2, 2].weight.Should().Be(5);
            result[1, 4].weight.Should().Be(6);
            result[2, 3].weight.Should().Be(6);
            result[2, 4].weight.Should().Be(7);
            result[3, 4].weight.Should().Be(8);
            result[4, 4].weight.Should().Be(9);

            result[5, 4].weight.Should().Be(10);
            result[4, 1].weight.Should().Be(30);
            result[4, 2].weight.Should().Be(31);
            result[5, 2].weight.Should().Be(32);
        }

        [Fact]
        public void Test16()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var grid = ParseGrid(input);
            var result = FindPaths(grid);
            result.Nodes.Single(x => x.height == 'E').weight.Should().Be(32);

        }

        [Fact]
        public void Test17()
        {
            const string input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
            var result = CalculateStepsToHighestPoint(input);
            result.Should().Be(31);
        }

        [Fact]
        public void Day12_PartA()
        {
            var input = File.ReadAllText("./Day12.txt");
            var grid = ParseGrid(input);
            var result = FindPaths(grid);

            var str = result.Nodes.GroupBy(x => x.y)
                .Select(x => x.Select(y => (y.x, y: x.Key, y.weight)))
                .Select(x => string.Join("\t", x.Select(y => $"{y.weight}")))
                .Bind(x => string.Join("\r\n", x));
                

            var end = result.Nodes.Single(x => x.height == 'E');
            end.weight.Should().Be(425);
        }
    }

}
