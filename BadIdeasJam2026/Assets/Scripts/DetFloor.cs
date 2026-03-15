using UnityEngine;

public class DetFloor : MonoBehaviour
{
    private bool onWall = false; //can use this for floor also?
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Floor"))
        {
            onWall = true;
        }
    }

    public bool GetOnWall()
    {
        return onWall;
    }
}
