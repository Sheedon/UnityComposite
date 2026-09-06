namespace Sheedon.Hex
{
    /**
     * 描述给定移动规则下某条有向边是否可通行，以及通过该边所需的正整数代价。
     */
    public interface IHexTraversalRule
    {
        /**
         * 判断是否允许从一个相邻节点移动到另一个相邻节点。
         * @param from 当前节点。
         * @param to 目标相邻节点。
         * @return 如果该方向的边允许通行则返回 true。
         */
        bool CanTraverse(HexCoord from, HexCoord to);

        /**
         * 获取通过一条可通行边的代价，返回值必须大于 0。
         * @param from 当前节点。
         * @param to 目标相邻节点。
         * @return 通过该有向边所需的正整数代价。
         */
        int GetCost(HexCoord from, HexCoord to);

        /**
         * 尝试获取从起点移动到目标点所需要的搜索成本。
         *
         * 返回 false 表示当前有向边不可通行；返回 true 时，cost 必须为大于 0 的整数。
         * @param from 当前节点。
         * @param to 目标相邻节点。
         * @param cost 边可通行时返回所需的正整数代价；不可通行时该值不会被算法使用。
         * @return 如果该有向边可通行且能够取得 Cost 则返回 true。
         */
        bool TryGetCost(HexCoord from, HexCoord to, out int cost);
    }
}
