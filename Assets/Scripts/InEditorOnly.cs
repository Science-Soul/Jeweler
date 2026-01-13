using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InEditorOnly : MonoBehaviour
{
#if UNITY_EDITOR
    void OnGUI()
    {
        Rect buttonRect = new Rect(1400, 200, 200, 150);
        if (GUI.Button(buttonRect, "Дальше")){
            gameObject.GetComponent<SceneFinisher>().LoadNextScene();
        }
    }
#endif
}
