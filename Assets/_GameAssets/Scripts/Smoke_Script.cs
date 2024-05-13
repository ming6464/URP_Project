using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Smoke_Script : MonoBehaviour
{
    [SerializeField] private Material _smokeMat;
    [Header("Light")] 
    [SerializeField] private Transform _lightDirection1;
    [SerializeField] private Transform _lightDirection2;
    [Header("Curve Info")]
    [SerializeField] private Transform _curvePoint0;
    [SerializeField] private Transform _curvePoint1;
    [SerializeField] private Transform _curvePoint2;
    [SerializeField] private Transform _curvePoint3;
    [Header("Draw")]
    [SerializeField] private Color _colorPoint;
    [SerializeField] private Color _colorLine;
    [SerializeField] private int _density;
    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.color = Color.yellow;
            Vector3 lightDir = _curvePoint3.position - _lightDirection1.position;
            if (lightDir != Vector3.zero)
            {
                _smokeMat.SetVector("_LightDirect1",Vector3.Normalize(lightDir));
                Gizmos.DrawLine(_curvePoint3.position,_lightDirection1.position);
            }
            
            Gizmos.color = Color.red;
            lightDir = _curvePoint3.position - _lightDirection2.position;
            if (lightDir != Vector3.zero)
            {
                _smokeMat.SetVector("_LightDirect2",Vector3.Normalize(lightDir));
                Gizmos.DrawLine(_curvePoint3.position,_lightDirection2.position);
            }
            
            Vector3 pos0 = _curvePoint0.localPosition;
            Vector3 pos1 = _curvePoint1.localPosition;
            Vector3 pos2 = _curvePoint2.localPosition;
            Vector3 pos3 = _curvePoint3.localPosition;
            Gizmos.color = _colorPoint;
            Gizmos.DrawSphere(pos0,.1f);
            Gizmos.DrawSphere(pos1,.1f);
            Gizmos.DrawSphere(pos2,.1f);
            Gizmos.DrawSphere(pos3,.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_curvePoint0.position,_curvePoint1.position);
            Gizmos.DrawLine(_curvePoint1.position,_curvePoint2.position);
            Gizmos.DrawLine(_curvePoint2.position,_curvePoint3.position);
            Gizmos.color = _colorLine;
            float tAdd = 1.0f / _density;
            Vector3 previousPoint = pos0;
            for (float i = tAdd; i < 1 ; i += tAdd)
            {
                Vector3 point = Vector3.Lerp(pos0, pos1, i);
                point = Vector3.Lerp(point, pos2, i);
                point = Vector3.Lerp(point, pos3, i);
                Gizmos.DrawLine(previousPoint,point);
                previousPoint = point;
            }
            Gizmos.DrawLine(previousPoint,pos3);
            _smokeMat.SetVector("_curve0",pos0);
            _smokeMat.SetVector("_curve1",pos1);
            _smokeMat.SetVector("_curve2",pos2);
            _smokeMat.SetVector("_curve3",pos3);
            
            
        }
        catch
        {
            //ignored
        }
        
    }
}
