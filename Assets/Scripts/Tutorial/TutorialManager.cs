using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject gameOver;
    [SerializeField] Grid grid;
    private AnvilPiece anvil;
    // Start is called before the first frame update
    void Start()
    {
        anvil = grid.GetComponentInChildren<AnvilPiece>();
        anvil.OnAnvilLocked += AnvilOffTrigger;
    }

    private void OnDestroy()
    {
        anvil.OnAnvilLocked -= AnvilOffTrigger;
    }

    void AnvilOffTrigger()
    {
        gameOver.SetActive(true);
        Invoke(nameof(StopGame), 1.5f);
    }

    void StopGame()
    {
        Time.timeScale = 0f;
    }
}
