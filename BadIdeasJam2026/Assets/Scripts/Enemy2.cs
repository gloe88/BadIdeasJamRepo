using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    private Transform player;
    private Vector2 initPos;
    private Vector2 raisePos;
    private float detDist = 2f;
    private float spd = 1f;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        Player.resetLevel.AddListener(Reset);
        initPos = transform.position;
        raisePos = new Vector2(initPos.x, initPos.y + 2f);
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < detDist) transform.position = Vector2.Lerp(transform.position, raisePos, spd * Time.deltaTime);
        else if (Vector2.Distance(transform.position, initPos) > 0.1f) transform.position = Vector2.Lerp(transform.position, initPos, spd * Time.deltaTime);
    }

    private void Reset()
    {
        transform.position = initPos; //might add this all to animations later instead?
    }
}
