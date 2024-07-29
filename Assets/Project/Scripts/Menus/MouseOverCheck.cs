using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class MouseOverCheck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouseOver;
    public delegate void OnMouseOver();
    public event OnMouseOver onMouseEnter;
    public event OnMouseOver onMouseExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        onMouseEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        onMouseExit?.Invoke();
    }
}
