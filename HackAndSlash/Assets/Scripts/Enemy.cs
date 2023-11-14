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
    public GameObject[] bloodEffects;
    public GameObject hitEffect2;
    float floor;
    Vector3 collisionPoint;
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
            RaycastHit hit;

        if(!golpe)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out hit, 200, 1 << 7))
            {
                if (hit.distance > 0.25f)
                {
                    GravityOn = true;

                }

            }
        }
        if(this.transform.position.y < floor)
        {
            this.transform.position = new Vector3(this.transform.position.x, floor, this.transform.position.z);

        }

        if (GravityOn)
        {

            if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out hit, 200, 1 << 7))
            {

                if (hit.distance > 0.25f)
                {
                    ApplyGravity();

                }
                else
                {
                    this.transform.position = new Vector3(this.transform.position.x, hit.point.y, this.transform.position.z);
                    GravityOn = false;

                }
                if ((hit.distance > -1)&& (hit.distance < 1) && (this.GetComponent<Rigidbody>().velocity.y < 0))
                {
                    this.transform.position = new Vector3(this.transform.position.x, hit.point.y, this.transform.position.z);

                }
                if (hit.distance < 0.85f && !(this.GetComponent<Rigidbody>().velocity.y > 0))
                {
                    floor = hit.point.y;

                    this.transform.position = new Vector3(this.transform.position.x, floor, this.transform.position.z);

                    GravityOn = false;
                }
                if (cayendo && hit.distance < 1 && !(this.GetComponent<Rigidbody>().velocity.y > 0))
                {
                    cayendo = false;
                    anim.CrossFadeInFixedTime("GolpeSuelo", 0.2f);
                    this.transform.position = new Vector3(this.transform.position.x, floor, this.transform.position.z);

                    dañoOn = false;

                    Invoke("Levantarse", 0.5f);
                }
            }


        }

        if(golpe)
        {
            rigidbody.AddForce(( transform.up * Time.deltaTime* VelFlotando) * fallSpeed.Evaluate(Time.deltaTime-delayCaer), ForceMode.Force);

                if((Time.time-delayCaer) > fallDelay)
                {

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
        this.transform.position = new Vector3(this.transform.position.x, floor, this.transform.position.z);

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
        rigidbody.AddForce(gravity * Time.deltaTime, ForceMode.Force);

    }
    void DelayAire()
    {
        anim.CrossFadeInFixedTime("Flotando", 0.2f);
        GravityOn = false;
        delayCaer = Time.time;
        golpe = true;

    }

    void StandUp()
    {
        anim.CrossFadeInFixedTime("StandUp", 0.2f);
        Invoke("ReturnIdle", 0.6f);

    }
    void ReturnIdle()
    {
        anim.CrossFadeInFixedTime("Idle", 0.2f);

    }
    private void OnTriggerEnter(Collider other)
    {
        if(dañoOn)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (other.CompareTag("GolpeVertical"))
            {
                //GameObject.FindObjectOfType<ControllerManager>().StartVibration(0.5f, 0.5f, 0.2f);

                anim.CrossFadeInFixedTime("GolpeSalto", 0.2f);
                fallStartTime = Time.time;
                GravityOn = true;
                golpe = false;
                other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();

                collisionPoint = FindClosestPointOnCollider(other, transform.position);
                SpawnWhiteSplash(collisionPoint);
                SpawnBloodSplash(collisionPoint);
                SpawnHitLine(collisionPoint);
                ComboManager.instance.IncreaseCombo();

                rigidbody.velocity = Vector3.zero;

                rigidbody.AddForce(this.transform.up * JumpForce, ForceMode.Impulse);

                Invoke("DelayAire", delayAire);
            }
            else if (other.CompareTag("GolpeAire") && (golpe || GravityOn))
            {
                delayCaer = Time.time;

                //GameObject.FindObjectOfType<ControllerManager>().StartVibration(0.2f,0.2f,0.2f);
                rigidbody.AddForce(this.transform.up * ImpulsoGolpeAire * Time.deltaTime, ForceMode.Impulse);
                anim.CrossFadeInFixedTime("AirDamage", 0.2f);

                other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();

                collisionPoint = FindClosestPointOnCollider(other, transform.position);
                SpawnWhiteSplash(collisionPoint);
                SpawnBloodSplash(collisionPoint);
                SpawnHitLine(collisionPoint);
                ComboManager.instance.IncreaseCombo();

                fallStartTime = Time.time;
                GravityOn = false;

                golpe = true;

            }
            else if (other.CompareTag("GolpeVerticalCircular"))
            {
                rigidbody.AddForce(this.transform.up * other.GetComponent<AttackCollider>().KnockbackY * Time.fixedDeltaTime, ForceMode.Impulse);
                anim.CrossFadeInFixedTime("GolpeSalto", 0.2f);
                fallStartTime = Time.time;
                GravityOn = true;
                golpe = false;
                other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();


                collisionPoint = FindClosestPointOnCollider(other, transform.position);
                SpawnWhiteSplash(collisionPoint);
                SpawnBloodSplash(collisionPoint);
                SpawnHitLine(collisionPoint);
                ComboManager.instance.IncreaseCombo();

                Invoke("DelayAire", delayAire);
            }
            else if (other.GetComponent<AttackCollider>() != null)
            {
                if (other.GetComponent<AttackCollider>().enemyStandUp)
                {
                    Invoke("StandUp", 1f);
                }
                else
                {
                    Invoke("ReturnIdle", 0.4f);

                }


                anim.CrossFadeInFixedTime(other.GetComponent<AttackCollider>().enemyHitAnim, 0.2f);

                rigidbody.AddForce(this.transform.up * other.GetComponent<AttackCollider>().KnockbackY * Time.deltaTime, ForceMode.Impulse);

                other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();


                Vector3 player = new Vector3(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position.x, 0, GameObject.FindGameObjectWithTag("PlayerCenter").transform.position.z);
                Vector3 enemy = new Vector3(this.transform.position.x, 0, this.transform.position.z);

                Vector3 ForceDirection = (player - (enemy)).normalized;
                rigidbody.AddForce(-ForceDirection * other.GetComponent<AttackCollider>().Knockback * Time.fixedDeltaTime, ForceMode.Impulse);

               // Vector3 collisionPosition = (GameObject.FindGameObjectWithTag("PlayerCenter").transform.position - (this.transform.position + new Vector3(0f, 2f, 0f))).normalized;
                collisionPoint = FindClosestPointOnCollider(other, transform.position);
                SpawnWhiteSplash(collisionPoint);
                SpawnBloodSplash(collisionPoint);
                SpawnHitLine(collisionPoint);
                ComboManager.instance.IncreaseCombo();
            }

        }

        void SpawnWhiteSplash(Vector3 spawnPoint)
        {
            GameObject hitToLook = Instantiate(hitWhiteEffects[Random.Range(0, hitWhiteEffects.Length)], spawnPoint + new Vector3(0f, 1f, 0f), Quaternion.identity);
            hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
        }

        void SpawnBloodSplash(Vector3 spawnPoint)
        {
            GameObject hitToLook = Instantiate(bloodEffects[Random.Range(0, bloodEffects.Length)], spawnPoint + new Vector3(0f, 1f, 0f), Quaternion.identity);
            hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
        }

        void SpawnHitLine(Vector3 spawnPoint)
        {
            GameObject hitToLook = Instantiate(hitEffect2, spawnPoint + new Vector3(Random.Range(0,0.15f), Random.Range(0.85f,1.15f), 0f), Quaternion.identity);
            hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
        }




        Vector3 FindClosestPointOnCollider(Collider collider, Vector3 point)
        {
            return collider.ClosestPointOnBounds(point);
        }
    }
}
