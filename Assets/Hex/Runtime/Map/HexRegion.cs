using System;
using System.Collections;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 表示有限六边形地图中实际存在的坐标集合，并允许动态增加或移除格子。
     */
    public sealed class HexRegion : IEnumerable<HexCoord>
    {
        private readonly Dictionary<HexCoord, long> _membershipTokens;
        private long _nextMembershipToken;

        /**
         * 创建一个不包含任何格子的空区域。
         */
        public HexRegion()
        {
            _membershipTokens = new Dictionary<HexCoord, long>();
        }

        /**
         * 使用给定坐标创建区域，重复坐标只会保留一次。
         * @param coordinates 初始坐标集合。
         */
        public HexRegion(IEnumerable<HexCoord> coordinates)
            : this()
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException(nameof(coordinates));
            }

            foreach (var coord in coordinates)
            {
                Add(coord);
            }
        }

        /**
         * 当前区域包含的格子数量。
         */
        public int Count => _membershipTokens.Count;

        /**
         * 判断坐标是否属于当前区域。
         * @param coord 要查询的坐标。
         * @return 如果坐标属于区域则返回 true。
         */
        public bool Contains(HexCoord coord) => _membershipTokens.ContainsKey(coord);

        /**
         * 向区域加入一个坐标。
         * @param coord 要加入的坐标。
         * @return 如果坐标此前不存在并成功加入则返回 true。
         */
        public bool Add(HexCoord coord)
        {
            if (_membershipTokens.ContainsKey(coord))
            {
                return false;
            }

            var membershipToken = checked(_nextMembershipToken + 1L);
            _membershipTokens.Add(coord, membershipToken);
            _nextMembershipToken = membershipToken;
            return true;
        }

        /**
         * 从区域移除一个坐标。
         * @param coord 要移除的坐标。
         * @return 如果坐标此前存在并成功移除则返回 true。
         */
        public bool Remove(HexCoord coord) => _membershipTokens.Remove(coord);

        internal bool TryGetMembershipToken(HexCoord coord, out long membershipToken) =>
            _membershipTokens.TryGetValue(coord, out membershipToken);

        /**
         * 获取区域坐标的泛型枚举器。
         * @return 坐标枚举器。
         */
        public IEnumerator<HexCoord> GetEnumerator() => _membershipTokens.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
