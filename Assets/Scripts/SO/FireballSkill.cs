using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Fireball")]
public class FireballSkill : SkillData
{
    public GameObject projectilePrefab;
    public float speed = 8f;

    public override void Activate(GameObject user)
    {
        Debug.Log("Fireball launched");

    }
}
