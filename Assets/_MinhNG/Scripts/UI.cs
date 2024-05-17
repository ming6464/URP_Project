using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private EditTransformSliderUI[] _transformSliderUis;
    [SerializeField] private Slider _slider;
    private void Update()
    {
        foreach (EditTransformSliderUI edit in _transformSliderUis)
        {
            edit.UpdateSlideTime(_slider.value);
        }
    }

    public void UpdateTimeSlider(float time)
    {
        
    }
    
}
