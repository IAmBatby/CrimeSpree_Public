using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

#if (UNITY_EDITOR)
[ExecuteInEditMode]
public class GetTexture2DInfo : MonoBehaviour
{
    [Button("Get Sprite Info")]
    public void GetSpriteInfo(Texture2D texture)
    {
        Texture2D icon = texture;
        string assetName = texture.name;

        Debug.Log
        (
        assetName + ":" +
        "Dimensions are " + icon.width + ", " + icon.height + ", " +
        "Filter Mode is " + icon.filterMode + ", " +
        "Ansio Level is " + icon.anisoLevel + ", " +
        "Format is " + icon.format + ", " +
        "Graphics Format is " + icon.graphicsFormat + ", " +
        "Wrap Mode is " + icon.wrapMode + ", " +
        "Mipmap Count is " + icon.mipmapCount + ", " +
        "Mapmap Bias Count is " + icon.mipMapBias + ",dwadwad"
        );
    }

    [MenuItem("Unity Editor Icons/Export All %e", priority = -1001)]
    private static void ExportIcons()
    {
        //EditorUtility.DisplayProgressBar("Export Icons", "Exporting...", 0.0f);
        try
        {
            var editorAssetBundle = GetEditorAssetBundle();
            var iconsPath = GetIconsPath();
            var count = 0;
            foreach (var assetName in EnumerateIcons(editorAssetBundle, iconsPath))
            {
                var icon = editorAssetBundle.LoadAsset<Texture2D>(assetName);
                if (icon == null)
                    continue;

                Debug.Log
                    (
                    assetName + ":" +
                    "Dimensions are " + icon.width + ", " + icon.height + ", " +
                    "Filter Mode is " + icon.filterMode + ", " +
                    "Ansio Level is " + icon.anisoLevel + ", " +
                    "Format is " + icon.format + ", " +
                    "Graphics Format is " + icon.graphicsFormat + ", " +
                    "Wrap Mode is " + icon.wrapMode + ", " +
                    "Mipmap Count is " + icon.mipmapCount + ", " +
                    "Mapmap Bias Count is " + icon.mipMapBias + ", "
                    );

                var readableTexture = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);

                //Graphics.CopyTexture(icon, readableTexture);

                //var folderPath = Path.GetDirectoryName(Path.Combine("icons/original/", assetName.Substring(iconsPath.Length)));
                //if (Directory.Exists(folderPath) == false)
                    //Directory.CreateDirectory(folderPath);

                //var iconPath = Path.Combine(folderPath, icon.name + ".png");
                //File.WriteAllBytes(iconPath, readableTexture.EncodeToPNG());

                //count++;
            }

            //Debug.Log($"{count} icons has been exported!");
        }
        finally
        {
            //EditorUtility.ClearProgressBar();
        }
    }

    private static IEnumerable<string> EnumerateIcons(AssetBundle editorAssetBundle, string iconsPath)
    {
        foreach (var assetName in editorAssetBundle.GetAllAssetNames())
        {
            if (assetName.StartsWith(iconsPath, StringComparison.OrdinalIgnoreCase) == false)
                continue;
            if (assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false &&
                assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) == false)
                continue;

            yield return assetName;
        }
    }

    private static string GetFileId(string proxyAssetPath)
    {
        var serializedAsset = File.ReadAllText(proxyAssetPath);
        var index = serializedAsset.IndexOf("_MainTex:", StringComparison.Ordinal);
        if (index == -1)
            return string.Empty;

        const string FileId = "fileID:";
        var startIndex = serializedAsset.IndexOf(FileId, index) + FileId.Length;
        var endIndex = serializedAsset.IndexOf(',', startIndex);
        return serializedAsset.Substring(startIndex, endIndex - startIndex).Trim();
    }

    private static AssetBundle GetEditorAssetBundle()
    {
        var editorGUIUtility = typeof(EditorGUIUtility);
        var getEditorAssetBundle = editorGUIUtility.GetMethod(
            "GetEditorAssetBundle",
            BindingFlags.NonPublic | BindingFlags.Static);

        return (AssetBundle)getEditorAssetBundle.Invoke(null, new object[] { });
    }

    private static string GetIconsPath()
    {
#if UNITY_2018_3_OR_NEWER
        return UnityEditor.Experimental.EditorResources.iconsPath;
#else
            var assembly = typeof(EditorGUIUtility).Assembly;
            var editorResourcesUtility = assembly.GetType("UnityEditorInternal.EditorResourcesUtility");

            var iconsPathProperty = editorResourcesUtility.GetProperty(
                "iconsPath",
                BindingFlags.Static | BindingFlags.Public);

            return (string)iconsPathProperty.GetValue(null, new object[] { });
#endif
    }
}
#endif