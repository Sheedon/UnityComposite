using System;

namespace Sheedon.Hex.Tests.Algorithms
{
    /**
     * 为 Algorithms 测试提供可注入的通行与 Cost 规则。
     */
    internal sealed class AlgorithmTestTraversalRule : IHexTraversalRule
    {
        private readonly Func<HexCoord, HexCoord, bool> _canTraverse;
        private readonly Func<HexCoord, HexCoord, int> _getCost;

        public AlgorithmTestTraversalRule(
            Func<HexCoord, HexCoord, bool> canTraverse = null,
            Func<HexCoord, HexCoord, int> getCost = null)
        {
            _canTraverse = canTraverse ?? ((from, to) => true);
            _getCost = getCost ?? ((from, to) => 1);
        }

        public bool CanTraverse(HexCoord from, HexCoord to) => _canTraverse(from, to);

        public int GetCost(HexCoord from, HexCoord to) => _getCost(from, to);
    }
}
