using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sly
{
    public class CreateDefaultFolders : EditorWindow
    {
       
        
        private static string mainFolderName = "MainGame";
        

        [MenuItem("SlyTools/创建游戏默认文件夹")]
        public static void ShowWindow()
        {
            GetWindow<CreateDefaultFolders>("创建游戏默认文件夹");
        }

        private void OnGUI()
        {
            GUILayout.Label("输入主文件夹名：", EditorStyles.boldLabel);
            mainFolderName = EditorGUILayout.TextField("主文件夹名", mainFolderName);

            if (GUILayout.Button("创建文件夹"))
            {
                CreateFolders(mainFolderName);
            }
        }

        public static void CreateFolders(string folderName)
        {
            string[] folders = {
                "Assets/Scripts",
                "Assets/Scenes",
                $"Assets/{mainFolderName}/Scenes",
                $"Assets/{mainFolderName}/Textures",
                $"Assets/{mainFolderName}/Models",
                $"Assets/{mainFolderName}/Sprites",
                $"Assets/{mainFolderName}/Scripts",
                $"Assets/{mainFolderName}/Audio",
                $"Assets/{mainFolderName}/Fonts",
                $"Assets/{mainFolderName}/Materials",
                $"Assets/{mainFolderName}/Animations",
                $"Assets/{mainFolderName}/Prefabs",
                "Assets/Resources",
                "Assets/Plugins",
                "Assets/Plugins/Editor",
                "Assets/Plugins/Runtime",
                "Assets/StreamingAssets",
          
            };
            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parentFolder = Path.GetDirectoryName(folder);
                    string folderNamePart = Path.GetFileName(folder);

                    if (!AssetDatabase.IsValidFolder(parentFolder))
                    {
                        AssetDatabase.CreateFolder(Path.GetDirectoryName(parentFolder), Path.GetFileName(parentFolder));
                    }

                    AssetDatabase.CreateFolder(parentFolder, folderNamePart);
                }
            }
            Debug.Log("===游戏默认文件夹创建成功!===");
        }
    }
}