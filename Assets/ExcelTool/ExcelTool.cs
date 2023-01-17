using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using UnityEngine;

public class ExcelTool
{
    private ExcelTool()
    {
    }

    /// Excel文件根目录
    public static readonly string EXCEL_PATH = Environment.CurrentDirectory + "/Excel/";


    /// 生成数据信息信息
    public static void GenerateExcelInfo(List<FileInfo> files)
    {
        foreach (var fileInfo in files)
        {
            // 非Excel文件不处理
            if (!IsExcelFile(fileInfo.Name)) continue;

            // 打开Excel得到表数据
            DataTableCollection tableCollection;
            using (FileStream fs = fileInfo.Open(FileMode.Open, FileAccess.Read))
            {
                // 得到Excel数据
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = excelReader.AsDataSet().Tables;
                fs.Close();
            }

            // 遍历表信息
            foreach (DataTable table in tableCollection)
            {
                Debug.Log(table.TableName);
                // 生成数据结构
                // 生成容器
                // 生成2进制
                // 生成Json
            }
        }
    }


    /// 检查是否是ExcelFile
    private static bool IsExcelFile(string fullName)
    {
        return fullName.EndsWith(".csv", StringComparison.Ordinal)
               || fullName.EndsWith(".xls", StringComparison.Ordinal)
               || fullName.EndsWith(".xlsx", StringComparison.Ordinal)
               || fullName.EndsWith(".xlsm", StringComparison.Ordinal);
    }


    /// 获得目录下所有文件包含所有子文件夹
    public static List<FileInfo> GetAllFiles(string path)
    {
        List<FileInfo> files = new List<FileInfo>();

        // 检查目录是否存在
        if (!string.IsNullOrWhiteSpace(path))
        {
            if (Directory.Exists(path)) GetAllFilesOfDir(files, path);
            else Directory.CreateDirectory(path);
        }
        else
        {
            Debug.Log($"{path}为空！！");
        }

        return files;
    }


    private static void GetAllFilesOfDir(List<FileInfo> files, string path)
    {
        try
        {
            // 文件夹列表   
            string[] dir = Directory.GetDirectories(path);
            DirectoryInfo dInfo = new DirectoryInfo(path);
            FileInfo[] file = dInfo.GetFiles();

            // 当前目录文件或文件夹不为空              
            if (file.Length != 0 || dir.Length != 0)
            {
                // 显示当前目录所有文件   
                foreach (FileInfo f in file)
                {
                    files.Add(f);
                }

                // 递归一下
                foreach (string d in dir)
                {
                    GetAllFilesOfDir(files, d);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log($"获取路径{path}目录下的文件失败！！！错误信息{e.Message}");
        }
    }
}