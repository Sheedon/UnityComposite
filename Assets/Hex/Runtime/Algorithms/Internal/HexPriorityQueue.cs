using System;
using System.Collections.Generic;

namespace Sheedon.Hex
{
    /**
     * 为加权搜索提供稳定的最小二叉堆；相同优先级按加入顺序取出。
     */
    internal sealed class HexPriorityQueue
    {
        private readonly List<Entry> _entries = new List<Entry>();
        private long _nextSequence;

        public void Enqueue(HexCoord coord, int pathCost, long priority)
        {
            var entry = new Entry(coord, pathCost, priority, _nextSequence);
            _nextSequence = checked(_nextSequence + 1L);
            _entries.Add(entry);
            SiftUp(_entries.Count - 1);
        }

        public bool TryDequeue(out HexCoord coord, out int pathCost)
        {
            if (_entries.Count == 0)
            {
                coord = default(HexCoord);
                pathCost = 0;
                return false;
            }

            var first = _entries[0];
            var lastIndex = _entries.Count - 1;
            var last = _entries[lastIndex];
            _entries.RemoveAt(lastIndex);

            if (_entries.Count > 0)
            {
                _entries[0] = last;
                SiftDown(0);
            }

            coord = first.Coord;
            pathCost = first.PathCost;
            return true;
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                var parentIndex = (index - 1) / 2;
                if (Compare(_entries[parentIndex], _entries[index]) <= 0)
                {
                    return;
                }

                Swap(parentIndex, index);
                index = parentIndex;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                var leftIndex = checked((index * 2) + 1);
                if (leftIndex >= _entries.Count)
                {
                    return;
                }

                var rightIndex = leftIndex + 1;
                var smallestIndex = rightIndex < _entries.Count &&
                                    Compare(_entries[rightIndex], _entries[leftIndex]) < 0
                    ? rightIndex
                    : leftIndex;

                if (Compare(_entries[index], _entries[smallestIndex]) <= 0)
                {
                    return;
                }

                Swap(index, smallestIndex);
                index = smallestIndex;
            }
        }

        private void Swap(int firstIndex, int secondIndex)
        {
            var temporary = _entries[firstIndex];
            _entries[firstIndex] = _entries[secondIndex];
            _entries[secondIndex] = temporary;
        }

        private static int Compare(Entry first, Entry second)
        {
            var priorityComparison = first.Priority.CompareTo(second.Priority);
            return priorityComparison != 0
                ? priorityComparison
                : first.Sequence.CompareTo(second.Sequence);
        }

        private readonly struct Entry
        {
            public Entry(HexCoord coord, int pathCost, long priority, long sequence)
            {
                Coord = coord;
                PathCost = pathCost;
                Priority = priority;
                Sequence = sequence;
            }

            public HexCoord Coord { get; }

            public int PathCost { get; }

            public long Priority { get; }

            public long Sequence { get; }
        }
    }
}
