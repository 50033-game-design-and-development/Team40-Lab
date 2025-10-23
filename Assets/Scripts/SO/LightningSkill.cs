using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Lightning")]
public class LightningSkill : SkillData
{
    public float radius = 2f;
    public float damage = 5f;
    public LayerMask enemyLayer;

    public override void Activate(GameObject user)
    {
        Debug.Log("Lightning strike!");
    }
}
