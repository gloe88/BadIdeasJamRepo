using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    private Transform player;
    private Animator anim;
    private Vector2 initPos;
    private Vector2 raisePos;
    private float detDist = 3f;
    private float spd = 1f;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        Player.resetLevel.AddListener(Reset);
        initPos = transform.position;
        raisePos = new Vector2(initPos.x, initPos.y + 2f);
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < detDist) //the animations are lagging?
        {
            anim.SetBool("attacking", true);
            transform.position = Vector2.Lerp(transform.position, raisePos, spd * Time.deltaTime); //wait it doesn't go back down?
        }
        else if (Vector2.Distance(transform.position, initPos) > 0.1f)
        {
            anim.SetBool("attacking", false);
            transform.position = Vector2.Lerp(transform.position, initPos, spd * Time.deltaTime);
        }
    }

    private void Reset()
    {
        anim.SetBool("attacking", false);
        transform.position = initPos; //might add this all to animations later instead?
    }
}
