using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

[CustomEditor(typeof(LevelEditorRuntime))]
public class LevelEditor : Editor
{
    private LevelEditorRuntime lvlEdit;
    private GridManager gridMana;

    private int width = 0;
    private int height = 0;
    private Color colorChosen;

    private int sceneLevelNumber;

    private enum GridLayout
    {
        Horizontal,
        Vertical,
    }

    private GridLayout gridLayout;
    private GameObject[] sceneGameObjects;

    private void OnEnable()
    {
        lvlEdit = (LevelEditorRuntime)target;
        gridMana = lvlEdit.gridManager;
        if (!PlayerPrefs.HasKey("SceneLevelNumber"))
        {
            PlayerPrefs.SetInt("SceneLevelNumber", sceneLevelNumber);
        }
        else
        {
            sceneLevelNumber = PlayerPrefs.GetInt("SceneLevelNumber");
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        gridLayout = (GridLayout)EditorGUILayout.EnumPopup("Grid Display", gridLayout);

        bool createGrid = GUILayout.Button("Create Grid");

        if (createGrid)
        {
            if(gridLayout == GridLayout.Horizontal)
            {
                gridMana.GetGridLayoutGroup.constraintCount = width;
            }
            else
            {
                gridMana.GetGridLayoutGroup.constraintCount = height;
            }
            gridMana.GridSize = width * height;
            gridMana.SpawnGrid();

            for(int i = 0; i < width; i++)
            {
                GUILayout.BeginHorizontal();
                for(int j = 0; j < height; j++)
                {

                }
                GUILayout.EndHorizontal();
            }
        }

        colorChosen = EditorGUILayout.ColorField(colorChosen);
        // Show grid in the editor


        // Creat Level (Scene)
        bool createLevel = GUILayout.Button("Create Level");

        if (createLevel)
        {
            Scene actualScene = EditorSceneManager.GetActiveScene();
            sceneGameObjects = actualScene.GetRootGameObjects();

            EditorSceneManager.newSceneCreated += SetLevelInNewScene;

            Scene sceneCreated = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            // Name The Scene and Save the scene
            string path = Application.dataPath + "/Scenes/Level" + sceneLevelNumber++ +".unity";
            PlayerPrefs.SetInt("SceneLevelNumber", sceneLevelNumber);

            EditorSceneManager.SaveScene(sceneCreated, path, false);
            EditorSceneManager.CloseScene(sceneCreated, true);
            Debug.Log("Saved Scene " + path);
        }
    }

    private void SetLevelInNewScene(Scene newSceneCreated, NewSceneSetup setup, NewSceneMode mode)
    {
        // To prevent any multiple same GameObjects
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < sceneGameObjects.Length; i++)
        {
            gameObjects.Add(sceneGameObjects[i]);
        }

        for (int i = 0; i < gameObjects.Count; i++)
        {
            Instantiate(sceneGameObjects[i]);
        }
        EditorSceneManager.newSceneCreated -= SetLevelInNewScene;
    }
}
