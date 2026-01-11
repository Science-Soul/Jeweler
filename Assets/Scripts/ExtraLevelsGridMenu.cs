using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraLevelsGridMenu : MonoBehaviour
{
    public Button buttonPrefab;
    public Transform gridParent;
    private int startExtraLevelsSceneIndex;
    // Start is called before the first frame update
    void Awake()
    {
        startExtraLevelsSceneIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/ExtraLevels/_ORDERED/1.1_StartExtraLevels.unity");
        GenerateButtons();
    }

    private void GenerateButtons()
    {
        for (int i = startExtraLevelsSceneIndex - 1; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            Button newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(gridParent, false);

            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                    buttonText.text = (i - startExtraLevelsSceneIndex + 2).ToString();
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
