using UnityEngine;

[CreateAssetMenu(menuName = "Mario/Decisions/Timer Expired")]
public class TimerExpireDecision : Decision
{
    public override bool Decide(MarioController controller)
    {
        return controller.BuffTimer == 0f;
    }
}
