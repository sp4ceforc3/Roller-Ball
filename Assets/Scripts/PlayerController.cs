using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] float rayLength = 0.55f;
    [SerializeField] float speed = 5f;
    Rigidbody rb;
    
    // Start is called before the first frame update
    public InputActions playerControls;

    // Level Manager
    [SerializeField] LevelManager levelManager;

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
        if (levelManager.gameState != LevelManager.GameState.Playing) {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material.color = Color.clear; //this will immediately change the color to the transparent.. and only for that specific object
        } else {
            moveVector = playerControls.Player.Move.ReadValue<Vector3>();
        }
        
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
        }
    }

    // Detect and handle collision with other objects
    private void OnCollisionEnter(Collision collision) {
        if (levelManager.gameState == LevelManager.GameState.Playing && !godmode) {
            switch (collision.gameObject.tag)
            {
                //** Ground
                case nameof(LevelManager.CollisionObjects.Ground):
                    levelManager.LoadEndScreen(LevelManager.GameState.Lose);
                    break;

                //** Items
                case nameof(LevelManager.CollisionObjects.Item):
                    levelManager.HandleItem(collision.gameObject);
                    break;

                //** Obstacle
                case nameof(LevelManager.CollisionObjects.Obstacle):
                    levelManager.LoadEndScreen(LevelManager.GameState.Lose);
                    break;

                //** Finish
                case nameof(LevelManager.CollisionObjects.Goal):
                    if (levelManager.cntItems == levelManager.totalCntItems)
                        levelManager.LoadEndScreen(LevelManager.GameState.Won);
                    break;

                default:
                    // Do nothing. E.g. at ceiling collision
                    break;
            }
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
