using System;
using Unity.Mathematics;
using UnityEngine;

public class EditRectTransformSliderUI : MonoBehaviour
{
    [Serializable]
    public struct  PhaseSetup
    {
        public Vector2 slideTime;
        public bool useAlpha;
        public float alpha;
        public bool useAnchoredPosition;
        public Vector2 anchoredPosition;
        public bool useSizeDelta;
        public Vector2 sizeDelta;
    }

    [SerializeField] private float _alphaDefault;
    [SerializeField] private bool _onlyApplyAlphaDefault;
    [SerializeField] private Vector2 _anchoredPositionDefault;
    [SerializeField] private Vector2 _sizeDeltaDefault;
    [SerializeField] private PhaseSetup[] _phaseSetups;
    [SerializeField] private EditAttrParticle _editAttr;
    private int _currentPhase = -1;
    private Vector2 _passSizeDelta;
    private Vector2 _passAnchoredPosition;
    private float _passAlpha;
    private RectTransform _myRectTf;
    
    
    private void Awake()
    {
        _myRectTf = GetComponent<RectTransform>();
    }

    private void Start()
    {
        LoadDefault();
    }

    public void UpdateSlideTime(float time)
    {
        if(_currentPhase >= _phaseSetups.Length) return;
        if (time <= 0.01f)
        {
            LoadDefault();
        }
        for (int i = 0; i < _phaseSetups.Length; i++)
        {
            PhaseSetup phaseSetup = _phaseSetups[i];
            if (!CheckRangeTime(phaseSetup.slideTime, time)) continue;
            if (_currentPhase != i)
            {
                _passAlpha = _editAttr.Alpha;
                _passAnchoredPosition = _myRectTf.anchoredPosition;
                _passSizeDelta = _myRectTf.sizeDelta;
                _currentPhase = i;
            }

            float percentTime = math.remap(phaseSetup.slideTime.x, phaseSetup.slideTime.y,0, 1, time);

            if (phaseSetup.useAnchoredPosition)
            {
                _myRectTf.anchoredPosition = Vector2.Lerp(_passAnchoredPosition, phaseSetup.anchoredPosition, percentTime);
            }
            
            if (phaseSetup.useSizeDelta)
            {
                _myRectTf.sizeDelta = Vector2.Lerp(_passSizeDelta, phaseSetup.sizeDelta, percentTime);
            }
            
            if (phaseSetup.useAlpha)
            {
                _editAttr.EditAlpha(Mathf.Lerp(_passAlpha, phaseSetup.alpha, percentTime));
            }
        }
    }

    

    private bool CheckRangeTime(Vector2 rangeTime, float value)
    {
        return rangeTime.x <= value && rangeTime.y >= value;
    }
    
    private void LoadDefault()
    {
        
        _editAttr.EditAlpha(_alphaDefault);
        _passAlpha = _alphaDefault;
        if(_onlyApplyAlphaDefault) return;
        _myRectTf.anchoredPosition = _anchoredPositionDefault;
        _myRectTf.sizeDelta = _sizeDeltaDefault;
        _passAnchoredPosition = _anchoredPositionDefault;
        _passSizeDelta = _sizeDeltaDefault;
    }
}
