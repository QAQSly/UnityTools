using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CreateObjectParent : EditorWindow
{
    static CreateObjectParent()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnParentColorChange;
    }

    static void OnParentColorChange(int instanceId, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

        if (gameObject != null && gameObject.name.StartsWith("======"))
        {
            Color originalColor = GUI.contentColor;
            GUI.contentColor = new Color(230f / 255f, 82f / 255f, 124f / 255f);
            Rect labelRect = new Rect(selectionRect.x + 17, selectionRect.y - 1, selectionRect.width - 20, selectionRect.height);
            GUIContent content = new GUIContent(gameObject.name);
            EditorGUI.LabelField(labelRect, content);
            GUI.contentColor = originalColor;
        }
    }
    // Start is called before the first frame update
    [MenuItem("SlyTools/创建选中对象父对象")]
    public static void CreateParent()
    {
        GameObject[] gos = Selection.gameObjects;
        if (gos == null || gos.Length == 0)
        {
            Debug.LogError("未选中物体");
            return;
        }

        GameObject parent = new GameObject($"======{gos[0].name}======");
        foreach (var go in gos)
        {
            go.transform.SetParent(parent.transform);
        }

        Selection.activeGameObject = parent;

    }
  
}
