using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

public class Events : Singleton<Events>
{
    // Gameplay Status
    private static GameplayStatus _gameplayStatus;
    public static GameplayStatus GameplayStatus
    {
        get
        {
            return _gameplayStatus;
        }
        set
        {
            if (_gameplayStatus == value)
                return;
            GameplayStatus oldStatus = _gameplayStatus;
            _gameplayStatus = value;
            if (GameConfig.Instance.Debug)
                Debug.Log("GameplayStatus=" + value);
            Events.Instance.FireGameplayStatusChange(oldStatus, _gameplayStatus);
        }
    }

    public delegate void GameplayStatusChange(GameplayStatus oldGameplayStatus, GameplayStatus newGameplayStatus);
    public event GameplayStatusChange OnGameplayStatusChange;

    public void FireGameplayStatusChange(GameplayStatus oldGameplayStatus, GameplayStatus newGameplayStatus)
    {
        if (OnGameplayStatusChange != null)
            OnGameplayStatusChange(oldGameplayStatus, newGameplayStatus);
    }

    // Layer
    public delegate void LayerAction(Layer layer, UIAction action);
    public event LayerAction OnLayerChange;

    public void FireLayerChange(Layer layer, UIAction action)
    {
        if (OnLayerChange != null)
            OnLayerChange(layer, action);
    }
}
