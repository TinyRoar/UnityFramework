using UnityEngine;
 
public class DontDestroyOnLoad : MonoBehaviour
{

    // Initialisation
    public void Awake() {
        DontDestroyOnLoad(this);
    }
 
}