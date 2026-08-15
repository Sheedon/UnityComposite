using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 为 Dijkstra 与 A* 共享加权最短路径搜索实现。
     */
    internal static class WeightedPathSearch
    {
        public static HexPathResult FindPath(
            IHexGraph graph,
            IHexTraversalRule traversalRule,
            HexCoord start,
            HexCoord goal,
            Func<HexCoord, HexCoord, int> estimateCost)
        {
            HexSearchUtility.ValidateDependencies(graph, traversalRule);
            if (estimateCost == null)
            {
                throw new ArgumentNullException(nameof(estimateCost));
            }

            if (!graph.Contains(start))
            {
                return HexPathResult.Failure(HexPathStatus.InvalidStart);
            }

            if (!graph.Contains(goal))
            {
                return HexPathResult.Failure(HexPathStatus.InvalidGoal);
            }

            if (start == goal)
            {
                return HexPathResult.Success(new[] { start }, 0);
            }

            var frontier = new HexPriorityQueue();
            var minimumCosts = new Dictionary<HexCoord, int> { [start] = 0 };
            var parents = new Dictionary<HexCoord, HexCoord>();
            frontier.Enqueue(start, 0, GetEstimate(estimateCost, start, goal));

            while (frontier.TryDequeue(out var current, out var queuedCost))
            {
                if (!minimumCosts.TryGetValue(current, out var currentCost) || currentCost != queuedCost)
                {
                    continue;
                }

                if (current == goal)
                {
                    return HexSearchUtility.BuildPath(parents, start, goal, currentCost);
                }

                foreach (var neighbor in HexSearchUtility.RequireNeighbors(graph, current))
                {
                    if (!graph.Contains(neighbor) || !traversalRule.CanTraverse(current, neighbor))
                    {
                        continue;
                    }

                    var edgeCost = HexSearchUtility.GetPositiveCost(traversalRule, current, neighbor);
                    var candidateCost = HexSearchUtility.AddCost(currentCost, edgeCost, current, neighbor);
                    if (minimumCosts.TryGetValue(neighbor, out var knownCost) && knownCost <= candidateCost)
                    {
                        continue;
                    }

                    minimumCosts[neighbor] = candidateCost;
                    parents[neighbor] = current;
                    var priority = (long)candidateCost + GetEstimate(estimateCost, neighbor, goal);
                    frontier.Enqueue(neighbor, candidateCost, priority);
                }
            }

            return HexPathResult.Failure(HexPathStatus.NoPath);
        }

        private static int GetEstimate(
            Func<HexCoord, HexCoord, int> estimateCost,
            HexCoord from,
            HexCoord goal)
        {
            var estimate = estimateCost(from, goal);
            if (estimate < 0)
            {
                throw new InvalidOperationException(
                    $"Estimated cost from {from} to {goal} cannot be negative, but was {estimate}.");
            }

            return estimate;
        }
    }
}
