public class PlayerMover : MonoBehaviour
{
    //Distinguishing each client 
    private PlayerInput playerInput; 
    private CharacterController controller;
    private Animator anim;

    [Header("Pertaining to Movement")]
    private Vector3 moveDir;
    [SerializeField]private float moveSpeed;
    [Range(0f, 5f)]
    [SerializeField] private float ySpeed;
    [SerializeField] private bool isGround;
    [SerializeField] private bool isRunning;

    [Header("Pertaining to Movement ability")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] private float jumpSpeed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        //moveSpeed = 3f; 
        jumpSpeed = 3f;
        moveSpeed = 3; 
    }

    private void OnDisable() 
    { 
        anim.Rebind();
    }

    private void Update()
    {
        Move();
        Jump(); 
    }
    private void FixedUpdate()
    {
        GroundCheck();
    }

    private void Move()
    {
        //로컬 기준 움직임 
        if (moveDir.magnitude <= 0) // 안움직임 
        {
            moveSpeed = Mathf.Lerp(moveSpeed, 0, 0.5f);
            if (moveSpeed <= 0.1) 
                moveSpeed = 0; 
        }
        else if (isRunning)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, 0.5f);
        }
        else 
        {
            //default movement
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, 0.5f);
        }
        controller.Move(transform.forward *  moveDir.z * moveSpeed * Time.deltaTime);
        controller.Move(transform.right * moveDir.x * moveSpeed * Time.deltaTime);

        anim.SetFloat("ZSpeed", moveDir.z, 0.1f, Time.deltaTime);
        anim.SetFloat("XSpeed", moveDir.x, 0.1f, Time.deltaTime);
        anim.SetFloat("Speed", moveSpeed); 
    }

    private void OnMove(InputValue value)
    {
        
        Vector2 input = value.Get<Vector2>();

        moveDir = new Vector3(input.x, 0, input.y);
    }

    private void Jump()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (GroundCheck() && ySpeed < 0)
        {
            ySpeed = 0;
        }
        // 2. if accerlation == gravity 
        if (ySpeed <= Physics.gravity.y)
            ySpeed = Physics.gravity.y; 

        controller.Move(Vector3.up * ySpeed * Time.deltaTime);
    }
    private void OnJump(InputValue value)
    {
        if (GroundCheck())
        ySpeed = jumpSpeed; 
    }
    RaycastHit groundHit;
    private bool GroundCheck()
    {
        isGround = Physics.SphereCast(transform.position + Vector3.up * 1, 0.5f, Vector3.down, out groundHit, 0.7f);
        return isGround; 
    }
    private void OnRun(InputValue value)
    {
        isRunning = value.isPressed;
    }
}