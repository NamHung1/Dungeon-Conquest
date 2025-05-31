using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Character Name")]
    public string characterName;

    [Header("Player Stats")]
    public int currentHealth;
    public int maxHealth;
    public int currentShield;
    public int maxShield;
    public int currentMana;
    public int maxMana;

    public Slider healthSlider;
    public Slider shieldSlider;
    public Slider manaSlider;

    public TMP_Text healthNumber;
    public TMP_Text shieldNumber;
    public TMP_Text manaNumber;

    public float shieldRegenDelay = 3f;
    public float shieldRegenRate = 1f;

    private Coroutine shieldRegenCoroutine;

    public void Start()
    {
        //HP
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthNumber.text = currentHealth + "/" + maxHealth;

        //SP
        shieldSlider.maxValue = maxShield;
        shieldSlider.value = currentShield;
        shieldNumber.text = currentShield + "/" + maxShield;

        //MP
        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;
        manaNumber.text = currentMana + "/" + maxMana;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        if (currentShield > 0)
        {
            currentShield -= damage;
            if (currentShield < 0)
            {
                currentHealth += currentShield;
                currentShield = 0;
            }

            StartShieldRegen();
        }
        else
        {
            currentHealth -= damage;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StartShieldRegen()
    {
        if (shieldRegenCoroutine != null)
        {
            StopCoroutine(shieldRegenCoroutine); 
        }

        shieldRegenCoroutine = StartCoroutine(RegenerateShield());
    }

    private IEnumerator RegenerateShield()
    {
        yield return new WaitForSeconds(shieldRegenDelay); 

        while (currentShield < maxShield)
        {
            currentShield += Mathf.CeilToInt(shieldRegenRate * Time.deltaTime);
            //currentShield = Mathf.Clamp(currentShield, 0, maxShield);
            UpdateUI();
            yield return new WaitForSeconds(shieldRegenRate); ;
        }

        shieldRegenCoroutine = null;
    }

    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        healthSlider.value = currentHealth;
        healthNumber.text = currentHealth + "/" + maxHealth;

        shieldSlider.value = currentShield;
        shieldNumber.text = currentShield + "/" + maxShield;

        manaSlider.value = currentMana;
        manaNumber.text = currentMana + "/" + maxMana;
    }

    public void Die()
    {
        gameObject.SetActive(false);

        if (GameOver.Instance != null)
        {
            GameOver.Instance.ShowGameOver(false);
        }
    }
}
