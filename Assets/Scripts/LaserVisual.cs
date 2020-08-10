﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserVisual : MonoBehaviour
{
    [SerializeField] private Transform _LaserStartPoint;

    private LineRenderer _LineRenderer;
    
    void DrawLaser(Transform target)
    {
        _LineRenderer.positionCount = 2;
            
        _LineRenderer.SetPosition(0, _LaserStartPoint.position);
        _LineRenderer.SetPosition(1, target.position);
    }
    
    void Start()
    {
        _LineRenderer = GetComponent<LineRenderer>();
    }

}