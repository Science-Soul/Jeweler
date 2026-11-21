using System;
using System.Collections.Generic;

public class GameEvents
{
    public static event Action<Instrument[]> OnSetupInstruments;

    public static void SetupLevelInstruments(Instrument[] allowedInstruments)
    {
        OnSetupInstruments?.Invoke(allowedInstruments);
    }
}
