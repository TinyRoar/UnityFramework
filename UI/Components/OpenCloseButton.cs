using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using UnityEngine.UI;

namespace TinyRoar.Framework
{
    [RequireComponent(typeof(Button))]
    public class OpenCloseButton : BaseButton
    {
        [Header("Actions")]
        public bool ExitGame = false;
        public bool BackButtonReacts = false;

        [Header("Environment")]
        public GameEnvironment Environment;

        [Header("Layer")]
        public bool UseBlend;
        public float Delay;
        public List<LayerEntry> ActionList;
        public List<AnimationConfig> Animations;

        [Header("Gameplay Status")]
        [SerializeField]
        private GameplayStatus GameplayStatus;
        [SerializeField]
        private GameplayStatus OrToggleWithStatus;

        // action
        protected override void ButtonAction()
        {
            if (ExitGame)
                Application.Quit();

            int count = ActionList.Count;
            for (var i = 0; i < count; i++)
            {
                LayerEntry layerEntry = ActionList[i];

                if (layerEntry.LayerConfig == null)
                {
                    Debug.LogWarning("LayerConfig is NULL!");
                    continue;
                }

                if (layerEntry.Action == UIAction.None)
                    continue;

                UIManager.Instance.Switch(layerEntry.LayerConfig, layerEntry.Action, Delay);
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

            // GameplayStatus
            if (GameplayStatus != GameplayStatus.None)
            {
                if (Events.Instance.GameplayStatus == GameplayStatus)
                    Events.Instance.GameplayStatus = OrToggleWithStatus;
                else
                    Events.Instance.GameplayStatus = GameplayStatus;
            }
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && BackButtonReacts)
                ButtonAction();
        }
    }


}
