using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIEndPanel : MonoBehaviour
{
    [SerializeField] private Text _WinnerText;
    [SerializeField] private Text _TimeText;
    
    private CanvasGroup _CanvasGroup;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Show(false);
    }

    private void Show(bool isVisible)
    {
        _CanvasGroup.interactable = isVisible;
        _CanvasGroup.alpha = (isVisible == true) ? 1 : 0;
        _CanvasGroup.blocksRaycasts = isVisible;
    }

    public void ShowEndGamePanel(Team winner, float gameTime)
    {
        _WinnerText.text = $@"Победила команда {winner.ToString()}";
        _TimeText.text = $@"Время: {gameTime.ToString()}";
        
        Show(true);
    }
}
