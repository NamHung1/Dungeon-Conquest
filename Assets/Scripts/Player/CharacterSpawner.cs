using UnityEngine;
using Cinemachine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public Transform spawnPoint;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        int index = CharacterData.SelectedCharacterIndex;

        if (index < 0 || index >= characterPrefabs.Length)
        {
            Debug.LogError("Invalid character index!");
            return;
        }

        GameObject player = Instantiate(characterPrefabs[index], spawnPoint.position, Quaternion.identity);
        player.tag = "Player";

        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;

        //SaveManager.Instance.LoadPlayer(data =>
        //{
        //    int index = data.characterIndex;
        //    if (index < 0 || index >= characterPrefabs.Length)
        //    {
        //        Debug.LogError("Invalid character index from save data!");
        //        return;
        //    }

        //    CharacterData.SelectedCharacterIndex = index;

        //    Vector2 spawnPos = new Vector2(data.posX, data.posY);
        //    GameObject player = Instantiate(characterPrefabs[index], spawnPos, Quaternion.identity);
        //    player.tag = "Player";

        //    PlayerStats stats = player.GetComponent<PlayerStats>();
        //    if (stats != null)
        //    {
        //        stats.currentHealth = data.hp;
        //        stats.currentMana = data.mp;
        //        stats.currentShield = data.sp;
        //    }

        //    virtualCamera.Follow = player.transform;
        //    virtualCamera.LookAt = player.transform;

        //    Debug.Log("Player loaded from Firebase and spawned.");
        //});

    }
}
