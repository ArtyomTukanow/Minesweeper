﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.UI.Utils
{
    public static class MathUtils
    {
        public static T RandomFromCollection<T>(this IList<T> target) =>
            target?.Count > 0 ? target[Random.Range(0, target.Count)] : default(T);
        
        public static T Clamp<T>(this T target, T min, T max) where T : IComparable
        {
            if (target.CompareTo(min) < 0) return min;
            if (target.CompareTo(max) > 0) return max;
            return target;
        }

        // Находится ли значение между двумя другими
        public static bool Between<T>(this T target, float min, float max) where T : IComparable =>
            target.CompareTo(min) >= 0 && target.CompareTo(max) <= 0;

        public static bool BetweenUnordered<T>(this float target, float val1, float val2) =>
            target.Between(val1, val2) || target.Between(val2, val1);

        // Тоже что и кламп но не важно что минимум, а что максимум
        public static float ClampBetween(this float target, float val1, float val2)
        {
            return val1 > val2 ? Mathf.Clamp(target, val2, val1) : Mathf.Clamp(target, val1, val2);
        }

        // Не помню для чего это?
        public static float FloatTowards(float from, float towards, float max)
        {
            max = Mathf.Abs(max);
            var d = towards - @from;
            if (Mathf.Abs(d) <= max) return d;
            return (d > 0) ? max : -max;
        }

        // Рандом между двумя числами, не важно какое максимум
        public static float RandomBetween(float val1, float val2)
        {
            return val1 > val2 ? Random.Range(val2, val1) : Random.Range(val1, val2);
        }

        public static bool CloseTo(this float val1, float val2, float? maxDist = null)
        {
            return Mathf.Abs(val1 - val2) <= (maxDist ?? Mathf.Epsilon);
        }

        /// <summary>
        /// То на сколько нужно умножить стороны target чтобы они вмещались внутри bounds.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bounds"></param>
        /// <param name="keepAspectRatio"></param>
        /// <returns></returns>
        public static Vector2 GetScalesToFit(this Vector2 target, Vector2 bounds, bool keepAspectRatio = true)
        {
            var scaleX = target.x != 0 ? bounds.x / target.x : 0;
            var scaleY = target.y != 0 ? bounds.y / target.y : 0;

            if (!keepAspectRatio) return new Vector2(scaleX, scaleY);

            var scale = Mathf.Min(scaleX, scaleY);
            return new Vector2(scale, scale);
        }
    }
}