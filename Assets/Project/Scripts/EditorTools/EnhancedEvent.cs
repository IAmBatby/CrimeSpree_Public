using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;

[Serializable]
public class EnhancedEvent : UnityEvent, ISerializationCallbackReceiver
{
    [SerializeField] public EventInfo[] eventInfo;
    public List<EventInfo> EventInfo => new List<EventInfo>(eventInfo);
    public delegate void OnEventChange();
    public event OnEventChange onEventChange;

    public void InvokeEventChange()
    {
        //onEventChange?.Invoke();
    }

    public void OnValidate()
    {
        Debug.Log("dwadawdawd");
    }
}

[System.Serializable]
public struct EventInfo
{
    [SerializeField] public enum Mode { Defined, Void, Object, Int, Float, String, Bool }
    [SerializeField] public Mode parameterType;
    [SerializeField] public string className;
    [SerializeField] public string functionName;
    [SerializeField] public UnityEngine.Object parameterObject;
    [SerializeField] public int parameterInt;
    [SerializeField] public float parameterFloat;
    [SerializeField] public string parameterString;
    [SerializeField] public bool parameterBool;

    public bool TryGetParameter(out UnityEngine.Object outParameterObject)
    {
        if (parameterType == Mode.Object)
        {
            outParameterObject = parameterObject;
            return (true);
        }
        else
        {
            outParameterObject = null;
            return (false);
        }
    }

    public bool TryGetParameter(out string outParameterString)
    {
        if (parameterType == Mode.String)
        {
            outParameterString = parameterString;
            return (true);
        }
        else
        {
            outParameterString = null;
            return (false);
        }
    }

    public bool TryGetParameter(out bool outParameterBool)
    {
        if (parameterType == Mode.Bool)
        {
            outParameterBool = parameterBool;
            return (true);
        }
        else
        {
            outParameterBool = false;
            return (false);
        }
    }

    public bool TryGetParameter(out int outParameterInt)
    {
        if (parameterType == Mode.Int)
        {
            outParameterInt = parameterInt;
            return (true);
        }
        else
        {
            outParameterInt = 0;
            return (false);
        }
    }

    public bool TryGetParameter(out float outParameterFloat)
    {
        if (parameterType == Mode.Float)
        {
            outParameterFloat = parameterFloat;
            return (true);
        }
        else
        {
            outParameterFloat = 0f;
            return (false);
        }
    }
}