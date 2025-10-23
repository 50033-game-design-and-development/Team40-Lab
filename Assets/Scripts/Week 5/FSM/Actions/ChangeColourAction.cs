using UnityEngine;

[CreateAssetMenu(menuName = "Mario/Actions/Change Color")]
public class ChangeColourAction : Action
{
    public Color color = Color.white;
    public override void Act(MarioController controller)
    {
        controller.ApplyVisual(color, controller.transform.localScale);
    }
}
