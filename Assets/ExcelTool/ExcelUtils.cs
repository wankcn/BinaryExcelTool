using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExcelUtils
{
    /// 首字母小写
    public static string FirstCharToLower(this string str)
    {
        if (String.IsNullOrEmpty(str)) return str;
        return str.First().ToString().ToLower() + str.Substring(1);
    }

    /// 首字母大写
    public static string FirstCharToUpper(this string str)
    {
        if (String.IsNullOrEmpty(str)) return str;
        return str.First().ToString().ToUpper() + str.Substring(1);
    }
    
    /// 检查是否是ExcelFile
    public static bool IsExcelFile(this string fullName)
    {
        return fullName.EndsWith(".csv", StringComparison.Ordinal)
               || fullName.EndsWith(".xls", StringComparison.Ordinal)
               || fullName.EndsWith(".xlsx", StringComparison.Ordinal)
               || fullName.EndsWith(".xlsm", StringComparison.Ordinal);
    }
}