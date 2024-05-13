using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Smoke_Script : MonoBehaviour
{
    public bool Run;
    public bool Reset;
    public float Speed;
    public float Radius;
    [SerializeField] private Material _smokeMat;
    [Header("Light")] 
    [SerializeField] private Transform _lightDirection1;
    [Header("Curve Info")]
    [SerializeField] private Transform _curvePoint0;
    [SerializeField] private Transform _curvePoint1;
    [SerializeField] private Transform _curvePoint2;
    [SerializeField] private Transform _curvePoint3;
    [Header("Edit")] [SerializeField] private bool _edit;
    [SerializeField] private Color _colorPoint;
    [SerializeField] private Color _colorLine;
    [SerializeField] private int _density;
    
    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _posDefault;
    private float _radius;
    private float _t;

    private void OnValidate()
    {
        if (_edit)
        {
            _edit = false;
            LoadEdit();
        }
    }

    private void Start()
    {
        _p0 = _curvePoint0.position;
        _p1 = _curvePoint1.position;
        _p2 = _curvePoint2.position;
        _posDefault = _curvePoint3.position;
        ResetValue();
        SetValue();
    }

    private void Update()
    {
        if (Reset)
        {
            Reset = false;
            ResetValue();
            SetValue();
        }

        if (_t == 0 && Run)
        {
            Run = false;
            _t += Time.deltaTime;
        }

        if (_t > 0)
        {
            float a = _t;
            if (_t > 1)
            {
                _t = 0;
                a = 1;
            }
            else
            {
                _t += Time.deltaTime * Speed;
            }
            _curvePoint0.position = Vector3.Lerp(_posDefault,_p0,a);
            _curvePoint1.position = Vector3.Lerp(_posDefault,_p1,a);
            _curvePoint2.position = Vector3.Lerp(_posDefault,_p2,a);
            _radius = Mathf.Lerp(0, Radius, a);
            transform.localScale = new Vector3(a, 1, a);
            SetValue();
            
        }
        
    }

    private void ResetValue()
    {
        _curvePoint0.position = _posDefault;
        _curvePoint1.position = _posDefault;
        _curvePoint2.position = _posDefault;
        _radius = 0;
        _t = 0;
        transform.localScale = new Vector3(0, 1, 0);
    }

    private void SetValue()
    {
        Vector3 pos0 = _curvePoint0.localPosition;
        Vector3 pos1 = _curvePoint1.localPosition;
        Vector3 pos2 = _curvePoint2.localPosition;
        Vector3 pos3 = _curvePoint3.localPosition;
        _smokeMat.SetVector("_curve0",pos0);
        _smokeMat.SetVector("_curve1",pos1);
        _smokeMat.SetVector("_curve2",pos2);
        _smokeMat.SetVector("_curve3",pos3);
        _smokeMat.SetFloat("_maxWidth",_radius);
    }

    private void LoadEdit()
    {
        try
        {
            Vector3 lightDir = _curvePoint3.position - _lightDirection1.position;
            if (lightDir != Vector3.zero)
            {
                _smokeMat.SetVector("_LightDirect1",Vector3.Normalize(lightDir));
            }
            _smokeMat.SetVector("_curve0",_curvePoint0.localPosition);
            _smokeMat.SetVector("_curve1",_curvePoint1.localPosition);
            _smokeMat.SetVector("_curve2",_curvePoint2.localPosition);
            _smokeMat.SetVector("_curve3",_curvePoint3.localPosition);
        }
        catch
        {
            //ignored
        }
    }
}
