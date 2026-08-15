namespace Sheedon.Hex
{
    /**
     * 使用正整数边 Cost 在六边形图上搜索总代价最低的路径。
     */
    public static class Dijkstra
    {
        /**
         * 搜索从起点到目标的最低 Cost 路径。
         * @param graph 提供节点和邻接关系的图。
         * @param traversalRule 提供通行判断和正整数边 Cost 的规则。
         * @param start 起点坐标。
         * @param goal 目标坐标。
         * @return 路径状态、完整路径和最低总代价。
         */
        public static HexPathResult FindPath(
            IHexGraph graph,
            IHexTraversalRule traversalRule,
            HexCoord start,
            HexCoord goal) =>
            WeightedPathSearch.FindPath(graph, traversalRule, start, goal, EstimateZero);

        private static int EstimateZero(HexCoord from, HexCoord goal) => 0;
    }
}
