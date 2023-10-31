using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chispas : MonoBehaviour
{
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
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Enemy"))
        {
            this.transform.GetChild(0).gameObject.SetActive(false);

        }
    }
}
