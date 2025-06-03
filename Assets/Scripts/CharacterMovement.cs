using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        action = new InputSystem_Actions();

        moveInput = action.Player.Move;
        moveInput.Enable();
        rotateInput = action.Player.Look;
        rotateInput.Enable();
    }
    private void OnDisable()
    {
        moveInput.Disable();
        rotateInput.Disable();

    }
    void Start()
    {
        characterController = GetComponent<NetworkCharacterController>();
    }
    private NetworkCharacterController characterController;

    public string MoveFloatName;
    Quaternion targetRotation;
    Quaternion rotation;float gravity;
    private InputAction moveInput;
    private InputSystem_Actions action;
    private InputAction rotateInput;
    Vector3 moveDirection;
    float vertical;
    float horizontal;
    float inputAmount;
    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();




        Movement();
    }


    void Movement()
    {
        moveDirection = Vector3.zero;
        inputAmount = 0;
        vertical = moveInput.ReadValue<Vector2>().x;
        horizontal = moveInput.ReadValue<Vector2>().y;

        //using new InputHere!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        Camera mainCamera = GetComponentInChildren<Camera>();
       
        // normalize so diagonal movement isnt twice as fast, clear the Y so your character doesnt try to
        // walk into the floor/ sky when your camera isn't level
        Vector3 correctedVertical = vertical * mainCamera.transform.right;
        Vector3 correctedHorizontal = horizontal * mainCamera.transform.forward;
        Vector3 combinedInput = correctedHorizontal + correctedVertical;

        float inputMag = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

        inputAmount = Mathf.Clamp01(inputMag);
        moveDirection = new Vector3((combinedInput).normalized.x, 0, (combinedInput).normalized.z);

        moveDirection.y += Physics.gravity.y * Time.deltaTime ;



        gravity += Time.deltaTime * Physics.gravity.y;

        Rigidbody rigidbody = GetComponent<Rigidbody>();

        Animator animator = transform.GetComponent<Animator>();

       
        rigidbody.linearVelocity = -( moveDirection * gravity * Runner.DeltaTime);

        rotation = Quaternion.LookRotation(moveDirection);

        targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * inputAmount * 10);
        transform.rotation = targetRotation;
        animator.SetFloat(MoveFloatName, inputAmount, 0.2f, Time.fixedDeltaTime);
        
    }
}
