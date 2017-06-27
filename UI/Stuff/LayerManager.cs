using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TinyRoar.Framework
{
    public class LayerManager : Singleton<LayerManager>
    {
        private readonly List<LayerEntry> _layerList = new List<LayerEntry>();

        public UIAction GetAction(Layer layer)
        {
            for (var i = 0; i < _layerList.Count; i++)
            {
                if(_layerList[i].Layer != layer)
                    continue;
                return _layerList[i].Action;
            }
            return UIAction.None;
        }

        public LayerEntry GetLayerEntry(Layer layer)
        {
            for (var i = 0; i < _layerList.Count; i++)
            {
                if (_layerList[i].Layer != layer)
                    continue;
                return _layerList[i];
            }
            Debug.LogWarning("No LayerEntry for Layer " + layer + " :'(");
            return null;
        }

        public LayerEntry GetLayerEntry(LayerConfig layerConfig)
        {
            for (var i = 0; i < _layerList.Count; i++)
            {
                if (_layerList[i].LayerConfig != layerConfig)
                    continue;
                return _layerList[i];
            }
            Debug.LogWarning("No LayerEntry for Layer " + layerConfig.name + " :'(");
            return null;
        }

        public bool IsAction(Layer layer, UIAction action)
        {
            for (var i = 0; i < _layerList.Count; i++)
            {
                if (_layerList[i].Layer != layer)
                    continue;
                return _layerList[i].Action == action;
            }
            return false;
        }

        public void AddLayerEntry(LayerEntry layer)
        {
            _layerList.Add(layer);
        }

        public void SetAction(Layer layer, UIAction action)
        {
            if (action == UIAction.None || action == UIAction.Toggle)
                return;
            for (var i = 0; i < _layerList.Count; i++)
            {
                if (_layerList[i].Layer != layer)
                    continue;
                _layerList[i].Action = action;
                Events.Instance.FireLayerChange(layer, action);
                return;
            }
        }

        public UIAction GetToggledStatus(Layer layer)
        {
            for (var i = 0; i < _layerList.Count; i++)
            {
                if (_layerList[i].Layer != layer)
                    continue;
                return (_layerList[i].Action == UIAction.Hide) ? UIAction.Show : UIAction.Hide;
            }
            return UIAction.None;
        }

        public List<LayerEntry> GetAllLayersWithAction(UIAction action, List<Layer> excludeList = null)
        {
            List<LayerEntry> layerList = new List<LayerEntry>();
            for (var i = 0; i < _layerList.Count; i++)
            {
                var layer = _layerList[i];
                if (layer.Action != action)
                    continue;
                if(excludeList != null && excludeList.Contains(layer.Layer))
                    continue;
                layerList.Add(layer);
            }
            return layerList;
        }

        public bool IsNothingVisible()
        {
            for (var i = 0; i < _layerList.Count; i++)
                if (_layerList[i].Action == UIAction.Show)
                    return false;
            return true;
        }


    }
}