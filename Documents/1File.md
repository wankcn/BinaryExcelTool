```csharp
// 1.判断文件是否存在
File.Exists();

// 2.创建文件
FileStream fs = File.Create();

// 3.写入文件
// 将指定字节数组 写入到指定路径的文件中
byte[] bytes = BitConverter.GetBytes(999);
File.WriteAllBytes(path, bytes);

// 将指定的string数组内容 一行行写入到指定路径中
string[] strs = new string[] { ... };
File.WriteAllLines(path, strs);
// 将指定字符串写入指定路径
File.WriteAllText(path, str);

// 4.读取文件
// 读取字节数据
bytes = File.ReadAllBytes();
// 读取所有行信息
strs = File.ReadAllLines();
// 读取所有文本信息
File.ReadAllText();

// 5.删除文件
// 注意 如果删除打开着的文件 会报错
File.Delete();

// 6.复制文件
//参数一：现有文件 需要是流关闭状态
//参数二：目标文件
File.Copy();

//7.文件替换
// 用A替换B，备份了B得到C，  B的文件名A的内容 C备份的是B的内容
File.Replace(A,B,C);
```

