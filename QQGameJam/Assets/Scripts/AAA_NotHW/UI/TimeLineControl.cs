using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineControl : MonoBehaviour
{
    public PlayableDirector director;

    void Awake()
    {
        if (!director) director = GetComponent<PlayableDirector>();
    }

    public void PauseTL()
    {
        if (!director) return;
        Debug.Log($"[TL] Pause on {name}, time={director.time:F2}");
        director.Pause();
    }

    public void ResumeTL()
    {
        if (!director) return;
        Debug.Log($"[TL] Resume on {name}, time={director.time:F2}");
        director.Resume();
    }

    // 仅当需要从头播才用它
    public void PlayTL()
    {
        if (!director) return;
        Debug.Log($"[TL] Play(from start) on {name}");
        director.Play();
    }
}
