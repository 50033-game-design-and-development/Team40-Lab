using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HomingAttack")]
public class HomingAttackSkill : SkillData
{

    public override void Activate(GameObject user)
    {
        // Animator anim = user.GetComponentInChildren<Animator>();

        // if (anim != null)
        // {
        //     anim.SetTrigger("onHomingAttack");
        // }
        // else
        // {
        //     Debug.LogWarning("No Animator found on user!");
        // }
        user.GetComponent<PlayerController>().hasHoming = true;
        Debug.Log("Player has homing passive");
    }

    
}