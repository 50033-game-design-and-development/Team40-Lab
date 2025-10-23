using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public Button[] skillButtons; // 7
    public Image[] equipSlots;    // 3

    private int nextEquipIndex = 0;

    private void Start()
    {
        if (skillButtons == null || skillButtons.Length == 0) return;
        HookButtons();
        RefreshUI();
    }

    private void HookButtons()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int idx = i;
            skillButtons[i].onClick.RemoveAllListeners();
            skillButtons[i].onClick.AddListener(() => OnClickSkill(idx));
        }
    }

    public void OnClickSkill(int skillIndex)
    {
        var mgr = PlayerSkillManager.Instance;
        if (mgr == null) { Debug.LogWarning("No PlayerSkillManager in scene."); return; }

        if (skillIndex < 0 || skillIndex >= mgr.allSkills.Length) return;
        var skill = mgr.allSkills[skillIndex];
        if (skill == null) return;

        if (!skill.isUnlocked)
        {
            Debug.Log($"{skill.skillName} is locked.");
            return;
        }

        mgr.equippedSkills[nextEquipIndex] = skill;
        nextEquipIndex = (nextEquipIndex + 1) % mgr.equippedSkills.Length;
        RefreshUI();
    }

    public void RefreshUI()
    {
        var mgr = PlayerSkillManager.Instance;
        if (mgr == null) return;

        // Set skill button icons + grey out if locked
        for (int i = 0; i < skillButtons.Length; i++)
        {
            var img = skillButtons[i].GetComponent<Image>();
            if (mgr.allSkills.Length > i && mgr.allSkills[i] != null)
            {
                img.sprite = mgr.allSkills[i].icon;
                img.color = mgr.allSkills[i].isUnlocked ? Color.white : Color.gray;
            }
            else img.color = new Color(1,1,1,0.2f);
        }

        // Set equip slot icons
        for (int i = 0; i < equipSlots.Length; i++)
        {
            var slot = equipSlots[i];
            var eq = (i < mgr.equippedSkills.Length) ? mgr.equippedSkills[i] : null;
            if (eq != null) { slot.sprite = eq.icon; slot.color = Color.white; }
            else { slot.sprite = null; slot.color = new Color(1,1,1,0.2f); }
        }
    }
}
