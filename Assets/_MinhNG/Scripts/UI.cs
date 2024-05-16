using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Transform particle1;
    public Vector3 scale1;
    public Vector2 output1;
    
    public Transform particle2;
    public Vector3 scale2;
    public Vector2 output2;
    
    public Transform particle3;
    public Vector3 scale3;
    public Vector2 output3;

    private Vector2 input = new Vector2(0, 1);

    public Slider slider;

    private void Update()
    {
        SliderUpdate(slider.value);
    }

    private void SliderUpdate(float value)
    {
        Run(output1, value, particle1, scale1);
        Run(output2, value, particle3, scale2);
        Run(output3, value, particle2, scale3);
    }

    private void Run(Vector2 output, float value,Transform tf,Vector3 scale)
    {
        float getvalue = Remap(input, output, value);
        if (getvalue < output.x || getvalue > output.y)
        {
            getvalue = 0;
        }

        tf.localScale = scale * getvalue;
    }

    private float Remap(Vector2 input, Vector2 outPut, float value)
    {
        float ratio = (value - input.x) / (input.y - input.x);
        float a = ratio * (outPut.y - outPut.x) + outPut.x;
        return a;
    }
    
}
