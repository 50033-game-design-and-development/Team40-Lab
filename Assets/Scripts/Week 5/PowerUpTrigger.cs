using UnityEngine;

public class PowerUpTrigger : MonoBehaviour
{
    [SerializeField] PowerUpData powerUpData;

    void OnTriggerEnter2D(Collider2D other)
    {
        var fsm = other.GetComponent<MarioController>();
        if (fsm && powerUpData != null)
        {
            Debug.Log($"[PowerUpAction] Trying to activate FireStar. Current state: {fsm.CurrentState.name}, buffTimer={fsm.BuffTimer}");

            bool canActivate = false;
            foreach (var valid in powerUpData.validFromStates)
            {
                if (fsm.CurrentState == valid)
                {
                    canActivate = true;
                    break;
                }
            }

            if (canActivate)
            {

                fsm.ChangeState(powerUpData.resultingState, powerUpData.duration);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"[PowerUpTrigger] Cannot activate {powerUpData.powerUpName} from {fsm.CurrentState.name}");
            }
        }
    }
}
