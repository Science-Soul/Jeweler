using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents
{
    private static event Action<Instrument[]> onSetupInstruments;
    private static Instrument[] cachedInstruments;

    public static event Action<Instrument[]> OnSetupInstruments
    {
        add
        {
            onSetupInstruments += value;
            // Если данные уже были отправлены ранее — отдаем их новому подписчику сразу!
            if (cachedInstruments != null)
            {
                value?.Invoke(cachedInstruments);
            }
        }
        remove
        {
            onSetupInstruments -= value;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        // Подписываемся на системное событие выгрузки любой сцены
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void OnSceneUnloaded(Scene current)
    {
        if (cachedInstruments != null)
        {
            cachedInstruments = null;
        }
    }

    public static void SetupLevelInstruments(Instrument[] allowedInstruments)
    {
        cachedInstruments = allowedInstruments;
        onSetupInstruments?.Invoke(allowedInstruments);
    }
}
