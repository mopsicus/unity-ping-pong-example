using System;
using UnityEngine;

sealed class FadeManager : MonoBehaviour {

    /// <summary>
    /// Color fade from
    /// </summary>
    Color _fadeFrom = Color.clear;

    /// <summary>
    /// Color fade to
    /// </summary>
    Color _fadeTo = Color.clear;

    /// <summary>
    /// Fade duration
    /// </summary>
    float _fadeTime = 0f;

    /// <summary>
    /// Local timer
    /// </summary>
    float _time = 0f;

    /// <summary>
    /// Callback cache on end
    /// </summary>
    Action _callback = null;

    void OnGUI () {
        if (_fadeTime <= 0f || Event.current.type != EventType.Repaint) {
            return;
        }
        _time = Mathf.Clamp01 (_time + Time.deltaTime * _fadeTime);
        Color color = Color.Lerp (_fadeFrom, _fadeTo, _time);
        Color savedColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill);
        GUI.color = savedColor;
        if (_time >= 1f) {
            if (_callback != null) {
                Action callback = _callback;
                _callback = null;
                callback ();
            }
        }
    }

    /// <summary>
    /// Process fading from one color to another
    /// </summary>
    /// <param name="start">Source opaque</param>
    /// <param name="end">Target opaque</param>
    /// <param name="time">Time of fading</param>
    /// <param name="onSuccess">Callback on success ending of fading</param>
    public void Process (Color start, Color end, float time, Action onSuccess = null) {
        _fadeFrom = start;
        _fadeTo = end;
        _callback = onSuccess;
        _time = 0f;
        _fadeTime = (time > 0f) ? (1f / time) : 0f;
    }
}