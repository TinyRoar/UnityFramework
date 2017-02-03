using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

public class AudioAnimationEvent : MonoBehaviour
{
    public void PlayAudio(string clip)
    {
        SoundManager.Instance.Play(clip, SoundManager.SoundType.Soundeffect);
    }
}
