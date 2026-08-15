using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 描述六边形图中节点是否存在以及节点之间的邻接关系。
     */
    public interface IHexGraph
    {
        /**
         * 判断指定坐标是否是图中的有效节点。
         * @param coord 要查询的坐标。
         * @return 如果坐标属于图则返回 true。
         */
        bool Contains(HexCoord coord);

        /**
         * 获取指定节点在图中的有效相邻六边形；返回值不得包含图外节点或非相邻坐标。
         * @param coord 要查询的节点。
         * @return 图中与该节点 Hex Distance 为 1 的坐标集合；无效或孤立节点返回空集合。
         */
        IEnumerable<HexCoord> GetNeighbors(HexCoord coord);
    }
}
