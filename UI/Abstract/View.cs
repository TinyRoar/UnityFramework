using System;
using UnityEngine;

namespace TinyRoar.Framework
{
    public abstract class View : MonoBehaviour
    {
        public static Action UpdateView;

        public abstract void onShow();

        public abstract void onHide();

        public abstract void onRefresh();

    }
}