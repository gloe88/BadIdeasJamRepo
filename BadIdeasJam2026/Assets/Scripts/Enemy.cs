using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
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
    }

    private void Update()
    {
        if (following) transform.position = Vector2.MoveTowards(transform.position, player.position, spd * Time.deltaTime);
        else if (!following && Vector2.Distance(transform.position, player.transform.position) < detDist) following = true;
    }

    private void Reset()
    {
        transform.position = initPos;
        following = false;
        rb.linearVelocity = Vector2.zero;
    }
}
