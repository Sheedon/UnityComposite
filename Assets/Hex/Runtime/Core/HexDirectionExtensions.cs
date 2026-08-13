using System;

namespace Sheedon.Hex
{
    /**
     * 为六边形方向提供反向、前进、回退等常用拓扑辅助方法。
     */
    public static class HexDirectionExtensions
    {
        private const int DirectionCount = 6;

        /**
         * 获取当前方向的相反方向，例如东与西互为相反方向。
         * @param direction 当前方向。
         * @return 与当前方向相反的方向。
         */
        public static HexDirection Opposite(this HexDirection direction)
        {
            Validate(direction);
            return (HexDirection)(((int)direction + 3) % DirectionCount);
        }

        /**
         * 获取当前方向顺时针（按照枚举顺序）下一格的方向。
         * @param direction 当前方向。
         * @return 下一方向。
         */
        public static HexDirection Next(this HexDirection direction)
        {
            Validate(direction);
            return (HexDirection)(((int)direction + 1) % DirectionCount);
        }

        /**
         * 获取当前方向前一个方向，用于逆时针步进。
         * @param direction 当前方向。
         * @return 前一方向。
         */
        public static HexDirection Previous(this HexDirection direction)
        {
            Validate(direction);
            return (HexDirection)(((int)direction + DirectionCount - 1) % DirectionCount);
        }

        /**
         * 将方向转换为其在枚举数组中的索引位置。
         * @param direction 当前方向。
         * @return 对应的索引值。
         */
        internal static int ToIndex(this HexDirection direction)
        {
            Validate(direction);
            return (int)direction;
        }

        /**
         * 校验方向值是否属于合法的六个方向之一。
         * @param direction 待校验的方向值。
         */
        private static void Validate(HexDirection direction)
        {
            if ((int)direction < 0 || (int)direction >= DirectionCount)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), direction,
                    "Direction must be one of the six defined hex directions.");
            }
        }
    }
}