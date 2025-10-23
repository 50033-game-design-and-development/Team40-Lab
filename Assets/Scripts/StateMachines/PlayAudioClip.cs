using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/PlayAudioClip")]
public class PlayAudioClip : Action
{
    [Header("Audio Settings")]
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;

    public override void Act(StateController controller)
    {
        AudioSource source = controller.GetComponent<AudioSource>();

        if (source == null)
        {
            Debug.LogWarning($"{controller.name}: No AudioSource found on this object.");
            return;
        }

        if (clip != null)
        {
            source.PlayOneShot(clip, volume);
        }
    }
}