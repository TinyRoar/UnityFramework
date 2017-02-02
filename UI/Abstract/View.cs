using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class View : MonoBehaviour
{

    public abstract void onShow();

    public abstract void onHide();

    public abstract void onRefresh();

}

