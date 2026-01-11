using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelGridMenu : MonoBehaviour
{
    public Button buttonPrefab;
    public Transform gridParent;
    // Start is called before the first frame update
    void Awake()
    {
        GenerateButtons();
    }

    private void GenerateButtons()
    {
        for (int i = 0; i < SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/ExtraLevels/_ORDERED/1.1_StartExtraLevels.unity") - 1; i++)
        {
            Button newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(gridParent, false);

            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (i == 0)
                {
                    buttonText.text = "Пролог";
                    buttonText.fontSize = 70;
                }
                else
                {
                    buttonText.text = i.ToString();
                }
            }

            int buttonIndex = i;
            newButton.onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
    }

    private void OnButtonClick(int index)
    {
        SceneManager.LoadSceneAsync(index + 1);
    }
}
