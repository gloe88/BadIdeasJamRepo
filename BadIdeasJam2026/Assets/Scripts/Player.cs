using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Security.Cryptography;

public class Player : MonoBehaviour
{
    //events
    public static UnityEvent resetLevel = new UnityEvent();

    //adjustable values
    [SerializeField] float spd;
    [SerializeField] float jumpSpd;
    [SerializeField] int wallDetect;
    [SerializeField] float goingFast;
    [SerializeField] float camAdj;

    //references
    private InputActionAsset input;
    private Rigidbody2D rb;
    private SpriteRenderer spr;
    private Animator anim;
    private CinemachineFollow cam;

    //move variables
    private Vector2 moveDir;
    private bool onGround = false;
    private bool onWall = false;
    private bool sliding = false;
    private float wallDist = 0f;

    //constants ig
    private Vector3 initPos;
    private float initDamp;
    private Vector3 initCamOffset;


    private void Start()
    {
        resetLevel.AddListener(Reset);

        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>().actions;
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cam = transform.GetChild(0).GetComponent<CinemachineFollow>();

        initPos = transform.position;
        initDamp = rb.linearDamping;
        initCamOffset = cam.TrackerSettings.PositionDamping;

        input.FindAction("Jump").performed += Jump;
        input.FindAction("Slide").canceled += Boost;
    }

    private void Update()
    {
        AnimControl();
        moveDir = input.FindAction("Move").ReadValue<Vector2>().normalized;

        if (input.FindAction("Slide").WasPressedThisFrame() && onGround)
        {
            sliding = true;
            rb.AddForce(transform.right * moveDir.x * spd * Time.fixedDeltaTime);
            rb.linearDamping = -3f;
        }

        onWall = CheckForWalls();
        onGround = CheckForFloor();

        AdjCam();
    }

    //actNum: 0 - idle; 1 - run; 2 - jump; 3 - fall; 4 - cling; 5 - slide
    private void AnimControl()
    {
        if (sliding) //neither this nor the next condition triggers
        {
            Debug.Log("slide is running");
            anim.SetInteger("actNum", 5);
            if (rb.linearVelocityX < 0f) spr.flipX = true;
            else if (rb.linearVelocityX > 0f) spr.flipX = false;
        }
        else if (onGround && moveDir.x != 0f)
        {
            Debug.Log("run is running");
            anim.SetInteger("actNum", 1);
            if (moveDir.x < 0f) spr.flipX = true;
            else if (moveDir.x > 0) spr.flipX = false;
        }
        if (onWall)
        {
            anim.SetInteger("actNum", 4);
            if (wallDist == 1f) spr.flipX = true;
            if (wallDist == -1f) spr.flipX = false;
        }
        else if (!onGround)
        {
            Debug.Log("this one is running for some reason");
            anim.SetInteger("actNum", 3);
            if (rb.linearVelocityX < 0f) spr.flipX = true;
            else if (rb.linearVelocityX > 0f) spr.flipX = false;
        }
        else
        {
            anim.SetInteger("actNum", 0);
        }
    }

    private void FixedUpdate()
    {
        if (!sliding) rb.AddForceX(moveDir.x * spd * Time.fixedDeltaTime);
    }

    private bool CheckForWalls()
    {
        if (onGround) return false; //idk if this will mess anythign up...

        //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 3), transform.right);
        RaycastHit2D rightCast = Physics2D.Raycast(transform.position, Vector3.right, wallDetect);
        RaycastHit2D rightCastU = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2), transform.right, wallDetect);
        RaycastHit2D rightCastD = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), transform.right, wallDetect);
        
        RaycastHit2D leftCast = Physics2D.Raycast(transform.position, -Vector3.right, wallDetect);
        RaycastHit2D leftCastU = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2), -transform.right, wallDetect);
        RaycastHit2D leftCastD = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), -transform.right, wallDetect);

        if (rightCast)
        {
            if (rightCast.transform.gameObject.tag.Equals("Floor"))
            {
                wallDist = -1f;
                return true;
            }
        }
        if (leftCast)
        {
            if (leftCast.transform.gameObject.tag.Equals("Floor"))
            {
                wallDist = 1f;
                return true;
            }
        }

        return false;
    }

    private bool CheckForFloor()
    {
        //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - transform.localScale.y), -transform.up);
        RaycastHit2D downCast = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y), -transform.up, wallDetect);
        if (downCast)
        {
            if (downCast.transform.gameObject.tag.Equals("Floor"))
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
        if (onGround)
        {
            rb.AddForce(transform.up * jumpSpd, ForceMode2D.Impulse);
        }
        else if (onWall) rb.AddForce((transform.up * jumpSpd) + (transform.right * jumpSpd * wallDist), ForceMode2D.Impulse);
    }

    private void Boost(InputAction.CallbackContext action)
    {
        rb.AddForce(transform.right * jumpSpd * moveDir, ForceMode2D.Impulse);
        sliding = false;
        rb.linearDamping = initDamp;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy")) resetLevel.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy")) resetLevel.Invoke(); //for trap enemies
        if (collision.gameObject.tag.Equals("Finish")) MainMenu.Credits();
    }

    private void Reset()
    {
        transform.position = initPos;
        rb.linearVelocity = Vector2.zero;
    }
}