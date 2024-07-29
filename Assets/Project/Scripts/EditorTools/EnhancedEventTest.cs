using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnhancedEventTest : MonoBehaviour
{
    [HideInInspector] public EventInfo[] eventInfos => enhancedEvent.eventInfo;
    [HideInInspector] public EventInfo[] eventInfos2 => otherEvent.eventInfo;
    [Header("Current Selection"), EnumToggleButtons, HideLabel, OnValueChanged("RefreshFoodList")]
    public FirstEnum EnumFieldWide;
    [EnumToggleButtons, HideLabel, OnValueChanged("RefreshFoodList")]
    public SecondEnum EnumFieldWide1;
    [Space(10), Header("Current Selection Order"), ListDrawerSettings(IsReadOnly = true, DraggableItems = false, Expanded = true)]
    public List<string> foodList = new List<string>();
    [HideInInspector] public EnhancedEvent enhancedEvent;
    [HideInInspector] public EnhancedEvent otherEvent;

    public void TestInfo()
    {
        //Debug.Log("Enhanced Event Function Name: " + enhancedEvent.eventInfo[0].m_MethodName);
        //Debug.Log("Enhanced Event Object: " + enhancedEvent.eventInfo[0].m_Object);
        Debug.Log(otherEvent.eventInfo[0].TryGetParameter(out UnityEngine.Object myObject));
        foreach (EventInfo eventInfo in otherEvent.eventInfo)
        {
            Debug.Log(eventInfo.parameterObject);
            Debug.Log(otherEvent.eventInfo[0].parameterObject);
        }
        enhancedEvent.onEventChange += Ding;
    }

    public void EmptyEventInfos()
    {
        enhancedEvent.eventInfo = null;
        otherEvent.eventInfo = null;
    }

    public void Ding()
    {
        Debug.Log("yay");
    }

    [System.Flags]
    public enum FirstEnum
    {
        Bun_Up = 1 << 1,
        Bun_Down = 1 << 2,
        Tomato_Up = 1 << 3,
    }

    [System.Flags]
    public enum SecondEnum
    {
        Lettuce_Up = 1 << 1,
        Beef_Up = 1 << 2,
        Beef_Down = 1 << 3,
    }

    public void RefreshFoodList()
    {
        foodList.Clear();
        int counter = 1;
        if (EnumFieldWide.HasFlag(FirstEnum.Bun_Down))
        {
            foodList.Add("Option " + counter + ": " + "Bun_Down");
            counter++;
        }
        if (EnumFieldWide.HasFlag(FirstEnum.Bun_Up))
        {
            foodList.Add("Option " + counter + ": " + "Bun_Up");
            counter++;
        }
        if (EnumFieldWide.HasFlag(FirstEnum.Tomato_Up))
        {
            foodList.Add("Option " + counter + ": " + "Tomato_Up");
            counter++;
        }
        if (EnumFieldWide1.HasFlag(SecondEnum.Lettuce_Up))
        {
            foodList.Add("Option " + counter + ": " + "Lettuce_Up");
            counter++;
        }
        if (EnumFieldWide1.HasFlag(SecondEnum.Beef_Up))
        {
            foodList.Add("Option " + counter + ": " + "Beef_Up");
            counter++;
        }
        if (EnumFieldWide1.HasFlag(SecondEnum.Beef_Down))
        {
            foodList.Add("Option " + counter + ": " + "Beef_Down");
            counter++;
        }
    }
}
