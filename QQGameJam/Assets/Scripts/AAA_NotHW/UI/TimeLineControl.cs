using UnityEngine;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    public PlayableDirector director;
    public void PauseTL()
    {
        if (director)
        {
            Debug.Log("not director"); director.Pause(); }
    }
    public void PlayTL() { if (director) director.Play(); }

    
}
