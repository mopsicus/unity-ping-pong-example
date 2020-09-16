using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sliders selector
/// </summary>
public enum SliderColor {

    RED = 0,

    GREEN = 1,

    BLUE = 2
}

public class SettingsView : ScreenView {

    /// <summary>
    /// Link to image ball
    /// </summary>
    [SerializeField]
    private Image BallImage = null;

    /// <summary>
    /// Link to image online state
    /// </summary>
    [SerializeField]
    private Image OnlineImage = null;    

    /// <summary>
    /// Link to slider red
    /// </summary>
    [SerializeField]
    private Slider RedSlider = null;

    /// <summary>
    /// Link to slider green
    /// </summary>
    [SerializeField]
    private Slider GreenSlider = null;

    /// <summary>
    /// Link to slider blue
    /// </summary>
    [SerializeField]
    private Slider BlueSlider = null;

    /// <summary>
    /// Screen name
    /// </summary>
    public override string Name {
        get {
            return "Settings";
        }
    }

    /// <summary>
    /// Set color for ball image
    /// </summary>
    public void SetBallColor (Color color) {
        BallImage.color = color;
    }

    /// <summary>
    /// Set ion for online type game
    /// </summary>
    public void SetOnlineIcon (Sprite icon) {
        OnlineImage.sprite = icon;
    }

    /// <summary>
    /// Set value for sliders
    /// </summary>
    /// <param name="slider">Slider color</param>
    /// <param name="value">Value for track</param>
    public void SetSliderValue (SliderColor slider, float value) {
        switch (slider) {
            case SliderColor.RED:
                RedSlider.value = value;
                break;
            case SliderColor.GREEN:
                GreenSlider.value = value;
                break;
            case SliderColor.BLUE:
                BlueSlider.value = value;
                break;
            default:
                break;
        }
    }

}