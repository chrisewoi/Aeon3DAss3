using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnInput : MonoBehaviour
{
    [SerializeField] private KeyCode triggerKey;
    [SerializeField] private UnityEvent onTrigger;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            onTrigger.Invoke();
        }
    }
}
