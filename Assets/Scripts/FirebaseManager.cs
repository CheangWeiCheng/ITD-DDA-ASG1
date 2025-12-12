using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static DatabaseReference DBref;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                DBref = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Ready");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies");
            }
        });
    }
}

