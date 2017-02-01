using UnityEngine;
using TinyRoar.Framework;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/*
 * Explanation:
 * Use this BaseCounter for a Counter like PointCounter or CoinCounter
 * Value is the real value which is displayed in UI
 * VirtualValue is an internal value for DataManagement, Analytics and so on.
 * (c) 2015 by Dario D. Müller, Tiny Roar UG
 *
 * Important: If you want to override Init()-method, make sure to set "Value = X"
 */

public abstract class BaseCounter<T> : MonoSingleton<T> where T : MonoSingleton<T>
{
    public event Action OnChange;
    public List<Text> textList;

    protected int maxValue = -1;
    protected Dictionary<int, string> ExceptionList = new Dictionary<int, string>();

    private int _value;
    private int _virtualValue;
    private bool _actionReady = false;
    private bool _virtualNeverChanged = true;
    private bool _isEnabled = true;

    public bool IsEnabled()
    {
        return _isEnabled;
    }

    public void Enable()
    {
        if (_isEnabled)
            return;
        textList.ForEach(text => text.enabled = true);
        _isEnabled = true;
    }

    public void Disable()
    {
        if (_isEnabled == false)
            return;
        textList.ForEach(text => text.enabled = false);
        _isEnabled = false;
    }

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    protected virtual void Init()
    {
        Value = 0;
    }
    public void Reset()
    {
        Init();
    }

    public int Value
    {
        get
        {
            return this._value;
        }
        set
        {
            int newVal = value;
            if (maxValue != -1 && newVal > maxValue)
                newVal = maxValue;
            this._value = newVal;

            // Update GUI
            this.UpdateUI();

            // UpdateValue not at first init
            if (this._actionReady)
            {
                this.UpdateValue();

                if (OnChange != null)
                {
                    OnChange();
                }
            }
            this._actionReady = true;
        }
    }

    public int VirtualValue
    {
        set
        {
            _virtualValue = value;
            _virtualNeverChanged = false;
            UpdateVirtualValue(value);
        }
        get
        {
            return this._virtualValue;
        }
    }

    protected abstract void UpdateValue();
    protected abstract void UpdateVirtualValue(int value);

    public virtual void UpdateUI()
    {
        string value = this.Value.ToString();
        string valueExcept = null;

        ExceptionList.TryGetValue(this.Value, out valueExcept);
        if (valueExcept != null)
            value = valueExcept;

        textList.ForEach(text => text.text = value);
    }

    public void TakeOverVirtualValue()
    {
        if (this.Value == this._virtualValue)
            return;

        // check against virtual value was nerver changed
        if (_virtualNeverChanged)
            return;

        Value = this._virtualValue;
    }

    public void TakeValue()
    {
        if (this.Value == this._virtualValue)
            return;

        VirtualValue = Value;
    }

}
