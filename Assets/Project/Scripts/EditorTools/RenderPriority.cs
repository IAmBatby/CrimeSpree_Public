using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.ProBuilder;
using System.IO;
using UnityEditor;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
public class RenderPriority : MonoBehaviour
{
    [Button("Set Render Priority")]
    public void SetRenderPriority(int renderPriorityValue)
    {
        Debug.Log("Test");
        var meshRendererArrays = FindObjectsOfType<MeshRenderer>();
        int debugCount = 0;

        Debug.Log("Setting Render Priorities");
        foreach (MeshRenderer meshRenderer in meshRendererArrays)
        {
            meshRenderer.rendererPriority = renderPriorityValue;
            debugCount++;
        }
        Debug.Log("Render Priorities Set! Amount Set Was: " + debugCount);
    }

    [Button("Search ProBuilder Meshes")]
    public Transform GetProbuilderMeshName(string searchName)
    {
        ProBuilderMesh[] meshRendererArrays = FindObjectsOfType<ProBuilderMesh>();

        foreach (ProBuilderMesh proBuilderMesh in meshRendererArrays)
        {
            string searchString = proBuilderMesh.gameObject.GetComponent<MeshFilter>().sharedMesh.name.Replace(" Instance", ""); //I Hate this but ProBuilderMesh stores it's filter privately.
            if (searchString == searchName)
                return (proBuilderMesh.transform);
        }
        return (null);
    }

    [Button("Clean Stray Probuilder Meshes")]
    public void DestroyMesh(string name)
    {
        Mesh[] meshArrays = FindObjectsOfType<Mesh>();

        foreach (Mesh mesh in meshArrays)
        {
            if (mesh.name.Contains("pb_Mesh-"))
                DestroyImmediate(mesh);
        }
    }

    public void GetTexture2DInfo(string name)
    {
        var editorAssetBundle = GetEditorAssetBundle();
    }

    AssetBundle GetEditorAssetBundle()
    {
        var editorGUIUtility = typeof(EditorGUIUtility);
        var getEditorAssetBundle = editorGUIUtility.GetMethod("GetEditorAssetBundle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        return (AssetBundle)getEditorAssetBundle.Invoke(null, new object[] { });
    }
}
#endif
