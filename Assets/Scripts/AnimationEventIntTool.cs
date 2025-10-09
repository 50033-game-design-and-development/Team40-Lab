using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventIntTool : MonoBehaviour
{
    public int parameter;
    public UnityEvent<int> useInt;

    void Awake()
    {
        var gm = FindFirstObjectByType<GameManager>();
        useInt.RemoveAllListeners();
        if (gm) useInt.AddListener(gm.IncreaseScore);

    }


    public void TriggerIntEvent()
    {
        Debug.Log("TriggerIntEvent: " + parameter);
        useInt.Invoke(parameter);

    }
}