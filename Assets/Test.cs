using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        string str = "tRue";
       
        bool value = Convert.ToBoolean(str);
        var b = BitConverter.GetBytes(value);

        string res = "";
        foreach (var n in b)
        {
            res += n;
            res += " ";
        }

        print($"{value} - {value.GetType()} - {res}");
    }
}