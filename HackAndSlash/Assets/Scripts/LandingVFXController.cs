using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LandingVFXController : MonoBehaviour
{
    public VisualEffect landingDustVFX; // Reference to the landing dust VFX
    // Start is called before the first frame update
    void Start()
    {
        landingDustVFX.Stop(); // Make sure the effect starts paused
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayDustVFX(Vector3 transform)
    {
        this.transform.position = transform;
        landingDustVFX.Play();
    }
}
