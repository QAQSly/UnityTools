using UnityEditor;
using UnityEngine;

namespace Sly
{
    [InitializeOnLoad]
    public class CreateSceneEditorGizmos : Editor
    {
        public static float arrowLength = 2.0f;    // 箭头长度
        public static float arrowHeadAngle = 20f;  // 箭头角度
        public static float arrowHeadSize = 0.2f;  // 箭头头部大小
        static CreateSceneEditorGizmos()
        {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            
            // 获取当前选中的游戏对象
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                return; // 如果没有选中的对象，直接返回
            }

          
            Vector3 position = go.transform.position;
        
            // 箭头方向（物体前方）
            Vector3 forward = go.transform.forward;
        
            // 设置 Handles 颜色
            Handles.color = Color.yellow;
            Vector3 arrowEnd = position + forward * arrowLength;

            // 绘制主线（从物体位置到箭头终点）
            Handles.DrawLine(position, arrowEnd, 2f);  // 2f 是线宽（Unity 2020+ 支持）

            // 计算箭头两侧点（形成三角形）
            Vector3 right = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(forward) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
        
            // 绘制箭头两侧线
            Handles.DrawLine(arrowEnd, arrowEnd + right * arrowHeadSize);
            Handles.DrawLine(arrowEnd, arrowEnd + left * arrowHeadSize);
        }
    }
}