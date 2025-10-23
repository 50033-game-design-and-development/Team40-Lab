using UnityEngine;

public abstract class StateController : MonoBehaviour
{
    public State startState;
    public State previousState;
    public State currentState;
    public State remainState;
    public bool transitionStateChanged = false;
    [HideInInspector] public float stateTimeElapsed;

    public bool isActive = true;

    public virtual void Start()
    {
        Debug.Log($"[FSM] Starting StateController on {gameObject.name}");
        OnSetupState();
    }
    
    public virtual void OnSetupState()
    {
        if (currentState)
        {
            Debug.Log($"[FSM] Setup state: {currentState.name}");
            currentState.DoSetupActions(this);
        }
        else
        {
            Debug.LogWarning("[FSM] No current state set during OnSetupState!");
        }
    }

    public virtual void OnExitState()
    {
        // reset time in this state
        stateTimeElapsed = 0;
        if (currentState)
        {
            Debug.Log($"[FSM] Exiting state: {currentState.name}");
            currentState.DoExitActions(this);
        }
    }

    // for visual aid to indicate which state this object is currently at
    public virtual void OnDrawGizmos()
    {
        if (currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(this.transform.position, 1.0f);
        }
    }

    /********************************/
    // Regular methods
    // no action should be done here, strictly for transition
    public void TransitionToState(State nextState)
    {
        if (nextState == remainState)
        {
            return; 
        }
        Debug.Log($"[FSM] Transitioning from {currentState?.name ?? "NULL"} ➜ {nextState.name}");
        // The following two methods only happens once if nextState != remainstate
        OnExitState(); // cast exit action if any,

        // transition the states
        previousState = currentState;
        currentState = nextState;
        transitionStateChanged = true;

        OnSetupState(); // cast entry action if any
    }

    // default method to check if we've been in the state long enough
    // this method assumes that you will call this once per update frame
    // Time.deltaTime: the interval in seconds from the last frame to the current one (Read Only).
    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return stateTimeElapsed >= duration;
    }

    public void Update()
    {
        if (!isActive) return; // this is different from gameObject active, allow for separate control

        if (currentState == null)
        {
            Debug.LogWarning("[FSM] No current state set! Did you forget to assign startState?");
            return;
        }

        currentState.UpdateState(this);
    }
    /********************************/




}
