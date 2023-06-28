using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    Rigidbody rb;
    // Start is called before the first frame update
    public InputActions playerControls;

    Vector3 moveVector;
    private void Awake()
    {
        playerControls = new InputActions();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        //playerControls.Player.Jump.performed += _ => ;
        //playerControls.Player.GodMode.performed += _ => ToggleGodMode();

        playerControls.Enable();
    }

    private void OnDisable() => playerControls.Disable();

    private void FixedUpdate()
    {
        moveVector = playerControls.Player.Move.ReadValue<Vector3>();
        
        rb.velocity = speed * moveVector;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
