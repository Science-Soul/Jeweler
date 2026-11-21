using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InstrumentPanel : MonoBehaviour
{
    public GameObject[] instrumentsGO;

    private void Start()
    {
        var instrumentsList = instrumentsGO.Select(x => x.GetComponent<Instrument>()).ToArray();
        GameEvents.SetupLevelInstruments(instrumentsList);
    }
}
