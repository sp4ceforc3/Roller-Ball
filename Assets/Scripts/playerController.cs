using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayLength = 0.55f;
    [SerializeField] float speed = 5f;
    Rigidbody rb;
    
    // Start is called before the first frame update
    public InputActions playerControls;

    private bool jump = false;
    private bool godmode = false;
    private int jumpCnt = 0;
    private GameObject player;

    Vector3 moveVector;
    private void Awake()
    {
        playerControls = new InputActions();
        rb = GetComponent<Rigidbody>();
        player = this.gameObject;
    }

    private void OnEnable()
    {
        playerControls.Player.Jump.performed += _ => {jump = true;};
        playerControls.Player.GodMode.performed += ToogleGodMode;

        playerControls.Enable();
    }

    private void OnDisable() => playerControls.Disable();

    private void FixedUpdate()
    {
        moveVector = playerControls.Player.Move.ReadValue<Vector3>();
        
        if(!godmode)
        {
            //Debug.Log(player.transform.position);
            rb.AddForce(moveVector.x * speed, 0f, moveVector.z * speed);
            var newVelocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0f, rb.velocity.z), 5f);
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
            
            // This sends a ray from the center of the ball downwards, so that we can check if the ball is on the ground
            if (Physics.Raycast(transform.position, Vector3.down, rayLength, groundMask))
                jumpCnt = 0;
        }else
        {
            moveVector.Normalize();
            gameObject.transform.position +=  moveVector * speed * Time.deltaTime;;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!godmode)
        {
            if (jump) {
                if(jumpCnt < 1)
                {
                    jumpCnt += 1;
                    // Setting velocity directly makes sure that each jump is the same, regardless of current velocity
                    rb.velocity = new Vector3(rb.velocity.x, 5f, rb.velocity.z); 
                }                
                jump = false;
            }
            if(player.transform.position.y < -5)
                SceneManager.LoadScene("SampleScene");

        }  
    }

    private void ToogleGodMode(InputAction.CallbackContext _) { 
        godmode = !godmode;
        rb.isKinematic = !rb.isKinematic;
        rb.useGravity = !rb.useGravity;
    }

    // This will draw the ray into the scene. This is only visible in the Editor and not in the final game
    private void OnDrawGizmos() => Gizmos.DrawRay(transform.position, Vector3.down * rayLength);
}
