using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Core;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Libraries.RSG;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Vector3 = System.Numerics.Vector3;

namespace AllUtils
{
    public static class Utils
    {
        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
        public static bool NotEmpty(this string s) => !string.IsNullOrEmpty(s);

        public static string Localize(this string key, params string[] keyLocales)
        {
            return Game.Static.Localization.GetLocale(key, keyLocales);
        }
                
        public static string GetNumericTime(this long time, bool needHours = true, bool needSeconds = true)
        {
            if (time == int.MaxValue)
            {
                return "∞";
            }

            var hours = (int) Math.Floor(time / 3600f);
            var minutes = (int) Math.Floor((time - hours*3600f) / 60f);
            var seconds = (int) time - hours* 3600 - minutes*60;

            if (!needSeconds && hours == 0 && minutes == 0 && seconds > 0)
            {
                minutes += 1;
            }

            if (needHours || hours > 0)
            {
                return fillDigits(hours) + ":" + fillDigits(minutes) + (needSeconds? ":" + fillDigits(seconds) : "");
            }
            else
            {
                return fillDigits(minutes) + ":" + (needSeconds? fillDigits(seconds) : "");
            }

            string fillDigits(int val)
            {
                string result = "";

                if (val == 0)
                    return "00";

                result = val.ToString();
                while (result.Length< 2)
                {
                    result = "0" + result;
                }

                return result;
            }
        }

        public static string GetCharTime(this long time, int groupCount = -1) => GetCharTime((int)time, groupCount);
        public static string GetCharTime(this int time, int groupCount = -1)
        {
            var result = "";

            int days = (int) Math.Floor(time / (24f * 60 * 60));
            var hours = (int) Math.Floor((time - days * (24 * 60 * 60)) / 3600f);
            var minutes = (int) Math.Floor((time - days * (24 * 60 * 60) - hours * 3600f) / 60);
            var seconds = (int) (time - days * (24 * 60 * 60) - hours * 3600 - minutes * 60);

            var strs = new List<string>();

            var maxGrpCount = 0;
            
            if (days > 0) maxGrpCount = 4;
            else if (hours > 0) maxGrpCount = 3;
            else if (minutes > 0) maxGrpCount = 2;
            else if (seconds > 0) maxGrpCount = 1;

            maxGrpCount = Math.Max(maxGrpCount, groupCount);

            strs.Add("s".Localize(seconds.ToString()));
            strs.Add("m".Localize(minutes.ToString()));
            strs.Add("h".Localize(hours.ToString()));
            strs.Add("d".Localize(days.ToString()));

            var needGroups = groupCount == -1 ? maxGrpCount : groupCount;
            var startGroup = maxGrpCount - needGroups;

            for (var i = startGroup; i < maxGrpCount; i++)
            {
                if (!result.Equals("")) 
                    result = strs[i] + " " + result;
                else
                    result = strs[i] + result;
            }

            return result;
		}
        
        
        public static T Shift<T>(this List<T> list)
        {
            if (list.Count > 0)
            {
                var result = list[0];
                list.RemoveAt(0);
                return result;
            }
            return default(T);
        }

        public static void AddOnce<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }
        
        public static bool AreEquals<T>(this List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
                return false;
            
            for(var i = 0; i < list2.Count; i ++)
                if (!list1[i].Equals(list2[i]))
                    return false;
            
            return true;
        }
        
        public static void ResolveOnce(this Promise p)
        {
            if(p.CurState == PromiseState.Pending)
                p.Resolve();
        }

        public static void RejectOnce(this Promise p) => RejectOnce(p, null);
        public static void RejectOnce(this Promise p, Exception e)
        {
            if(p.CurState == PromiseState.Pending)
                p.Reject(e);
        }
        
        public static IPromise ToPromise(this TweenerCore<float, float, FloatOptions> tween, bool alwaysComplete = true)
        {
            var promise = new Promise();
            tween.OnComplete(() => TimerUtils.NextFrame().Then(promise.ResolveOnce));
            tween.OnKill(() => TimerUtils.NextFrame().Then(() =>
            {
                if (alwaysComplete)
                    promise.ResolveOnce();
                else
                    promise.RejectOnce();
            }));
            return promise;
        }

        public static IPromise ToPromise(this TweenerCore<Vector3, Vector3, VectorOptions> tween, bool alwaysComplete = true)
        {
            var promise = new Promise();
            tween.OnComplete(() => TimerUtils.NextFrame().Then(promise.ResolveOnce));
            tween.OnKill(() => TimerUtils.NextFrame().Then(() =>
            {
                if (alwaysComplete)
                    promise.ResolveOnce();
                else
                    promise.RejectOnce();
            }));
            return promise;
        }

        /// <summary>
        /// Добавляет в <see cref="Tween"/> завершение <see cref="Promise"/>.
        /// Использовать с осторожностью, заменяет предыдущие функции OnComplete и OnKill у Tween.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="alwaysComplete">Всегда завершать <see cref="Promise"/>, даже если твин прерывается</param>
        /// <returns></returns>
        public static IPromise ToPromise(this Tween sequence, bool alwaysComplete = false)
        {
            var promise = new Promise();
            var seqCallBack = sequence.onComplete;
            sequence.OnComplete(() =>
            {
                seqCallBack?.Invoke();
                TimerUtils.NextFrame().Then(promise.ResolveOnce);
            });
            sequence.OnKill(() => TimerUtils.NextFrame().Then(() =>
            {
                if (alwaysComplete)
                    promise.ResolveOnce();
                else
                    promise.RejectOnce();
            }));
            return promise;
        }
        
        

        public static T ReadFile<T>(string fileName)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            FileStream file;

            if(File.Exists(destination))
                file = File.OpenRead(destination);
            else
            {
                Debug.LogWarning("[ReadFile] " + fileName + " not found");
                return default(T);
            }

            BinaryFormatter bf = new BinaryFormatter();

            object data = bf.Deserialize(file);
            file.Close();

            return (T) data;
        }

        public static void SaveFile(string fileName, object data)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            FileStream file;

            if (File.Exists(destination))
                File.Delete(destination);
			
            file = File.Create(destination);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, data);
            file.Close();
        }

        public static void AppendToFile(string fileName, string data)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            File.AppendAllText(destination, data + Environment.NewLine);
        }

        public static string[] ReadLines(string fileName)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            return File.ReadAllLines(destination);
        }

        public static void WriteLines(string fileName, string[] lines)
        {
            string destination = Application.persistentDataPath + "/" + fileName;
            if (File.Exists(destination))
                File.Delete(destination);
            File.WriteAllLines(destination, lines);
        }

        public static bool IsFileExists(string fileName)
        {
            string destination = Application.persistentDataPath + "/" + fileName;

            return File.Exists(destination);
        }
        
        
        private static List<RaycastResult> raycastResults = new List<RaycastResult>();
        private static PointerEventData eventDataCurrentPosition = null;
        public static bool IsPointerOverUIObject(bool ignoreNonBlocking = false)
        {
            if (eventDataCurrentPosition == null)
                eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
            EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);

            if (ignoreNonBlocking)
            {
                const int UINonBlockingLayer = 8;
                for (var i = 0; i < raycastResults.Count; i++)
                    if (raycastResults[i].gameObject.layer != UINonBlockingLayer)
                        return true;

                return false;
            }

            return raycastResults.Count > 0;
        }
    }
}