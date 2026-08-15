using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 保存路径搜索状态、按顺序排列的完整路径以及路径总代价。
     */
    public sealed class HexPathResult
    {
        private static readonly IReadOnlyList<HexCoord> EmptyPath = Array.Empty<HexCoord>();

        private HexPathResult(HexPathStatus status, IReadOnlyList<HexCoord> path, int totalCost)
        {
            Status = status;
            Path = path;
            TotalCost = totalCost;
        }

        /**
         * 本次搜索的最终状态。
         */
        public HexPathStatus Status { get; }

        /**
         * 成功时包含起点和目标的完整路径；失败时为空集合。
         */
        public IReadOnlyList<HexCoord> Path { get; }

        /**
         * 成功时的路径总代价；失败时为 0。BFS 使用经过的边数作为代价。
         */
        public int TotalCost { get; }

        internal static HexPathResult Success(IReadOnlyList<HexCoord> path, int totalCost) =>
            new HexPathResult(HexPathStatus.Success, path, totalCost);

        internal static HexPathResult Failure(HexPathStatus status)
        {
            if (status == HexPathStatus.Success)
            {
                throw new ArgumentException("A failure result cannot use the Success status.", nameof(status));
            }

            return new HexPathResult(status, EmptyPath, 0);
        }
    }
}
