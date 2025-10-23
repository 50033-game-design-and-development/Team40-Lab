using UnityEngine;

[CreateAssetMenu(menuName = "Mario/Actions/Change Scale")]
public class ChangeScaleAction : Action
{
    public float scaleMultiplier = 1f;
    public override void Act(MarioController controller)
    {
        controller.ApplyVisual(controller.GetComponent<SpriteRenderer>().color,
                               Vector3.one * scaleMultiplier);
    }
}
