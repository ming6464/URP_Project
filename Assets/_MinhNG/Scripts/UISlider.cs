using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    [SerializeField] private EditRectTransformSliderUI[] _transformSliderUis;
    [SerializeField] private EditTf2 _editTf2Particle;
    [SerializeField] private Slider _slider;
    [SerializeField] private float _speed;
    [SerializeField] private bool _isRun;
    
    
    private void Update()
    {
        if (_isRun)
        {
            _slider.value += _speed * Time.deltaTime;
        }
        else
        {
            _slider.value = 0;
        }
        
    }
    
    public void OnChangeSliderValue(float value)
    {
        foreach (EditRectTransformSliderUI edit in _transformSliderUis)
        {
            edit.UpdateSlideTime(value);
        }

        if (_editTf2Particle)
        {
            _editTf2Particle.UpdateSlideTime(value);
        }
    }
    
}
