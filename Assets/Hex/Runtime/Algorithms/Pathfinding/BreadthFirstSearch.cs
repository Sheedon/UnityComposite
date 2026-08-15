using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 在无权六边形图上按最少边数搜索路径，并使用遍历规则过滤不可通行边。
     */
    public static class BreadthFirstSearch
    {
        /**
         * 搜索从起点到目标的最少步数路径。
         * @param graph 提供节点和邻接关系的图。
         * @param traversalRule 决定边是否可通行的规则；BFS 不读取边 Cost。
         * @param start 起点坐标。
         * @param goal 目标坐标。
         * @return 路径状态、完整路径和以边数表示的总代价。
         */
        public static HexPathResult FindPath(
            IHexGraph graph,
            IHexTraversalRule traversalRule,
            HexCoord start,
            HexCoord goal)
        {
            HexSearchUtility.ValidateDependencies(graph, traversalRule);

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

            var frontier = new Queue<HexCoord>();
            var visited = new HashSet<HexCoord> { start };
            var parents = new Dictionary<HexCoord, HexCoord>();
            frontier.Enqueue(start);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                foreach (var neighbor in HexSearchUtility.RequireNeighbors(graph, current))
                {
                    if (!graph.Contains(neighbor) || visited.Contains(neighbor) ||
                        !traversalRule.CanTraverse(current, neighbor))
                    {
                        continue;
                    }

                    visited.Add(neighbor);
                    parents.Add(neighbor, current);

                    if (neighbor == goal)
                    {
                        var stepCount = GetStepCount(parents, start, goal);
                        return HexSearchUtility.BuildPath(parents, start, goal, stepCount);
                    }

                    frontier.Enqueue(neighbor);
                }
            }

            return HexPathResult.Failure(HexPathStatus.NoPath);
        }

        private static int GetStepCount(
            IReadOnlyDictionary<HexCoord, HexCoord> parents,
            HexCoord start,
            HexCoord goal)
        {
            var stepCount = 0;
            var current = goal;
            while (current != start)
            {
                current = parents[current];
                stepCount = checked(stepCount + 1);
            }

            return stepCount;
        }
    }
}
