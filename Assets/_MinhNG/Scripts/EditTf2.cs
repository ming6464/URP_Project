using System;
using Unity.Mathematics;
using UnityEngine;

public class EditTf2 : MonoBehaviour
{
    [Serializable]
    public struct  PhaseSetupParticle
    {
        public Vector2 slideTime;
        public bool useRotation;
        public Vector3 rotation;
    }
    
    [SerializeField] private PhaseSetupParticle[] _phaseSetups;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private Vector3 _rotationDefault;
    //
    private int _currentPhase;
    private Vector3 _passRotation;
    private Transform _myTf;
    private bool _isActive;
    
    private void Start()
    {
        _myTf = transform;
        LoadDefault();
    }
    
    public void UpdateSlideTime(float time)
    {
        if(_currentPhase >= _phaseSetups.Length) return;
        if (time <= 0.01f)
        {
            LoadDefault();
        }

        if (time > 8 && time < 8.5f)
        {
            _particle.Play();
            _isActive = true;
        }else if (time > 34.5f && time <= 36)
        {
            _particle.Stop();
            _isActive = false;
        }
        
        if(!_isActive) return;
        
        for (int i = 0; i < _phaseSetups.Length; i++)
        {
            PhaseSetupParticle phaseSetup = _phaseSetups[i];
            if (!CheckRangeTime(phaseSetup.slideTime, time)) continue;
            if (_currentPhase != i)
            {
                _passRotation = _myTf.localRotation.eulerAngles;
                _currentPhase = i;
            }

            float percentTime = math.remap(phaseSetup.slideTime.x, phaseSetup.slideTime.y,0, 1, time);

            if (phaseSetup.useRotation)
            {
                _myTf.localRotation = Quaternion.Lerp(Quaternion.Euler(_passRotation),
                    Quaternion.Euler(phaseSetup.rotation), percentTime);
            }
            return;
        }
    }

    

    private bool CheckRangeTime(Vector2 rangeTime, float value)
    {
        return rangeTime.x <= value && rangeTime.y >= value;
    }

    private void LoadDefault()
    {
        _isActive = false;
        _passRotation = _rotationDefault;
        _myTf.localRotation = Quaternion.Euler(_rotationDefault);
        _particle.Stop();
    }
}