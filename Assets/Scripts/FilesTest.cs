using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FilesTest : MonoBehaviour
{
    private static readonly string EXCEL_PATH = Environment.CurrentDirectory + "/Assets/";

    // 文件列表
    private static readonly List<FileInfo> m_filesList = new List<FileInfo>();


    private void Start()
    {
        GetAllFiles(EXCEL_PATH);
        foreach (var n in m_filesList)
        {
            print(n.Name);
        }
    }


    /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
    public static List<FileInfo> GetAllFiles(string path)
    {
        //检查目录是否存在
        if (!string.IsNullOrWhiteSpace(path))
        {
            if (Directory.Exists(path))
            {
                GetAllFilesOfDir(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }
        else
        {
            //注意这里的EverydayLog.Write()是我自定义的日志文件，可以根据需要保留或删除
            Debug.Log("GetAllFileOfFolder/GetallFile()/存储视频文件的路径为空，请检查！！！");
        }

        return m_filesList;
    }


    /// 递归获取指定类型文件,包含子文件夹
    private static void GetAllFilesOfDir(string path)
    {
        try
        {
            string[] dir = Directory.GetDirectories(path); //文件夹列表   
            DirectoryInfo dInfo = new DirectoryInfo(path);
            FileInfo[] file = dInfo.GetFiles();

            if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空                   
            {
                foreach (FileInfo f in file) //显示当前目录所有文件   
                {
                    if (IsMetaFile(f.Name))
                    {
                        continue;
                    }

                    m_filesList.Add(f);
                }

                foreach (string d in dir)
                {
                    GetAllFilesOfDir(d); //递归   
                }
            }
        }
        catch (Exception ex)
        {
            //注意这里的EverydayLog.Write()是我自定义的日志文件，可以根据需要保留或删除
            Debug.Log("/GetAllFileOfFolder()/GetAllFilesOfDir()/获取指定路径：" + path + "   下的文件失败！！！，错误信息=" +
                      ex.Message);
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