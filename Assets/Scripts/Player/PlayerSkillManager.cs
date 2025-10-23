using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public static PlayerSkillManager Instance { get; private set; }

    [Header("All available skills (7)")]
    public SkillData[] allSkills = new SkillData[7];

    [Header("Equipped skills (3)")]
    public SkillData[] equippedSkills = new SkillData[3];

    // cooldown timers for the 3 equipped slots
    private float[] cooldownTimers = new float[3];

    public ChargeAttackSkill activeChargeSkill;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (activeChargeSkill)
        {
            activeChargeSkill.isCharging = false;
        }
        
    }

    private void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
            if (cooldownTimers[i] > 0f) cooldownTimers[i] -= Time.deltaTime;

        if (activeChargeSkill != null)
        {
            activeChargeSkill.updateChargeTimer();
        }
    }

    // call from PlayerController input
    public void UseEquippedSkill(int slotIndex, GameObject user)
    {
        if (slotIndex < 0 || slotIndex >= equippedSkills.Length) return;
        var skill = equippedSkills[slotIndex];
        if (skill == null) return;

        if (cooldownTimers[slotIndex] <= 0f)
        {
            skill.Activate(user);
            cooldownTimers[slotIndex] = skill.cooldown;

            if (skill is ChargeAttackSkill chargeSkill)
            {
                activeChargeSkill = chargeSkill;
            }
        }
        else
        {
            Debug.Log($"{skill.skillName} cooldown: {cooldownTimers[slotIndex]:F1}s");
        }
        user.GetComponent<PlayerController>().SetActiveSkill(skill);
    }
}
