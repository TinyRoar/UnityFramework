using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace TinyRoar.Framework
{
    [Serializable]
    public class AnimationConfig
    {
        public Animator Object;
        [FormerlySerializedAs("AnimationName")] public string State;

    }
}
