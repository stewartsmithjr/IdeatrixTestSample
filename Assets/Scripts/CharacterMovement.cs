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
    Quaternion rotation;Vector3 gravity;
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
        FixedMovement();
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

       


       

        Animator animator = transform.GetComponent<Animator>();

       
       

        rotation = Quaternion.LookRotation(moveDirection);

        targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.fixedDeltaTime * inputAmount * 10);
        transform.rotation = targetRotation;
        animator.SetFloat(MoveFloatName, inputAmount, 0.2f, Time.fixedDeltaTime);
        
    }
    void FixedMovement()
    {

        Animator animator = GetComponent<Animator>();
        // if not grounded , increase down force
        animator.SetBool("IsGrounded", (FloorRaycasts(0, 0, rayLengthMuliplier) != Vector3.zero));
        if (FloorRaycasts(0, 0, rayLengthMuliplier) == Vector3.zero)
        {
            gravity += Vector3.up * Physics.gravity.y * Time.fixedDeltaTime;
        }
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        // find the Y position via raycasts
        floorMovement = new Vector3(rigidbody.position.x, FindFloor().y , rigidbody.position.z);



        // only stick to floor when grounded
        if ((FloorRaycasts(0, 0, rayLengthMuliplier) != Vector3.zero && floorMovement != rigidbody.position))
        {
            animator.SetBool("IsGrounded", true);
            rigidbody.MovePosition(floorMovement);

            gravity.y = 0;
        }

        
            rigidbody.linearVelocity = (moveDirection * (float)5 * inputAmount) + gravity;


         



        if (animator.GetBool("IsGrounded") == true)
        {
            gravity.y = 0;

        }
    }
    Vector3 CombinedRaycast;
    Vector3 raycastFloorPos;
    Vector3 floorMovement;

    Vector3 FindFloor()
    {
        // width of raycasts around the centre of your character
        float raycastWidth = 0.25f;
        // check floor on 5 raycasts   , get the average when not Vector3.zero  
        int floorAverage = 1;

        CombinedRaycast = FloorRaycasts(0, 0, rayLengthMuliplier);
        floorAverage += (getFloorAverage(raycastWidth, 0) + getFloorAverage(-raycastWidth, 0) + getFloorAverage(0, raycastWidth) + getFloorAverage(0, -raycastWidth));

        return CombinedRaycast / floorAverage;
    }

    // only add to average floor position if its not Vector3.zero
    int getFloorAverage(float offsetx, float offsetz)
    {

        if (FloorRaycasts(offsetx, offsetz, rayLengthMuliplier) != Vector3.zero)
        {

            CombinedRaycast += FloorRaycasts(offsetx, offsetz, rayLengthMuliplier);
            return 1;
        }
        else { return 0; }
    }
    [SerializeField] LayerMask water;
    [SerializeField] float rayLengthMuliplier;
    Vector3 FloorRaycasts(float offsetx, float offsetz, float raycastLength)
    {

        RaycastHit hit;
        // move raycast
        raycastFloorPos = transform.TransformPoint(0 + offsetx, 0 + 0.5f, 0 + offsetz);

        Debug.DrawRay(raycastFloorPos, Vector3.down, Color.magenta);



        if (Physics.Raycast(raycastFloorPos, -Vector3.up, out hit, raycastLength))
        {
            return hit.point;
        }


        else
        {
            return Vector3.zero;
        }

    }

}
