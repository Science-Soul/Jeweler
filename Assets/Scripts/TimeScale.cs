using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke(nameof(StopGame), 1.5f);
    }

    private void OnDisable()
    {
        ResumeGame();
    }

    public void StopGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
}
