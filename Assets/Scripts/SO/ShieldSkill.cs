using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Shield")]
public class ShieldSkill : SkillData
{
    public float duration = 5f;

    [SerializeField] private float shieldSpellCost = 1f;
    [SerializeField] private GameObject shieldPrefab;
    private Shield activeShield;

    public override void Activate(GameObject user)
    {
        var player = user.GetComponent<PlayerController>();
        var health = user.GetComponent<Health>();
        // Don’t allow spamming
        if (activeShield != null) return;

        // Shoot fireball
        if (health.UseMana(shieldSpellCost))
        {
            // Spawn shield prefab
            activeShield = Instantiate(shieldPrefab, player.transform.position, Quaternion.identity).GetComponent<Shield>();

            if (activeShield != null)
            {
                activeShield.Initialize(player.transform);
            }
        }
        else
        {
            Debug.Log("Not enough mana to cast shield!");
        }

    }
}
