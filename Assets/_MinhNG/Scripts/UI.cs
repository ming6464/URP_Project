using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{ 
    [Serializable]
    public struct PhaseInfo
    {
        public Vector2 rangeTime;
        public InfoEff[] infoEffs;
    }
    [Serializable]
    public struct InfoEff
    {
        public int indexTf;
        public bool usePosition;
        public Vector3 position;
        public bool useScale;
        public Vector3 scale;
        public bool useAlpha;
        public Vector2 alphaRange;
    }
    [Serializable]
    public struct Eff
    {
        public Transform objectTf;
        public EditAttrParticle attrParticle;
        [HideInInspector]
        public Vector3 passPosition;
        [HideInInspector]
        public Vector3 passScale;
    }
    
    public bool isRun;

    [SerializeField] private Slider _slider;
    [SerializeField] private float _speedSlider;
    // ---
    [Header("Phase Info")] [SerializeField]
    private PhaseInfo[] _phaseInfos;
    [SerializeField] private Eff[] _effs;

    private float _slideValue;

    private void Update()
    {
        if (isRun)
        {
            isRun = false;
            _slideValue = Time.deltaTime * _speedSlider;
        }

        if (_slideValue > 0)
        {
            _slider.value = _slideValue;
            _slideValue += Time.deltaTime * _speedSlider;
        }
    }

    public void SlideValueChange(float value)
    {
        foreach (PhaseInfo phaseInfo in _phaseInfos)
        {
            CheckAndRunPhase(phaseInfo, value);
        }
    }
    private bool CheckAndRunPhase(PhaseInfo phase,float value)
    {
        if(value < phase.rangeTime.x || value > phase.rangeTime.y) return false;

        float percentRange = math.remap(0, 1, phase.rangeTime.x, phase.rangeTime.y, value);
        foreach (InfoEff infoEff in phase.infoEffs)
        {
            Eff eff = GetTf(infoEff.indexTf);
            Transform objectTf = eff.objectTf;
            if(!objectTf) continue;

            if (percentRange >= 0.9f || percentRange <= 0.1f)
            {
                _effs[infoEff.indexTf].passPosition = objectTf.localPosition;
                _effs[infoEff.indexTf].passScale = objectTf.localScale;
            }
            
            if (infoEff.usePosition)
            {
                objectTf.localPosition = Vector3.Lerp(eff.passPosition, infoEff.position,percentRange);
            }

            if (infoEff.useScale)
            {
                objectTf.localScale = Vector3.Lerp(eff.passScale, infoEff.scale,percentRange);
            }

            if (infoEff.useAlpha)
            {
                float alphaValue = Mathf.Lerp(infoEff.alphaRange.x, infoEff.alphaRange.y, percentRange);
                eff.attrParticle.EditAlpha(alphaValue);
            }
            
        }

        return true;
    }

    private Eff GetTf(int index)
    {
        if (_effs.Length - 1 >= index)
        {
            return _effs[index];
        }

        return new Eff();
    }
}
