using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float spd;
    [SerializeField] float jumpSpd;
    private InputActionAsset input;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        input = GetComponent<PlayerInput>().actions;

        input.FindAction("Jump").performed += Jump;
    }

    void Update()
    {
        Vector2 moveDir = input.FindAction("Move").ReadValue<Vector2>();
        rb.linearVelocityX = moveDir.x * spd * Time.deltaTime;

        if (input.FindAction("Slide").IsPressed()) col.size = new Vector2(1f, 0.5f);
        else col.size = new Vector2(1f, 1f);
    }

    void Jump(InputAction.CallbackContext action)
    {
        rb.AddForce(transform.up * jumpSpd * Time.deltaTime);
    }
}
