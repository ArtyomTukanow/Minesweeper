using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class PathFinder
    {
        public static Dictionary<Vector2Int, Vector2Int> getWave(Vector2Int target, Func<Vector2Int, int, bool> canAddFunc, bool diagonal = false, bool checkTargetPoint = false)
        {
            return getWave(new []{target}, canAddFunc, diagonal, checkTargetPoint);
        }

        public static Dictionary<Vector2Int, Vector2Int> getWave(Vector2Int[] targets, Func<Vector2Int, int, bool> canAddFunc, bool diagonal = false, bool checkTargetPoints = false)
        {
            var wasPoints = new Dictionary<Vector2Int, Vector2Int>();
            var curPoints = new Dictionary<Vector2Int, Vector2Int>();
            var approvedPoints = new Dictionary<Vector2Int, Vector2Int>();
            var discardedPoints = new Dictionary<Vector2Int, Vector2Int>();
            
            var iteration = 0;
            
            
            //проверяем внутренние точки как нулевую итерацию волны
            foreach (var target in targets)
                if(!checkTargetPoints || check(target))
                    curPoints[target] = target;

            var wasKeys = true;

            while (wasKeys)
            {
                wasKeys = false;
                approvedPoints.Clear();
                discardedPoints.Clear();
                foreach (var key in curPoints)
                {
                    wasKeys = true;
                    var p = key.Key;

                    checkAndAdd(new Vector2Int(p.x - 1, p.y), p);
                    checkAndAdd(new Vector2Int(p.x + 1, p.y), p);
                    checkAndAdd(new Vector2Int(p.x, p.y - 1), p);
                    checkAndAdd(new Vector2Int(p.x, p.y + 1), p);

                    if (diagonal)
                    {
                        checkAndAdd(new Vector2Int(p.x - 1, p.y - 1), p);
                        checkAndAdd(new Vector2Int(p.x + 1, p.y - 1), p);
                        checkAndAdd(new Vector2Int(p.x + 1, p.y + 1), p);
                        checkAndAdd(new Vector2Int(p.x - 1, p.y + 1), p);
                    }
                }

                foreach (var point in curPoints)
                    wasPoints[point.Key] = point.Value;
                curPoints = new Dictionary<Vector2Int, Vector2Int>(approvedPoints);

                iteration ++;
            }

            return wasPoints;

            void checkAndAdd(Vector2Int v, Vector2Int p)
            {
                if (check(v))
                    approvedPoints[v] = p;
                else
                    discardedPoints[v] = p;
            }

            bool check(Vector2Int v)
            {
                return !wasPoints.ContainsKey(v)
                       && !discardedPoints.ContainsKey(v)
                       && !approvedPoints.ContainsKey(v)
                       && !curPoints.ContainsKey(v)
                       && canAddFunc(v, iteration);
            }
        }
        
        
        public static List<Vector2Int> getPathFromWave(Dictionary<Vector2Int, Vector2Int> wave, Vector2Int toPos)
        {
	        var result = new List<Vector2Int>{toPos};
	        var curPoint = toPos;
	        while (wave.ContainsKey(curPoint))
	        {
		        result.Add(wave[curPoint]);
		        if(curPoint.Equals(wave[curPoint]))
			        break;
		        curPoint = wave[curPoint];
	        }
	        result.Reverse();
	        return result;
        }
    }
}