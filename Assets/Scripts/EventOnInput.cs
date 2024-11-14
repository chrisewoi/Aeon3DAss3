using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnInput : MonoBehaviour
{
    public enum InputType {
        Down,
        Pressed,
        Released
    }

    [SerializeField] private InputType type;
    [SerializeField] private KeyCode triggerKey;
    [SerializeField] private UnityEvent onTrigger;

    void Update()
    {
        switch (type)
        {
            case InputType.Down:
                if (Input.GetKeyDown(triggerKey)) onTrigger.Invoke();
                break;
            case InputType.Pressed:
                if (Input.GetKey(triggerKey)) onTrigger?.Invoke();
                break;
            case InputType.Released:
                if (Input.GetKeyUp(triggerKey)) onTrigger?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (Input.GetKeyDown(triggerKey))
        {
            onTrigger.Invoke();
        }
    }
}
