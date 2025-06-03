using UnityEngine;
using System.Collections;
using System;
using TMPro;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using OpenAI_API.Models;
using Fusion;
using Firebase.Auth;

public class OpenAiController : MonoBehaviour
{
    //created with the assistance of Youtuber Immersive Limits video "how to use ChatGPT in Unity - Simple Tutorial"

    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Chats
    {
        public User UserInfo { get; set; }
        public string role {  get; set; }
        public string userResponse { get; set; }
        public string aiResponse { get; set; }
    }
    public TMP_Text textField;

    

    private OpenAIAPI aPI;
    private List<ChatMessage> messages;

    bool ableToSendChattoAi = true;
    string chatMessageRoleString = "You're a imaginary friend that knows a lot.";
    [SerializeField] GameObject boxPrefab;
    [SerializeField] GameObject responderPrefab;
    [SerializeField] float deceleration;
    int responses = 0;
    GameObject activeResponseAvatar;
    string response;
    string name = "Stewart";
    public Vector3 aiDirection;
    public Transform playerPos;
    void Start()
    {
        aPI = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY",EnvironmentVariableTarget.User));
        //whenn does this it start

    }
    public void StartConversation(  string starterString)
    {
        if (messages==null)
        {


            messages = new List<ChatMessage> { new ChatMessage(ChatMessageRole.System, chatMessageRoleString) };
            textField.text = starterString;
            Debug.Log(starterString);
        }
    }
    public async void GetResponce(string messageToChat)
    {

        if (messageToChat.Length < 1)
        {
            return;
        }

        //Disable ability to send spam messages
        ableToSendChattoAi = false;

        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.TextContent = messageToChat;
        if (userMessage.TextContent.Length > 100)
        {
            //limit message to 100 chars test if whitespace counts
            userMessage.TextContent = userMessage.TextContent.Substring(0, 100);
        }
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.TextContent));


        messages.Add(userMessage);

        //update textfield with user message
        textField.text = string.Format("You: {0}", userMessage.TextContent);


        messageToChat = "";

        var chatResult = await aPI.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo, //model we want to use
            Temperature = 0.1, //must be between 0-1 and apparently gauges how creative responses will be from the language model (0 = confident/right, 1 = creative and wrong)
            MaxTokens = 50, // limits to how much back we get from language model
            Messages = messages
        });


        //get response message

        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.TextContent = chatResult.Choices[0].Message.TextContent;

        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.TextContent));

        //add resonse to message list
      
        messages.Add(responseMessage);
        //update text field

        textField.text = string.Format("You: {0}: \n\n Other Guy: {1}", userMessage.TextContent, responseMessage.TextContent);
        responses++;

        response = responseMessage.TextContent;
        if (responses == 1)
        {
            CreateResponseAvatar(responseMessage.TextContent, playerPos.position);
        }
        float distanceBetween = Vector3.Distance(activeResponseAvatar.transform.position, playerPos.position);

        if (Math.Abs(distanceBetween) > 10)
        {
            CreateResponseAvatar(responseMessage.TextContent, playerPos.position);
        }
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser firebaseUser = auth.CurrentUser;
        User user = new User { Id = firebaseUser.UserId, FirstName = PlayerPrefs.GetString("FirstName"), LastName = PlayerPrefs.GetString("LastName") };
        Chats responseDB = new Chats { userResponse = userMessage.TextContent, aiResponse = responseMessage.TextContent, role = chatMessageRoleString, UserInfo = user };
        var userData = JsonUtility.ToJson(user);
        var data = JsonUtility.ToJson(responseDB);

        await PlayerSaveScript.Singleton().SaveData(data);

    }


    async void CreateResponseAvatar(string response , Vector3 playerPos)
    {
        Vector3 resposeSpawmPos = new Vector3(playerPos.x+6,playerPos.y,playerPos.z+6);

        Quaternion responseIniitalLook = Quaternion.LookRotation(playerPos - resposeSpawmPos);

        GameObject responseModel = Instantiate(responderPrefab, resposeSpawmPos, responseIniitalLook);

        
       TMP_Text tMP_Text = responseModel.GetComponentInChildren<TMP_Text>();

        tMP_Text.text = response;

        activeResponseAvatar = responseModel;
        await Awaitable.WaitForSecondsAsync(5);

        
        ableToSendChattoAi = true;

    }
    private void Update()
    {
        if (activeResponseAvatar != null)
        {
           
            activeResponseAvatar.GetComponentInChildren<TMP_Text>().text = response;
        }



    }
   


   
    #region SteeringBehaviours
    Vector3 Seek( Transform avatarPos)
    {
        Vector3 desiredVelocity = (playerPos.position - avatarPos.position).normalized * 5;

        return desiredVelocity - avatarPos.GetComponent<CharacterController>().velocity;
    }

    Vector3 Arrive( Transform avatarPos) 
    {
        Vector3 toTarget = playerPos.position - avatarPos.position;

        float distance = toTarget.magnitude;

        if(distance > 0.5f)
        {
            float decel = 0.3f;

            float speed = distance / decel * deceleration;

            speed = Mathf.Clamp(speed, 0, 5);

            Vector3 desiredVelocity = toTarget * speed / distance;

            return desiredVelocity - avatarPos.position;
        }

        return Vector3.zero;
    }

    Vector3 Pursue(Transform avatarPos)
    {
        Vector3 toPlayer = playerPos.position - avatarPos.position;

        float relativeHeading  = Vector3.Dot(avatarPos.forward,playerPos.forward);

        if(Vector3.Dot(toPlayer,avatarPos.forward)>0 && relativeHeading < -0.95)
        {
            return Seek(avatarPos);
        }

        float lookaheadTime = toPlayer.magnitude / 5 + playerPos.GetComponent<CharacterController>().velocity.magnitude;

        return Seek(avatarPos) + playerPos.GetComponent<CharacterController>().velocity * lookaheadTime;
    }

     Vector3 Flee(Transform avatarPos)
    {
        Vector3 desiredVelocity = ((avatarPos.position - playerPos.position) * 5).normalized;

        return desiredVelocity - avatarPos.GetComponent<Rigidbody>().linearVelocity;

    }


    #endregion


}
