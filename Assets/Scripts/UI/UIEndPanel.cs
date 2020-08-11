using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEndPanel : MonoBehaviour
{
    private CanvasGroup _CanvasGroup;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Show(false);
    }

    public void Show(bool isVisible)
    {
        _CanvasGroup.interactable = isVisible;
        _CanvasGroup.alpha = (isVisible == true) ? 1 : 0;
        _CanvasGroup.blocksRaycasts = isVisible;
    }
}
