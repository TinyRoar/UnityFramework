using TinyRoar.Framework;
using UnityEngine;

public class OpenWebpageButton : BaseButton
{
    [SerializeField]
    private string URL = "http://www.tinyroar.de";

    protected override void ButtonAction()
    {
        Application.OpenURL(URL);
    }
}
