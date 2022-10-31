﻿using System.Collections.Generic;
using System.ComponentModel;

namespace NVivo.Sorting
{
    /// <summary>
    /// Implementation of the merge sort algorithm.
    /// </summary>
    public sealed class MergeSort<T>
    {
        private IComparer<T> _comparer;
        private bool _isAscending;
        private IList<T> _list;
        private T[] _buffer;

        /// <summary>
        /// Creates a new MergeSort class.
        /// </summary>
        private MergeSort(IList<T> list, IComparer<T> comparer, ListSortDirection direction)
        {
            _list = list;
            _buffer = new T[list.Count];
            _comparer = comparer;
            _isAscending = (direction == ListSortDirection.Ascending);
        }

        /// <summary>
        /// Sorts an IList using the specified comparer and the sort direction.
        /// </summary>
        public static void Sort(IList<T> list, IComparer<T> comparer, ListSortDirection direction)
        {
            MergeSort<T> sort = new MergeSort<T>(list, comparer, direction);
            sort._MergeSort(0, list.Count - 1);
        }

        private int Compare(T x, T y)
        {
            if (_isAscending)
                return _comparer.Compare(x, y);
            else
                return _comparer.Compare(y, x);
        }

        #region Algorithm Implementation

        private void _MergeSort(int firstIndex, int lastIndex)
        {
            int lastRelativeIndex = lastIndex - firstIndex;

            if (lastRelativeIndex < 1)
                return;

            int middle = (lastRelativeIndex / 2) + firstIndex;
            int postMiddle = middle + 1;

            _MergeSort(firstIndex, middle);
            _MergeSort(postMiddle, lastIndex);

            Merge(firstIndex, middle, postMiddle, lastIndex);
        }

        private void Merge(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            int bufferIndex = leftStart;
            int leftIndex = leftStart;
            int rightIndex = rightStart;

            // copy to the buffer the sortable items
            while (leftIndex <= leftEnd && rightIndex <= rightEnd)
            {
                if (Compare(_list[leftIndex], _list[rightIndex]) > 0)
                    _buffer[bufferIndex++] = _list[rightIndex++];
                else
                    _buffer[bufferIndex++] = _list[leftIndex++];
            }

            // copy the rest of the items to the buffer

            for (int i = leftIndex; i <= leftEnd; i++)
                _buffer[bufferIndex++] = _list[i];

            for (int i = rightIndex; i <= rightEnd; i++)
                _buffer[bufferIndex++] = _list[i];

            // copy the buffer back to the list

            for (int i = leftStart; i <= rightEnd; i++)
                _list[i] = _buffer[i];
        }

        #endregion

    }
}
