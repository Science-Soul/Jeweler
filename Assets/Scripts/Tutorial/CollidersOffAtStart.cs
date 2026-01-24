using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidersOffAtStart : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] List<Collider2D> cols;
    // Start is called before the first frame update
    void Start()
    {
        cols = new List<Collider2D>();
        foreach (Collider2D col in grid.GetComponentsInChildren<Collider2D>())
        {
            cols.Add(col);
        }
    
        CollidersOff();
    }


    public void CollidersOff()
    {
        foreach (Collider2D col in cols)
        {
            col.enabled = false;
        }
    }

    public void CollidersOn()
    {
        foreach (Collider2D col in cols)
        {
            Time.timeScale = 1;
            col.enabled = true;
            Debug.Log("Colliders on");
        }
    }
}
