using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AnimationEvents : MonoBehaviour
{
    public Transform thunder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void IncreaseThunder()
    {
        thunder.localPosition *= 4;

    }
    public void DecreaseThunder()
    {
        thunder.localPosition /= 4;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
