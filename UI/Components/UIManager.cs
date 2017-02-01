using UnityEngine;
using TinyRoar.Framework;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIManager : MonoSingleton<UIManager>
{

    [SerializeField]
    private float _blendTime = 0.5f;
    [SerializeField]
    private float BaseDelay = 0.5f;
    private float _delay = 0.5f;

    [SerializeField]
    private GameObject _blend;

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

        // get Canvas UI
        GameObject ui = GameObject.Find("UI");
        if (ui == null)
        {
            Debug.LogWarning("Canvas named 'UI' not found :'(");
            return;
        }

        // save all Layer and hide it
        Debug.Log(ui.transform);
        foreach (Transform item in ui.transform)
        {
            LayerEntry layer = new LayerEntry(item.name, item.gameObject);
            LayerManager.AddLayerEntry(layer);
            Hide(layer.Layer);
        }

        // get Environments
        GameObject env = GameObject.Find("Environment");
        if (env == null)
        {
            Debug.LogWarning("Container named 'Environment' not found :'(");
            return;
        }

        // save all Environment and hide it
        foreach (Transform item in env.transform)
        {
            GameEnvironment envKey = GameEnvironment.None;
            envKey = (GameEnvironment)Enum.Parse(typeof(GameEnvironment), item.name);
            Hide(item);
            _environmentList.Add(envKey, item);
        }
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
            action = LayerManager.GetToggledStatus(layer);
        if(LayerManager.IsAction(layer, action))
            return;

        if (action == UIAction.Hide)
        {
            // Do CloseAnimation already open Layer
            DoAnimation(layer);
        }
        else if (action == UIAction.Show)
        {
            // Delayed or not
            if (delay == 0 || LayerManager.IsNothingVisible())
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
            UIAction action = LayerManager.GetToggledStatus(layer);
            if (action == UIAction.Hide)
            {
                this.Hide(layer);
                LayerManager.SetAction(layer, action);
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

        float blendDelay = _delay - _blendTime / 2;
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

    // Animation

    private void DoAnimation(Layer layer)
    {
        UIAction action = LayerManager.GetToggledStatus(layer);

        if (layer == Layer.None)
            return;

        // Fade In Animation
        if (action == UIAction.Show)
        {
            // enable triggers OpenAnimations via Animator
            this.Show(layer);

            // sound
            LayerEntry layerEntry = LayerManager.GetLayerEntry(layer);
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

            LayerManager.SetAction(layer, action);

        }
        // Fade Out animation
        else
        {
            //bool isAnimation = false;
            // check if UIConfig Component exists
            LayerEntry layerEntry = LayerManager.GetLayerEntry(layer);
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
        LayerEntry layerEntry = LayerManager.GetLayerEntry(layer);
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
        LayerEntry layerEntry = LayerManager.GetLayerEntry(layer);
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
