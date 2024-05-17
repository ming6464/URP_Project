using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Smoke_Script : MonoBehaviour
{
    public bool Run;
    public bool Reset;
    public bool Edit;
    [Space(3)]
    [SerializeField] private float Speed;
    [Space(3)]
    [SerializeField] private MeshRenderer _smokeMeshRenderer;

    [Space(3)] [SerializeField] private Transform _smokeTf;
    [Header("Edit")] 
    [SerializeField] private float _radiusMax = 5;
    [SerializeField] private Color _colorBelow;
    [SerializeField] private Color _colorAbove;
    [SerializeField] private Color _colorStoke;
    [SerializeField] private Color _colorOutline;
    [Space(3)]
    [SerializeField] private Transform _lightDirection1;
    [Space(3)]
    [SerializeField] private Transform _curvePoint0;
    [SerializeField] private Transform _curvePoint1;
    [SerializeField] private Transform _curvePoint2;
    [SerializeField] private Transform _curvePoint3;
    
    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _posDefault;
    private float _radius;
    private float _t;
    private MaterialPropertyBlock _materialProperty;
    
    //ID
    private static readonly int IDRadiusMax = Shader.PropertyToID("_radiusMax");
    private static readonly int IDColorBelow = Shader.PropertyToID("_colorBelow");
    private static readonly int IDColorAbove = Shader.PropertyToID("_colorAbove");
    private static readonly int IDColorStoke = Shader.PropertyToID("_colorStoke");
    private static readonly int IDColorOutline = Shader.PropertyToID("_colorOutline");
    private static readonly int IDLightDirection = Shader.PropertyToID("_lightDirect");
    private static readonly int IDCurve0 = Shader.PropertyToID("_curve0");
    private static readonly int IDCurve1 = Shader.PropertyToID("_curve1");
    private static readonly int IDCurve2 = Shader.PropertyToID("_curve2");
    private static readonly int IDCurve3 = Shader.PropertyToID("_curve3");
    
    
    private void Awake()
    {
        _materialProperty = new MaterialPropertyBlock();
        _smokeMeshRenderer.GetPropertyBlock(_materialProperty);
    }
    
    private void OnDisable()
    {
        ResetValue();
    }

    private void Start()
    {
        _p0 = _curvePoint0.localPosition;
        _p1 = _curvePoint1.localPosition;
        _p2 = _curvePoint2.localPosition;
        _posDefault = _curvePoint3.localPosition;
        _materialProperty.SetColor(IDColorAbove,_colorAbove);
        _materialProperty.SetColor(IDColorBelow,_colorBelow);
        _materialProperty.SetColor(IDColorOutline,_colorOutline);
        _materialProperty.SetColor(IDColorStoke,_colorStoke);
        ResetValue();
        
    }

    private void Update()
    {
        if (Reset)
        {
            Reset = false;
            ResetValue();
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
            _curvePoint0.localPosition = Vector3.Lerp(_posDefault,_p0,a);
            _curvePoint1.localPosition = Vector3.Lerp(_posDefault,_p1,a);
            _curvePoint2.localPosition = Vector3.Lerp(_posDefault,_p2,a);
            _radius = _radiusMax * a;
            _smokeTf.localScale = new Vector3(a, a, a);
            SetValue();
        }
        
    }

    #region Public Func

    public void ResetValue()
    {
        _curvePoint0.localPosition = _posDefault;
        _curvePoint1.localPosition = _posDefault;
        _curvePoint2.localPosition = _posDefault;
        _radius = 0;
        _t = 0;
        _smokeTf.localScale = new Vector3(0, 0, 0);
        SetValue();
    }

    public void Play()
    {
        _t = Time.deltaTime;
    }

    #endregion
    
    
    private void SetValue()
    {
        _materialProperty.SetVector(IDCurve0,_curvePoint0.localPosition);
        _materialProperty.SetVector(IDCurve1,_curvePoint1.localPosition);
        _materialProperty.SetVector(IDCurve2,_curvePoint2.localPosition);
        _materialProperty.SetVector(IDCurve3,_curvePoint3.localPosition);
        _materialProperty.SetFloat(IDRadiusMax,_radius);
        SaveMaterialChange();
    }

    private void SaveMaterialChange()
    {
        _smokeMeshRenderer.SetPropertyBlock(_materialProperty);
    }

    private void OnDrawGizmos()
    {
        if(!Edit) return;
        _materialProperty = new MaterialPropertyBlock();
        _smokeMeshRenderer.GetPropertyBlock(_materialProperty);
        Gizmos.color = Color.green;
        Vector3 vt = _lightDirection1.forward;
        Gizmos. DrawRay(_lightDirection1.position,vt * 10);
        
        _materialProperty.SetVector(IDLightDirection,-vt);
        
        _materialProperty.SetVector(IDCurve0,_curvePoint0.localPosition);
        _materialProperty.SetVector(IDCurve1,_curvePoint1.localPosition);
        _materialProperty.SetVector(IDCurve2,_curvePoint2.localPosition);
        _materialProperty.SetVector(IDCurve3,_curvePoint3.localPosition);
        
        _materialProperty.SetColor(IDColorAbove,_colorAbove);
        _materialProperty.SetColor(IDColorBelow,_colorBelow);
        _materialProperty.SetColor(IDColorOutline,_colorOutline);
        _materialProperty.SetColor(IDColorStoke,_colorStoke);
        
        _materialProperty.SetFloat(IDRadiusMax,_radiusMax);
        
        SaveMaterialChange();
    }

}
