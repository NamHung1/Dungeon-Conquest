using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    private ISkill skill;

    private void Start()
    {
        skill = GetComponent<ISkill>();
        if (skill is BaseSkill baseSkill)
        {
            baseSkill.playerStats = GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skill?.ActivateSkill();
        }
    }
}
