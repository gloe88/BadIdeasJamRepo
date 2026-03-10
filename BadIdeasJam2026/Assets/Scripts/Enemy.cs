using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private float spd = 3f; //probably can't be serialized if it's a prefab, unless there's a larger enemy controller
    private float detDist = 4f;
    private Vector2 initPos;
    private bool following = false;
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        initPos = transform.position;
        Player.resetLevel.AddListener(Reset);
    }

    private void Update()
    {
        if (following) transform.position = Vector2.Lerp(transform.position, player.position, spd * Time.deltaTime);
        else if (!following && Vector2.Distance(transform.position, player.transform.position) < detDist) following = true;
    }

    private void Reset()
    {
        transform.position = initPos;
        following = false;
    }
}
