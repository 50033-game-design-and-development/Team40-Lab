using UnityEngine;

public class PlayerAERelay : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] public ChargeAttackSkill CAskill;


    public void AE_ShootProjectile() => player?.AE_ShootProjectile();
    public void AE_GameOver() => player?.AE_GameOver();
    public void AE_ShootChargeProjectile() => CAskill?.launchChargeProjectile();
}
