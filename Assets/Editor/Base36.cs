using System;
using System.Linq;
using UnityEngine;

namespace Base36Encoder
{
    public static class Base36
    {
        private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string Encode(long value)
        {
            if (value == long.MinValue)
            {
                //hard coded value due to error when getting absolute value below: "Negating the minimum value of a twos complement number is invalid.".
                return "-1Y2P0IJ32E8E8";
            }
            bool negative = value < 0;
            value = Math.Abs(value);
            string encoded = string.Empty;
            do
                encoded = Digits[(int)(value % Digits.Length)] + encoded;
            while ((value /= Digits.Length) != 0);
            return negative ? "-" + encoded : encoded;
        }
    }
}