using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chispas : MonoBehaviour
{
    public ParticleSystem[] effects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Enemy"))
        {
            foreach(ParticleSystem effect in effects)
            {
                if(!effect.isEmitting)
                effect.Play();
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Enemy"))
        {
            foreach (ParticleSystem effect in effects)
            {
                if (effect.isEmitting)
                    effect.Stop();
            }
        }
    }
}
