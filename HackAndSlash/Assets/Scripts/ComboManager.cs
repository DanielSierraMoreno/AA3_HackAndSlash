using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
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
    public Image comboLetterImage;
    public Slider comboSlider;
    public TMP_Text comboNumber;
    public static ComboManager instance;

    public int comboCount;
    private int hitsToUpgradeLetter;
    private int hitsNeededForNextLetter;
    public float comboDecayTime = 5.0f;
    private float animationDuration = 0.77f;

    private Material originalMaterial;
    public ComboLetter currentComboLetter = ComboLetter.NONE;
    private float lastHitTime;
    private float comboDecayTimer = 0.0f;
    public int totalComboHitsPlusHowManyINeed;

    // Define a mapping between ComboLetter and hits to upgrade
    private Dictionary<ComboLetter, int> hitsToUpgradeMapping;
    private Dictionary<ComboLetter, float> comboDecayRates;

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
        comboLetterImage.gameObject.SetActive(false);
        originalMaterial = comboLetterImage.material;
        lastHitTime = Time.time;
        comboCount = 0;
        currentComboLetter = ComboLetter.NONE;

        // Initialize the mapping between ComboLetter and required hits to upgrade
        hitsToUpgradeMapping = new Dictionary<ComboLetter, int>
        {
            { ComboLetter.NONE, 0 },
            { ComboLetter.D, 4 },
            { ComboLetter.C, 10 },
            { ComboLetter.B, 20 },
            { ComboLetter.A, 35 },
            { ComboLetter.S, 50 }
        };
        comboDecayRates = new Dictionary<ComboLetter, float>
        {
            { ComboLetter.NONE, 1.0f },  
            { ComboLetter.D, 1.0f },    
            { ComboLetter.C, 1.75f },   
            { ComboLetter.B, 1.9f },    
            { ComboLetter.A, 2.0f },    
            { ComboLetter.S, 5f }     
        };

        hitsToUpgradeLetter = hitsToUpgradeMapping[currentComboLetter];
        hitsNeededForNextLetter = CalculateHitsNeededForLetter(currentComboLetter);
    }

    private void Update()
    {
        if (comboCount >= hitsNeededForNextLetter)
        {
            TryChangeLetter(currentComboLetter + 1);
        }
        else if (comboCount < hitsNeededForNextLetter && currentComboLetter != ComboLetter.NONE)
        {
            TryChangeLetter(currentComboLetter - 1);
        }

        if (currentComboLetter == ComboLetter.NONE)
        {
            comboLetterImage.gameObject.SetActive(false);
            comboSlider.gameObject.SetActive(false);
            comboNumber.gameObject.SetActive(false);
        }
        else
        {
            comboLetterImage.gameObject.SetActive(true);
            comboSlider.gameObject.SetActive(true);
            comboNumber.gameObject.SetActive(true);


            comboNumber.text = comboCount.ToString();
            comboSlider.value -= comboDecayRates[currentComboLetter] * Time.deltaTime;

            if (comboSlider.value <= 0)
            {
                // Slider value reached 0, reset combo count and letter
                comboCount = 0;
                currentComboLetter = ComboLetter.NONE;
                comboSlider.value = 0;
            }
        }

   
        comboDecayTimer += Time.deltaTime;

        if (comboDecayTimer >= comboDecayTime)
        {
            comboDecayTimer = 0.0f;
            if (comboCount > 0)
            {
                comboCount -= 1;
            }
        }
    }

    private void OnLetterChangeIn()
    {
        comboLetterImage.material.DOFloat(0.77f, "_Animation_Factor", animationDuration);
    }

    private void OnLetterChangeOut()
    {
        comboLetterImage.material.DOFloat(0.0f, "_Animation_Factor", animationDuration);
    }

    private void TryChangeLetter(ComboLetter newLetter)
{
    if (newLetter != currentComboLetter)
    {
        if (currentComboLetter == ComboLetter.S)
        {
                // If the current combo letter is 'S', keep the combo going
                comboSlider.maxValue = 50;
                return;
        }
        else if (comboCount >= CalculateHitsNeededForLetter(newLetter))
        {
            hitsToUpgradeLetter = hitsToUpgradeMapping[newLetter];
            totalComboHitsPlusHowManyINeed = hitsToUpgradeMapping.ContainsKey(newLetter + 1) ? hitsToUpgradeMapping[newLetter + 1] : 0;
            comboSlider.maxValue = totalComboHitsPlusHowManyINeed;
            ChangeLetterWithTransition(newLetter);
            currentComboLetter = newLetter;
        }
    }
}

    private int CalculateHitsNeededForLetter(ComboLetter letter)
    {
        if (letter == ComboLetter.NONE)
        {
            return hitsToUpgradeMapping[ComboLetter.NONE];
        }

        int hitsNeeded = hitsToUpgradeMapping[letter];
        return hitsNeeded;
    }

    public void IncreaseCombo()
    {
        comboCount++;
        lastHitTime = Time.time;
        comboDecayTimer = 0.0f;
        if (currentComboLetter == ComboLetter.S)
        {
            comboSlider.value = comboSlider.maxValue;
        }
        else
        {
            comboSlider.value = comboCount;
        }
    }

    public int GetComboCount()
    {
        return comboCount;
    }

    public int GetTotalComboHits()
    {
        return comboCount;
    }

    public void ResetCombo()
    {
        if (comboCount > 0)
        {
            comboCount--;
        }
    }

    private void ChangeLetterWithTransition(ComboLetter letter)
    {
        OnLetterChangeOut();
        DOTween.Sequence().AppendInterval(animationDuration).OnComplete(() =>
        {
            ChangeImage(letter);
            OnLetterChangeIn();
        });
    }

    private void ChangeImage(ComboLetter letter)
    {
        Material materialReturn = ChangeMaterial(letter);

        if (comboLetterImage != null)
        {
            if (letter != ComboLetter.NONE)
            {
                comboLetterImage.material = materialReturn;
            }
            else
            {
                comboLetterImage.material = originalMaterial;
            }
        }
    }

    private Material ChangeMaterial(ComboLetter letter)
    {
        Material materialReturn = null;

        switch (letter)
        {
            case ComboLetter.NONE:
                break;
            case ComboLetter.D:
                materialReturn = letterImageMaterial[0];
                break;
            case ComboLetter.C:
                materialReturn = letterImageMaterial[1];
                break;
            case ComboLetter.B:
                materialReturn = letterImageMaterial[2];
                break;
            case ComboLetter.A:
                materialReturn = letterImageMaterial[3];
                break;
            case ComboLetter.S:
                materialReturn = letterImageMaterial[4];
                break;
            default:
                materialReturn = null;
                break;
        }

        return materialReturn;
    }
}
