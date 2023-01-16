using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using UnityEditor;
using UnityEngine;

public class ExcelTool
{
    /// Excel文件根目录
    private static readonly string EXCEL_PATH = Environment.CurrentDirectory + "/Excel/";


    [MenuItem("Tools/GenerateExcelInfo")]
    private static void GenerateExcelInfo()
    {
        // 获得Excel中所有文件
        DirectoryInfo dInfo = Directory.CreateDirectory(EXCEL_PATH);
        FileInfo[] files = dInfo.GetFiles();
        GenerateExcelInfo(files);
    }


    private static void GenerateExcelInfo(FileInfo[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            // 非Excel文件不处理
            if (!IsExcelFile(files[i].Name)) continue;

            // 打开Excel得到表数据
            DataTableCollection tableCollection;
            using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
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
}