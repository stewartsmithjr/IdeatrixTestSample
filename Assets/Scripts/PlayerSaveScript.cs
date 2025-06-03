using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;

public class PlayerSaveScript : MonoBehaviour
{
    // written with help from FirebaseDatabase documentation
    private PlayerSaveScript() { }

    private static PlayerSaveScript instance;

    DatabaseReference database;

    public string PlayerName;

    public static PlayerSaveScript Singleton()
    {
        if (instance == null)
        {
            instance = new PlayerSaveScript();
        }
        return instance;
    }
    FirebaseApp app;
    private void Awake()
    {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance.RootReference;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    private void Start()
    {
        instance = this;
        
       
    }
   
    public async Task SaveData( string data)
    {
        
        await database.SetRawJsonValueAsync(data);
    }
    public async Task <bool> SaveExists()
    {
        var dataSnapShot = await database.GetValueAsync();
        return dataSnapShot.Exists;
    }
}
