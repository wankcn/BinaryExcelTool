using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileIOTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Student s = new Student();
        //s.age = 18;
        //s.name = "唐老狮";
        //s.number = 1;
        //s.sex = false;

        //s.Save("唐老狮");


        Student s2 = Student.Load("唐老狮");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Student
{
    public int age;
    public string name;
    public int number;
    public bool sex;

    public void Save(string fileName)
    {
        Debug.Log(Application.persistentDataPath);
        //如果不存在指定路径 则创建一个文件夹
        if( !Directory.Exists(Application.persistentDataPath + "/Student") )
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Student");
        }
        //新建一个指定名字的文件 并且返回 文件流 进行字节的存储
        using (FileStream fs = new FileStream(Application.persistentDataPath + "/Student/" + fileName + ".tang", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //写age
            byte[] bytes = BitConverter.GetBytes(age);
            fs.Write(bytes, 0, bytes.Length);
            //写name
            bytes = Encoding.UTF8.GetBytes(name);
            //存储字符串字节数组的长度
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            //存储字符串字节数组了
            fs.Write(bytes, 0, bytes.Length);
            //写number
            bytes = BitConverter.GetBytes(number);
            fs.Write(bytes, 0, bytes.Length);
            //写sex
            bytes = BitConverter.GetBytes(sex);
            fs.Write(bytes, 0, bytes.Length);

            fs.Flush();
            fs.Close();
        }
    }

    public static Student Load(string fileName)
    {
        //判断文件是否存在
        if( !File.Exists(Application.persistentDataPath + "/Student/" + fileName + ".tang") )
        {
            Debug.LogWarning("没有找到对应的文件");
            return null;
        }
        //申明对象
        Student s = new Student();
        //加载2进制文件 进行赋值
        using (FileStream fs = File.Open(Application.persistentDataPath + "/Student/" + fileName + ".tang", FileMode.Open, FileAccess.Read))
        {
            //把我们文件中的字节 全部读取出来
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();

            int index = 0;
            //挨个读取其中的内容
            s.age = BitConverter.ToInt32(bytes, index);
            index += 4;
            //字符串字节数组的长度
            int length = BitConverter.ToInt32(bytes, index);
            index += 4;
            s.name = Encoding.UTF8.GetString(bytes, index, length);
            index += length;
            s.number = BitConverter.ToInt32(bytes, index);
            index += 4;
            s.sex = BitConverter.ToBoolean(bytes, index);
            index += 1;
        }

        return s;
    }
}
