using UnityEngine;
using Unity.Collections;

public class MarioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MarioData data;
    [SerializeField] State smallState;
    [SerializeField] State bigState;
    [SerializeField] State fireState;
    [SerializeField] State deathState;

    [Header("Runtime Info")]
    [SerializeField, ReadOnly] string currentStateName;
    [SerializeField, ReadOnly] float buffTimer;

    State current;
    SpriteRenderer sr;

    public float BuffTimer => buffTimer;
    public State CurrentState => current;
    public State SmallState => smallState;
    public State BigState => bigState;
    public State FireState => fireState;
    public State DeathState => deathState;
    public MarioData Data => data;
    public bool gotHit;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Debug.Log($"[MarioController] Awake scene: {gameObject.scene.name}");
        Debug.Log($"[MarioController] Data file path: {UnityEditor.AssetDatabase.GetAssetPath(data)}");
        Debug.Log($"[MarioController] MarioData.currentStateName = {data.currentStateName}");

        switch (data.currentStateName)
        {
            case "bigMario":
                current = bigState;
                break;
            case "fireMario":
                current = fireState;
                break;
            default:
                current = smallState;
                break;
        }

        buffTimer = data.buffTimer;

        current?.EnterState(this);
        currentStateName = current ? current.name : "None";
    }

    void Update()
    {
        current?.UpdateState(this);
        currentStateName = current ? current.name : "None";

        if (data.buffTimer > 0f)
        {
            data.buffTimer -= Time.deltaTime;

            if (data.buffTimer <= 0f)
            {
                data.buffTimer = 0f;
                Debug.Log("[MarioController] Buff expired!");
            }
        }

        buffTimer = data.buffTimer;
    }


    public void ChangeState(State newState, float duration = 0)
    {
        if (current == newState && duration <= 0f) return;

        current = newState;
        data.currentStateName = newState.name;
        currentStateName = newState.name;
        current?.EnterState(this);

        if (duration > 0f)
        {
            data.buffTimer = duration;
            Debug.Log($"[MarioController] Buff started for {duration}s");
        }
        else if (duration == 0f)
        {
            data.buffTimer = 0f;
        }
        else
        {
            data.buffTimer = -1f;
        }
    }


    public void ApplyVisual(Color c, Vector3 s)
    {
        sr.color = c;
        transform.localScale = s;
    }
}
