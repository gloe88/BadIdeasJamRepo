using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private float spd = 5f; //probably can't be serialized if it's a prefab, unless there's a larger enemy controller
    private float detDist = 6f;
    private Vector2 initPos;
    private bool following = false;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        initPos = transform.position;
        Player.resetLevel.AddListener(Reset);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (following)
        {
            anim.SetBool("attacking", true);
            transform.position = Vector2.MoveTowards(transform.position, player.position, spd * Time.deltaTime);
            transform.up = player.position - transform.position; //got help from somewhere for this line but didn't save it?
        }
        else if (!following && Vector2.Distance(transform.position, player.transform.position) < detDist) following = true;
    }

    private void Reset()
    {
        transform.position = initPos;
        following = false;
        rb.linearVelocity = Vector2.zero;
        transform.up = Vector2.up;
        anim.SetBool("attacking", false);
    }
}
