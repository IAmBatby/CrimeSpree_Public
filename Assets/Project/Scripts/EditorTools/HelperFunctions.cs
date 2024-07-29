#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class HelperFunctions
{
    public static IEnumerable<ValueDropdownItem> GetItemDataAssets(Type type)
    {
        List<ValueDropdownItem> newScriptableObjects = new List<ValueDropdownItem>();
        IEnumerable<ValueDropdownItem> scriptableObjects;

        scriptableObjects = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject")
        .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
        .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x)));

        foreach (ValueDropdownItem item in scriptableObjects)
        {
            if (item.Value.GetType() == type)
                newScriptableObjects.Add(item);
            else if (item.Value.GetType().BaseType != null && item.Value.GetType().BaseType == type)
                    newScriptableObjects.Add(item);
        }
        return (newScriptableObjects);
    }

    public static ScriptableManagerSettings GetScriptableManager()
    {
        List<ValueDropdownItem> newScriptableObjects = new List<ValueDropdownItem>();
        IEnumerable<ValueDropdownItem> scriptableObjects;

        scriptableObjects = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject")
        .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
        .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x)));

        foreach (ValueDropdownItem item in scriptableObjects)
        {
            if (item.Value.GetType() == typeof(ScriptableManagerSettings))
                return ((ScriptableManagerSettings)item.Value);
        }
        return (null);
    }


    public static ItemData GetItemDataAsset(UnityEngine.Object paramObject)
    {
        List<ValueDropdownItem> newScriptableObjects = new List<ValueDropdownItem>();
        IEnumerable<ValueDropdownItem> scriptableObjects;
        UnityEngine.Object returnObject = null;

        scriptableObjects = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject")
        .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
        .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x)));

        foreach (ValueDropdownItem item in scriptableObjects)
            if ((UnityEngine.Object)item.Value == paramObject)
                returnObject = (UnityEngine.Object)item.Value;

        return ((ItemData)returnObject);
    }

    public static IEnumerable<ValueDropdownItem> GetIconSpriteAssets()
    {
        var root = "Assets/Project/Textures/Sprites";
        List<ValueDropdownItem> returnSpriteAssets = new List<ValueDropdownItem>();
        IEnumerable<ValueDropdownItem> spriteAssets = UnityEditor.AssetDatabase.GetAllAssetPaths()
        .Where(x => x.StartsWith(root))
        .Select(x => x.Substring(root.Length))
        .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(root + x)));

        foreach (ValueDropdownItem item in spriteAssets)
        {
            if (item.Value is Texture2D)
            {
                UnityEngine.Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath((UnityEngine.Object)item.Value));
                foreach(UnityEngine.Object obj in objects)
                {
                    if (obj.GetType() == typeof(Sprite))
                    {
                        ValueDropdownItem valueItem;
                        valueItem.Text = item.Text;
                        valueItem.Value = obj;
                        returnSpriteAssets.Add(valueItem);
                    }
                }
            }
        }

        return (returnSpriteAssets);

    }


    public static IEnumerable<ValueDropdownItem> GetModifierSprites()
    {
        var root = "Assets/Project/Textures/Sprites/Modifiers";
        List<ValueDropdownItem> returnSpriteAssets = new List<ValueDropdownItem>();
        IEnumerable<ValueDropdownItem> spriteAssets = UnityEditor.AssetDatabase.GetAllAssetPaths()
        .Where(x => x.StartsWith(root))
        .Select(x => x.Substring(root.Length))
        .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(root + x)));

        foreach (ValueDropdownItem item in spriteAssets)
        {
            if (item.Value is Texture2D)
            {
                UnityEngine.Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath((UnityEngine.Object)item.Value));
                foreach (UnityEngine.Object obj in objects)
                {
                    if (obj.GetType() == typeof(Sprite))
                    {
                        ValueDropdownItem valueItem;
                        valueItem.Text = item.Text;
                        valueItem.Value = obj;
                        returnSpriteAssets.Add(valueItem);
                    }
                }
            }
        }
        return (returnSpriteAssets);
    }


    public static IEnumerable<ValueDropdownItem> GetSkillSprites()
    {
        var root = "Assets/Project/Textures/Sprites/Skills";
        List<ValueDropdownItem> returnSpriteAssets = new List<ValueDropdownItem>();
        IEnumerable<ValueDropdownItem> spriteAssets = UnityEditor.AssetDatabase.GetAllAssetPaths()
        .Where(x => x.StartsWith(root))
        .Select(x => x.Substring(root.Length))
        .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(root + x)));

        foreach (ValueDropdownItem item in spriteAssets)
        {
            if (item.Value is Texture2D)
            {
                UnityEngine.Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath((UnityEngine.Object)item.Value));
                foreach (UnityEngine.Object obj in objects)
                {
                    if (obj.GetType() == typeof(Sprite))
                    {
                        ValueDropdownItem valueItem;
                        valueItem.Text = item.Text;
                        valueItem.Value = obj;
                        returnSpriteAssets.Add(valueItem);
                    }
                }
            }
        }
        return (returnSpriteAssets);
    }

    public static IEnumerable GetWaypointCollections()
    {
        Func<Transform, string> getPath = null;
        getPath = x => (x ? getPath(x.parent) + "/" + x.gameObject.name : "");
        return GameObject.FindObjectsOfType<WaypointCollection>().Select(x => new ValueDropdownItem(getPath(x.transform), x));
    }

    public static IEnumerable GetNPCs()
    {
        Func<Transform, string> getPath = null;
        getPath = x => (x ? getPath(x.parent) + "/" + x.gameObject.name : "");
        return GameObject.FindObjectsOfType<NPC>().Select(x => new ValueDropdownItem(getPath(x.transform), x));
    }
}
#endif