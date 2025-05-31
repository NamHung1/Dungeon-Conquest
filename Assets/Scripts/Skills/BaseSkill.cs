using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour, ISkill
{
    public int mpCost = 10;
    public float cooldownTime = 2f;
    public PlayerStats playerStats;
    private float lastUsedTime = -Mathf.Infinity;

    //protected virtual void Awake()
    //{
    //    playerStats = GetComponent<PlayerStats>();
    //}

    public void ActivateSkill()
    {
        if (Time.time < lastUsedTime + cooldownTime)
        {
            Debug.Log("Skill is on cooldown!");
            return;
        }

        if (playerStats != null && playerStats.currentMana >= mpCost)
        {
            lastUsedTime = Time.time;
            playerStats.UseMana(mpCost);
            ExecuteSkill();
        }
        else
        {
            Debug.Log("Not enough MP!");
        }
    }

    protected abstract void ExecuteSkill();
}
