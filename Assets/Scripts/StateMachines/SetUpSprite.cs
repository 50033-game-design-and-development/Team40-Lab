using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupSprite")]
public class SetupSprite : Action
{
    [Header("Sprite to Set")]
    public Sprite newSprite;

    public override void Act(StateController controller)
    {
        if (newSprite == null)
        {
            Debug.LogWarning($"{controller.name}: newSprite is not set!");
            return;
        }

        // Find the child called "Visual"
        Transform visualChild = controller.transform.Find("Visual");

        if (visualChild == null)
        {
            Debug.LogWarning($"{controller.name}: No child named 'Visual' found!");
            return;
        }

        SpriteRenderer spriteRenderer = visualChild.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{controller.name}: 'Visual' child has no SpriteRenderer!");
            return;
        }

        // Set the sprite
        spriteRenderer.sprite = newSprite;
    }
}