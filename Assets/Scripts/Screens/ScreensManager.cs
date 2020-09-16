using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensManager : MonoBehaviour {

    /// <summary>
    /// Screens on menu
    /// </summary>
    const int MAX_SCREENS_COUNT = 10;

    /// <summary>
    /// Screens dictionary with name and interface
    /// </summary>
    private Dictionary<string, IScreen> _screens = new Dictionary<string, IScreen> (MAX_SCREENS_COUNT);

    /// <summary>
    /// Current screen name
    /// </summary>
    private string _current = null;

    /// <summary>
    /// Cache for previous screen name
    /// </summary>
    private string _previousScreen = null;

    /// <summary>
    /// Get all IScreen interfaces in scene and add them to dictionary
    /// </summary>
    private void Awake () {
        IScreen[] screens = GetComponentsInChildren<IScreen> (true);
        foreach (IScreen screen in screens) {
            _screens.Add (screen.Name, screen);
        }
    }
    /// <summary>
    /// Show screen
    /// </summary>
    /// <param name="name">Screen name</param>
    public void Show (string name) {
        if (!_screens.ContainsKey (name)) {
#if DEBUG
            Debug.LogError (string.Format ("Screen \"{0}\" does not exists", name));
#endif
            return;
        }
        if (_current != null) {
            _screens[_current].Hide ();
        }
        _previousScreen = _current;
        _current = name;
        _screens[name].Show ();
    }

    /// <summary>
    /// Return to previous screen
    /// </summary>
    public void Back () {
        Show (_previousScreen);
    }

    /// <summary>
    /// Get current screen name 
    /// </summary>
    public string CurrentScreen {
        get {
            return _current;
        }
    }

    /// <summary>
    /// Get previous screen name 
    /// </summary>
    public string PreviousScreen {
        get {
            return _previousScreen;
        }
    }

}