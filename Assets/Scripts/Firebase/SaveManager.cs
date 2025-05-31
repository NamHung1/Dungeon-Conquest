using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private DatabaseReference dbReference;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                if (task.Result == DependencyStatus.Available)
                {
                    dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                }
                else
                {
                    Debug.LogError("Firebase not ready" + task.Result);
                }
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayer(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        dbReference.Child("players").Child("player1").SetRawJsonValueAsync(json);
    }

    public void LoadPlayer(System.Action<SaveData> onLoaded)
    {
        dbReference.Child("players").Child("player1").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                SaveData data = JsonUtility.FromJson<SaveData>(snapshot.GetRawJsonValue());
                onLoaded?.Invoke(data);
            }
            else
            {
                Debug.LogWarning("Failed");
            }
        });
    }
}
