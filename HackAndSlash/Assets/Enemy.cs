using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.Feedbacks;
public class Enemy : MonoBehaviour
{
    Rigidbody rigidbody;
    public float JumpForce;
    public float gravity;
    float fallStartTime;
    public bool GravityOn;
    public float delayAire;
    Animator anim;
    public float VelFlotando;
    float delayCaer;
    public bool cayendo;
    bool golpe;
    public float fallDelay;
    public float ImpulsoGolpeAire;

    public AnimationCurve fallSpeed;

    bool dañoOn;

    public GameObject[] hitWhiteEffects;

    public GameObject hitEffect2;

    public MMFeedbacks enemyHitFeedback;
    //public GameObject hitEffect1;
    // Start is called before the first frame update
    void Start()
    {
        dañoOn = true;
        golpe = false;
        cayendo = false;
        anim = this.GetComponent<Animator>();
        rigidbody = this.GetComponent<Rigidbody>();
        GravityOn = false;
        anim.CrossFadeInFixedTime("Idle", 0.2f);
        fallStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(GravityOn)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out hit, 200, 1 << 7))
            {

                if (hit.distance > 0.25f)
                {
                    ApplyGravity();

                }
                if (hit.distance < 0.65f)
                {
                    GravityOn = false;

                }
                if (cayendo && hit.distance < 1)
                {
                    cayendo = false;
                    anim.CrossFadeInFixedTime("GolpeSuelo", 0.2f);

                    dañoOn = false;

                    Invoke("Levantarse", 0.5f);
                }
            }


        }
        else if(golpe)
        {
            this.GetComponent<Rigidbody>().AddForce(( transform.up * Time.deltaTime* VelFlotando) * fallSpeed.Evaluate(Time.deltaTime-delayCaer), ForceMode.Force);

                if((Time.time-delayCaer) > fallDelay)
                {
                Debug.Log(delayCaer);
                    GravityOn = true;
                    anim.CrossFadeInFixedTime("Caer", 0.2f);
                    cayendo = true;
                    golpe = false;
                }


        }
    }

    void Levantarse()
    {
        anim.CrossFadeInFixedTime("Levantarse", 0.2f);

        Invoke("Levantado", 0.4f);

    }
    void Levantado()
    {
        anim.CrossFadeInFixedTime("Idle", 0.2f);
        dañoOn = true;

        this.GetComponent<CapsuleCollider>().enabled = true;

    }
    void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, this.gravity * (Time.time - fallStartTime), 0);
        this.GetComponent<Rigidbody>().AddForce(gravity * Time.deltaTime, ForceMode.Force);

    }
    void DelayAire()
    {
        anim.CrossFadeInFixedTime("Flotando", 0.2f);
        GravityOn = false;
        delayCaer = Time.time;
        golpe = true;

    }
    void delayFuerza()
    {
        rigidbody.AddForce(this.transform.up * JumpForce * Time.fixedDeltaTime, ForceMode.Impulse);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(dañoOn)
        {
            if(other.CompareTag("GolpeVertical"))
            {
                //GameObject.FindObjectOfType<ControllerManager>().StartVibration(0.5f, 0.5f, 0.2f);

                anim.CrossFadeInFixedTime("GolpeSalto", 0.2f);
                fallStartTime = Time.time;
                GravityOn = true; 


                Invoke("delayFuerza", 0.1f);

                Invoke("DelayAire", delayAire);
            }
            else if (other.CompareTag("GolpeAire") && (golpe||GravityOn))
            {
                delayCaer = Time.time;
                Debug.Log(delayCaer);

                //GameObject.FindObjectOfType<ControllerManager>().StartVibration(0.2f,0.2f,0.2f);
                rigidbody.AddForce(this.transform.up * ImpulsoGolpeAire * Time.deltaTime, ForceMode.Impulse);
                anim.CrossFadeInFixedTime("AirDamage", 0.2f);

                enemyHitFeedback?.PlayFeedbacks();

                Vector3 collisionPosition = (GameObject.FindGameObjectWithTag("PlayerCenter").transform.position - (this.transform.position + new Vector3(0f, 2f, 0f))).normalized;
                int spawnIndex = Random.Range(0, hitWhiteEffects.Length);
                GameObject hitToLook;
                hitToLook = Instantiate(hitWhiteEffects[spawnIndex], (this.transform.position + new Vector3(0f, 2f, 0f)) + (collisionPosition * 1f), Quaternion.identity);
                hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);



                hitToLook = Instantiate(hitEffect2, (this.transform.position + new Vector3(0f, 2f, 0f)) + (collisionPosition * 1f), Quaternion.identity);
                hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);




                fallStartTime = Time.time;
                GravityOn = false;

                golpe = true;

            }
            else if (other.CompareTag("GolpeLigero"))
            {
                anim.CrossFadeInFixedTime("Hit", 0.2f);
                Vector3 collisionPosition = (GameObject.FindGameObjectWithTag("PlayerCenter").transform.position - (this.transform.position + new Vector3(0f, 2f, 0f))).normalized;
                int spawnIndex = Random.Range(0, hitWhiteEffects.Length);
                GameObject hitToLook;
                hitToLook = Instantiate(hitWhiteEffects[spawnIndex], (this.transform.position + new Vector3(0f, 2f, 0f)) + (collisionPosition * 1f), Quaternion.identity);
                hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);



                hitToLook = Instantiate(hitEffect2, (this.transform.position + new Vector3(0f, 2f, 0f)) + (collisionPosition * 1f), Quaternion.identity);
                hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
            }
        }
     
    }
}
