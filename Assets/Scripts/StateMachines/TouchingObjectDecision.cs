using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Decisions/TouchingObject")]
public class TouchingObjectDecision : Decision
{
    public string targetTag;

    public override bool Decide(StateController controller)
    {
        var reimuController = controller as ReimuStateController;
        return reimuController.isTouching && reimuController.touchedObjectTag == targetTag;
    }
}