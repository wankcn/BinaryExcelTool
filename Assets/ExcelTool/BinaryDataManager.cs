using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;


/// 二进制数据管理器
public class BinaryDataManager
{
    private static BinaryDataManager instance = new BinaryDataManager();
    public static BinaryDataManager Instance => instance;

    /// 所有Excel表数据容器
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();

    /// 数据存储的位置
    private static string SAVE_PATH = Application.persistentDataPath + "/Data/";

    private BinaryDataManager()
    {
    }

    /// <summary>
    /// 加载二进制数据到内存中
    /// </summary>
    /// <typeparam name="T">容器类</typeparam>
    /// <typeparam name="K">数据结构类名</typeparam>
    public void LoadTable<T, K>()
    {
        using (FileStream fs = File.Open(ExcelTool.DATA_BINARY_PATH + typeof(K).Name, FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            int index = 0;

            // 有多少行数据
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            int pKeyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string pKeyName = Encoding.UTF8.GetString(bytes, index, pKeyNameLength);
            index += pKeyNameLength;

            // 创建容器类对象
            Type tableType = typeof(T);
            object tableObj = Activator.CreateInstance(tableType);
            // 得到数据结构类Type
            Type classType = typeof(K);
            // 得到所有字段信息
            FieldInfo[] infos = classType.GetFields();
            // 读取每一行数据
            for (int i = 0; i < count; i++)
            {
                object dataObj = Activator.CreateInstance(classType);
                foreach (FieldInfo info in infos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        // 读取字符串字符数组的长度
                        int length = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, length));
                        index += length;
                    }
                }

                object dicObject = tableType.GetField("dataDic").GetValue(tableObj);
                //通过字典对象得到其中的 Add方法
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                //得到数据结构类对象中 指定主键字段的值
                object keyValue = classType.GetField(pKeyName).GetValue(dataObj);
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObj });
            }

            //把读取完的表记录下来
            tableDic.Add(typeof(T).Name, tableObj);

            fs.Close();
        }
    }


    /// <summary>
    /// 存储类对象数据
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fileName"></param>
    public void Save(object obj, string fileName)
    {
        //先判断路径文件夹有没有
        if (!Directory.Exists(SAVE_PATH))
            Directory.CreateDirectory(SAVE_PATH);

        using (FileStream fs = new FileStream(SAVE_PATH + fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
            fs.Close();
        }
    }

    /// <summary>
    /// 读取2进制数据转换成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public T Load<T>(string fileName) where T : class
    {
        //如果不存在这个文件 就直接返回泛型对象的默认值
        if (!File.Exists(SAVE_PATH + fileName + ".wr"))
            return default(T);

        T obj;
        using (FileStream fs = File.Open(SAVE_PATH + fileName, FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs) as T;
            fs.Close();
        }

        return obj;
    }
    
    
    
    /// 得到一张表的信息
    public T GetVOData<T>() where T:class
    {
        string tableName = typeof(T).Name;
        if (tableDic.ContainsKey(tableName))
            return tableDic[tableName] as T;
        return null;
    }
}