using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 将一个实时 HexRegion 适配为六边形图，并过滤所有不属于区域的理论邻居。
     */
    public sealed class RegionHexGraph : IHexGraph
    {
        /**
         * 创建绑定到指定区域的图适配器。
         * @param region 用于定义图节点的有限区域。
         */
        public RegionHexGraph(HexRegion region)
        {
            Region = region ?? throw new ArgumentNullException(nameof(region));
        }

        /**
         * 当前图实时使用的有限区域。
         */
        public HexRegion Region { get; }

        /**
         * 判断坐标是否属于当前区域。
         * @param coord 要查询的坐标。
         * @return 如果坐标属于区域则返回 true。
         */
        public bool Contains(HexCoord coord) => Region.Contains(coord);

        /**
         * 获取区域内的六方向邻居，并保持 HexTopology 的稳定方向顺序。
         * @param coord 要查询的节点。
         * @return 当前仍然属于区域的相邻节点。
         */
        public IEnumerable<HexCoord> GetNeighbors(HexCoord coord)
        {
            if (!Region.Contains(coord))
            {
                yield break;
            }

            foreach (var neighbor in HexTopology.GetNeighbors(coord))
            {
                if (Region.Contains(neighbor))
                {
                    yield return neighbor;
                }
            }
        }
    }
}
