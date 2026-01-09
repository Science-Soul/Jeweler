using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class LevelSelectionMenu : MonoBehaviour
{
    private Button[] levelButtons;
    // Start is called before the first frame update
    void Start()
    {
        levelButtons = gameObject.GetComponentsInChildren<Button>();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i <= YG2.saves.highestUnlockedLevelIndex - 1) // build index 0 => main menu, поэтому надо отнять единицу
            {
                levelButtons[i].interactable = true;
                levelButtons[i].GetComponent<Image>().color = Color.white;
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }
}
