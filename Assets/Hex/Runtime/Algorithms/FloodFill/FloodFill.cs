using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 按图邻接关系和通行规则查找从起点可到达的完整连通区域。
     */
    public static class FloodFill
    {
        /**
         * 以稳定的广度优先顺序返回从起点可到达的全部节点。
         * @param graph 提供节点和邻接关系的图。
         * @param traversalRule 决定边是否允许通过的规则；不会读取边 Cost。
         * @param start 搜索起点。
         * @return 包含起点的可达节点；起点无效时返回空集合。
         */
        public static IReadOnlyList<HexCoord> FindConnected(
            IHexGraph graph,
            IHexTraversalRule traversalRule,
            HexCoord start)
        {
            HexSearchUtility.ValidateDependencies(graph, traversalRule);
            if (!graph.Contains(start))
            {
                return Array.Empty<HexCoord>();
            }

            var frontier = new Queue<HexCoord>();
            var visited = new HashSet<HexCoord> { start };
            var connected = new List<HexCoord> { start };
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
                    connected.Add(neighbor);
                    frontier.Enqueue(neighbor);
                }
            }

            return connected.ToArray();
        }
    }
}
