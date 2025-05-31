using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public DungeonManager dungeonManager;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    private void Start()
    {
        if (dungeonManager == null)
            dungeonManager = FindObjectOfType<DungeonManager>();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (dungeonManager.IsAtFinalLevel() && !dungeonManager.IsBossDefeated())
            {
                Debug.Log("Defeat the Boss first!");
                return;
            }

            //SaveBeforeTransition();
            dungeonManager.GoToNextLevel();
        }
    }

    private void SaveBeforeTransition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        SaveData saveData = new SaveData
        {
            characterIndex = CharacterData.SelectedCharacterIndex,
            hp = stats.currentHealth,
            mp = stats.currentMana,
            sp = stats.currentShield,
            currentStage = dungeonManager.currentLevel,
            posX = player.transform.position.x,
            posY = player.transform.position.y
        };

        SaveManager.Instance.SavePlayer(saveData);
        Debug.Log("Progress saved to Firebase.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
