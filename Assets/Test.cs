using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        List<int> list1 = new List<int>() { 1, 12, 14, 5, 6, 7, 8, 90, 20 };
        List<int> list2 = new List<int>() { 1, 90, 20, 7 };

        MoveDataInSituFront(list1, list2);
        string str = "";
        foreach (var n in list1)
        {
            str += n;
            str += " ";
        }

        print($"!! {str}");
    }


    /// <summary>
    /// 将t2里的元素全部移动到t1的末尾
    /// t1包含t2
    /// </summary>
    public static void MoveDataInSituAfter<T>(T t1, T t2) where T : IList
    {
        int tag = 0;

        for (int i = 0; i < t1.Count; i++)
            if (!t2.Contains(t1[i]))
                t1[tag++] = t1[i];

        for (int i = tag; i < t1.Count; i++)
            t1[i] = t2[i - tag];
    }

    /// <summary>
    /// 将t2里的元素全部移动到t1的末尾
    /// t1包含t2
    /// </summary>
    public static void MoveDataInSituFront<T>(T t1, T t2) where T : IList
    {
        int tag = t1.Count;

        // List<int> list1 = new List<int>() { 12, 14, 5, 6,  8, };
        // List<int> list2 = new List<int>() { 1, 90, 20, 7 };

        for (int i = t1.Count - 1; i >= 0; i--)
            if (!t2.Contains(t1[i]))
                t1[--tag] = t1[i];

        print(tag);
        for (int i = 0; i < tag; i++)
            t1[i] = t2[i];
    }
}