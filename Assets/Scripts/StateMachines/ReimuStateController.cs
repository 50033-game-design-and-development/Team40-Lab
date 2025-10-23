using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReimuStateController : StateController
{
    public bool isTouching;
    public string touchedObjectTag;

    public override void Start()
    {
        base.Start();
    }

private void OnTriggerEnter2D(Collider2D other)
{
    isTouching = true;
    touchedObjectTag = other.tag;
    Debug.Log($"[CONTACT] Triggered with: {other.name}, Tag: {touchedObjectTag}");
}

private void OnTriggerExit2D(Collider2D other)
{
    isTouching = false;
    touchedObjectTag = null;
    Debug.Log("[CONTACT] Exited trigger");
}
}