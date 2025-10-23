using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Teleport Attack")]
public class TeleportSkill : SkillData
{
    [Header("Teleport Settings")]
    public GameObject teleportPrefab;
    public float teleportSpellCost = 0.5f;
    public float timeBetweenTeleport = 5f;
    public float timeBetweenTeleportRecast = 0.5f;

    private TeleportBall activeTeleportBall;
    private float timeSinceTeleport = 999f;
    private float timeSinceTeleportShot = 999f;
    private bool teleportShot = false;

    public override void Activate(GameObject user)
    {
        // Get references from player
        var player = user.GetComponent<PlayerController>();
        var health = user.GetComponent<Health>();
        if (player == null || health == null) return;

        // Fire or recall teleport ball
        if (!teleportShot)
        {
            TryShootTeleportBall(user, health, player);
        }
        else if (teleportShot)
        {
            RecallTeleport(user);
        }
        else
        {
            Debug.Log("timeSinceTeleportShot: " + timeSinceTeleport + " / " + timeBetweenTeleport);
            Debug.Log("timeSinceTeleportShot: " + timeSinceTeleportShot + " / " + timeBetweenTeleportRecast);
            Debug.Log("Teleport skill on cooldown.");
        }
    }

    private void TryShootTeleportBall(GameObject user, Health health, PlayerController player)
    {
        if (!health.UseMana(teleportSpellCost))
        {
            Debug.Log("Not enough mana to cast teleport!");
            return;
        }

        Vector2 facingDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) facingDir.y = 1;
        if (Input.GetKey(KeyCode.S)) facingDir.y = -1;
        if (Input.GetKey(KeyCode.A)) facingDir.x = -1;
        if (Input.GetKey(KeyCode.D)) facingDir.x = 1;

        // Instantiate teleport ball
        activeTeleportBall = Instantiate(teleportPrefab, user.transform.position, Quaternion.identity)
            .GetComponent<TeleportBall>();

        if (activeTeleportBall != null)
        {
            activeTeleportBall.Launch(facingDir, player.Facing);
            player.anim?.SetTrigger("isFarAttack");
        }

        teleportShot = true;
        timeSinceTeleport = 0f;
        timeSinceTeleportShot = 0f;
    }

    private void RecallTeleport(GameObject user)
    {
        if (activeTeleportBall != null)
        {
            Vector3 playerPos = user.transform.position;
            user.transform.position = activeTeleportBall.transform.position;
            activeTeleportBall.kill();
        }

        teleportShot = false;
        timeSinceTeleport = 0f;
    }
}
