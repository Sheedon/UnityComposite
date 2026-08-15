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
    }
}
