using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilLocker : MonoBehaviour
{
    private AnvilPiece anvil;
    [SerializeField] Grid grid;
    // Start is called before the first frame update
    void Start()
    {
        anvil = grid.GetComponentInChildren<AnvilPiece>();

        if (anvil != null)
        {
            anvil.Lock();
        }
    }
}
