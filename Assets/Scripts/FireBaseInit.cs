using Firebase.Auth;
using UnityEngine;
using System;
using System.Collections;
using Firebase;
using Firebase.Analytics;
using UnityEngine.Events;
using DG.Tweening;
using Firebase.Database;
using Firebase.Extensions;
public class FireBaseInit : MonoBehaviour
{
    // created using FireBases's documentation

    public UnityEvent OnFireBaseInit = new UnityEvent();
    FirebaseApp app;
    [SerializeField] CanvasGroup button;
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

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

    void InitializeFireBaseAgain()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Exception != null)
            {
                Debug.Log("Firebase Failed to Initialize");
                return;
            }

            OnFireBaseInit.Invoke();

            button.DOFade(1, 1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
