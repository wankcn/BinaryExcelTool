using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;
using Excel;

public class ExcelReadTest
{
    public void Test()
    {
    }


    [MenuItem("Tools/GenerateExcelInfo")]
    private static void GenerateExcelInfo()
    {
        List<FileInfo> files = ExcelTool.GetAllFiles(ExcelTool.EXCEL_PATH);
        ExcelTool.GenerateExcelInfo(files);
    }


    [MenuItem("Tools/Test/LogDataPath")]
    private static void LogDataPath()
    {
        Debug.Log($"System.Environment.CurrentDirectory - {System.Environment.CurrentDirectory}");
        Debug.Log($"Application.dataPath - {Application.dataPath}");
        Debug.Log($"Application.persistentDataPath - {Application.persistentDataPath}");
    }


    [MenuItem("Tools/Test/OpenExcel")]
    private static void OpenExcel()
    {
        using (FileStream fs = File.Open(ExcelTool.EXCEL_PATH + "TestInfo.xlsx", FileMode.Open, FileAccess.Read))
        {
            // 文件流获取Excel数据
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);

            // 将Excel表中的数据转为DataSet
            DataSet result = excelReader.AsDataSet();

            for (int i = 0; i < result.Tables.Count; i++)
            {
                Debug.Log($"表名 {result.Tables[i].TableName}");
                Debug.Log($"行数 {result.Tables[i].Rows.Count}");
                Debug.Log($"列数 {result.Tables[i].Columns.Count}");
            }

            fs.Close();
        }
    }

    [MenuItem("Tools/Test/ReadExcel")]
    private static void ReadExcel()
    {
        using (FileStream fs = File.Open(ExcelTool.EXCEL_PATH + "TestInfo.xlsx", FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            DataSet result = excelReader.AsDataSet();
            DataTable table = result.Tables[0];
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Debug.Log($"----- 第 {i} 行 ------");
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    Debug.Log($"{table.Rows[i][j]}");
                }
            }

            fs.Close();
        }
    }
}