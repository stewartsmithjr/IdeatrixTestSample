
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;
using UnityEngine.Tilemaps;
public class LoginAuth : MonoBehaviour
{
    [SerializeField] Button startButton;

    [SerializeField] TMP_InputField firstNameField;
    [SerializeField] TMP_InputField lastNameField;
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Button signInButton;
    [SerializeField] TextMeshProUGUI textMeshProUGUI;

    
    string email;
    string password;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.interactable = false;
        startButton.onClick.AddListener(LoadPlayScene);
        firstNameField.onEndEdit.AddListener(delegate { ValueChange(firstNameField); });
        lastNameField.onEndEdit.AddListener (delegate { ValueChange(lastNameField); });
        emailField.onEndEdit.AddListener(delegate { ValueChange(emailField); });
        passwordField.onEndEdit.AddListener(delegate { ValueChange(passwordField); });  
        DontDestroyOnLoad(gameObject);

        email = PlayerPrefs.GetString("Email");
        password = PlayerPrefs.GetString("Password");
        auth= Firebase.Auth.FirebaseAuth.DefaultInstance;
        SignInUser();
        auth.StateChanged += Auth_StateChanged;
        
    }

    private void Auth_StateChanged(object sender, System.EventArgs e)
    {
      emailField.interactable = false;
        passwordField.interactable = false;
        textMeshProUGUI.gameObject.SetActive(true);
        startButton.interactable = true;
    }

    async void LoadPlayScene()
    {
        await SceneManager.LoadSceneAsync("ChatScene", LoadSceneMode.Single);
    }

    void ValueChange(TMP_InputField input)
    {

        if (input.text.Length > 0)
        {
           if(input == firstNameField)
            {
                PlayerPrefs.SetString("FirstName",input.text);

                input.interactable = false;
            }
           else if (input == lastNameField)
            {
                PlayerPrefs.SetString ("LastName",input.text);
                input.interactable = false;
            }
           else if(input == emailField)
            {
                if(!input.text.Contains("@") && !input.text.Contains("."))
                {
                    input.text = "Invalid Email Try Again";
                    Debug.LogError("Invalid Email Try Again");
                    return;
                }
                email = input.text;
                PlayerPrefs.SetString("Email", input.text);
                input.interactable = false;
            }
           else if((input == passwordField))
            {
                if(input.text.Length > 12)
                {
                    input.text = ("Password is too long must be less than 12 characters");
                    Debug.LogError("Password is too long must be less than 12 characters");
                    return;
                }
                password = input.text;
                PlayerPrefs.SetString("Password", input.text);
                input.interactable = false;


            }
        }
        else if (input.text.Length == 0)
        {
           
            Debug.Log("Main Input Empty");
        }
        if(email != null && password != null)
        {
            PlayerPrefs.SetString("Email", email);
            PlayerPrefs.SetString("Password", password);
            CreateUserSignIn();
        }
    }
    Firebase.Auth.FirebaseAuth auth;
     async void CreateUserSignIn()
    {
         
        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
        SignInUser();
    }
    async void SignInUser()
    {
        Firebase.Auth.Credential credential =
     
        Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
        
        await auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });


    }
 
}
