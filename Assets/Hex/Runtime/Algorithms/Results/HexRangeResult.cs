using System.Collections;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 保存 CostRange 搜索得到的可达坐标以及到达每个坐标的最小代价。
     */
    public sealed class HexRangeResult : IEnumerable<KeyValuePair<HexCoord, int>>
    {
        private readonly Dictionary<HexCoord, int> _minimumCosts;

        internal HexRangeResult(bool isStartValid, Dictionary<HexCoord, int> minimumCosts)
        {
            IsStartValid = isStartValid;
            _minimumCosts = minimumCosts;
        }

        /**
         * 起点是否是图中的有效节点；无效起点会得到空结果。
         */
        public bool IsStartValid { get; }

        /**
         * 当前结果包含的可达坐标数量。
         */
        public int Count => _minimumCosts.Count;

        /**
         * 枚举所有可达坐标。
         */
        public IEnumerable<HexCoord> Coordinates => _minimumCosts.Keys;

        /**
         * 判断指定坐标是否在给定 Cost 范围内可达。
         * @param coord 要查询的坐标。
         * @return 如果结果包含该坐标则返回 true。
         */
        public bool Contains(HexCoord coord) => _minimumCosts.ContainsKey(coord);

        /**
         * 获取到达指定坐标的最小代价；坐标不可达时抛出 KeyNotFoundException。
         * @param coord 要查询的可达坐标。
         * @return 从起点到该坐标的最小代价。
         */
        public int GetMinimumCost(HexCoord coord) => _minimumCosts[coord];

        /**
         * 尝试获取到达指定坐标的最小代价。
         * @param coord 要查询的坐标。
         * @param minimumCost 查询成功时返回最小代价，否则返回 0。
         * @return 如果该坐标在范围内可达则返回 true。
         */
        public bool TryGetMinimumCost(HexCoord coord, out int minimumCost) =>
            _minimumCosts.TryGetValue(coord, out minimumCost);

        /**
         * 获取可达坐标与最小代价的枚举器。
         * @return 结果条目枚举器。
         */
        public IEnumerator<KeyValuePair<HexCoord, int>> GetEnumerator() => _minimumCosts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
