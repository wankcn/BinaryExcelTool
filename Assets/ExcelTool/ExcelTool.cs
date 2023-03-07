using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Excel;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 新的表结构
/// 第一行 字段名
/// 第二行 类型名
/// 第三行 主设置键
/// 第四行 备注
/// </summary>
public class ExcelTool
{
    private ExcelTool()
    {
    }

    /// Excel文件根目录 
    public static readonly string EXCEL_PATH = Environment.CurrentDirectory + "/Excel/";

    /// Class脚本存储位置
    public static string DATA_CLASS_PATH = Application.dataPath + "/Scripts/VOs/DataClass/";

    /// 容器类脚本存储位置
    public static string DATA_TABLE_PATH = Application.dataPath + "/Scripts/VOs/DataTable/";

    /// 二进制文本生成路径 放在只读文件夹
    public static string DATA_BINARY_PATH = Application.dataPath + "/Scripts/VOs/ExcelBinaryData/";
    // public static string DATA_BINARY_PATH = Application.streamingAssetsPath + "/Scripts/WenRuoVOTest/ExcelBinaryData/";

    /// 真正内容开始的行号 后面改自动读
    public static int BEGIN_INDEX = 4;

    /// 生成数据信息信息 !核心执行接口
    public static void GenerateExcelInfos(List<FileInfo> files)
    {
        foreach (var fileInfo in files)
        {
            GenerateExcelInfo(fileInfo);
        }
    }

    private static void GenerateExcelInfo(FileInfo fileInfo)
    {
        // 非Excel文件不处理
        if (!fileInfo.Name.IsExcelFile()) return;

        // 打开Excel得到表数据
        DataTableCollection tableCollection;
        using (FileStream fs = fileInfo.Open(FileMode.Open, FileAccess.Read))
        {
            // 得到Excel数据
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
            tableCollection = excelReader.AsDataSet().Tables;
            fs.Close();
        }

        // 遍历sheet信息 这里的DataSheet指的是sheet信息
        foreach (DataTable table in tableCollection)
        {
            // =============== 一些修改想法
            // 1. 一张表对应一个表结构 如果1，1对应 不需要foreach，直接 取table[0]
            // 2. 目前是在生成方法中写的 基础的字符串写入 ，这里先有一个流程跑起来 变成读模板文件


            Debug.Log(table.TableName);
            // 生成数据结构
            GenerateExcelDataClass(table);
            // 生成容器类
            GenerateExcelContainer(table);
            // 生成二进制
            GenerateExcelBinary(table);
            // TODO 生成Json
        }
    }

    #region 获得目录下所有文件 包含所有子文件

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

    #endregion

    #region 生成数据 写入数据 先写如一部分字符跑通流程 之后再锦上添花增加功能和纠错等

    /// 生成数据结构类
    private static void GenerateExcelDataClass(DataTable table)
    {
        string className = table.TableName.FirstCharToUpper();
        // 字段名行
        DataRow rowName = GetVariableNameRow(table);
        DataRow rowType = GetVariableTypeRow(table);

        // 文件夹不存在则创建
        if (!Directory.Exists(DATA_CLASS_PATH)) Directory.CreateDirectory(DATA_CLASS_PATH);

        // 写入类
        StringBuilder sb = new StringBuilder();
        sb.Append($"public class {className} \n" + "{\n");
        for (int i = 0; i < table.Columns.Count; i++)
        {
            string str =
                $"\tprivate {rowType[i].ToString().Trim()} {rowName[i].ToString().FirstCharToLower().Trim()};\n";
            sb.Append(str);
        }

        sb.Append("\n");
        for (int i = 0; i < table.Columns.Count; i++)
        {
            string str =
                $"\tpublic {rowType[i].ToString().Trim()} {rowName[i].ToString().FirstCharToUpper().Trim()} => {rowName[i].ToString().FirstCharToLower().Trim()};\n";
            sb.Append(str);
        }

        sb.Append("}");
        File.WriteAllText(DATA_CLASS_PATH + className + ".cs", sb.ToString());
        AssetDatabase.Refresh();
    }

    /// 生成容器类
    private static void GenerateExcelContainer(DataTable table)
    {
        string className = table.TableName.FirstCharToUpper();
        int primaryKey = GetPrimaryKeyIndex(table);
        // 得到字段类型行
        DataRow rowType = GetVariableTypeRow(table);

        if (!Directory.Exists(DATA_TABLE_PATH)) Directory.CreateDirectory(DATA_TABLE_PATH);

        StringBuilder sb = new StringBuilder();
        sb.Append("using System.Collections.Generic;\n");
        sb.Append($"\npublic class {className}TB \n");
        sb.Append("{\n");
        sb.Append($"\tpublic Dictionary<{rowType[primaryKey]},{className}> ");
        sb.Append($"dataMap = new Dictionary<{rowType[primaryKey]},{className}>();\n");
        sb.Append($"\tpublic List<{className}> dataList = new List<{className}>();\n");
        sb.Append("}");

        File.WriteAllText(DATA_TABLE_PATH + className + "TB.cs", sb.ToString());
        AssetDatabase.Refresh();
    }


    /// 生成二进制数据
    private static void GenerateExcelBinary(DataTable table)
    {
        // 文件名首字母大写
        string fileName = table.TableName.FirstCharToUpper();

        if (!Directory.Exists(DATA_BINARY_PATH)) Directory.CreateDirectory(DATA_BINARY_PATH);

        // 创建二进制文件
        using (FileStream fs = new FileStream(DATA_BINARY_PATH + fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            // 存储具体Excel数据有多少行
            int lines = 4;
            fs.Write(BitConverter.GetBytes(table.Rows.Count - lines), 0, 4);

            // 写入主键变量名
            int pKeyIndex = GetPrimaryKeyIndex(table);
            string pKeyName = GetVariableNameRow(table)[pKeyIndex].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(pKeyName); // 得到主键字符数组
            // 存储字符串字节数组的长度int
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            fs.Write(bytes, 0, bytes.Length);

            // 写入每一行
            DataRow row;
            DataRow rowType = GetVariableTypeRow(table);
            for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            int intData = int.Parse(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(intData), 0, 4);
                            break;
                        case "float":
                            float fData = float.Parse(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(fData), 0, 4);
                            break;
                        case "bool":
                            bool boolData = bool.Parse(row[j].ToString());
                            fs.Write(BitConverter.GetBytes(boolData), 0, 1);
                            break;
                        case "string":
                            byte[] strBytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            // 先写入字符串字节数组的长度
                            fs.Write(BitConverter.GetBytes(strBytes.Length), 0, 4);
                            fs.Write(strBytes, 0, strBytes.Length);
                            break;
                        case "List<int>":
                            string[] strListInt = row[j].ToString().Split(',');
                            fs.Write(BitConverter.GetBytes(strListInt.Length), 0, 4);
                            foreach (var t in strListInt)
                            {
                                int tmp = int.Parse(t);
                                fs.Write(BitConverter.GetBytes(tmp), 0, 4);
                            }
                            break;
                        case "List<float>":
                            string[] strListFloat = row[j].ToString().Split(',');
                            fs.Write(BitConverter.GetBytes(strListFloat.Length), 0, 4);
                            foreach (var t in strListFloat)
                            {
                                float tmp = float.Parse(t);
                                fs.Write(BitConverter.GetBytes(tmp), 0, 4);
                            }
                            break;
                        case "List<bool>":
                            string[] strListBool = row[j].ToString().Split(',');
                            fs.Write(BitConverter.GetBytes(strListBool.Length), 0, 4);
                            foreach (var t in strListBool)
                            {
                                bool tmp = bool.Parse(t);
                                fs.Write(BitConverter.GetBytes(tmp), 0, 1);
                            }
                            break;
                        case "List<string>":
                            string[] strListStr = row[j].ToString().Split(',');
                            fs.Write(BitConverter.GetBytes(strListStr.Length), 0, 4);
                            foreach (var t in strListStr)
                            {
                                byte[] tmp = Encoding.UTF8.GetBytes(t);
                                // 先写入字符串字节数组的长度
                                fs.Write(BitConverter.GetBytes(tmp.Length), 0, 4);
                                fs.Write(tmp, 0, tmp.Length);
                            }
                            break;
                        // TODO: 增加其他类型
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }


    /// 得到变量名所在行
    private static DataRow GetVariableNameRow(DataTable table)
    {
        // TODO: v2.0扩展检索功能
        return table.Rows[0];
    }

    /// 得到变字段名所在行
    private static DataRow GetVariableTypeRow(DataTable table)
    {
        // TODO: v2.0扩展检索功能
        return table.Rows[1];
    }

    /// 返回主键索引
    private static int GetPrimaryKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString() == "key") return i;
        }

        return 0;
    }

    #endregion
}