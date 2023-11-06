using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SingleJumpVFXController : MonoBehaviour
{
    public VisualEffect singleJumpVFX;
    // Start is called before the first frame update
    void Start()
    {
        singleJumpVFX.Stop(); // Make sure the effect starts paused
    }

    public void PlaySingleJumpVFX(Vector3 transform)
    {
        this.transform.position = transform;
        singleJumpVFX.Play();
    }
}
