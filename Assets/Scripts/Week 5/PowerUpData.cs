using UnityEngine;

[CreateAssetMenu(menuName = "Mario/PowerUp Data")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName;
    public State resultingState;
    public State[] validFromStates;
    public float duration = 5f;
}
