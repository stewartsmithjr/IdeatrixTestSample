using UnityEngine;
using TMPro;
using Fusion;
using Fusion.Sockets;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChatBox : NetworkBehaviour
{
    [SerializeField] private CanvasGroup overlayCanvas;
    [SerializeField] public CanvasGroup wrldCanvas;
  
    [SerializeField] private GameObject boxPrefab;
    [SerializeField]TMP_InputField inputField1;
    [SerializeField]TMP_InputField inputField2;
    Transform parent;
    string inputValue;
    NetworkRunner runner;

    InputAction chatIn;
    InputAction chatOut;
    private InputSystem_Actions inputActions;

    [SerializeField]Button askAi;
    OpenAiController openAiController;
    string aiRole;

    TMP_InputField activeInputField;

    [SerializeField] TMP_Text tmpText;
    bool chatopen = false;
    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        chatIn = inputActions.UI.ChatIn;
        chatOut = inputActions.UI.ChatOut;
        chatIn.Enable();
        chatOut.Enable();
        chatIn.performed += ChatIn;
        chatOut.performed += ChatOut;
    }

    private void ChatIn(InputAction.CallbackContext obj)
    {
        if(!chatopen && overlayCanvas.alpha== 0)
        {
            chatopen = true;

            overlayCanvas.DOFade(1, 0.5f);
        }
        

    }

    private void ChatOut(InputAction.CallbackContext obj)
    {
       if(chatopen && overlayCanvas.alpha ==1)
        {
            chatopen = false;
            overlayCanvas.DOFade(0, 0.5f);
        }
    }

    private void Start()
    {
        
        inputField1.onEndEdit.AddListener(delegate { ValueChange(inputField1); });
       
        runner = GameObject.FindFirstObjectByType<NetworkRunner>();
        wrldCanvas.alpha = 0f;
        overlayCanvas.alpha = 0f;
       chatopen = false;
        openAiController = GameObject.FindFirstObjectByType<OpenAiController>();
        activeInputField = inputField1;
        askAi.onClick.AddListener(AskChatGpt);

    }
    public override void FixedUpdateNetwork()
    {
        InputSystem.Update();
        openAiController.playerPos = gameObject.transform;
       
        if(activeInputField != inputField1 )
        {
            inputField1.gameObject.SetActive(false);
        }
        if(activeInputField != inputField2 )
        {
            inputField2.gameObject.SetActive(false);
        } 
        activeInputField.gameObject.SetActive(true);
    }

    void ValueChange(TMP_InputField input)
    {

        if (input.text.Length > 0)
        {
            wrldCanvas.DOFade(1, 0.5f);
            inputValue = input.text;
            SpawnBox(inputValue);
            Debug.Log("Text has been entered");

            if(input == inputField2)
            {
                openAiController.textField = tmpText;
                openAiController.StartConversation( inputValue);
                openAiController.GetResponce(inputValue);
            }
        }
        else if (input.text.Length == 0)
        {
            inputValue = null;
            Debug.Log("Main Input Empty");
        }
    }
    private void OnDisable()
    {
        inputField1.onEndEdit.RemoveListener(delegate { ValueChange(inputField1); });
        inputField2.onEndEdit.RemoveListener(delegate { ValueChange(inputField2); });
    }

    async void SpawnBox(string message)
    {
        
        Vector3 spawnPosition = wrldCanvas.transform.position;

       Vector3 reverse = new Vector3(0f, 0, 0f);

        NetworkObject networkBoxObject = runner.Spawn(boxPrefab, spawnPosition,Quaternion.identity);
        networkBoxObject.transform.parent = wrldCanvas.transform; 
        networkBoxObject.GetComponentInChildren<TextMeshProUGUI>().text = message;
        networkBoxObject.GetComponent<RectTransform>().localEulerAngles = reverse;

        await Awaitable.WaitForSecondsAsync(10);

        Destroy(networkBoxObject);

       if(inputValue == message)
        {
            wrldCanvas.DOFade(0f, 0.5f);
        }
    }
    
    int chats = 0;
    void AskChatGpt()
    {
        activeInputField = inputField2;
        ValueChange(inputField2);

        
    }


}
