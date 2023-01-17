using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FilesTest : MonoBehaviour
{
    private static readonly string EXCEL_PATH = Environment.CurrentDirectory + "/Assets/";

    // 文件列表
    private static List<FileInfo> fileInfos = new List<FileInfo>();


    private void Start()
    {
        // fileInfos = GetAllFiles(EXCEL_PATH);
        foreach (var n in fileInfos)
        {
            print(n.Name);
        }
    }


   

    /// 检查是否是是文件
    private static bool IsMetaFile(string fullName)
    {
        return fullName.EndsWith(".meta", StringComparison.Ordinal);
        // || fullName.EndsWith(".xls", StringComparison.Ordinal)
        // || fullName.EndsWith(".xlsx", StringComparison.Ordinal)
        // || fullName.EndsWith(".xlsm", StringComparison.Ordinal);
    }
}