using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace NVivo.Sorting
{
    /// <summary>
    /// Sorts a collection of objects by property names.
    /// </summary>
    /// <remarks>
    /// The sortExpression is similar to SQL order by clause. The format is:
    /// <code>
    /// Property1 [ASC|DESC][, Property2 [ASC|DESC], ...]
    /// </code>
    /// </remarks>
    public class CollectionSort<T>
    {
        /// <summary>
        /// Sorts an IList&lt;T&gt; by properties using the specified sort expression.
        /// </summary>
        public static void Sort(IList<T> list, string sortExpression)
        {
            SortInfo[] sorts = BuildSorts(sortExpression);

            for (int i = sorts.Length - 1; i >= 0; i--)
            {
                SortInfo si = sorts[i];
                MergeSort<T>.Sort(list, si.Comparer, si.Direction);
            }
        }

        // parses the sort expression and builds the list of property comparers and directions
        private static SortInfo[] BuildSorts(string sortExpression)
        {
            Type itemType = typeof(T);
            string[] sortArray = sortExpression.Split(',');
            int arrayLength = sortArray.Length;
            List<SortInfo> comparers = new List<SortInfo>(arrayLength);

            // loops through each of the sort expressions, parse them and
            // add to the list
            for (int i = 0; i < arrayLength; i++)
            {
                string sortExp = sortArray[i].Trim();
                ListSortDirection direction;
                string propertyName;
                int spacePos = sortExp.IndexOf(' ');

                // since there shouldn't be spaces on property names, spacePos >= 0
                // means that there should be an ASC or DESC in the expression
                if (spacePos >= 0)
                {
                    propertyName = sortExp.Substring(0, spacePos);
                    string sortOrder = sortExp.Substring(spacePos + 1);
                    // checks the sort order
                    if (String.Compare(sortOrder, "asc", StringComparison.OrdinalIgnoreCase) == 0)
                        direction = ListSortDirection.Ascending;
                    else if (String.Compare(sortOrder, "desc", StringComparison.OrdinalIgnoreCase) == 0)
                        direction = ListSortDirection.Descending;
                    else
                        throw new ArgumentException("Sort order '" + sortOrder + "' is invalid. Must be ASC or DESC.");
                }
                else
                {
                    propertyName = sortExp;
                    direction = ListSortDirection.Ascending;
                }
                PropertyComparer<T> comparer = new PropertyComparer<T>(propertyName);
                comparers.Add(new SortInfo(comparer, direction));
            }

            return comparers.ToArray();
        }

        // container class for the sort builder
        private class SortInfo
        {
            public SortInfo(PropertyComparer<T> comparer, ListSortDirection direction)
            {
                this.Comparer = comparer;
                this.Direction = direction;
            }

            public PropertyComparer<T> Comparer;
            public ListSortDirection Direction;
        }
    }
}