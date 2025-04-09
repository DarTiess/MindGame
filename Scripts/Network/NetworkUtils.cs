using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public static class NetworkUtils
    {
        public static List<string> SplitString(string str, int chunkSize)
        {
            var parts = new List<string>();
            for (int i = 0; i < str.Length; i += chunkSize)
            {
                parts.Add(str.Substring(i, Mathf.Min(chunkSize, str.Length - i)));
            }
            return parts;
        }
        public static string CombineStrings(List<string> parts)
        {
            return string.Join("", parts);
        }
    }
}