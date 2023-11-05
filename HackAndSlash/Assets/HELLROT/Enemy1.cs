using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : MonoBehaviour
{
    enum States { MOVE, ATTACK, FLY, IDLE,DELAY, JUMP, HIT};
    enum Move { WALK, RUN};
    enum Fly {START,UP,AIR,DOWN,ATTACK};
    enum Jump { JUMP,AIR,LAND,DELAY};
    enum Hits { UP, LAND, DOWN, HIT1,AIR };
    enum Attack { ATTACK1,ATTACK2};
    Fly fly;
    Attack ataques;
    Hits hit;
    Jump jump;
    Move move;
    States state;
    NavMeshAgent agent;
    Animator anim;
    GameObject player;
    bool delay;
    public GameObject[] hitWhiteEffects;
    public GameObject[] bloodEffects;
    public GameObject hitEffect2; 
    Vector3 collisionPoint;
    Rigidbody rigidbody;
    public float gravity;
    public float JumpForce;
    public float delayCaer; 
    public float delayJumpHit;
    float floor;
    float fallStartTime;
    public AnimationCurve curve;
    public AnimationCurve flyCurve;
    public float speedFlotando;
    public float ImpulsoGolpeAire;
    int hitCount;
    bool GetDamage;

    float flyTimer;
    float attackDelay;
    float actionsDelay;
    float attackTimer;

    public AnimationCurve attack1;
    public AnimationCurve attack2;

    // Start is called before the first frame update
    void Start()
    {
        
           actionsDelay = Time.time;
        attackDelay = 2;
           GetDamage = true;
           rigidbody = this.GetComponent<Rigidbody>();
           delay = false;
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        anim = this.GetComponent<Animator>();
        ChangeToMove();
        agent = this.GetComponent<NavMeshAgent>(); 
    }
    void StartWalk()
    {
        if (state == States.HIT)
            return;
        if(Vector3.Distance(this.transform.position, player.transform.position) > agent.stoppingDistance)
        {
            move = Move.WALK;
            state = States.MOVE;
            anim.CrossFadeInFixedTime("walk", 0.2f);
            agent.speed = 2;
        }
        else
        {
            ChangeToIdle();
        }
    }
    void StartRun()
    {
        if (state == States.HIT)
            return;
        if (Vector3.Distance(this.transform.position, player.transform.position) > agent.stoppingDistance)
        {
            move = Move.RUN;
            state = States.MOVE;
            anim.CrossFadeInFixedTime("run", 0.2f);
            agent.speed = 4;

        }
        else
        {
            ChangeToIdle();
        }
    }
    void ChangeToMove()
    {
        int random = (int)Random.RandomRange(0, 5);

        if(random<3)
        {
            Invoke("StartWalk", 0.3f);
            
        }
        else
        {
            anim.CrossFadeInFixedTime("roar", 0.2f);

            Invoke("StartRun", 1f);
        }


    }

    void ChangeToIdle()
    {
        attackDelay = 0.25f;

        attackTimer = Time.time;
           GetDamage = true;

        agent.enabled = true;

        state = States.IDLE;
        anim.CrossFadeInFixedTime("idle", 0.2f);
    }

    void ChangeToFly()
    {
        agent.enabled = false;
        flyTimer = Time.time;
        state = States.FLY;
        fly = Fly.START; 
        anim.CrossFadeInFixedTime("jump", 0.2f);

    }
    // Update is called once per frame
    void Update()
    {
        if(state != States.MOVE)
            agent.speed = 0;

        switch (state)
        {
            case States.MOVE:
                switch(move)
                {
                    case Move.WALK:

                        break;
                    case Move.RUN:

                        break;
                }
                
                if(agent.enabled == true)
                {
                    agent.destination = player.transform.position;
                    if(agent.remainingDistance <= agent.stoppingDistance)
                    {
                        ChangeToIdle();
                    }
                    if(agent.isOnOffMeshLink)
                    {
                        anim.CrossFadeInFixedTime("jump", 0.2f);
                        jump = Jump.JUMP;
                        state = States.JUMP;
                    }
                }
                if((Time.time - actionsDelay) > 5)
                {
                    actionsDelay = Time.time;
                    int fly = (int)Random.RandomRange(0, 10);

                    if(fly < 2)
                    {
                        ChangeToFly();
                    }
                }
                break;
            case States.IDLE:

                var targetRotation = Quaternion.LookRotation(player.transform.position - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime);


                if (Vector3.Distance(this.transform.position,player.transform.position) > agent.stoppingDistance)
                {
                    ChangeToMove();
                    state = States.DELAY;
                }
                else
                {
                    if ((Time.time - attackTimer) > attackDelay)
                    {
                        int attack = (int)Random.RandomRange(0, 10);

                        attackTimer = Time.time;
                        agent.enabled = false;
                        if (attack < 4)
                        {
                            state = States.ATTACK;
                            ataques = Attack.ATTACK1;
                            anim.CrossFadeInFixedTime("attack1", 0.2f);

                        }
                        else
                        {
                            state = States.ATTACK;

                            ataques = Attack.ATTACK2;
                            anim.CrossFadeInFixedTime("attack2", 0.2f);



                        }
                    }
                }

                break;
            case States.FLY:

                Vector3 look = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
                var targetRotation2 = Quaternion.LookRotation(look - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation2, 3 * Time.deltaTime);
                
                switch (fly)
                {
                    case Fly.START:
                        if((Time.time-flyTimer)>0.15f)
                        {
                            flyTimer = Time.time;
                            anim.CrossFadeInFixedTime("fly", 0.2f);
                            fly = Fly.UP;
                        }
                        break;
                    case Fly.UP:
                        rigidbody.AddForce(this.transform.up * 1750 * flyCurve.Evaluate((Time.time - flyTimer))*Time.deltaTime, ForceMode.Force);

                        if ((Time.time - flyTimer) > 1.5f)
                        {
                            flyTimer = Time.time;
                            fly = Fly.AIR;
                        }
                        break;               
                    case Fly.AIR:

                        if ((Time.time - flyTimer) > attackDelay)
                        {
                            attackDelay = Random.RandomRange(2, 5);
                            int attack = (int)Random.RandomRange(0, 10);

                            flyTimer = Time.time;

                            if(attack < 4)
                            {
                                fly = Fly.DOWN;
                            }
                            else
                            {
                                fly = Fly.ATTACK;
                                anim.CrossFadeInFixedTime("flyAttack", 0.2f);

                            }
                        }
                        break;                      
                    case Fly.DOWN:
                        rigidbody.AddForce(-this.transform.up * 1750 * Time.deltaTime, ForceMode.Force);

                        RaycastHit rayhit;

                        if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out rayhit, 200, 1 << 7))
                        {

                            if (rayhit.distance < 0.25f)
                            {
                                floor = rayhit.point.y;
                                anim.CrossFadeInFixedTime("land", 0.2f);
                                this.transform.position = new Vector3(this.transform.position.x, rayhit.point.y, this.transform.position.z);

                                Invoke("ChangeToIdle", 0.3f);

                            }
                        }

                        break;
                    case Fly.ATTACK:
                        if ((Time.time - flyTimer) > 1f)
                        {
                            flyTimer = Time.time;
                            fly = Fly.AIR;
                            anim.CrossFadeInFixedTime("fly", 0.2f);

                        }
                        break;
                }
                break;
            case States.ATTACK:
                //Vector3 look2 = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
                //var targetRotation3 = Quaternion.LookRotation(look2 - this.transform.position);
                //this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation3, 3 * Time.deltaTime);

                switch(ataques)
                {
                    case Attack.ATTACK1:
                        rigidbody.AddForce(this.transform.forward * 500 * attack1.Evaluate((Time.time - attackTimer) )* Time.deltaTime, ForceMode.Force);
                        if((Time.time- attackTimer) > 0.75f)
                        {
                            ChangeToIdle();
                            attackDelay = Random.RandomRange(2, 5);

                        }

                        break;
                    case Attack.ATTACK2:
                        rigidbody.AddForce(this.transform.forward * 500 * attack2.Evaluate((Time.time - attackTimer) )* Time.deltaTime, ForceMode.Force);
                        if ((Time.time - attackTimer) > 2f)
                        {
                            ChangeToIdle();
                            attackDelay = Random.RandomRange(2, 5);

                        }
                        break;
                }

                break;
            case States.DELAY:

                break;
            case States.JUMP:

                switch(jump)
                {
                    case Jump.JUMP:
                            if (!agent.isOnOffMeshLink)
                            {
              
                                anim.CrossFadeInFixedTime("land", 0.2f);
                                Invoke("ChangeToIdle", 0.3f);
                                jump = Jump.LAND;
  

                            }
                        break;
                    case Jump.AIR:

                        break;
                    case Jump.LAND:

                        break;
                    case Jump.DELAY:

                        break;
                }

                break;
            case States.HIT:

                switch(hit)
                {
                    case Hits.UP:
                        ApplyGravity();
                        break;
                    case Hits.DOWN:
                        ApplyGravity();
                        RaycastHit rayhit;

                        if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out rayhit, 200, 1 << 7))
                        {

                            if (rayhit.distance < 0.5f)
                            {
                                floor = rayhit.point.y;
                                anim.CrossFadeInFixedTime("GolpeSuelo", 0.2f);
                                this.transform.position = new Vector3(this.transform.position.x, rayhit.point.y, this.transform.position.z);
                                hit = Hits.LAND;
                                Invoke("Levantarse", 0.5f);
                                GetDamage = false;

                            }
                        }
                            break;
                    case Hits.LAND:

                        break;
                    case Hits.HIT1:

                        break;
                    case Hits.AIR:
                        rigidbody.AddForce(this.transform.up * speedFlotando* curve.Evaluate((Time.time - delayCaer)), ForceMode.Force);
                        if ((Time.time-delayCaer) > 0.25f)
                        {
                            hit = Hits.DOWN;
                            anim.CrossFadeInFixedTime("fall", 0.2f);
                            fallStartTime = Time.time;

                        }
                        break;
                }
                break;
        }
    }
    void Levantarse()
    {
        anim.CrossFadeInFixedTime("Levantarse", 0.2f);
        this.transform.position = new Vector3(this.transform.position.x, floor, this.transform.position.z);

        Invoke("ChangeToIdle", 0.6f);

    }

    void SpawnWhiteSplash(Vector3 spawnPoint)
    {
        GameObject hitToLook = Instantiate(hitWhiteEffects[Random.Range(0, hitWhiteEffects.Length-1)], spawnPoint + new Vector3(0f, 1f, 0f), Quaternion.identity);
        hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
    }

    void SpawnBloodSplash(Vector3 spawnPoint)
    {
        GameObject hitToLook = Instantiate(bloodEffects[Random.Range(0, bloodEffects.Length)], spawnPoint + new Vector3(0f, 1f, 0f), Quaternion.identity);
        hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
    }

    void SpawnHitLine(Vector3 spawnPoint)
    {
        GameObject hitToLook = Instantiate(hitEffect2, spawnPoint + new Vector3(Random.Range(0, 0.15f), Random.Range(0.85f, 1.15f), 0f), Quaternion.identity);
        hitToLook.transform.LookAt(GameObject.FindGameObjectWithTag("PlayerCenter").transform.position);
    }
    Vector3 FindClosestPointOnCollider(Collider collider, Vector3 point)
    {
        return collider.ClosestPointOnBounds(point);
    }
    void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, this.gravity * (Time.time - fallStartTime), 0);
        rigidbody.AddForce(gravity * Time.fixedDeltaTime, ForceMode.Force);

    }
    void DelayAire()
    {
        anim.CrossFadeInFixedTime("Flotando", 0.2f);
        delayCaer = Time.time;
        hit = Hits.AIR;
    }
    void StandUp()
    {
        anim.CrossFadeInFixedTime("StandUp", 0.2f);
        Invoke("ChangeToIdleHit", 0.4f);

    }
    void ChangeToIdleHit()
    {
        hitCount--;
        if(hitCount == 0)
        {
            attackDelay = 0.5f;

            GetDamage = true;
            attackTimer = Time.time;

            agent.enabled = true;
            state = States.IDLE;
            anim.CrossFadeInFixedTime("idle", 0.2f);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<AttackCollider>() != null && GetDamage)
        {

            agent.enabled = false;
            if (state == States.FLY)
            {
                delayCaer = Time.time;

                rigidbody.AddForce(this.transform.up * ImpulsoGolpeAire * Time.fixedDeltaTime, ForceMode.Impulse);
                anim.CrossFadeInFixedTime("AirDamage", 0.2f);

                other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();

                collisionPoint = FindClosestPointOnCollider(other, transform.position);
                SpawnWhiteSplash(collisionPoint);
                SpawnBloodSplash(collisionPoint);
                SpawnHitLine(collisionPoint);
                ComboManager.instance.IncreaseCombo();

                fallStartTime = Time.time;
                state = States.HIT;
                hit = Hits.AIR;
            }
            else
            {
                if (other.CompareTag("GolpeVertical"))
                {
                    fallStartTime = Time.time;

                    anim.CrossFadeInFixedTime("GolpeSalto", 0.2f);

                    other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();

                    collisionPoint = FindClosestPointOnCollider(other, transform.position);
                    SpawnWhiteSplash(collisionPoint);
                    SpawnBloodSplash(collisionPoint);
                    SpawnHitLine(collisionPoint);
                    ComboManager.instance.IncreaseCombo();


                    rigidbody.AddForce(this.transform.up * JumpForce, ForceMode.Impulse);

                    Invoke("DelayAire", delayJumpHit);

                    state = States.HIT;
                    hit = Hits.UP;

                }
                else if (other.CompareTag("GolpeAire"))
                {
                    delayCaer = Time.time;

                    rigidbody.AddForce(this.transform.up * ImpulsoGolpeAire * Time.fixedDeltaTime, ForceMode.Impulse);
                    anim.CrossFadeInFixedTime("AirDamage", 0.2f);

                    other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();

                    collisionPoint = FindClosestPointOnCollider(other, transform.position);
                    SpawnWhiteSplash(collisionPoint);
                    SpawnBloodSplash(collisionPoint);
                    SpawnHitLine(collisionPoint);
                    ComboManager.instance.IncreaseCombo();

                    fallStartTime = Time.time;
                    state = States.HIT;
                    hit = Hits.AIR;
                }
                else if (other.CompareTag("GolpeVerticalCircular"))
                {
                    rigidbody.AddForce(this.transform.up * other.GetComponent<AttackCollider>().KnockbackY * Time.fixedDeltaTime, ForceMode.Impulse);
                    anim.CrossFadeInFixedTime("GolpeSalto", 0.2f);
                    fallStartTime = Time.time;
                    other.GetComponent<AttackCollider>().enemyHitFeedback?.PlayFeedbacks();


                    collisionPoint = FindClosestPointOnCollider(other, transform.position);
                    SpawnWhiteSplash(collisionPoint);
                    SpawnBloodSplash(collisionPoint);
                    SpawnHitLine(collisionPoint);
                    ComboManager.instance.IncreaseCombo();
                    state = States.HIT;
                    hit = Hits.UP;
                    Invoke("DelayAire", delayJumpHit);
                }
                else if (other.GetComponent<AttackCollider>() != null)
                {
                    hitCount++;

                    if (other.GetComponent<AttackCollider>().enemyStandUp)
                    {
                        Invoke("StandUp", 1f);
                    }
                    else
                    {
                        Invoke("ChangeToIdleHit", 0.5f);

                    }

                    state = States.HIT;
                    hit = Hits.HIT1;

                    anim.CrossFadeInFixedTime(other.GetComponent<AttackCollider>().enemyHitAnim, 0.2f);

                    rigidbody.AddForce(this.transform.up * other.GetComponent<AttackCollider>().KnockbackY * Time.fixedDeltaTime, ForceMode.Impulse);

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
        }
    }
}
