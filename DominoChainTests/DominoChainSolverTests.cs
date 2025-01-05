using Xunit;

namespace DominoChainTests
{
    public class DominoChainSolverTests
    {
        [Theory]
        [MemberData(nameof(GetValidDominoTestCases))]
        public void FindChain_WithValidCircularDominos_ReturnsValidChain(List<Domino> dominos)
        {
            var result = DominoChainSolver.FindCircularChain(dominos);
            Assert.NotNull(result);
            Assert.True(IsCircular(result));
        }

        [Fact]
        public void FindChain_WithNullInput_ReturnsNull()
        {
            var result = DominoChainSolver.FindCircularChain(null);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1, 1)] // Self-circular
        [InlineData(1, 2)] // Non-circular
        public void FindChain_WithSingleDomino_ReturnsExpectedResult(int first, int second)
        {
            var dominos = new List<Domino> { new(first, second) };
            var result = DominoChainSolver.FindCircularChain(dominos);

            if (first == second)
                Assert.NotNull(result);
            else
                Assert.Null(result);
        }

        private bool IsCircular(IReadOnlyList<Domino> chain) =>
            chain.Count > 0 && chain[0].First == chain[^1].Second;

        public static IEnumerable<object[]> GetValidDominoTestCases()
        {
            yield return new object[]
            {
            new List<Domino> { new(2, 1), new(2, 3), new(1, 3) }
            };

            yield return new object[]
            {
            new List<Domino> { new(6, 3), new(3, 4), new(4, 6) }
            };

            yield return new object[]
            {
            new List<Domino> { new(1, 1), new(1, 2), new(2, 1) }
            };

            yield return new object[]
            {
            new List<Domino> { new(5, 2), new(2, 3), new(3, 4), new(4, 5) }
            };
        }

        public static IEnumerable<object[]> GetInvalidDominoTestCases()
        {
            yield return new object[]
            {
            new List<Domino> { new(1, 4), new(2, 3), new(4, 5) }
            };

            yield return new object[]
            {
            new List<Domino> { new(1, 2), new(4, 1), new(2, 3) }
            };

            yield return new object[]
            {
            new List<Domino> { new(1, 2), new(3, 4), new(5, 6) }
            };
        }
    }

    public record Domino(int First, int Second)
    {
        public Domino Flip() => new(Second, First);
    }

    public class DominoChainSolver
    {
        public static IReadOnlyList<Domino>? FindCircularChain(IReadOnlyList<Domino>? dominos)
        {
            if (dominos == null || !dominos.Any())
                return null;

            var used = new bool[dominos.Count];
            var currentChain = new List<Domino>();

            return TryBuildChain(dominos, used, currentChain) ? currentChain : null;
        }

        private static bool TryBuildChain(
            IReadOnlyList<Domino> dominos,
            bool[] used,
            List<Domino> currentChain)
        {
            if (currentChain.Count == dominos.Count)
                return IsCircular(currentChain);

            int lastValue = currentChain.Count == 0 ? -1 : currentChain[^1].Second;

            for (int i = 0; i < dominos.Count; i++)
            {
                if (used[i]) continue;

                var domino = dominos[i];
                if (lastValue == -1 || domino.First == lastValue)
                {
                    used[i] = true;
                    currentChain.Add(domino);
                    if (TryBuildChain(dominos, used, currentChain))
                        return true;
                    used[i] = false;
                    currentChain.RemoveAt(currentChain.Count - 1);
                }

                if (lastValue == -1 || domino.Second == lastValue)
                {
                    used[i] = true;
                    currentChain.Add(domino.Flip());
                    if (TryBuildChain(dominos, used, currentChain))
                        return true;
                    used[i] = false;
                    currentChain.RemoveAt(currentChain.Count - 1);
                }
            }

            return false;
        }

        private static bool IsCircular(IReadOnlyList<Domino> chain) =>
            chain[0].First == chain[^1].Second;
    }


}