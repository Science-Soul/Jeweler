using System.Linq;

public class LevelAnvil : Level
{
    
    public override void OnAnvilScoreAdd(AnvilPiece anvil)
    {
        if (remainingScoresDict != null)
        {
            void UpdateScore()
            {
                int currentRemaining = --remainingScoresDict[anvil.AnvilColor];
                OnScoreUpdate(anvil.AnvilColor, currentRemaining);
            }

            bool isInDict = remainingScoresDict.ContainsKey(anvil.AnvilColor);

            if (isInDict && remainingScoresDict[anvil.AnvilColor] > 0)
            {
                UpdateScore();
            }
        }

        CheckWin();
    }

    public override void OnAnvilScoreSubtract(AnvilPiece anvil)
    {
        if (remainingScoresDict != null)
        {
            void UpdateScore()
            {
                int currentRemaining = ++remainingScoresDict[anvil.AnvilColor];
                OnScoreUpdate(anvil.AnvilColor, currentRemaining);
            }

            bool isInDict = remainingScoresDict.ContainsKey(anvil.AnvilColor);

            if (isInDict && remainingScoresDict[anvil.AnvilColor] < remainingScoresDict0[anvil.AnvilColor])
            {
                UpdateScore();
            }
        }
    }

    private void CheckWin()
    {
        if (remainingScoresDict.All(x => x.Value == 0))
        {
            gameOver.SetActive(true);
            gameOver.GetComponent<GameOver>().SetGameLayersInvisible();
            grid.gameOver = true;
        }
    }
}
