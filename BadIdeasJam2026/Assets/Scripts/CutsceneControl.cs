using UnityEngine;
using UnityEngine.Video;

public class CutsceneControl : MonoBehaviour
{
    VideoPlayer vid;
    private void Start()
    {
        vid = GetComponent<VideoPlayer>();
        vid.loopPointReached += SwitchToGame;
    }

    private void SwitchToGame(VideoPlayer vp)
    {
        MainMenu.StartGame();
    }
}
