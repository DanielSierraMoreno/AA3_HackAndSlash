using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    enum States { MOVE, DASH, JUMP, ATTACK, IDLE, DELAYMOVE };

    enum Attacks { GROUND, AIR, RUN };

    enum Moves { WALK, RUN, IDLE };

    enum Dash { DASH, DOUBLEDASH, AIRDASH };

    enum Jump { JUMP, FALL, LAND };

    States states;
    Attacks attacks;
    Moves moves;
    Dash dash;
    Jump jump;

    public GameObject camera;
    public float CameraRotatSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float walkSpeedAir;
    public float runSpeedAir;

    public GameObject movementController;
    public GameObject player;

    Vector3 moveDir;
    Vector3 moveDirSaved;

    Animator playerAnim;

    float delayMove;

    public float delayIdleToMoveTime;

    public enum ComboAtaques { combo1, combo2, combo3, combo4, air, run};

    public int currentScroll;
    [System.Serializable]
    public struct Ataques
    {
        public float ataque;
        public float delay;
        public bool nextAttack;
        public GameObject effects;

        public float effectDelay;
        public string name;

        public float transition;

        public AnimationCurve curvaDeVelocidadMovimiento;
        public float velocidadMovimiento;

        public AnimationCurve curvaDeVelocidadMovimientoY;
        public float velocidadMovimientoY;
    }
    [System.Serializable]
    public class ListaAtaques
    {
        public ComboAtaques combo;
        public Ataques[] attacks;


    }
    public ListaAtaques[] ataques;
    ListaAtaques currentComboAttacks;
    public ListaAtaques airCombo;
    public ListaAtaques runCombo;

    int currentComboAttack;

    float attackStartTime;
    float fallStartTime;
    public float gravity;
    float timeJumping;
    float timeLanding;

    bool doubleJump;
    public float jumpForce;
    public float distanceFloor;

    public float delayCombos;
    float comboFinishedTime;
    bool attackFinished;
    ListaAtaques GetAttacks(ComboAtaques combo)
    {
        for(int i = 0; i < ataques.Length; i++)
        {
            if(ataques[i].combo == combo)
            {
                return ataques[i];
            }
        }
        if(combo == ComboAtaques.air)
            return airCombo;

        if (combo == ComboAtaques.run)
            return runCombo;

        return null;
    }
    bool CheckIfNextAttack()
    {
        if((Time.time- attackStartTime) >= currentComboAttacks.attacks[currentComboAttack].ataque)
        {
            if(currentComboAttack +1 == currentComboAttacks.attacks.Length)
            {
                comboFinishedTime = Time.time;
                attackFinished = true; 
                currentComboAttack = -1;

            }
            CheckIfReturnIdle();
            CheckIfStartMove();
            CheckIfIsFalling();
            return true;
        }
        else
        {
            return false;
        }
    }
    void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0,this.gravity*(Time.time-fallStartTime),0);
        this.GetComponent<Rigidbody>().AddForce(gravity * Time.deltaTime, ForceMode.Force);

    }
    void moveInAir(float vel)
    {
        if(moves == Moves.IDLE || controller.LeftStickValue().magnitude < 0.2f)
            this.GetComponent<Rigidbody>().AddForce(moveDirSaved * 0 * Time.deltaTime, ForceMode.Force);
        else
            this.GetComponent<Rigidbody>().AddForce(moveDirSaved * vel * Time.deltaTime, ForceMode.Force);

    }
    void PlayAttack()
    {

        currentComboAttack++;
        playerAnim.CrossFadeInFixedTime(currentComboAttacks.attacks[currentComboAttack].name, currentComboAttacks.attacks[currentComboAttack].transition);

        attackStartTime = Time.time;
    }
    void AttackMovement()
    {
        this.GetComponent<Rigidbody>().AddForce(this.transform.up * currentComboAttacks.attacks[currentComboAttack].curvaDeVelocidadMovimientoY.Evaluate(Time.time - attackStartTime) * currentComboAttacks.attacks[currentComboAttack].velocidadMovimientoY * Time.deltaTime, ForceMode.Force);

        this.GetComponent<Rigidbody>().AddForce(moveDirSaved * currentComboAttacks.attacks[currentComboAttack].curvaDeVelocidadMovimiento.Evaluate(Time.time-attackStartTime) * currentComboAttacks.attacks[currentComboAttack].velocidadMovimiento * Time.deltaTime, ForceMode.Force);
    }

    bool CheckIfIsFalling()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out hit, 20, 7))
        {
            if (hit.distance > distanceFloor)
            {
                fallStartTime = Time.time;
                states = States.JUMP;
                jump = Jump.FALL;
                playerAnim.CrossFadeInFixedTime("Fall", 0.2f);

                return true;

            }

        }
        return false;
        if (false)
        {

        }


        return false;
    }
    ControllerManager controller; 


    // Start is called before the first frame update
    void Start()
    {
        attackFinished = false;

        controller = GameObject.FindAnyObjectByType<ControllerManager>().GetComponent<ControllerManager>();
        currentComboAttack = -1;
        playerAnim = player.GetComponent<Animator>();
        states = States.IDLE;
        moves = Moves.IDLE;
    }
    ComboAtaques GetCurrentAttackCombo()
    {
        return currentComboAttacks.combo;
    }
    bool CheckIfJump()
    {

        if (controller.CheckIfJump())
        {        
            this.GetComponent<Rigidbody>().drag = 5;

            if(states != States.JUMP)
            {
                timeJumping = Time.time;
                fallStartTime = Time.time;
                states = States.JUMP;
                jump = Jump.JUMP;
                playerAnim.CrossFadeInFixedTime("Jump", 0.1f);
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.GetComponent<Rigidbody>().AddForce( this.transform.up *jumpForce, ForceMode.Impulse);
                return true;

            }
            else if(!doubleJump)
            {
                Move(1);
                doubleJump = true;
                timeJumping = Time.time;
                fallStartTime = Time.time;
                states = States.JUMP;
                jump = Jump.JUMP;
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                this.GetComponent<Rigidbody>().AddForce(this.transform.up * jumpForce*1.5f, ForceMode.Impulse);
                playerAnim.CrossFadeInFixedTime("DoubleJump", 0.2f);
                return true;

            }


        }

        return false;
    }
    bool CheckIfLand()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(-this.transform.up), out hit,20,7))
        {
            if(hit.distance < distanceFloor)
            {
                timeLanding = Time.time;
                playerAnim.CrossFadeInFixedTime("Land", 0.2f);
                jump = Jump.LAND;
                doubleJump = false;

                return true;
            }

        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.GetController())
            return;

        RotateCamera();
        switch (states)
        {
            case States.IDLE:
                if (CheckAtaques())
                    break;
                if (CheckIfIsFalling())
                    break;
                if (CheckIfJump())
                    break;
                CheckIfStartMove();
                this.GetComponent<Rigidbody>().drag = 15;

                break;
            case States.ATTACK:
                this.GetComponent<Rigidbody>().drag = 15;
                CheckNextAttack();
                if (CheckIfNextAttack())
                    break;
                AttackMovement();
                switch (attacks)
                {
                    case Attacks.GROUND:

                        break;
                    case Attacks.AIR:


                        break;
                    case Attacks.RUN:

                        break;
                }


                break;
            case States.DASH:
                if (CheckAtaques())
                    break;


                break;
            case States.JUMP:
                ApplyGravity();
                this.GetComponent<Rigidbody>().drag = 5;

                switch (moves)
                {
                    case Moves.WALK:
                        moveInAir(walkSpeedAir);

                        break;
                    case Moves.RUN:
                        moveInAir(runSpeedAir);
                        break;
                }
                switch (jump)
                {
                    case Jump.JUMP:
                        if((Time.time- timeJumping) > 0.2f)
                        {
                            playerAnim.CrossFadeInFixedTime("Fall", 0.2f);

                            jump = Jump.FALL;
                        }

                        break;
                    case Jump.FALL:
                        if (CheckIfJump())
                            break;
                        if (CheckAtaques())
                            break;
                        if (CheckIfLand())
                            break;
                        break;
                    case Jump.LAND:
                        if ((Time.time - timeLanding) > 0.10f)
                        {
                            playerAnim.CrossFadeInFixedTime("Land", 0.2f);

                            states = States.IDLE;
                        }
                        break;
                }
                break;
            case States.MOVE:
                this.GetComponent<Rigidbody>().drag = 15;

                if (CheckIfIsFalling())
                    break;
                if (CheckAtaques())
                    break;
                if (CheckIfJump())
                    break;
                switch (moves)
                {
                    case Moves.WALK:
                        Move(walkSpeed);
                        
                        break;
                    case Moves.RUN:
                        Move(runSpeed);
                        break;
                    default:
                        Move(0);
                        break;
                }

                CheckIfReturnIdle();
                CheckMove();

                break;
            case States.DELAYMOVE:
                if (CheckAtaques())
                    break;
                CheckIfReturnIdle();

                if(Time.time - delayMove > delayIdleToMoveTime)
                {
                    CheckMove();
                }
                break;
        }
    }

    bool CheckAtaques()
    {
        if (attackFinished && (Time.time - comboFinishedTime) >= delayCombos)
        {
            attackFinished = false;
        }
        else if(attackFinished && (Time.time - comboFinishedTime) < delayCombos)
            return false;

        float delay = 0;
        if(currentComboAttack != -1 && currentComboAttacks != null && (currentComboAttack+1) != currentComboAttacks.attacks.Length)
        {

            delay = currentComboAttacks.attacks[currentComboAttack].delay;

        }
        if ((Time.time - attackStartTime) >= delay)
        {
            currentComboAttack = -1;
            if (controller.ataqueCuadrado)
            {

                if (states == States.JUMP)
                {
                    attacks = Attacks.AIR;
                    currentComboAttacks = GetAttacks(ComboAtaques.air);
                    PlayAttack();
                }
                else if (states == States.MOVE)
                {
                    if (moves == Moves.RUN)
                    {
                        attacks = Attacks.RUN;
                        currentComboAttacks = GetAttacks(ComboAtaques.run);
                        PlayAttack();
                    }
                    else
                    {
                        attacks = Attacks.GROUND;
                        currentComboAttacks = GetAttacks(ComboAtaques.combo1);
                        PlayAttack();
                    }

                }
                else if (states == States.IDLE)
                {

                    attacks = Attacks.GROUND;
                    currentComboAttacks = GetAttacks(ComboAtaques.combo1);
                    PlayAttack();
                }

                states = States.ATTACK;

                controller.ResetBotonesAtaques();
                return true;
            }

            if (controller.ataqueTriangulo && states != States.JUMP)
            {
                states = States.ATTACK;
                attacks = Attacks.GROUND;
                currentComboAttacks = GetAttacks(ComboAtaques.combo3);
                PlayAttack();
                controller.ResetBotonesAtaques();
                return true;
            }

            if (controller.ataqueCuadradoCargado && states != States.JUMP)
            {
                states = States.ATTACK;
                attacks = Attacks.GROUND;
                currentComboAttacks = GetAttacks(ComboAtaques.combo2);
                PlayAttack();
                controller.ResetBotonesAtaques();
                return true;
            }

            if (controller.ataqueTrianguloCargado && states != States.JUMP)
            {
                states = States.ATTACK;
                attacks = Attacks.GROUND;
                currentComboAttacks = GetAttacks(ComboAtaques.combo4);
                PlayAttack();
                controller.ResetBotonesAtaques();
                return true;
            }
        }
        else
        {
            if(currentComboAttacks.attacks[currentComboAttack].nextAttack)
            {
                states = States.ATTACK;
                currentComboAttacks.attacks[currentComboAttack].nextAttack = false;
                PlayAttack();
                return true;
            }

            if(controller.ataqueTriangulo && (GetCurrentAttackCombo() == ComboAtaques.combo3|| GetCurrentAttackCombo() == ComboAtaques.combo4))
            {
                states = States.ATTACK;

                PlayAttack();
                return true;

            }
            if (controller.ataqueCuadrado && (GetCurrentAttackCombo() == ComboAtaques.combo1 || GetCurrentAttackCombo() == ComboAtaques.combo2 || GetCurrentAttackCombo() == ComboAtaques.air || GetCurrentAttackCombo() == ComboAtaques.run))
            {
                states = States.ATTACK;

                PlayAttack();
                return true;
            }
        }
        return false;

    }
    void CheckNextAttack()
    {
        switch(GetCurrentAttackCombo())
        {
            case ComboAtaques.combo1:
                if (controller.ataqueCuadrado)
                    currentComboAttacks.attacks[currentComboAttack].nextAttack = true;
                break;
            case ComboAtaques.combo2:
                if (controller.ataqueCuadrado)
                    currentComboAttacks.attacks[currentComboAttack].nextAttack = true;
                break;
            case ComboAtaques.combo3:
                if (controller.ataqueTriangulo)
                    currentComboAttacks.attacks[currentComboAttack].nextAttack = true;
                break;
            case ComboAtaques.combo4:
                if (controller.ataqueTriangulo)
                    currentComboAttacks.attacks[currentComboAttack].nextAttack = true;
                break;
            case ComboAtaques.air:
                if (controller.ataqueCuadrado)
                    currentComboAttacks.attacks[currentComboAttack].nextAttack = true;
                break;
            case ComboAtaques.run:
                if (controller.ataqueCuadrado)
                    currentComboAttacks.attacks[currentComboAttack].nextAttack = true;
                break;
        }
    }
    void CheckIfStartMove()
    {
        if (controller.StartMove())
        {
            delayMove = Time.time;
            playerAnim.CrossFadeInFixedTime("StartMoving", 0.1f);
            states = States.DELAYMOVE;
        }
    }
    void CheckMove()
    {
        if(controller.StartMove() && states != States.MOVE)
        {
            states = States.MOVE;

            if(controller.RightTriggerPressed())
            {
                moves = Moves.RUN;
                playerAnim.CrossFadeInFixedTime("Run", 0.2f);

            }
            else
            {
                moves = Moves.WALK;
                playerAnim.CrossFadeInFixedTime("Walk", 0.2f);

            }
        }
        else if(states == States.MOVE)
        {
            if (controller.RightTriggerPressed() && moves == Moves.WALK)
            {
                moves = Moves.RUN;
                playerAnim.CrossFadeInFixedTime("Run", 0.2f);

            }
            else if(!controller.RightTriggerPressed() && moves == Moves.RUN)
            {
                moves = Moves.WALK;
                playerAnim.CrossFadeInFixedTime("Walk", 0.2f);

            }
        }


    }
    void CheckIfReturnIdle()
    {
        if (!controller.StartMove() && states != States.IDLE)
        {
            states = States.IDLE;
            moves = Moves.IDLE;
            playerAnim.CrossFadeInFixedTime("Idle", 0.2f);


        }
    }

    void Move(float velocity)
    {
        movementController.transform.localPosition += new Vector3(controller.LeftStickValue().x, 0, controller.LeftStickValue().y).normalized;
        player.transform.LookAt(movementController.transform.position);
        moveDir = (movementController.transform.position - this.transform.position).normalized;
        movementController.transform.localPosition = new Vector3();
        if(moveDir.magnitude != 0)
            moveDirSaved = moveDir;
        this.GetComponent<Rigidbody>().AddForce(moveDir * velocity * Time.deltaTime, ForceMode.Force);
    }
    void RotateCamera()
    {
        if (Mathf.Abs(controller.RightStickValue().x) > 0.2f)
        {
            camera.transform.Rotate(new Vector3(0, controller.RightStickValue().x, 0) * Time.deltaTime * CameraRotatSpeed);
        }
    }
}
