
using UnityEngine;
using System;
[CreateAssetMenu(menuName = "PluggableSM/Decisions/Transform")]
public class TransformDecision : Decision
{
    public StateTransformMap[] map;
    public string targetTag = "Powerup";

    public override bool Decide(StateController controller)
    {
        ReimuStateController m = (ReimuStateController)controller;
        ReimuState currentState = EnumExtension.ParseEnum<ReimuState>(m.currentState.name);

        if (!m.isTouching || m.touchedObjectTag != targetTag)
            return false;

        foreach (var entry in map)
        {
            if (currentState == entry.fromState)
            {
                Debug.Log($"[Decision] TRUE — transforming from {currentState}");

                m.isTouching = false;
                m.touchedObjectTag = null;

                return true;
            }
        }

        return false;
    }

}

[System.Serializable]
public struct StateTransformMap
{
    public ReimuState fromState;
}
