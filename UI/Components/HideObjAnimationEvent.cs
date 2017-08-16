using UnityEngine;
using System.Collections;

namespace TinyRoar.Framework
{
    public class HideObjAnimationEvent : MonoBehaviour
    {

        public void HideObj(string path)
        {
            //Debug.Log("HideObj");       
            Print.Log("HideObj");

            //Debug.Log(GameObject.Find(path));
            GameObject.Find(path).SetActive(false);
        }

    }
}
