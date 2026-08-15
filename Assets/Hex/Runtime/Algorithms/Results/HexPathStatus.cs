namespace Sheedon.Hex
{
    /**
     * 表示一次路径搜索的最终状态。
     */
    public enum HexPathStatus
    {
        /**
         * 已找到从起点到目标的路径。
         */
        Success = 0,

        /**
         * 起点和目标有效，但不存在可通行路径。
         */
        NoPath = 1,

        /**
         * 起点不是图中的有效节点。
         */
        InvalidStart = 2,

        /**
         * 目标不是图中的有效节点。
         */
        InvalidGoal = 3
    }
}
