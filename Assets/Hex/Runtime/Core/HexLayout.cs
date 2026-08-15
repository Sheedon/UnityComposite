using System;

namespace Sheedon.Hex
{
    /**
     * 负责在六边形轴向坐标与尖顶布局的二维点坐标之间进行转换，并提供单元格角点计算。
     */
    public sealed class HexLayout
    {
        private static readonly double SquareRootOfThree = Math.Sqrt(3d);

        /**
         * 使用默认原点创建一个六边形布局。
         * @param hexSize 六边形半径或尺寸。
         */
        public HexLayout(double hexSize)
            : this(hexSize, HexPoint.Zero)
        {
        }

        /**
         * 使用指定尺寸和原点创建六边形布局。
         * @param hexSize 六边形尺寸，必须为有限且大于 0 的值。
         * @param origin 布局的原点位置。
         */
        public HexLayout(double hexSize, HexPoint origin)
        {
            if (double.IsNaN(hexSize) || double.IsInfinity(hexSize) || hexSize <= 0d)
            {
                throw new ArgumentOutOfRangeException(nameof(hexSize), hexSize,
                    "Hex size must be finite and greater than zero.");
            }

            if (double.IsNaN(origin.X) || double.IsInfinity(origin.X) ||
                double.IsNaN(origin.Y) || double.IsInfinity(origin.Y))
            {
                throw new ArgumentOutOfRangeException(nameof(origin), origin, "Origin values must be finite.");
            }

            HexSize = hexSize;
            Origin = origin;
        }

        /**
         * 当前布局中单个六边形的尺寸。
         */
        public double HexSize { get; }

        /**
         * 二维空间中的布局原点。
         */
        public HexPoint Origin { get; }

        /**
         * 将六边形坐标转换为对应二维平面上的中心点位置。
         * @param coord 待转换的六边形坐标。
         * @return 对应的二维中心点。
         */
        public HexPoint HexToPoint(HexCoord coord)
        {
            var x = HexSize * (SquareRootOfThree * (coord.Q + (coord.R / 2d)));
            var y = HexSize * (1.5d * coord.R);
            return new HexPoint(x + Origin.X, y + Origin.Y);
        }

        /**
         * 将二维点坐标转换回最接近的六边形格子坐标。
         * @param point 二维点位置。
         * @return 对应的六边形坐标。
         */
        public HexCoord PointToHex(HexPoint point)
        {
            var x = (point.X - Origin.X) / HexSize;
            var y = (point.Y - Origin.Y) / HexSize;
            var q = ((SquareRootOfThree / 3d) * x) - ((1d / 3d) * y);
            var r = (2d / 3d) * y;
            return HexGeometry.Round(q, r);
        }

        /**
         * 获取指定六边形在按逆时针顺序枚举的角点中的某个顶点坐标。
         * @param coord 六边形坐标。
         * @param cornerIndex 角点索引，范围为 0 到 5。
         * @return 对应角点的二维坐标。
         */
        public HexPoint GetCorner(HexCoord coord, int cornerIndex)
        {
            if (cornerIndex < 0 || cornerIndex >= 6)
            {
                throw new ArgumentOutOfRangeException(nameof(cornerIndex), cornerIndex,
                    "Corner index must be from 0 through 5.");
            }

            var center = HexToPoint(coord);
            var angle = (Math.PI / 3d * cornerIndex) + (Math.PI / 6d);
            return new HexPoint(
                center.X + (HexSize * Math.Cos(angle)),
                center.Y + (HexSize * Math.Sin(angle)));
        }
    }
}
