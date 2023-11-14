using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpVFXController : MonoBehaviour
{
    public ParticleSystem doubleJumpVFX;
    // Start is called before the first frame update
    void Start()
    {
        doubleJumpVFX.Stop(); // Make sure the effect starts paused
    }

    public void PlayDoubleJumpVFX(Vector3 transform)
    {
        this.transform.position = transform;
        doubleJumpVFX.Play();
    }
}
