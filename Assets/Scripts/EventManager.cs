using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Action ObjectivesReached;
    public static void OnObjectivesReached()
    {
        ObjectivesReached?.Invoke();
    }
}
