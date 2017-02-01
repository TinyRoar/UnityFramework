using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenCloseButton : BaseButton
{
    // config
    public bool ExitGame = false;
    public GameEnvironment Environment;
    public bool UseBlend;
    public float Delay;
    public List<LayerEntry> ActionList;
    public List<AnimationConfig> Animations;

    // action
    protected override void ButtonAction()
    {
        if (ExitGame)
            Application.Quit();

        int count = ActionList.Count;
        for (var i = 0; i < count; i++)
        {
            LayerEntry layerEntry = ActionList[i];

            if(layerEntry.Layer == Layer.None || layerEntry.Action == UIAction.None)
                continue;

            UIManager.Instance.Switch(layerEntry.Layer, layerEntry.Action, Delay);
        }

        // Play Animations
        foreach (var anim in Animations)
        {
            if (anim.Object != null)
                anim.Object.Play(anim.State);
        }

        // Environment
        if (Environment != global::GameEnvironment.None)
        {
            UIManager.Instance.Switch(Environment, Delay);
        }

        // Blend
        if (UseBlend)
        {
            UIManager.Instance.DoBlende();
        }

    }

}
