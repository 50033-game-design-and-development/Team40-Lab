// GotHitDecision.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Mario/Decisions/Got Hit")]
public class HitDecision : Decision
{
    public override bool Decide(MarioController controller)
    {
        if (controller == null) return false;
        if (!controller.gotHit) return false;

        controller.gotHit = false; // consume event

        // Ignore if fire form (immune)
        if (controller.CurrentState == controller.FireState)
        {
            Debug.Log("[GotHitDecision] Fire Mario: immune.");
            return false;
        }

        if (controller.CurrentState == controller.BigState)
        {
            controller.ChangeState(controller.SmallState);
            Debug.Log("[GotHitDecision] Big → Small.");
            return false;
        }

        if (controller.CurrentState == controller.SmallState)
        {
            controller.ChangeState(controller.DeathState);
            Debug.Log("[GotHitDecision] Small → Die.");
            return false;
        }

        return false;
    }
}
