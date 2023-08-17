using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 10;

    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI energyText;

    [SerializeField] private float[] energyStages;

    public void UpdateEnergy(int energy)
    {
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        else if (energy < 0)
        {
            energy = 0;
        }

        energyBar.fillAmount = energyStages[energy];
        energyText.text = energy.ToString();
    }
}
