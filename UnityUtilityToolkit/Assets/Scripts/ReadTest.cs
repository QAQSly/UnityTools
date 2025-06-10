using System;
using System.IO;
using UnityEngine;

namespace Sly
{
    public class ReadTest : MonoBehaviour
    {
        private string _sheetName = "属性表";
        private string _outputPath = "Assets/Jsons";
        private void Start()
        {
            Debug.Log("读取测试");
            string jsonFilePath = Path.Combine(_outputPath, $"{_sheetName}.json");
            string json = File.ReadAllText(jsonFilePath);
            Wrapper<DataItem> wp = JsonUtility.FromJson<Wrapper<DataItem>>(json);
            DataItem item = wp.items.Find(p => p.id == 123);
            Debug.Log($"当前item的名字 {item.name}");
        }
    }
}