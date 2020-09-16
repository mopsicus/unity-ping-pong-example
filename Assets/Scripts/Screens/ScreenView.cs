using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Screen base view class
/// Each UI screen view must be inherited from this class
/// </summary>
public class ScreenView : MonoBehaviour, IScreen {

    /// <summary>
    /// Show/hide duration
    /// </summary>
    const float DURATION = 0.25f;

    /// <summary>
    /// RectTransform cache for moving
    /// </summary>
    private RectTransform _transform = null;

    /// <summary>
    /// Screen name
    /// </summary>
    public virtual string Name {
        get {
            throw new NotImplementedException ();
        }
    }

    /// <summary>
    /// Hide screen
    /// </summary>
    /// <param name="transition">Screen transition/animation type</param>
    /// <param name="callback">Callback action when screen start hide</param>
    public virtual void Hide () {
        _transform.DOAnchorPosX (-Config.ReferenceScreenWidth, DURATION).OnComplete (() => {
            SwitchVisible (false);
        });
    }

    /// <summary>
    /// Show screen
    /// </summary>
    /// <param name="transition">Screen transition/animation type</param>
    /// <param name="callbackStart">Callback action when screen start show</param>
    /// <param name="callbackShow">Callback action when screen showed</param>
    public virtual void Show () {
        SwitchVisible (true);
        _transform.DOAnchorPosX (Config.ReferenceScreenWidth, 0f);
        _transform.DOAnchorPosX (0f, DURATION);
    }

    /// <summary>
    /// Switch visible for current gameobject
    /// </summary>
    /// <param name="value">True | false</param>
    void SwitchVisible (bool value) {
        gameObject.SetActive (value);
        if ((object) _transform == null) {
            _transform = GetComponent<RectTransform> ();
        }
    }

}