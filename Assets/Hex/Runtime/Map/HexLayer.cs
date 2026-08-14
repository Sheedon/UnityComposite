using System;
using System.Collections;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 为指定 HexRegion 中的坐标保存一种泛型数据，且不包含任何游戏业务含义。
     * @param T 每个已赋值坐标承载的数据类型。
     */
    public sealed class HexLayer<T> : IEnumerable<KeyValuePair<HexCoord, T>>
    {
        private readonly Dictionary<HexCoord, Entry> _values = new Dictionary<HexCoord, Entry>();

        /**
         * 创建一个绑定到给定区域的数据层。
         * @param region 定义合法坐标范围的区域。
         */
        public HexLayer(HexRegion region)
        {
            Region = region ?? throw new ArgumentNullException(nameof(region));
        }

        /**
         * 当前数据层使用的坐标区域。
         */
        public HexRegion Region { get; }

        /**
         * 获取指定坐标保存的数据。
         * @param coord 要读取的坐标。
         * @return 坐标对应的数据。
         */
        public T Get(HexCoord coord)
        {
            var membershipToken = RequireMembership(coord);
            if (_values.TryGetValue(coord, out var entry) && entry.MembershipToken == membershipToken)
            {
                return entry.Value;
            }

            _values.Remove(coord);
            throw new KeyNotFoundException($"No value has been assigned to coordinate {coord}.");
        }

        /**
         * 为区域中的指定坐标设置数据；再次设置会覆盖旧值。
         * @param coord 要写入的坐标。
         * @param value 要保存的数据。
         */
        public void Set(HexCoord coord, T value)
        {
            var membershipToken = RequireMembership(coord);
            _values[coord] = new Entry(value, membershipToken);
        }

        /**
         * 尝试获取指定坐标当前保存的数据。
         * @param coord 要查询的坐标。
         * @param value 查询成功时返回数据，否则返回 T 的默认值。
         * @return 坐标属于区域且已经赋值时返回 true。
         */
        public bool TryGet(HexCoord coord, out T value)
        {
            if (!Region.TryGetMembershipToken(coord, out var membershipToken))
            {
                _values.Remove(coord);
                value = default(T);
                return false;
            }

            if (_values.TryGetValue(coord, out var entry) && entry.MembershipToken == membershipToken)
            {
                value = entry.Value;
                return true;
            }

            _values.Remove(coord);
            value = default(T);
            return false;
        }

        /**
         * 判断指定坐标在当前区域成员代次中是否已经保存数据。
         * @param coord 要查询的坐标。
         * @return 如果坐标当前有效且已经赋值则返回 true。
         */
        public bool Contains(HexCoord coord)
        {
            if (!Region.TryGetMembershipToken(coord, out var membershipToken))
            {
                return false;
            }

            return _values.TryGetValue(coord, out var entry) && entry.MembershipToken == membershipToken;
        }

        /**
         * 枚举当前区域内所有已经赋值的坐标与数据。
         * @return 有效数据条目的枚举器。
         */
        public IEnumerator<KeyValuePair<HexCoord, T>> GetEnumerator()
        {
            foreach (var pair in _values)
            {
                if (Region.TryGetMembershipToken(pair.Key, out var membershipToken) &&
                    pair.Value.MembershipToken == membershipToken)
                {
                    yield return new KeyValuePair<HexCoord, T>(pair.Key, pair.Value.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private long RequireMembership(HexCoord coord)
        {
            if (Region.TryGetMembershipToken(coord, out var membershipToken))
            {
                return membershipToken;
            }

            _values.Remove(coord);
            throw new ArgumentOutOfRangeException(nameof(coord), coord, "Coordinate does not belong to the layer region.");
        }

        private readonly struct Entry
        {
            public Entry(T value, long membershipToken)
            {
                Value = value;
                MembershipToken = membershipToken;
            }

            public T Value { get; }

            public long MembershipToken { get; }
        }
    }
}
