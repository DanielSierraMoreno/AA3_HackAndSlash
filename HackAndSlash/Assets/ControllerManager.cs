using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    public Gamepad controller;

    float delayCuadrado;
    float delayTriangulo;
    bool cuadradoHold;
    bool trianguloHold;

    public bool ataqueCuadrado;
    public bool ataqueCuadradoCargado;
    public bool ataqueTriangulo;
    public bool ataqueTrianguloCargado;

    bool dejarMantenerCuadrado;
    bool dejarMantenerTriangulo;

    bool jump;
    bool canJump;

    public bool MouseControl;
    Vector2 leftStick;
    Vector2 rightStick;
    InputAction.CallbackContext X;
    InputAction.CallbackContext O;
    InputAction.CallbackContext Box;
    InputAction.CallbackContext Triangle;
    InputAction.CallbackContext R2;
    // Start is called before the first frame update
    void Start()
    {
        leftStick = new Vector2();
        rightStick = new Vector2();

        ataqueCuadrado = false;
        ataqueTriangulo = false;
        ataqueCuadradoCargado = false;
        ataqueTrianguloCargado = false;

        jump = false;
        canJump = true;
        dejarMantenerCuadrado = false;
        dejarMantenerTriangulo = false;
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            controller = Gamepad.all[i];
        }
    }
    public bool GetController()
    {
        return true;
    }

    public void GetLeftJoystick(InputAction.CallbackContext context)
    {
        leftStick = context.ReadValue<Vector2>();
    }
    public void GetRightJoystick(InputAction.CallbackContext context)
    {
        rightStick = context.ReadValue<Vector2>();
    }
    public void GetR2(InputAction.CallbackContext context)
    {
        R2 = context;
    }
    public void GetX(InputAction.CallbackContext context)
    {
        X = context;
    }
    public void GetO(InputAction.CallbackContext context)
    {
        O = context;
    }
    public void GetBox(InputAction.CallbackContext context)
    {
        Box = context;
    }
    public void GetTriangle(InputAction.CallbackContext context)
    {
        Triangle = context;
    }
    public bool StartMove()
    {

        return leftStick.magnitude > 0.3f;

        

    }
    public Vector2 LeftStickValue()
    {

            return leftStick;

        

    }
    public Vector2 RightStickValue()
    {

        return rightStick;
    }

    public bool RightTriggerPressed()
    {
        if (R2.action == null)
            return false;
        return R2.action.IsPressed();
    }
    public void ResetBotonesAtaques()
    {
        ataqueCuadrado = false;
        ataqueCuadradoCargado = false;
        ataqueTriangulo = false;
        ataqueTrianguloCargado = false;
    }

    public bool CheckIfJump()
    {
        if (X.action == null)
            return false;
        if (X.action.WasPressedThisFrame() && canJump)
        {
            jump = true;
            canJump = false;
        }


        if (jump)
        {
            jump = false;
            canJump = true;

            return true;
        }
        return false;
    }

    public void ControlesAtaques()
    {
        //ataqueCuadrado = false;
        //ataqueTriangulo = false;
        //ataqueCuadradoCargado = false;
        //ataqueTrianguloCargado = false;

        if (Box.action != null)
        {
            if (Box.action.WasPressedThisFrame())
            {
                delayCuadrado = Time.time;
                cuadradoHold = true;
            }

            if (Box.action.WasReleasedThisFrame() && (Time.time - delayCuadrado) <= 0.25f)
            {
                ataqueCuadrado = true;

                ataqueTriangulo = false;
                ataqueCuadradoCargado = false;
                ataqueTrianguloCargado = false;
            }
            if (Box.action.IsPressed() && (Time.time - delayCuadrado) > 0.25f && cuadradoHold)
            {
                ataqueCuadradoCargado = true;
                cuadradoHold = false;

                ataqueCuadrado = false;
                ataqueTriangulo = false;
                ataqueTrianguloCargado = false;
            }
        }
        if (Triangle.action != null)
        {
            if (Triangle.action.WasPressedThisFrame())
            {
                delayTriangulo = Time.time;
                trianguloHold = true;

            }
            if (Triangle.action.WasReleasedThisFrame() && (Time.time - delayTriangulo) <= 0.25f)
            {
                ataqueTriangulo = true;

                ataqueCuadrado = false;
                ataqueCuadradoCargado = false;
                ataqueTrianguloCargado = false;
            }

            if (Triangle.action.IsPressed() && (Time.time - delayTriangulo) > 0.25f && trianguloHold)
            {
                ataqueTrianguloCargado = true;
                trianguloHold = false;

                ataqueCuadrado = false;
                ataqueTriangulo = false;
                ataqueCuadradoCargado = false;
            }
        }





    }
    
    // Update is called once per frame
    void Update()
    {
        
        if(X.action != null)
        {
            if (X.action.WasReleasedThisFrame() && !canJump)
            {
                jump = false;
                canJump = true;
            }
        }

        ControlesAtaques();
    }

    public bool GetDash()
    {
        if (O.action == null)
            return false;

        return O.action.WasPressedThisFrame();
    }


}
