using System;
using Unity.Mathematics;
using UnityEngine;

public class EditTransformSliderUI : MonoBehaviour
{
    [Serializable]
    public struct  PhaseSetup
    {
        public Vector2 slideTime;
        public bool useScale;
        public Vector3 scale;
        public bool usePosition;
        public Vector3 position;
        public bool useAlpha;
        public Vector2 alphaRange;
    }

    [SerializeField] private PhaseSetup[] _phaseSetups;
    [SerializeField] private EditAttrParticle _editAttr;
    private int _currentPhase = -1;
    private Vector3 _passScale;
    private Vector3 _passPosition;
    
    public void UpdateSlideTime(float time)
    {
        if(_currentPhase >= _phaseSetups.Length) return;
        for (int i = _currentPhase; i < _phaseSetups.Length; i++)
        {
            if(i < 0) continue;
            PhaseSetup phaseSetup = _phaseSetups[i];
            if (!CheckRangeTime(phaseSetup.slideTime, time)) continue;
            if (_currentPhase != i)
            {
                _passPosition = transform.localPosition;
                _passScale = transform.localScale;
                _currentPhase = i;
            }

            float percentTime = math.remap(0, 1, phaseSetup.slideTime.x, phaseSetup.slideTime.y, time);

            if (phaseSetup.usePosition)
            {
                transform.localPosition = Vector3.Lerp(_passPosition, phaseSetup.position, percentTime);
            }
            
            if (phaseSetup.useScale)
            {
                transform.localScale = Vector3.Lerp(_passScale, phaseSetup.scale, percentTime);
            }
            
            if (phaseSetup.useAlpha)
            {
                _editAttr.EditAlpha(Mathf.Lerp(phaseSetup.alphaRange.x, phaseSetup.alphaRange.y, percentTime));
            }
            
        }
    }

    private bool CheckRangeTime(Vector2 rangeTime, float value)
    {
        return rangeTime.x <= value && rangeTime.y >= value;
    }
}
