using UnityEditor;
using UnityEngine;

namespace Sly
{
    [InitializeOnLoad]
    [CustomEditor(typeof(GameObject))]
    public class CreateSceneEditorGizmos : Editor
    {
        private void OnSceneGUI()
        {
            // 获取当前选中的游戏对象
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                return; // 如果没有选中的对象，直接返回
            }
            // 设置 Handles 的颜色
            Handles.color = Color.red;

            Handles.DrawWireCube(go.transform.position, Vector3.one);
        }
    }
}