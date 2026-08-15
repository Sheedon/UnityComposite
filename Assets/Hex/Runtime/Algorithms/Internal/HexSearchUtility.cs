using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 集中保存搜索算法共享的契约校验、代价累加和路径重建逻辑。
     */
    internal static class HexSearchUtility
    {
        public static void ValidateDependencies(IHexGraph graph, IHexTraversalRule traversalRule)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (traversalRule == null)
            {
                throw new ArgumentNullException(nameof(traversalRule));
            }
        }

        public static IEnumerable<HexCoord> RequireNeighbors(IHexGraph graph, HexCoord coord)
        {
            var neighbors = graph.GetNeighbors(coord);
            if (neighbors == null)
            {
                throw new InvalidOperationException($"Graph returned null neighbors for coordinate {coord}.");
            }

            return neighbors;
        }

        public static int GetPositiveCost(IHexTraversalRule traversalRule, HexCoord from, HexCoord to)
        {
            var cost = traversalRule.GetCost(from, to);
            if (cost <= 0)
            {
                throw new InvalidOperationException(
                    $"Traversal cost from {from} to {to} must be greater than zero, but was {cost}.");
            }

            return cost;
        }

        public static int AddCost(int currentCost, int edgeCost, HexCoord from, HexCoord to)
        {
            var totalCost = (long)currentCost + edgeCost;
            if (totalCost > int.MaxValue)
            {
                throw new OverflowException($"Traversal cost overflowed Int32 while moving from {from} to {to}.");
            }

            return (int)totalCost;
        }

        public static HexPathResult BuildPath(
            IReadOnlyDictionary<HexCoord, HexCoord> parents,
            HexCoord start,
            HexCoord goal,
            int totalCost)
        {
            var reversedPath = new List<HexCoord> { goal };
            var current = goal;

            while (current != start)
            {
                current = parents[current];
                reversedPath.Add(current);
            }

            reversedPath.Reverse();
            return HexPathResult.Success(reversedPath.ToArray(), totalCost);
        }
    }
}
