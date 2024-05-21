using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Run : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem particle;
    public string nameAnim;
    public bool Play;
    private float _t;

    // Update is called once per frame
    void Update()
    {
        if (Play)
        {
            particle.Stop();
            particle.Play();
            animator.Play(nameAnim);
            Play = false;
        }
    }
    
}
