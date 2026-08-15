using System;

namespace Sheedon.Hex
{
    /**
     * 使用正整数边 Cost 和启发函数在六边形图上搜索最低总代价路径。
     */
    public static class AStar
    {
        /**
         * 搜索从起点到目标的最低 Cost 路径；未提供启发函数时默认使用 Hex Distance。
         * 自定义启发值必须非负，并且只有在不高估真实剩余 Cost 时才保证结果最优。
         * @param graph 提供节点和邻接关系的图。
         * @param traversalRule 提供通行判断和正整数边 Cost 的规则。
         * @param start 起点坐标。
         * @param goal 目标坐标。
         * @param estimateCost 可选的剩余 Cost 估算回调，参数依次为当前节点和目标。
         * @return 路径状态、完整路径和最低总代价。
         */
        public static HexPathResult FindPath(
            IHexGraph graph,
            IHexTraversalRule traversalRule,
            HexCoord start,
            HexCoord goal,
            Func<HexCoord, HexCoord, int> estimateCost = null) =>
            WeightedPathSearch.FindPath(
                graph,
                traversalRule,
                start,
                goal,
                estimateCost ?? HexTopology.Distance);
    }
}
