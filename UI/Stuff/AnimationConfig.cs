using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
public class AnimationConfig
{
    public Animator Object;
    [FormerlySerializedAs("AnimationName")]
    public string State;

}
