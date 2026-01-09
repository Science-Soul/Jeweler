using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PSReturnToPool : MonoBehaviour
{
    private ParticleSystem ps;
    Dictionary<ColorPiece.ColorType, Color> colorDict = new()
    {
        { ColorPiece.ColorType.RED, Color.red },
        { ColorPiece.ColorType.BLUE, Color.blue },
        { ColorPiece.ColorType.GREEN, Color.green },
        { ColorPiece.ColorType.YELLOW, Color.yellow },
        { ColorPiece.ColorType.PURPLE, Color.magenta },
        { ColorPiece.ColorType.CYAN, Color.cyan },
        { ColorPiece.ColorType.ANY, Color.blue },
    };

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        if (PSPooler.Instance != null)
        {
            PSPooler.Instance.ReturnToPool(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetPSColorFromPiece(GamePiece piece)
    {
        var main = ps.main;
        main.startColor = colorDict[piece.ColorComponent.Color];
    }
}
