using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NPOI.SS.UserModel;
using UnityEditor;
using UnityEngine;
using NPOI.XSSF.UserModel;


namespace Sly
{
    [InitializeOnLoad]
    public class CreateJsonForExcel : EditorWindow
    {
        private string _excelPath = "Assets/Excels";
        // editor 输出目录
        private string _editorPath = "Assets/Editor";
        // 读取工作蒲文件名
        private string _fileName = "loop文档5.28.11.xlsx";
        // 读取工作表名
        private string _sheetName = "属性表";
        private string _outputPath = "Assets/Jsons";
        StringBuilder sb = new StringBuilder(); 
        // 类名
        private string _className = "DataItem";
        [MenuItem("SlyTools/表格转json")]
        public static void ExcelWindowShow()
        {
            GetWindow<CreateJsonForExcel>("表格转json");
        }

        private void OnGUI()
        {
            GUILayout.Label("表格转json", EditorStyles.boldLabel);
            _excelPath = EditorGUILayout.TextField("表格路径", _excelPath);
            _fileName = EditorGUILayout.TextField("读取文件名", _fileName);
            _sheetName = EditorGUILayout.TextField("表名", _sheetName);
            _outputPath = EditorGUILayout.TextField("json输出路径", _outputPath);
            _className = EditorGUILayout.TextField("类名", _className);

            if (GUILayout.Button("结构表转换数据表"))
            {
                CoverDataExcel();
            }
            
            if (GUILayout.Button("数据表转类"))
            {
                ReadExcelToClass();
            }

            if (GUILayout.Button("数据表转换Json"))
            {
                CoverJson<DataItem>();
            }

            if (GUILayout.Button("数据表转换二进制"))
            {
                CoverBinary<DataItem>();
            }

            if (GUILayout.Button("json读取测试"))
            {
                string jsonFilePath = Path.Combine(_outputPath, $"{_sheetName}.json");
                string json = File.ReadAllText(jsonFilePath);
                Wrapper<DataItem> wp = JsonUtility.FromJson<Wrapper<DataItem>>(json);
                Debug.Log($"{wp.items[0].texts[0]}");
            }

            if (GUILayout.Button("二进制读取测试"))
            {
                string binaryFilePath = Path.Combine(_outputPath, $"{_sheetName}.bin");
                using (FileStream fs = new FileStream(binaryFilePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Wrapper<DataItem> wp = (Wrapper<DataItem>)bf.Deserialize(fs);
                    Debug.Log($"{wp.items[0].id}");
                }
                
            }
        }

    

        void CoverDataExcel()
        {
            // 注释
            List<string> annotations = new List<string>();
            // 字段
            List<string> fields = new List<string>();
            // 数据类型
            List<string> dataTypes = new List<string>();
            // 说明
            List<string> explains = new List<string>(); 
            string filePath =  Path.Combine(_excelPath, _fileName);
            Debug.Log($"读取文件夹路径 {filePath}");
            try
            {
                IWorkbook workbook = new XSSFWorkbook(filePath);
                ISheet sheet1 = workbook.GetSheet(_sheetName); //

                // 遍历行
                for (int i = 1; i <= sheet1.LastRowNum; i++)
                {
                    IRow row = sheet1.GetRow(i);
                    if (row == null) continue;

                    // 注释
                    ICell cell1 = row.GetCell(0);
                    annotations.Add(cell1?.ToString() ?? string.Empty);
                    // 字段
                    ICell cell2 = row.GetCell(1);
                    fields.Add(cell2?.ToString() ?? string.Empty);
                    // 数据类型
                    ICell cell3 = row.GetCell(2);
                    dataTypes.Add(cell3?.ToString() ?? string.Empty);

                    // 说明
                    ICell cell4 = row.GetCell(3);
                    explains.Add(cell4?.ToString() ?? string.Empty);

                    Debug.Log($"工作 {cell1?.ToString()} {cell2?.ToString()} {cell3?.ToString()}");
                }
                
                
                
            }
            catch (Exception e)
            {
                Debug.LogError($"无法读取表 {e.Message}");
            }
            var newFile = Path.Combine(_excelPath, $"{_sheetName}.xlsx"); 
            using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
            {
                IWorkbook newWorkbook = new XSSFWorkbook();
                ISheet newSheet1 = newWorkbook.CreateSheet(_sheetName);

                   
                // 创建注释
                IRow annotationRow = newSheet1.CreateRow(0);
                for (int i = 0; i < annotations.Count; i++)
                {
                    annotationRow.CreateCell(i).SetCellValue(annotations[i]);
                    newSheet1.AutoSizeColumn(i);
                }
                // 创建字段
                IRow fieldRow = newSheet1.CreateRow(1);

                for (int i = 0; i < fields.Count; i++)
                {
                    fieldRow.CreateCell(i).SetCellValue(fields[i]);
                    newSheet1.AutoSizeColumn(i);
                }
                // 创建数据类型
                IRow dataTypeRow = newSheet1.CreateRow(2);
                for (int i = 0; i < dataTypes.Count; i++)
                {
                    dataTypeRow.CreateCell(i).SetCellValue(dataTypes[i]);
                    newSheet1.AutoSizeColumn(i);
                }

                IRow explainsRow = newSheet1.CreateRow(3);
                for (int i = 0; i < explains.Count; i++)
                {
                    explainsRow.CreateCell(i).SetCellValue(explains[i]);
                    newSheet1.AutoSizeColumn(i);
                }

                // 写入文件
                newWorkbook.Write(fs);


            }
            Debug.Log($"新文件已生成：{newFile}");
            
        }
        

        void CreateFolder()
        {
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
                Debug.Log($"创建文件夹 路径{_outputPath}");
            }
        }
    

        [Serializable]
        public class Wrapper<T>
        {
            public List<T> items;
        }

        void CreateClassBegin()
        {
            sb.Clear();
            sb.Append("using System;\nusing System.Collections.Generic;\n");
            sb.Append("namespace Sly");
            sb.Append("\n{");
            sb.Append("\t[System.Serializable]");
            sb.Append($"\n\tpublic class {_className}");
            sb.Append("\n\t{");
           
        }

        void CreateClassEnd()
        {
            sb.Append("\t}");
            sb.Append("\n}");
            string classPath = Path.Combine(_editorPath, $"{_className}.cs");
            File.WriteAllText(classPath ,sb.ToString());
            Debug.Log("类创建完成"); 
        }
        
        void CreateClass(string an, string type, string field)
        {
            sb.Append($"\n\t\t//{an}"); 
            sb.Append($"\n\t\tpublic {type} {field};\n");
        }

        void ReadExcelToClass()
        {
             string filePath =  Path.Combine(_excelPath, $"{_sheetName}.xlsx");
            Debug.Log($"读取文件夹路径 {filePath}");
            CreateClassBegin();
            try
            {
                IWorkbook workbook = new XSSFWorkbook(filePath);
                ISheet sheet1 = workbook.GetSheetAt(0); // 获取到第一个工作表
                Debug.Log($"工作蒲名称 {workbook.NumberOfSheets} 工作表 {sheet1.SheetName}");

                IRow annotationRow = sheet1.GetRow(0);
                IRow fieldRow = sheet1.GetRow(1);
                IRow typeRow = sheet1.GetRow(2);
                // 遍历行 第⑤行为数据行


                for (int i = 0; i < fieldRow.LastCellNum; i++)
                {
                    ICell cell = fieldRow.GetCell(i);
                    ICell cell1 = typeRow.GetCell(i);
                    ICell cell2 = annotationRow.GetCell(i);
                    if (cell == null) continue;

                    string fieldName = cell.ToString().Trim().ToLower();
                    string typeName = cell1.ToString().Trim();
                    string annotationName = cell2.ToString().Trim();
                    CreateClass(annotationName, typeName, fieldName);
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"类创建失败 {e.Message}");
            }
            CreateClassEnd();   
        }
       
        List<T> ReadExcel<T>()
        {
            List<T> data = new List<T>();
       
           
            string filePath =  Path.Combine(_excelPath, $"{_sheetName}.xlsx");
            Debug.Log($"读取文件夹路径 {filePath}");
            try
            {
                IWorkbook workbook = new XSSFWorkbook(filePath);
                ISheet sheet1 = workbook.GetSheetAt(0); // 获取到第一个工作表
                Debug.Log($"工作蒲名称 {workbook.NumberOfSheets} 工作表 {sheet1.SheetName}");

                IRow headerRow = sheet1.GetRow(1);
                // 遍历行 第⑤行为数据行
                Dictionary<int, FieldInfo> columnMap = new Dictionary<int, FieldInfo>();


                FieldInfo[] fields = typeof(T).GetFields();

                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    ICell cell = headerRow.GetCell(i);
                    if (cell == null) continue;
                
                    string headerName = cell.ToString().Trim().ToLower();
                    FieldInfo field = fields.FirstOrDefault(f => f.Name.ToLower() == headerName);
                    if (field != null)
                    {
                        columnMap.Add(i, field);
                    }
                }
                
                for (int i = 4; i < sheet1.LastRowNum; i++)
                {
                    IRow row = sheet1.GetRow(i);
                    T item = Activator.CreateInstance<T>();
                    bool hasData = false;
                    if (row == null) continue;
                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        ICell cell = row.GetCell(j);

                        if (cell == null)
                        {
                            continue;
                        }
                        string value = cell.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            columnMap.TryGetValue(j, out FieldInfo field);
                            hasData = true;
                            // 简单类型转换
                            if (field != null)
                            {
                                Debug.Log(field.Name + $" {value}");
                                if (field.FieldType == typeof(string))
                                {
                                    field.SetValue(item, value);
                                }
                                else if (field.FieldType == typeof(int))
                                {
                                    if (int.TryParse(value, out int intValue))
                                        field.SetValue(item, intValue);
                                }
                                else if (field.FieldType == typeof(float))
                                {
                                    if (float.TryParse(value, out float floatValue))
                                        field.SetValue(item, floatValue);
                                }
                                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    char[] delimiters = new char[] { ',', '，' };
                                    string[] values = value.Trim().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                                    Debug.Log($"子字符串长度 {values.Length}");
                                    Type elementType = field.FieldType.GetGenericArguments()[0];
                                    if (elementType == typeof(int))
                                    {
                                        List<int> list = (List<int>)Activator.CreateInstance(field.FieldType); 
                                 
                                        foreach (string v in values)
                                        {
                                            if (int.TryParse(v.Trim(), out int intValue))
                                                list.Add(intValue);
                                        }
                                        field.SetValue(item, list); 
                                    }

                                    if (elementType == typeof(string))
                                    {
                                        List<string> list = (List<string>)Activator.CreateInstance(field.FieldType); 
                                 
                                        foreach (string v in values)
                                        {
                                            Debug.Log($"==== {v}");
                                                list.Add(v);
                                        }
                                        field.SetValue(item, list);  
                                    }
                                   
                                }
                                // 可以添加其他类型的转换...
                            }
                        }
                    
                    }
                    if (hasData)
                    {
                        data.Add(item); 
                    }
                   
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"无法读取表 {e.Message}");
            }
            return data;
        }

        void CoverJson<T>()
        {
            List<T> data  = ReadExcel<T>();
            CreateFolder();
            if (data.Count == 0)
            {
                Debug.LogError("未读取到任何数据，请检查 Excel 文件内容。");
                return;
            }

            // 将数据转换为 JSON
            Wrapper<T> wrapper = new Wrapper<T>()
            {
                items = data
            };
            string json = JsonUtility.ToJson(wrapper, true);
            string jsonFilePath = Path.Combine(_outputPath, $"{_sheetName}.json");
            File.WriteAllText(jsonFilePath, json);
            Debug.Log($"JSON 文件已生成：{jsonFilePath}");
        }

        void CoverBinary<T>()
        {
            List<T> data  = ReadExcel<T>();
            CreateFolder();
            if (data.Count == 0)
            {
                Debug.LogError("未读取到任何数据，请检查 Excel 文件内容。");
                return;
            }
            string binaryFilePath = Path.Combine(_outputPath, $"{_sheetName}.bin");
            using (FileStream fs = new FileStream(binaryFilePath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Wrapper<T> wp = new Wrapper<T>() {items = data};
                bf.Serialize(fs, wp);
            }
            // 将数据转换为 JSO
            
            Debug.Log($"二进制 文件已生成：{binaryFilePath}"); 
        }
    }
}