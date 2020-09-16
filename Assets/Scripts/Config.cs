using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour {

    /// <summary>
    /// Reference screen width for base develop
    /// </summary>
    public static float ReferenceScreenWidth {
        get {
            return 1080f;
        }
    }

    /// <summary>
    /// Reference screen height for base develop
    /// </summary>
    public static float ReferenceScreenHeight {
        get {
            return 1920f;
        }
    }

    public static float FadeDuration {
        get {
            return 0.5f;
        }
    }

    public static float FadeDurationFlash {
        get {
            return 0.00001f;
        }
    }

    public static string MenuScene {
        get {
            return "Menu";
        }
    }

    public static float MinBallSize {
        get {
            return 0.08f;
        }
    }

    public static float MaxBallSize {
        get {
            return 0.2f;
        }
    }        

    public static float MinBallSpeed {
        get {
            return 100f;
        }
    }

    public static float MaxBallSpeed {
        get {
            return 200f;
        }
    }

    public static string ServerHost {
        get {
            return "192.168.0.111";
        }
    }

    public static int ServerPort {
        get {
            return 3001;
        }
    }    

}