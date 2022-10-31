using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NVivo.Sorting
{
    /// <summary>
    /// This class compares objects using a property value.
    /// </summary>
    /// <typeparam name="T">The type of class to compare.</typeparam>
    [DebuggerDisplay("{GetDebuggerText()}")]
    public class PropertyComparer<T> : IComparer<T>
    {
        private string _propertyName;
        private MethodInfo _getMethod;
        private Comparer _comparer;

        /// <summary>
        /// Creates a new PropertyComparer using a default Comparer.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <
        public PropertyComparer(string propertyName)
            : this(propertyName, null)
        { }

        /// <summary>
        /// Creates a new PropertyComparer.
        /// </summary>
        /// <param name="propertyName">The name of the property to compare.</param>
        /// <param name="valueComparer">A Comparer to compare the property value.</param>
        public PropertyComparer(string propertyName, Comparer valueComparer)
        {
            ExtractMethod(typeof(T), propertyName);

            if (valueComparer != null)
                _comparer = valueComparer;
            else
                _comparer = Comparer.Default;
        }

        /// <summary>
        /// Extracts the "get" accessor from the type.
        /// </summary>
        private void ExtractMethod(Type t, string propertyName)
        {
            // gets the property
            PropertyInfo pi = t.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (pi == null)
                throw new InvalidOperationException("Cannot find a instance property named '" + propertyName + "'.");

            // sets the property name to use in the debugger
            _propertyName = pi.Name;

            // gets the "get" acessor for the property
            MethodInfo mi = pi.GetGetMethod();

            if (mi == null)
                throw new InvalidOperationException("The property '" + propertyName + "' does not have a get accessor.");

            _getMethod = mi;
        }

        /// <summary>
        /// Compares two object's properties.
        /// </summary>
        public int Compare(T x, T y)
        {
            object xValue = _getMethod.Invoke(x, null);
            object yValue = _getMethod.Invoke(y, null);

            return _comparer.Compare(xValue, yValue);
        }

        // this method is used to display a more descriptive text in the debugger
        private string GetDebuggerText()
        {
            return String.Format("{0}.{1}", _getMethod.DeclaringType.Name, _propertyName);
        }
    }
}


