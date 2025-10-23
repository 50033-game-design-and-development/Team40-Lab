using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/ClearAudio")]
public class ClearAudioAction : Action
{
    public override void Act(StateController controller)
    {
        AudioSource audio = controller.GetComponent<AudioSource>();
        if (audio && audio.isPlaying)
        {
            audio.Stop(); 
            audio.clip = null; 
        }
    }
}