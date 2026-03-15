using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Security.Cryptography;

public class Player : MonoBehaviour
{
    public static UnityEvent resetLevel = new UnityEvent(); //so that everything resets
    [SerializeField] float spd;
    [SerializeField] float jumpSpd;
    [SerializeField] int wallDetect;
    private InputActionAsset input;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer spr;
    private Animator anim;
    private bool onGround = false;
    private bool onWall = false;
    private float wallDist = 0f;
    private Vector2 moveDir;
    private CinemachineFollow cam;
    private Vector3 initPos;
    [SerializeField] float goingFast; //sort all these out later lol
    [SerializeField] float camAdj;
    private Vector3 initCamOffset;
    private bool jumping = false;
    //private Vector2 initColSize;
    //private Vector2 newColSize;
    private bool sliding = false;
    private float initDamp;

    private DetFloor detR;
    private DetFloor detL;
    private DetFloor detD;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        input = GetComponent<PlayerInput>().actions;
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        cam = transform.GetChild(0).GetComponent<CinemachineFollow>();
        initCamOffset = cam.TrackerSettings.PositionDamping;

        detR = transform.GetChild(1).GetComponent<DetFloor>();
        detL = transform.GetChild(2).GetComponent<DetFloor>();
        detD = transform.GetChild(3).GetComponent<DetFloor>();

        resetLevel.AddListener(Reset);

        //initColSize = col.size;
        //newColSize = new Vector2(initColSize.x, initColSize.y / 2f);

        input.FindAction("Jump").performed += Jump;
        input.FindAction("Slide").canceled += Boost;

        initPos = transform.position;

        initDamp = rb.linearDamping;
    }

    private void Update()
    {
        AnimControl();
        moveDir = input.FindAction("Move").ReadValue<Vector2>().normalized;

        if (input.FindAction("Slide").WasPressedThisFrame() && onGround)
        {
            //col.size = newColSize;
            sliding = true;
            rb.AddForce(transform.right * moveDir.x * spd * Time.fixedDeltaTime);
            rb.linearDamping = -3f;
        }
        //else if (input.FindAction("sliding").canceled) sliding = false;//col.size = initColSize; //needs a cooldown or some other limit

        onWall = CheckWalls(); //detR.GetOnWall() || detL.GetOnWall(); //idk if this will work...g
        onGround = detD.GetOnWall();
        //onWall = CheckForWalls();
        //onGround = CheckForFloor();

        AdjCam();
    }

    private bool CheckWalls()
    {
        if (detR.GetOnWall())
        {
            wallDist = -1f;
            return true;
        }
        else if (detL.GetOnWall())
        {
            wallDist = 1f;
            return true;
        }
        return false;
    }

    //actNum: 0 - idle; 1 - run; 2 - jump; 3 - fall; 4 - cling; 5 - slide
    private void AnimControl() //mb a little too much checking for every frame, idk. Also the wall jumping looks really off...
    {
        //if (onWall) anim.SetInteger("actNum", 4);
        //else if (jumping) anim.SetInteger("actNum", 2);
        //else if (rb.linearVelocityY != 0) anim.SetInteger("actNum", 3);
        //else if (sliding) anim.SetInteger("actNum", 5);
        //else if (moveDir.x != 0) anim.SetInteger("actNum", 1);
        //else anim.SetInteger("actNum", 0);

        //if (!onGround)
        //{
        //    if (onWall) anim.SetInteger("actNum", 4);
        //    else if (jumping) anim.SetInteger("actNum", 2);
        //    else if (rb.linearVelocityY != 0) anim.SetInteger("actNum", 3);

        //}
        //else if (sliding) anim.SetInteger("actNum", 5); //(col.size == newColSize)
        //else if (moveDir.x != 0) anim.SetInteger("actNum", 1);
        //else anim.SetInteger("actNum", 0);
    }

    private void FixedUpdate()
    {
        if (!sliding) rb.AddForceX(moveDir.x * spd * Time.fixedDeltaTime); //if (col.size != newColSize) 
        //if (onGround && moveDir.x != 0) anim.SetInteger("actNum", 1);
        //else if () anim.SetInteger("actNum", 0);
        //else if () anim.SetInteger("actNum", 3);
        if (moveDir.x < 0f) spr.flipX = true;
                else if (moveDir.x > 0) spr.flipX = false;
    }

    //private bool CheckForWalls() //idk if this is the most efficient way to do this but whatever
    //{
    //    Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 3), transform.right);
    //    RaycastHit2D rightCast = Physics2D.Raycast(transform.position, Vector3.right, wallDetect);
    //    RaycastHit2D rightCastU = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2), transform.right, wallDetect);
    //    RaycastHit2D rightCastD = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), transform.right, wallDetect);
    //    RaycastHit2D leftCast = Physics2D.Raycast(transform.position, -Vector3.right, wallDetect);
    //    RaycastHit2D leftCastU = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2), -transform.right, wallDetect);
    //    RaycastHit2D leftCastD = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), -transform.right, wallDetect);
        
    //    if (rightCast)
    //    {
    //        if (rightCast.transform.gameObject.tag.Equals("Floor"))
    //        {
    //            wallDist = -1f;
    //            return true;
    //        }
    //    }
    //    if (leftCast)
    //    {
    //        if (leftCast.transform.gameObject.tag.Equals("Floor")) //leftCast.transform.gameObject.tag.Equals("Wall") || 
    //        {
    //            wallDist = 1f;
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //private bool CheckForFloor()
    //{
    //    Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - transform.localScale.y), -transform.up);
    //    RaycastHit2D downCast = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y), -transform.up, wallDetect);
    //    if (downCast)
    //    {
    //        if (downCast.transform.gameObject.tag.Equals("Floor")) //leftCast.transform.gameObject.tag.Equals("Wall") || 
    //        {
    //            wallDist = 1f;
    //            return true;
    //        }
    //    }
    //    return false;
    //}

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
            jumping = true;
            rb.AddForce(transform.up * jumpSpd, ForceMode2D.Impulse);
        }
        else if (onWall) rb.AddForce((transform.up * jumpSpd) + (transform.right * jumpSpd * wallDist), ForceMode2D.Impulse);
        jumping = false;
    }

    private void Boost(InputAction.CallbackContext action)
    {
        rb.AddForce(transform.right * jumpSpd * moveDir, ForceMode2D.Impulse);
        sliding = false;
        rb.linearDamping = initDamp;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag.Equals("Floor")) onGround = true;
        if (collision.gameObject.tag.Equals("Enemy")) resetLevel.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision) //for trap enemies
    {
        if (collision.gameObject.tag.Equals("Enemy")) resetLevel.Invoke();
        if (collision.gameObject.tag.Equals("Finish")) MainMenu.Credits();
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag.Equals("Floor")) onGround = false;
    //}


    private void Reset()
    {
        transform.position = initPos;
        rb.linearVelocity = Vector2.zero;
    }
}