using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace xmas
{
    public class Day09_PartOne
    {
        public record Rope
        {
            public IEnumerable<(int, int)> TailPath{ get; set; }

            public (int x, int y) Tail { get; set; }
            public (int x, int y) Head { get; set; }
        }

        public static Rope MoveTail(Rope r) =>
            ((deltaX: r.Head.x - r.Tail.x, deltaY: r.Head.y - r.Tail.y) switch
            {
                { deltaX: 2 } => r with { Tail = (r.Tail.x + 1, r.Head.y) },
                { deltaY: 2} => r with {Tail = (r.Head.x, r.Tail.y + 1) },
                { deltaX: -2} => r with { Tail = (r.Tail.x - 1, r.Head.y)  },
                { deltaY: -2 } => r with { Tail = (r.Head.x, r.Tail.y - 1) },
                _ => r
            }).Bind(x => x with {TailPath = x.TailPath.Append(x.Tail).Distinct().ToArray()} );
                
                
                                                  
        public static Rope MoveHead(Rope r, char Direction) =>
            Direction switch
            {
                'R' => r with { Head = (r.Head.x + 1, r.Head.y)},
                'U' => r with {Head = (r.Head.x, r.Head.y + 1)},
                'L' => r with { Head = (r.Head.x - 1, r.Head.y)},
                'D' => r with { Head = (r.Head.x, r.Head.y - 1)},
                _ => r
            };


        public static Rope MakeMove(Rope r, string instruction) =>
            Enumerable.Repeat(instruction[0], int.Parse(instruction[1..]))
                .Aggregate(r, (agg, x) => MoveHead(agg, x).Bind(MoveTail));

        public static Rope MakeAllMoves(Rope r, string input) =>
            input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(r, MakeMove);


                    [Fact]
        public void Test01()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (3, 1),
                Tail = (2, 1),
                TailPath = Enumerable.Empty<(int, int)>()
            }, "R1");

            newPos.Head.Should().Be((4, 1));
            newPos.Tail.Should().Be((3, 1));
            newPos.TailPath.Should().BeEquivalentTo(new[] { newPos.Tail });
        }

        [Fact]
        public void Test02()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (0, 0),
                Tail = (0, 0),
                TailPath = new[] { (0, 0) }
            }, "R4");

            newPos.Head.Should().Be((4, 0));
            newPos.Tail.Should().Be((3, 0));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (0, 0), (1, 0), (2, 0), (3, 0)  });
        }

        [Fact]
        public void Test03()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (4, 0),
                Tail = (3, 0),
                TailPath = new[] { (3, 0) }
            }, "U4");

            newPos.Head.Should().Be((4, 4));
            newPos.Tail.Should().Be((4, 3));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (3, 0), (4, 1), (4, 2), (4, 3)  });
        }

        [Fact]
        public void Test04()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (4, 4),
                Tail = (4, 3),
                TailPath = new[] { (4, 3) }
            }, "L3");

            newPos.Head.Should().Be((1, 4));
            newPos.Tail.Should().Be((2, 4));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (4, 3),  (3, 4), (2, 4) });
        }

        [Fact]
        public void Test05()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (1, 4),
                Tail = (2, 4),
                TailPath = new[] { (2, 4) }
            }, "D1");

            newPos.Head.Should().Be((1, 3));
            newPos.Tail.Should().Be((2, 4));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (2, 4) });
        }

        [Fact]
        public void Test06()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (1, 3),
                Tail = (2, 4),
                TailPath = new[] { (2, 4) }
            }, "R4");

            newPos.Head.Should().Be((5, 3));
            newPos.Tail.Should().Be((4, 3));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (2, 4), (3, 3), (4, 3) });
        }

        [Fact]
        public void Test07()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (5, 4),
                Tail = (4, 3),
                TailPath = new[] { (4, 3) }
            }, "D1");

            newPos.Head.Should().Be((5, 3));
            newPos.Tail.Should().Be((4, 3));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (4, 3) });
        }

        [Fact]
        public void Test08()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (5, 2),
                Tail = (4, 3),
                TailPath = new[] { (4, 3) }
            }, "L5");

            newPos.Head.Should().Be((0, 2));
            newPos.Tail.Should().Be((1, 2));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (4, 3), (3, 2), (2, 2), (1, 2) });
        }

        [Fact]
        public void Test09()
        {
            var newPos = MakeMove(new Rope
            {
                Head = (0, 2),
                Tail = (1, 2),
                TailPath = new[] { (1, 2) }
            }, "R2");

            newPos.Head.Should().Be((2, 2));
            newPos.Tail.Should().Be((1, 2));
            newPos.TailPath.Should().BeEquivalentTo(new[] { (1, 2) });
        }

        [Fact]
        public void Test10()
        {
            const string instructions = @"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2";


            var newPos = MakeAllMoves(new Rope
            {
                Head = (0, 0),
                Tail = (0, 0),
                TailPath = new[] { (0, 0) }
            }, instructions);

            newPos.Head.Should().Be((2, 2));
            newPos.Tail.Should().Be((1, 2));

            newPos.TailPath.Count().Should().Be(13);
        }

        [Fact]
        public void Day09_PartA()
        {
            var input = File.ReadAllText("./Day09.txt");
            var newPos = MakeAllMoves(new Rope
            {
                Head = (0, 0),
                Tail = (0, 0),
                TailPath = new[] { (0, 0) }
            }, input);

            newPos.TailPath.Count().Should().Be(5513);
        }
    }


    public class Day09_PartTwo
    {
        public class Rope
        {
            public IEnumerable<(int, int)> TailPath { get; set; } = new[] { (0, 0) };

            public IEnumerable<(int, int)> RopeSegments { get; set; } = new[] { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) };
        }

        public static (int x, int y) MoveHead((int x, int y) r, char Direction) =>
            Direction switch
            {
                'R' => (r.x + 1, r.y),
                'U' => (r.x, r.y+1),
                'L' => (r.x - 1, r.y),
                'D' => (r.x, r.y-1),
                _ => r
            };


        public static IEnumerable<(int, int)> MoveTail(IEnumerable<(int x, int y)> r, (int x, int y) oldSegment) =>
            r.ToArray()
                .Bind(r1 =>
                    r1.Append(
                        (deltaX: r1[^1].x - oldSegment.x, deltaY: r1[^1].y - oldSegment.y) switch
                        {
                            { deltaX: 2, deltaY: 2} => (oldSegment.x + 1, oldSegment.y + 1),
                            { deltaX: 2, deltaY: -2 } => (oldSegment.x + 1, oldSegment.y - 1),
                            { deltaX: -2, deltaY: 2 } => (oldSegment.x - 1, oldSegment.y + 1),
                            { deltaX: -2, deltaY: -2 } => (oldSegment.x - 1, oldSegment.y - 1),

                            { deltaX: 2 } => (oldSegment.x + 1, r1[^1].y),
                            { deltaY: 2 } => (r1[^1].x, oldSegment.y + 1),
                            { deltaX: -2 } => (oldSegment.x - 1, r1[^1].y),
                            { deltaY: -2 } => (r1[^1].x, oldSegment.y - 1),
                            _ => oldSegment
                        })
                );

        public static Rope MakeMove(Rope r, string instruction) =>
            Enumerable.Repeat(instruction[0], int.Parse(instruction[1..]))
                .Aggregate(r, (agg, x) => 
                        
                        MoveHead(agg.RopeSegments.First(), x)
                            .Bind(newHead => 
                                    agg.RopeSegments.Skip(1).Aggregate(new [] { newHead}, (agg2, x2) => MoveTail(agg2, x2).ToArray()))
                            .Bind(r1 => new Rope
                            {
                                RopeSegments = r1,
                                TailPath = agg.TailPath.Append(r1[^1]).Distinct().ToArray()
                            }));

        public static Rope MakeAllMoves(string instructions) =>
            instructions.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(new Rope(), MakeMove);

        [Fact]
        public void Test01()
        {
            var updatedRope = MakeMove(new Rope(), "R 1");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((1, 0));
            rope[1..].Should().AllBeEquivalentTo((0, 0));

        }

        [Fact]
        public void Test02()
        {
            var updatedRope = MakeMove(new Rope(), "R 2");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((2, 0));
            rope[1].Should().Be((1, 0));
            rope[2..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test03()
        {
            var updatedRope = MakeMove(new Rope(), "R 4");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((4, 0));
            rope[1].Should().Be((3, 0));
            rope[2].Should().Be((2, 0));
            rope[3].Should().Be((1, 0));
            rope[4..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test04()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((4, 4));
            rope[1].Should().Be((4, 3));
            rope[2].Should().Be((4, 2));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test05()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4\r\nL 3");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((1, 4));
            rope[1].Should().Be((2, 4));
            rope[2].Should().Be((3, 3));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test06()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4\r\nL 3\r\nD 1");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((1, 3));
            rope[1].Should().Be((2, 4));
            rope[2].Should().Be((3, 3));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test07()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((5, 3));
            rope[1].Should().Be((4, 3));
            rope[2].Should().Be((3, 3));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test08()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4\r\nD 1");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((5, 2));
            rope[1].Should().Be((4, 3));
            rope[2].Should().Be((3, 3));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test09()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4\r\nD 1\r\nL 5");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((0, 2));
            rope[1].Should().Be((1, 2));
            rope[2].Should().Be((2, 2));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }

        [Fact]
        public void Test10()
        {
            var updatedRope = MakeAllMoves("R 4\r\nU 4\r\nL 3\r\nD 1\r\nR 4\r\nD 1\r\nL 5\r\nR 2");
            var rope = updatedRope.RopeSegments.ToArray();
            rope[0].Should().Be((2, 2));
            rope[1].Should().Be((1, 2));
            rope[2].Should().Be((2, 2));
            rope[3].Should().Be((3, 2));
            rope[4].Should().Be((2, 2));
            rope[5].Should().Be((1, 1));
            rope[6..].Should().AllBeEquivalentTo((0, 0));
        }


        [Fact]
        public void Test11()
        {
            const string input = @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20";
            var updatedRope = MakeAllMoves(input);
            var segments = updatedRope.RopeSegments.ToArray();

            segments[0].Should().Be((-11, 15));
            segments[1].Should().Be((-11, 14));
            segments[2].Should().Be((-11, 13));
            segments[3].Should().Be((-11, 12));
            segments[4].Should().Be((-11, 11));
            segments[5].Should().Be((-11, 10));
            segments[6].Should().Be((-11, 9));
            segments[7].Should().Be((-11, 8));
            segments[8].Should().Be((-11, 7));

            updatedRope.TailPath.Should().HaveCount(36);
        }

        [Fact]
        public void Test12()
        {
            const string input = @"R 5";
            var updatedRope = MakeAllMoves(input);
            var segments = updatedRope.RopeSegments.ToArray();

            segments[0].Should().Be((5, 0));
            segments[1].Should().Be((4, 0));
            segments[2].Should().Be((3, 0));
            segments[3].Should().Be((2, 0));
            segments[4].Should().Be((1, 0));
            segments[5..].Should().AllBeEquivalentTo((0, 0));

            updatedRope.TailPath.Should().HaveCount(1);
        }

        [Fact]
        public void Test13()
        {
            const string input = @"R 5
U 8";
            var updatedRope = MakeAllMoves(input);
            var segments = updatedRope.RopeSegments.ToArray();

            segments[0].Should().Be((5, 8));
            segments[1].Should().Be((5, 7));
            segments[2].Should().Be((5, 6));
            segments[3].Should().Be((5, 5));
            segments[4].Should().Be((5, 4));
            segments[5].Should().Be((4, 4));
            segments[6].Should().Be((3, 3));
            segments[7].Should().Be((2, 2));
            segments[8].Should().Be((1, 1));
            segments[9].Should().Be((0, 0));


            updatedRope.TailPath.Should().HaveCount(1);
        }

        [Fact]
        public void Test14()
        {
            const string input = @"R 5
U 8
L 8";
            var updatedRope = MakeAllMoves(input);
            var segments = updatedRope.RopeSegments.ToArray();

            segments[0].Should().Be((-3, 8));
            segments[1].Should().Be((-2, 8));
            segments[2].Should().Be((-1, 8));
            segments[3].Should().Be((0, 8));
            segments[4].Should().Be((1, 8));
            segments[5].Should().Be((1, 7));
            segments[6].Should().Be((1, 6));
            segments[7].Should().Be((1, 5));
            segments[8].Should().Be((1, 4));
            segments[9].Should().Be((1, 3));


            updatedRope.TailPath.Should().HaveCount(4);
        }

        [Fact]
        public void Day09_PartB()
        {
            var input = File.ReadAllText("./Day09.txt");
            var updatedRope = MakeAllMoves(input);

            updatedRope.TailPath.Should().HaveCount(2427);
        }

    }
}
