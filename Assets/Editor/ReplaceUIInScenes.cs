using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class ReplaceChildUIInScenes : EditorWindow
{
    [MenuItem("Tools/Replace Child UI in All Scenes")]
    public static void ReplaceChildUI()
    {
        // 1. Путь к префабу вашей новой панели способностей
        GameObject panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Instruments/Instruments.prefab");
        // 2. Папка со сценами уровней
        string scenesFolder = "Assets/Scenes/Story";

        // 3. Имя родительского объекта на сцене (куда нужно положить префаб)
        string parentObjectName = "---GAME---";
        // 4. Имя старой дочерней панели внутри родителя, которую надо удалить
        string oldPanelName = "Instruments";

        if (panelPrefab == null)
        {
            Debug.LogError("Не найден префаб панели по указанному пути!");
            return;
        }

        string[] sceneFiles = Directory.GetFiles(scenesFolder, "*.unity", SearchOption.AllDirectories);

        foreach (string scenePath in sceneFiles)
        {
            var scene = EditorSceneManager.OpenScene(scenePath);

            // Ищем родительский Canvas на сцене
            GameObject parentObj = FindObjectInScene(scene, parentObjectName);
            if (parentObj == null)
            {
                Debug.LogWarning($"На сцене {scene.name} не найден {parentObjectName}, пропускаем.");
                continue;
            }

            // Ищем старую панель среди дочерних объектов Canvas (даже если она выключена)
            Transform oldPanel = FindDeepChild(parentObj.transform, oldPanelName);
            Transform parentContainer = oldPanel.parent;
            int siblingIndex = -1;

            if (oldPanel != null)
            {
                // Запоминаем порядковый номер в иерархии (чтобы новая панель встала на то же место)
                siblingIndex = oldPanel.GetSiblingIndex();
                DestroyImmediate(oldPanel.gameObject);
            }

            // Создаем новый префаб сразу внутри Canvas (второй аргумент — родитель)
            GameObject newPanel = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, parentContainer);

            // Возвращаем на то же место в списке дочерних объектов (важно для порядка отрисовки UI)
            if (siblingIndex != -1)
            {
                newPanel.transform.SetSiblingIndex(siblingIndex);
            }

            EditorSceneManager.SaveScene(scene);
        }

        Debug.Log($"Готово! Обновлено сцен: {sceneFiles.Length}");
    }

    // Метод ищет любой объект на сцене по имени, даже если он или его родители выключены
    private static GameObject FindObjectInScene(UnityEngine.SceneManagement.Scene scene, string objectName)
    {
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            if (rootObj.name == objectName)
                return rootObj;

            Transform foundChild = FindDeepChild(rootObj.transform, objectName);
            if (foundChild != null)
                return foundChild.gameObject;
        }
        return null;
    }

    // Вспомогательный метод для поиска дочернего объекта на любой глубине вложенности
    private static Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
