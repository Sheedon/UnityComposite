using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 计算从起点出发、在给定最大 Cost 内可以到达的全部六边形节点。
     */
    public static class CostRange
    {
        /**
         * 搜索最大 Cost 内的所有可达节点及其最小到达代价。
         * @param graph 提供节点和邻接关系的图。
         * @param traversalRule 提供通行判断和正整数边 Cost 的规则。
         * @param start 搜索起点。
         * @param maximumCost 允许使用的最大总代价，必须大于或等于 0。
         * @return 可达坐标与最小 Cost；无效起点返回 IsStartValid 为 false 的空结果。
         */
        public static HexRangeResult Find(
            IHexGraph graph,
            IHexTraversalRule traversalRule,
            HexCoord start,
            int maximumCost)
        {
            HexSearchUtility.ValidateDependencies(graph, traversalRule);
            if (maximumCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumCost), maximumCost,
                    "Maximum cost cannot be negative.");
            }

            var minimumCosts = new Dictionary<HexCoord, int>();
            if (!graph.Contains(start))
            {
                return new HexRangeResult(false, minimumCosts);
            }

            var frontier = new HexPriorityQueue();
            minimumCosts.Add(start, 0);
            frontier.Enqueue(start, 0, 0L);

            while (frontier.TryDequeue(out var current, out var queuedCost))
            {
                if (!minimumCosts.TryGetValue(current, out var currentCost) || currentCost != queuedCost)
                {
                    continue;
                }

                foreach (var neighbor in HexSearchUtility.RequireNeighbors(graph, current))
                {
                    if (!graph.Contains(neighbor) || !traversalRule.CanTraverse(current, neighbor))
                    {
                        continue;
                    }

                    var edgeCost = HexSearchUtility.GetPositiveCost(traversalRule, current, neighbor);
                    var candidateCostValue = (long)currentCost + edgeCost;
                    if (candidateCostValue > maximumCost)
                    {
                        continue;
                    }

                    var candidateCost = (int)candidateCostValue;
                    if (minimumCosts.TryGetValue(neighbor, out var knownCost) && knownCost <= candidateCost)
                    {
                        continue;
                    }

                    minimumCosts[neighbor] = candidateCost;
                    frontier.Enqueue(neighbor, candidateCost, candidateCost);
                }
            }

            return new HexRangeResult(true, minimumCosts);
        }
    }
}
