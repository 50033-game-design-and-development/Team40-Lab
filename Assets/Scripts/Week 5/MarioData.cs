using UnityEngine;

[CreateAssetMenu(menuName = "Mario/Data")]
public class MarioData : ScriptableObject
{
    public string currentStateName = "smallMario";
    public float buffTimer = 0f;
}
