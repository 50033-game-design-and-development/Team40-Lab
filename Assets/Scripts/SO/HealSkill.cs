using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Heal")]
public class HealSkill : SkillData
{
    public float healPercent = 0.3f;

    public override void Activate(GameObject user)
    {
        Debug.Log($"Healed health.");
    }
}
