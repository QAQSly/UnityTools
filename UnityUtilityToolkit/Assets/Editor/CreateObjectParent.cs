using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateObjectParent : EditorWindow
{
    // Start is called before the first frame update
    [MenuItem("SlyTools/创建选中对象父对象")]
    public static void CreateParent()
    {
        GameObject[] gos = Selection.gameObjects;
        Debug.Log("===当前选中的物体===");
        if (gos == null || gos.Length == 0)
        {
            Debug.LogError("未选中物体");
        }

        GameObject parent = new GameObject("" + gos[0].name);
        foreach (var go in gos)
        {
            go.transform.SetParent(parent.transform);
        }

        Selection.activeGameObject = parent;

    }
  
}
