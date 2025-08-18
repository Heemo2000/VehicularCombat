
using System;
using System.Collections.Generic;

namespace Game.Core
{
    public class Utility
    {
        private static Dictionary<System.Type, System.Array> enumsDict = new Dictionary<System.Type, System.Array>();
        public static T GetRandomEnum<T>() where T: Enum
        {
            System.Array values = null;
            if (enumsDict.ContainsKey(typeof(T)))
            {
                values = enumsDict[typeof(T)];
            }
            else
            {
                values = System.Enum.GetValues(typeof(T));
                enumsDict[typeof(T)] = values;
            }

            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        public static T GetRandomEnum<T>(int minInclusive, int maxExclusive) where T : Enum
        {
            System.Array values = null;
            if (enumsDict.ContainsKey(typeof(T)))
            {
                values = enumsDict[typeof(T)];
            }
            else
            {
                values = System.Enum.GetValues(typeof(T));
                enumsDict.Add(typeof(T), values);
            }

            if (minInclusive < 0)
            {
                minInclusive = 0;
            }

            if (maxExclusive > values.Length)
            {
                maxExclusive = values.Length;
            }

            return (T)values.GetValue(UnityEngine.Random.Range(minInclusive, maxExclusive));
        }


    }

}
