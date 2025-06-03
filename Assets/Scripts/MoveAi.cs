using UnityEngine;
using System;

public class MoveAi : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    OpenAiController openAiController;
    [SerializeField] string floatName;
    void Start()
    {
        openAiController = GameObject.FindFirstObjectByType<OpenAiController>();
    }

    
    void Update()
    {
      
       
        Quaternion quaternion = Quaternion.LookRotation(openAiController.playerPos.position - transform.position);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation,quaternion , Time.fixedDeltaTime );
        transform.rotation = targetRotation;

    }
}
