#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEditorInternal;
using Unity.VisualScripting;
using System;

[CustomPropertyDrawer(typeof(EnhancedEvent))]
public class EnhancedEventDrawer : PropertyDrawer
{
    UnityEventDrawer eventDrawer;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (eventDrawer == null)
            eventDrawer = new UnityEventDrawer();

        EditorGUI.BeginChangeCheck();

        eventDrawer.OnGUI(position, property, label);

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log("Running Drawer End Code");
            
            SerializedProperty unityEventProperty = property.FindPropertyRelative("m_PersistentCalls.m_Calls.Array");

            SerializedProperty eventInfoProperty = property.FindPropertyRelative("eventInfo");

            eventInfoProperty.arraySize = unityEventProperty.arraySize;

            for (int i = 0; i < unityEventProperty.arraySize; i++)
            {
                CopySingleEventInfo(eventInfoProperty.GetArrayElementAtIndex(i), unityEventProperty.GetArrayElementAtIndex(i));
            }

            EnhancedEvent enhancedEvent = (EnhancedEvent)property.GetUnderlyingValue();
            enhancedEvent.InvokeEventChange();
        }
    }

    public void CopySingleEventInfo(SerializedProperty eventInfoProperty, SerializedProperty unityEventActionProperty)
    {
        EventInfo.Mode eventMode = (EventInfo.Mode)unityEventActionProperty.FindPropertyRelative("m_Mode").enumValueIndex;

        eventInfoProperty.FindPropertyRelative("parameterObject").objectReferenceValue = null;
        eventInfoProperty.FindPropertyRelative("parameterInt").intValue = 0;
        eventInfoProperty.FindPropertyRelative("parameterFloat").floatValue = 0f;
        eventInfoProperty.FindPropertyRelative("parameterString").stringValue = string.Empty;
        eventInfoProperty.FindPropertyRelative("parameterBool").boolValue = false;

        if (eventMode == EventInfo.Mode.Object)
            eventInfoProperty.FindPropertyRelative("parameterObject").objectReferenceValue = unityEventActionProperty.FindPropertyRelative("m_Arguments.m_ObjectArgument").objectReferenceValue;

        eventInfoProperty.FindPropertyRelative("className").stringValue = unityEventActionProperty.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue;
        eventInfoProperty.FindPropertyRelative("functionName").stringValue = unityEventActionProperty.FindPropertyRelative("m_MethodName").stringValue;
        eventInfoProperty.FindPropertyRelative("parameterType").enumValueIndex = (int)eventMode;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (eventDrawer == null)
            eventDrawer = new UnityEventDrawer();

        return eventDrawer.GetPropertyHeight(property, label);
    }
}
#endif
