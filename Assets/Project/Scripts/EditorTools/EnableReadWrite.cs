using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if (UNITY_EDITOR)
public class EnableReadWrite
{
    [MenuItem("Assets/Enable Read And Write On Raw Mesh")]
    public static void SetMeshReadWrite()
    {
        Mesh mesh = (Mesh)Selection.activeObject;
        string path = Path.GetFullPath(mesh.name).Replace(mesh.name, string.Empty).Replace(@"\", @"/") + AssetDatabase.GetAssetPath(mesh);
        if (File.Exists(path))
        {
            File.WriteAllText(path, File.ReadAllText(path).Replace("m_IsReadable: 0", "m_IsReadable: 1"));
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mesh));
            AssetDatabase.Refresh();
        }
    }

    //[MenuItem("Assets/Enable Read And Write On Raw Mesh", true)]
    //public static bool SetMeshReadWriteValidation()
    //{
        //return Selection.activeObject.GetType() == typeof(Mesh);
    //}
}
#endif
