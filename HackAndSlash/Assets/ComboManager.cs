using DigitalRuby.ThunderAndLightning;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static ComboManager;

public class ComboManager : MonoBehaviour
{
    public enum ComboLetter
    {
        NONE,
        D,
        C,
        B,
        A,
        S
    }

    public Material[] letterImageMaterial;
    public Image comboLetterImage; // Renamed to avoid conflict
    public static ComboManager instance;

    public int comboCount;

    private Material originalMaterial;
    private float animationDuration = 0.77f;
    private ComboLetter currentComboLetter = ComboLetter.NONE; // Track the current combo letter

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnLetterChangeIn();
        originalMaterial = comboLetterImage.material;
    }

    private void OnLetterChangeIn()
    {
        comboLetterImage.material.DOFloat(0.77f, "_Animation_Factor", animationDuration);
    }

    private void OnLetterChangeOut()
    {
        comboLetterImage.material.DOFloat(0.0f, "_Animation_Factor", animationDuration);
    }

    private void Update()
    {
        if (comboCount >= 0 && comboCount < 10)
        {
            TryChangeLetter(ComboLetter.D);
        }
        else if (comboCount >= 10 && comboCount < 20)
        {
            TryChangeLetter(ComboLetter.C);
        }
        else if (comboCount >= 20 && comboCount < 30)
        {
            TryChangeLetter(ComboLetter.B);
        }
        else if (comboCount >= 30 && comboCount < 40)
        {
            TryChangeLetter(ComboLetter.A);
        }
        else if (comboCount >= 40 && comboCount <= 100)
        {
            TryChangeLetter(ComboLetter.S);
        }
        else
        {
            TryChangeLetter(ComboLetter.NONE);
        }
    }

    private void TryChangeLetter(ComboLetter newLetter)
    {
        if (newLetter != currentComboLetter)
        {
            // If the new letter is different from the current letter, change it
            ChangeLetterWithTransition(newLetter);
            currentComboLetter = newLetter;
        }
    }

    public void IncreaseCombo()
    {
        comboCount++;
    }

    public int GetComboCount()
    {
        return comboCount;
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }

    private void ChangeLetterWithTransition(ComboLetter letter)
    {
        // Hide the previous letter with a transition
        OnLetterChangeOut();

        // Delay for the duration of the transition
        DOTween.Sequence().AppendInterval(animationDuration).OnComplete(() =>
        {
            ChangeImage(letter); // Bring in the new letter
            OnLetterChangeIn(); // Show the new letter with a transition
        });
    }

    private void ChangeImage(ComboLetter letter)
    {
        Material materialReturn = ChangeMaterial(letter);
        if (comboLetterImage != null)
        {
            comboLetterImage.material = materialReturn;
        }
    }

    private Material ChangeMaterial(ComboLetter letter)
    {
        switch (letter)
        {
            case ComboLetter.NONE:
                return null;
            case ComboLetter.D:
                return letterImageMaterial[0];
            case ComboLetter.C:
                return letterImageMaterial[1];
            case ComboLetter.B:
                return letterImageMaterial[2];
            case ComboLetter.A:
                return letterImageMaterial[3];
            case ComboLetter.S:
                return letterImageMaterial[4];
            default:
                return null;
        }
    }
}