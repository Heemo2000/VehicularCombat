
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        public static void DrawPath(NavMeshPath path, Color color)
        {
            if (path == null || path.corners.Length == 0)
            {
                return;
            }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Vector3 current = path.corners[i];
                Vector3 next = path.corners[i + 1];
                Debug.DrawLine(current, next, color, 0.1f);
            }

            if (path.corners.Length > 1)
            {
                Debug.DrawLine(path.corners[path.corners.Length - 2], 
                               path.corners[path.corners.Length - 1], 
                               color, 
                               0.1f);
            }
        }
    
        public static bool IsUnderDistance(Vector3 start, Vector3 end, float distance)
        {
            float sqrDistance = Vector3.SqrMagnitude(end - start);
            return sqrDistance <= distance * distance;
        }
    }

}
