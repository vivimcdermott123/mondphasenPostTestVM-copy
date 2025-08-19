using UnityEngine;

public enum LunarPhaseType {
    NewMoon,
    WaxingCrescent,
    FirstQuarter,
    WaxingGibbous,
    FullMoon,
    WaningGibbous,
    LastQuarter,
    WaningCrescent
}

[System.Serializable]
public class LunarPhase {
    public LunarPhaseType phaseType;
    public string displayName;
    public float correctAngle; // In degrees, 0 = New Moon, 180 = Full Moon, etc.
    public Sprite phaseSprite;
} 