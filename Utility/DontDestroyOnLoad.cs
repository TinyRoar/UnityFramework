using UnityEngine;

namespace TinyRoar.Framework
{
    public class DontDestroyOnLoad : MonoBehaviour
    {

        // Initialisation
        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

    }
}