using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIConnector : MonoBehaviour
{
    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider manaSlider;

    public TMP_Text healthNumber;
    public TMP_Text shieldNumber;
    public TMP_Text manaNumber;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.healthSlider = healthSlider;
                stats.shieldSlider = shieldSlider;
                stats.manaSlider = manaSlider;

                stats.healthNumber = healthNumber;
                stats.shieldNumber = shieldNumber;
                stats.manaNumber = manaNumber;

                stats.Start();
            }
        }
    }
}
