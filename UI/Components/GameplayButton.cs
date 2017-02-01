using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

public class GameplayButton : BaseButton
{
    [SerializeField]
    private GameplayStatus Status;

    [SerializeField]
    private GameplayStatus OrToggleWithStatus;

    protected override void ButtonAction()
    {
        if(Events.GameplayStatus == Status)
            Events.GameplayStatus = OrToggleWithStatus;
        else
            Events.GameplayStatus = Status;
    }

}
