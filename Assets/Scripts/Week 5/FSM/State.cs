using UnityEngine;

[CreateAssetMenu(menuName = "Mario/State")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;

    public void UpdateState(MarioController controller)
    {
        foreach (var a in actions)
            a.Act(controller);

        foreach (var t in transitions)
        {
            if (t.decision == null) continue;
            bool result = t.decision.Decide(controller);
            if (result && t.trueState != null)
                controller.ChangeState(t.trueState);
            else if (!result && t.falseState != null)
                controller.ChangeState(t.falseState);
        }
    }

    public void EnterState(MarioController controller)
    {
        controller.Data.buffTimer = -1f;
        foreach (var a in actions)
            a.Act(controller);
    }
}
