using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sly
{
    /// <summary>
    /// 合并对象mesh 使用时需要配合创建父对象使用
    /// </summary>
    [InitializeOnLoad]
    public class CreateObjectMesh
    {
        // 选中所有对象
        [MenuItem("SlyTools/合并子对象mesh")]
        public static void Combine()
        {
            GameObject go = Selection.activeGameObject;
            if (go == null)
            {
                Debug.LogError("没有选中对象或没有激活 请激活");
                return;
            }
            // 获得meshFilter[] huo CombineInstance[]
            MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>();
            // 收集子对象的材质
            List<Material> materials = new List<Material>();
            
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].gameObject.transform.localToWorldMatrix;

                Renderer renderer = meshFilters[i].gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    foreach (var mat in renderer.sharedMaterials)
                    {
                        if (!materials.Contains(mat))
                        {
                            materials.Add(mat);
                        }
                    }
                }
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            // 获得mesh
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = go.AddComponent<MeshFilter>();
            }
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.CombineMeshes(combine);

            Renderer combineRender = go.GetComponent<Renderer>();
            if (combineRender == null)
            {
                combineRender = go.AddComponent<MeshRenderer>();
            }

            combineRender.materials = materials.ToArray();
            Debug.Log("网格和材质合并完成!");
        }
    }
}