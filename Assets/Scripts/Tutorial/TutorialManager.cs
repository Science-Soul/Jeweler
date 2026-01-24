using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject gameOver;
    [SerializeField] Grid grid;
    [SerializeField] List<Collider2D> cols;

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
        Invoke(nameof(PauseGame), 1.5f);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
