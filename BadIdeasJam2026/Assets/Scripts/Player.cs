using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static UnityEvent resetLevel = new UnityEvent(); //so that everything resets
    [SerializeField] float spd;
    [SerializeField] float jumpSpd;
    [SerializeField] int wallDetect;
    private InputActionAsset input;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool onGround = false;
    private bool onWall = false;
    private float wallDist = 0f;
    private Vector2 moveDir;
    private CinemachineFollow cam;
    private Vector3 initPos;
    [SerializeField] float goingFast; //sort all these out later lol
    [SerializeField] float camAdj;
    private Vector3 initCamOffset;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        input = GetComponent<PlayerInput>().actions;

        input.FindAction("Jump").performed += Jump;
        input.FindAction("Slide").canceled += Boost;

        cam = transform.GetChild(0).GetComponent<CinemachineFollow>();

        initPos = transform.position;

        resetLevel.AddListener(Reset);
        initCamOffset = cam.TrackerSettings.PositionDamping;
    }

    private void Update()
    {
        moveDir = input.FindAction("Move").ReadValue<Vector2>().normalized;

        if (input.FindAction("Slide").IsPressed()) col.size = new Vector2(1f, 0.5f);
        else col.size = new Vector2(1f, 1f); //needs a cooldown or some other limit

        onWall = CheckForWalls();

        AdjCam();
    }

    private void FixedUpdate()
    {
        rb.AddForceX(moveDir.x * spd * Time.fixedDeltaTime);
    }

    private bool CheckForWalls() //idk if this is the most efficient way to do this but whatever
    {
        RaycastHit2D rightCast = Physics2D.Raycast(transform.position, Vector3.right, wallDetect);
        RaycastHit2D leftCast = Physics2D.Raycast(transform.position, -Vector3.right, wallDetect);
        if (rightCast)
        {
            if (rightCast.transform.gameObject.tag.Equals("Wall") || rightCast.transform.gameObject.tag.Equals("Floor"))
            {
                wallDist = -1f;
                return true;
            }
        }
        if (leftCast)
        {
            if (leftCast.transform.gameObject.tag.Equals("Wall") || leftCast.transform.gameObject.tag.Equals("Floor"))
            {
                wallDist = 1f;
                return true;
            }
        }
        return false;
    }

    private void AdjCam()
    {
        float camChangeSpd = 5f * Time.deltaTime;
        if (Mathf.Abs(rb.linearVelocity.x) > goingFast) cam.TrackerSettings.PositionDamping = Vector3.Lerp(cam.TrackerSettings.PositionDamping, new Vector3(0f, initCamOffset.y, initCamOffset.z), camChangeSpd);
        else cam.TrackerSettings.PositionDamping = cam.TrackerSettings.PositionDamping = Vector3.Lerp(cam.TrackerSettings.PositionDamping, initCamOffset, camChangeSpd);
    }

    private void Jump(InputAction.CallbackContext action)
    {
        if (onGround) rb.AddForce(transform.up * jumpSpd, ForceMode2D.Impulse);
        else if (onWall) rb.AddForce((transform.up * jumpSpd) + (transform.right * jumpSpd * wallDist), ForceMode2D.Impulse);
    }

    private void Boost(InputAction.CallbackContext action)
    {
        rb.AddForce(transform.right * jumpSpd * moveDir, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Floor")) onGround = true;
        if (collision.gameObject.tag.Equals("Enemy")) resetLevel.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision) //for trap enemies
    {
        if (collision.gameObject.tag.Equals("Enemy")) resetLevel.Invoke();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Floor")) onGround = false;
    }


    private void Reset()
    {
        transform.position = initPos;
    }
}