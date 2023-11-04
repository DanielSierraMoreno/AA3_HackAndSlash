using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public Slider comboSlider; // Reference to the combo slider
    public static ComboManager instance;

    public int comboCount;
    public int hitsPerLetter = 10;
    public float comboDecayTime = 5.0f;
    public float animationDuration = 0.77f;

    private Material originalMaterial;
    public ComboLetter currentComboLetter = ComboLetter.NONE;
    public float lastHitTime;
    public float comboDecayTimer = 0.0f; // Timer to control combo decay

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

        // Set initial values
        comboCount = 0;
        currentComboLetter = ComboLetter.NONE;
    }

    private void Update()
    {
        int hitsNeededForNextLetter = hitsPerLetter * (int)currentComboLetter;
        if (comboCount >= hitsNeededForNextLetter)
        {
            TryChangeLetter(currentComboLetter + 1);
        }
        else if (comboCount < hitsNeededForNextLetter && currentComboLetter != ComboLetter.NONE)
        {
            TryChangeLetter(currentComboLetter - 1);
        }

        // Update the combo slider value based on combo count
        comboSlider.value = comboCount;

        comboDecayTimer += Time.deltaTime;
        if (comboDecayTimer >= 3.0f) // Adjust the time here as needed
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
        int hitsNeededForNewLetter = hitsPerLetter * (int)newLetter;

        if (newLetter != currentComboLetter && comboCount >= hitsNeededForNewLetter)
        {
            ChangeLetterWithTransition(newLetter);
            currentComboLetter = newLetter;
            comboLetterImage.gameObject.SetActive(true);

            // Reset the slider value when the letter changes
            
        }
        else if (comboCount < 10)
        {
            comboLetterImage.gameObject.SetActive(false);
        }
    }

    public void IncreaseCombo()
    {
        comboCount++;
        lastHitTime = Time.time;
        comboDecayTimer = 0.0f; // Reset the combo decay timer
    }

    public int GetComboCount()
    {
        return comboCount;
    }

    public void ResetCombo()
    {
        if (comboCount > 0)
        {
            comboCount -= 1;
        }
    }

    private void ChangeLetterWithTransition(ComboLetter letter)
    {
        OnLetterChangeOut();
        //comboSlider.value = 0.0f;
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