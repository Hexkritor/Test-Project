using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheHexCore.Basic
{
    public static class BasicExtention
    {
        /// <summary>
        /// Returns if value in range between left and right
        /// </summary>
        /// <typeparam name="T">Comparable type</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="left">Left border of range (included)</param>
        /// <param name="right">Right border of range (included)</param>
        public static bool InRange<T>(T value, T left, T right) where T : IComparable<T>
        {
            if (left.CompareTo(right) > 0)
                Swap(ref left, ref right);
            return value.CompareTo(left) >= 0 && value.CompareTo(right) <= 0;
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
