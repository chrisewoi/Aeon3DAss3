using UnityEngine;

//The RequireComponent attribute checks a GameObject for the defined components, and adds them if they are missing.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CameraSelector))]
public class CustomController : MonoBehaviour
{
    //This enumerator will define all our possible player states
    public enum State
    {
        Walk,
        Rise,
        Fall
    }

    //The Tooltip attribute displays text in the Unity inspector when hovering the mouse over this variable
    [Tooltip("To track the current behaviour of the player")]
    //The SerializeField attribute makes a private variable visible and editable in the Unity inspector.
    [SerializeField] private State currentState;

    [Tooltip("How many units per sec the player should move by default")]
    [SerializeField] private float speedWalk;

    [Tooltip("How much upward momentum to start with when jumping")]
    [SerializeField] private float jumpPower;

    [Tooltip("Reduce the player's vertical momentum by this many units per second")]
    [SerializeField] private float gravity;

    [Tooltip("What physics layer should the player object recognise as the ground")]
    [SerializeField] private LayerMask groundLayer;

    //To hold the rigidbody on the player object
    private Rigidbody rb;

    //To hold the camera selector component on the player object
    private CameraSelector cameraSelector;

    //To hold the player's collider
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        //Get the rigidbody component from the player object
        rb = GetComponent<Rigidbody>();

        //Prevent the rigidbody from rotating
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        //This script manages gravity its own way
        rb.useGravity = false;

        //Get the camera selector component from the player object
        cameraSelector = GetComponent<CameraSelector>();

        //Get the capsule collider component from the player object
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        //Depending on our current state, choose a different set of behaviour to follow
        switch (currentState)
        {
            case State.Walk:
                WalkState();
                break;
            case State.Rise:
                RiseState();
                break;
            case State.Fall:
                FallState();
                break;
        }
    }

    private void WalkState()
    {
        //Get a movement direction based on inputs and translated using the camera direction
        Vector3 inputMovement = GetMovementFromInput();

        //increase that using our base walk speed 
        inputMovement *= speedWalk;

        //Adjust our up/down speed based on gravity,
        //but since we're walking, we shouldn't fall
        inputMovement.y = Mathf.Clamp(rb.velocity.y - gravity * Time.deltaTime, 0f, float.PositiveInfinity);


        //Apply the movement we determined to our rigidbody
        rb.velocity = inputMovement;

        //If we are no longer on the ground...
        if (!IsGrounded())
        {
            //... we should be falling
            currentState = State.Fall;

            //return means "stop here"
            return;
        }

        //If we make it here, we must be on the ground.

        //If the player presses jump...
        if (Input.GetButton("Jump"))
        {
            //... add upwards momentum to the player and change to the Rise state 
            RiseAtSpeed(jumpPower);
        }
    }

    private void RiseState()
    {
        //Set movement based on input direction, camera direction, and walking speed
        Vector3 inputMovement = GetMovementFromInput();
        inputMovement *= speedWalk;

        //Apply gravity
        inputMovement.y = rb.velocity.y - gravity * Time.deltaTime;

        //Apply the determined movement to our rigidbody
        rb.velocity = inputMovement;

        //if velocity.y is less than 0, we are moving down, so we should enter Fall state
        if (rb.velocity.y < 0f)
        {
            currentState = State.Fall;
        }
    }

    private void FallState()
    {
        //Set movement based on input, camera, walk speed, and apply gravity
        Vector3 inputMovement = GetMovementFromInput();
        inputMovement *= speedWalk;
        inputMovement.y = rb.velocity.y - gravity * Time.deltaTime;

        //Apply movement to the rigidbody
        rb.velocity = inputMovement;

        //If we are on the ground..
        if (IsGrounded())
        {
            //Change to the Walk state
            currentState = State.Walk;
        }
    }

    private void RiseAtSpeed(float speed)
    {
        //Set our vertical momentum upward using the provided speed 
        rb.velocity = new Vector3(rb.velocity.x, speed, rb.velocity.z);

        //Change to the Rise state
        currentState = State.Rise;
    }
    
    /// <summary>
    /// Get current inputs and translate to a movement direction
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMovementFromInput()
    {
        //Get a local Vector2,                              | local - the variable cannot be called on by other methods or scripts, and the value is lost at the end of the function
        //constructing a new one                            | "new Vector2()" is a constructor. We can give the constructor values to set starting x and y values, e.g. "new Vector2(0.2f, -1f)"
        //using our horizontal and vertical input axes      | "Input.GetAxis()" looks for an axis in the Input Manager with the name provided
        Vector2 inputThisFrame = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Get a local Vector3, constructing a new one using the inputs.
        //Since we're moving in 3D space, we need to convert the "Up/Down" input (the y axis), to a "Forward/Back" input (the Z axis)
        Vector3 moveDirection = new Vector3(inputThisFrame.x, 0, inputThisFrame.y);

        //Get the transform of the currently active camera
        Transform cameraTransform = cameraSelector.GetCameraTransform();

        //translate the movement direction based on the camera's transform
        moveDirection = cameraTransform.TransformDirection(moveDirection);

        //return that result
        return moveDirection;
    }

    private bool IsGrounded()
    {
        //Raycast downwards from our centre, using half of our collider's height
        return Physics.Raycast(transform.position, Vector3.down, capsuleCollider.height / 2f + 0.01f, groundLayer);
    }
}
