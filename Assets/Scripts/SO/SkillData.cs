using UnityEngine;

[CreateAssetMenu(menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public bool isUnlocked = true; // set false to grey out
    public float cooldown = 3f;
    [TextArea] public string description;

    // Override in subclasses or use this default behavior.
    public virtual void Activate(GameObject user)
    {
        Debug.Log($"Activated skill: {skillName}");
    }
}
