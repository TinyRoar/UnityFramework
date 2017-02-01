using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TinyRoar.Framework
{
    public class LayerConfig : MonoBehaviour
    {
        public List<AnimationConfig> OpenAnimations;
        public List<AnimationConfig> CloseAnimations;
        //public float Delay = 0.5f;
        public string OpenSound = "";
        public string CloseSound = "";

    }
}
