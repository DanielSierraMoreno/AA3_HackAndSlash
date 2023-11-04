using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComboSlider : MonoBehaviour
{
    public Slider comboSlider;
    public ComboManager comboManager;

    private void Update()
    {
        if (comboManager != null && comboSlider != null)
        {
            int hitsNeededForNextLetter = comboManager.hitsPerLetter * (int)comboManager.currentComboLetter;
            comboSlider.value = (float)comboManager.comboCount / hitsNeededForNextLetter;
            if (comboSlider.value >= 1.0f)
            {
                comboSlider.value = 0.0f;
            }
        }
    }
}
