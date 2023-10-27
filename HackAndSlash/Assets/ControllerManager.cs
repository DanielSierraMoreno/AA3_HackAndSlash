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

    // Start is called before the first frame update
    void Start()
    {
        jump = false;
        canJump = true;
        dejarMantenerCuadrado = false;
        dejarMantenerTriangulo = false;
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log(Gamepad.all[i].name);
            controller = Gamepad.all[i];
        }
    }
    public bool GetController()
    {
        if (controller == null)
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                Debug.Log(Gamepad.all[i].name);
                controller = Gamepad.all[i];
                return true;
            }

            return false;

        }
        else
        {
            return true;

        }
    }
    public bool StartMove()
    {

        return controller.leftStick.ReadValue().magnitude > 0.3f;
    }
    public Vector2 LeftStickValue()
    {
        return controller.leftStick.ReadValue();
    }
    public Vector2 RightStickValue()
    {
        return controller.rightStick.ReadValue();
    }
    public bool RightTriggerPressed()
    {

        return controller.rightTrigger.isPressed;
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
        if (controller.aButton.wasPressedThisFrame && canJump)
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
        ataqueCuadrado = false;
        ataqueTriangulo = false;
        ataqueCuadradoCargado = false;
        ataqueTrianguloCargado = false;

        if (controller.xButton.wasPressedThisFrame)
        {
            delayCuadrado = Time.time;
        }
        if (controller.yButton.wasPressedThisFrame)
        {
            delayTriangulo = Time.time;
        }
        if (controller.xButton.wasReleasedThisFrame && (Time.time - delayCuadrado) <= 0.5f)
        {
            ataqueCuadrado = true;
        }
        if (controller.yButton.wasReleasedThisFrame && (Time.time - delayTriangulo) <= 0.5f)
        {
            ataqueTriangulo = true;

        }
        if (controller.xButton.wasReleasedThisFrame && (Time.time - delayCuadrado) > 0.5f)
        {
            ataqueCuadradoCargado = true;
        }
        if (controller.yButton.wasReleasedThisFrame && (Time.time - delayTriangulo) > 0.5f)
        {
            ataqueTrianguloCargado = true;

        }
    }
    // Update is called once per frame
    void Update()
    {
        if (controller.aButton.wasReleasedThisFrame && !canJump)
        {
            jump = false;
            canJump = true;
        }
        ControlesAtaques();
    }
}
