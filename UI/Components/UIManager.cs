using UnityEngine;
using TinyRoar.Framework;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

namespace TinyRoar.Framework
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private string UIName = "UI";
        [SerializeField]
        private string EnvironmentName = "Environment";

        [SerializeField] private float _blendTime = 0.5f;
        [SerializeField] private float BaseDelay = 0.5f;
        private float _delay = 0.5f;

        [SerializeField] private GameObject _blend;

        public GameEnvironment ActiveEnvironment { get; private set; }
        private GameEnvironment _endTimerEnvironment;
        private Dictionary<GameEnvironment, Transform> _environmentList;

        public Transform GetEnvironment(GameEnvironment env)
        {
            return _environmentList[env];
        }

        public void Init()
        {
            // init
            _environmentList = new Dictionary<GameEnvironment, Transform>();

            // get UI
            Transform ui = GetTransformWithName(UIName);

            // save all Layer and hide it
            foreach (Transform item in ui)
            {
                LayerEntry layer = new LayerEntry(item.name, item.gameObject);
                LayerManager.Instance.AddLayerEntry(layer);
                Hide(layer.Layer);
            }

            // get Environments
            Transform env = GetTransformWithName(EnvironmentName);

            // save all Environment and hide it
            foreach (Transform item in env)
            {
                GameEnvironment envKey = GameEnvironment.None;
                envKey = (GameEnvironment) Enum.Parse(typeof (GameEnvironment), item.name);
                Hide(item);
                _environmentList.Add(envKey, item);
            }

        }

        private Transform GetTransformWithName(string name)
        {
            var objects = GameObject.FindObjectsOfType(typeof(GameObject)); //get all gameobjects
            Transform obj = null;
            for (var f = 0; f < objects.Length; f++) //filter the objects that don't match
            {
                if (objects[f].name == name)
                {
                    if (obj != null)
                    {
                        // check if 2 or more objects
                        Debug.LogError("UI Manager: Multiple GameObject with name '" + name + "' found :'(");
                        return null;
                    }
                    obj = ((GameObject)objects[f]).transform;
                }
            }

            // check if no object
            if (obj == null)
            {
                Debug.LogError("UI Manager: No GameObject with name '" + name + "' found :'(");
                return null;
            }
            return obj;
        }

        public void Switch(GameEnvironment environment, float delay = -1)
        {
            if (environment == GameEnvironment.None)
            {
                Debug.LogWarning("Not allowed to set Environment.None");
                return;
            }

            if (environment == ActiveEnvironment)
            {
                Debug.LogWarning("A script is trying to set same Environment that is already active");
                return;
            }

            // get delay by old Layer
            /*if (LayerManager.Layer != Layer.None)
        {
            LayerConfig UIConfig = LayerManager.GetLayerEntry()LayerManager.Layer.ToString().GetComponent<LayerConfig>();
            delay = UIConfig.Delay;
        }*/

            _endTimerEnvironment = environment;

            if (delay == -1)
                delay = _delay;

            if (delay != 0)
                Timer.Instance.Add(delay, TimerEndEnvironment);
            else
                TimerEndEnvironment();

        }

        // show / hide Environment
        private void TimerEndEnvironment()
        {
            if (ActiveEnvironment != GameEnvironment.None)
                this.Hide(_environmentList[ActiveEnvironment]);
            if (_endTimerEnvironment != GameEnvironment.None)
                this.Show(_environmentList[_endTimerEnvironment]);
            ActiveEnvironment = _endTimerEnvironment;

            // make sure to switch to active camera
            foreach (Camera cam in Camera.allCameras)
            {
                //Debug.Log(cam);
                GameObject.Find("UI").GetComponent<Canvas>().worldCamera = cam;
            }
        }

        private bool _timerStarted = false;
        private List<Layer> _hideLayerList = new List<Layer>();

        public void Switch(Layer layer, UIAction action = UIAction.Show, float delay = -1)
        {
            if (delay != -1)
                _delay = delay;
            else
                _delay = BaseDelay;

            if (InitManager.Instance.UseAnalytics)
                Analytics.Instance.GameplayEvent("Layer" + layer + "_" + action, 1);

            if (layer == Layer.None || action == UIAction.None)
                return;
            if (action == UIAction.Toggle)
                action = LayerManager.Instance.GetToggledStatus(layer);
            if (LayerManager.Instance.IsAction(layer, action))
                return;

            if (action == UIAction.Hide)
            {
                // Do CloseAnimation already open Layer
                DoAnimation(layer);
            }
            else if (action == UIAction.Show)
            {
                // Delayed or not
                if (delay == 0 || LayerManager.Instance.IsNothingVisible())
                {
                    // set layer immediately
                    DoAnimation(layer);
                }
                else
                {
                    // save for delayed animation
                    AddHideLayerList(layer);
                }
            }
        }

        private void AddHideLayerList(Layer layer)
        {
            // Save next Layer temporary
            _hideLayerList.Add(layer);

            // check if timer already started
            if (_timerStarted == false)
            {
                // start timer
                _timerStarted = true;
                Timer.Instance.Add(_delay, TimerEndLayer);
            }

        }

        // Timer End event in Update Loop of next frame
        private void TimerEndLayer()
        {
            int count = _hideLayerList.Count;
            for (var i = 0; i < count; i++)
            {
                Layer layer = _hideLayerList[i];
                UIAction action = LayerManager.Instance.GetToggledStatus(layer);
                if (action == UIAction.Hide)
                {
                    // important: do trigger action/event first, then hide it
                    LayerManager.Instance.SetAction(layer, action);
                    this.Hide(layer);
                }
                else
                {
                    DoAnimation(layer);
                }
            }

            // reset
            _hideLayerList.Clear();
            _timerStarted = false;
        }

        public void DoBlende()
        {
            if (_delay == 0)
            {
                Debug.Log("Use 'Blende' with Delay 0 not possible.");
                return;
            }

            float blendDelay = _delay - _blendTime/2;
            if (blendDelay > 0)
                Timer.Instance.Add(blendDelay, TimerEndBlende);
            else
                TimerEndBlende();

        }

        private void TimerEndBlende()
        {
            this._blend.SetActive(true);
            Timer.Instance.Add(_blendTime, TimerEndBlendeDisable);
        }

        private void TimerEndBlendeDisable()
        {
            this._blend.SetActive(false);
        }

        public void HideAll(Layer exception)
        {
            List<LayerEntry> layerList = LayerManager.Instance.GetAllLayersWithAction(UIAction.Show);
            for (int i = 0; i < layerList.Count; i++)
            {
                if(layerList[i].Layer == exception)
                    continue;

                this.Switch(layerList[i].Layer, UIAction.Hide);
            }
        }

        // Animation

        private void DoAnimation(Layer layer)
        {
            UIAction action = LayerManager.Instance.GetToggledStatus(layer);

            if (layer == Layer.None)
                return;

            // Fade In Animation
            if (action == UIAction.Show)
            {
                // enable triggers OpenAnimations via Animator
                this.Show(layer);

                // sound
                LayerEntry layerEntry = LayerManager.Instance.GetLayerEntry(layer);
                if (layerEntry == null)
                {
                    Debug.LogWarning("Layer named " + layer + " not found");
                    return;
                }
                LayerConfig UIConfig = layerEntry.LayerConfig;
                if (UIConfig != null)
                {
                    // play each animation
                    foreach (var anim in UIConfig.OpenAnimations)
                    {
                        anim.Object.Play(anim.State);
                    }

                    if (UIConfig.OpenSound != "")
                        SoundManager.Instance.Play(UIConfig.OpenSound, SoundManager.SoundType.Soundeffect, false, 0.5f);
                }

                LayerManager.Instance.SetAction(layer, action);

            }
            // Fade Out animation
            else
            {
                //bool isAnimation = false;
                // check if UIConfig Component exists
                LayerEntry layerEntry = LayerManager.Instance.GetLayerEntry(layer);
                if (layerEntry == null)
                {
                    Debug.LogWarning("Layer named " + layer + " not found");
                    return;
                }
                LayerConfig UIConfig = layerEntry.LayerConfig;
                if (UIConfig != null && _delay != 0)
                {
                    // play each animation
                    foreach (var anim in UIConfig.CloseAnimations)
                    {
                        //isAnimation = true;
                        anim.Object.Play(anim.State);
                    }

                    // sound
                    if (UIConfig.CloseSound != "")
                    {
                        SoundManager.Instance.Play(UIConfig.CloseSound, SoundManager.SoundType.Soundeffect, false, 0.5f);
                    }
                }

                // save for delayed animation
                /*if (isAnimation == false)
            {
                this.Hide(layer);
                LayerManager.SetAction(layer, action);
            }
            else
            {*/
                AddHideLayerList(layer);
                //}
            }
        }

        // Show / Hide UI Elements

        private void ShowHide(Layer layer, UIAction action)
        {
            if (action == UIAction.Show)
                this.Show(layer);
            else
                this.Hide(layer);
        }

        private void ShowHide(GameObject obj, UIAction action)
        {
            if (action == UIAction.Show)
                this.Show(obj);
            else
                this.Hide(obj);
        }

        // Show UI Elements

        private void Show(Layer layer)
        {
            LayerEntry layerEntry = LayerManager.Instance.GetLayerEntry(layer);
            if (layerEntry == null)
            {
                Debug.LogWarning("Layer named " + layer + " not found");
                return;
            }
            this.Show(layerEntry.GameObject);
        }

        private void Show(Transform obj)
        {
            Show(obj.gameObject);
        }

        private void Show(GameObject obj)
        {
            if (GameConfig.Instance.Debug)
                Debug.LogWarning("Show(" + obj.name + ")");
            obj.SetActive(true);
        }

        // Hide UI Elements

        private void Hide(Layer layer)
        {
            LayerEntry layerEntry = LayerManager.Instance.GetLayerEntry(layer);
            if (layerEntry == null)
            {
                Debug.LogWarning("Layer named " + layer + " not found");
                return;
            }
            this.Hide(layerEntry.GameObject);
        }

        private void Hide(Transform obj)
        {
            Hide(obj.gameObject);
        }

        private void Hide(GameObject obj)
        {
            if (!obj.activeSelf)
                return;
            if (GameConfig.Instance.Debug && obj != null)
                Debug.LogWarning("Hide(" + obj.name + ")");
            if (obj == null)
            {
                Debug.LogWarning("Hide failed -> obj null");
                return;
            }
            obj.SetActive(false);
        }

    }
}
