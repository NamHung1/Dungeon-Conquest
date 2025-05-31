using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
    public Transform charactersParent; // Characters GameObject
    private GameObject[] characters;
    private int currentIndex = 0;

    [Header("UI Elements")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text spText;
    public TMP_Text mpText;
    //public Button continueBtn;

    [Header("Menu")]
    public GameObject homeMenu;
    public GameObject settingMenu;

    private void Start()
    {
        charactersParent.gameObject.SetActive(true);

        currentIndex = CharacterData.SelectedCharacterIndex;

        characters = new GameObject[charactersParent.childCount];
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = charactersParent.GetChild(i).gameObject;
        }

        ShowCharacter(currentIndex);

        homeMenu.SetActive(true);

        //SaveManager.Instance.LoadPlayer(data =>
        //{
        //    if (data != null && data.currentStage < 6)
        //    {
        //        continueBtn.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        continueBtn.gameObject.SetActive(false);
        //    }
        //});
    }

    public void ShowCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        UpdateUI(index);
    }

    private void UpdateUI(int index)
    {
        GameObject character = characters[index];

        CharacterUIStats stats = character.GetComponent<CharacterUIStats>();

        if (stats != null)
        {
            nameText.text = stats.characterName;
            hpText.text = "HP: " + stats.HP.ToString();
            spText.text = "SP: " + stats.SP.ToString();
            mpText.text = "MP: " + stats.MP.ToString();
        }
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCharacter(currentIndex);
    }

    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCharacter(currentIndex);
    }

    public void Play()
    {
        CharacterData.SelectedCharacterIndex = currentIndex;
        SceneManager.LoadScene("Game");
    }

    //public void ContinueGame()
    //{
    //    SaveManager.Instance.LoadPlayer(data =>
    //    {
    //        if (data != null)
    //        {
    //            CharacterData.SelectedCharacterIndex = data.characterIndex;
    //            SceneManager.LoadScene("Game");
    //        }
    //        else
    //        {
    //            Debug.Log("No saved progress found.");
    //        }
    //    });
    //}

    public void OpenSetting()
    {
        if (settingMenu != null)
        {
            settingMenu.SetActive(true);
            homeMenu.SetActive(false);
        }
    }

    public void CloseSetting()
    {
        if (settingMenu != null)
        {
            settingMenu.SetActive(false);
            homeMenu.SetActive(true);
        }
    }

    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
