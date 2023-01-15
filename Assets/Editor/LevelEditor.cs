using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using PlasticGui.WorkspaceWindow.Home;

[CustomEditor(typeof(LevelEditorRuntime))]
public class LevelEditor : Editor
{
    private LevelEditorRuntime lvlEdit;
    private GridManager gridMana;

    private int width = 0;
    private int height = 0;

    private Color colorChosen;
    private Color originalColor;
    private Sprite originalCellImg;

    private int sceneLevelNumber;

    private bool gridCreated;
    private bool resetColor;

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
            originalCellImg = gridMana.GetGrid[0].GetComponent<Image>().sprite;
            originalColor = gridMana.GetGrid[0].GetComponent<Image>().color;

            gridCreated = true;
            createGrid = false;
        }

        resetColor = EditorGUILayout.Toggle("Reset Color ? ", resetColor);

        colorChosen = EditorGUILayout.ColorField(colorChosen);

        // Show grid in the editor
        if (gridCreated)
        {
            int cellCount = 0;
            if(gridLayout == GridLayout.Horizontal)
            {
                if (resetColor)
                {
                    for(int i = 0; i < gridMana.GetGrid.Count; i++)
                    {
                        gridMana.GetGrid[i].GetComponent<Image>().sprite = originalCellImg;
                        gridMana.GetGrid[i].GetComponent<Image>().color = originalColor;
                    }
                    SceneView.RepaintAll();
                }
                else
                {
                    for (int i = 0; i < height; i++)
                    {
                        GUILayout.BeginHorizontal();
                        for(int j = 0; j < width; j++)
                        {
                            if(GUILayout.Button(" "))
                            {
                                gridMana.GetGrid[cellCount].GetComponent<Image>().color = colorChosen;
                                SceneView.RepaintAll();
                            }
                            cellCount++;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                for (int i = 0; i < width; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < height; j++)
                    {
                        GUILayout.Button(" ");
                    }
                    GUILayout.EndHorizontal();
                }
            }

        }

        // Create Level (Scene)
        bool createLevel = GUILayout.Button("Create Level");

        if (createLevel)
        {
            List<GameObject> formsCreated = new List<GameObject>();

            for(int i = 0; i < gridMana.GetGrid.Count; i++)
            {
                formsCreated.Add(gridMana.GetGrid[i]);
            }

            for (int i = 0; i < formsCreated.Count; i++)
            {
                List<GameObject> newFormCreated = new List<GameObject>();

                GameObject form = formsCreated[i];

                newFormCreated.Add(form);

                formsCreated.Remove(form);

                for (int j = 0; j < formsCreated.Count; j++)
                {
                    if(formsCreated[j].GetComponent<Image>().color == form.GetComponent<Image>().color)
                    {
                        newFormCreated.Add(formsCreated[j]);
                        formsCreated.Remove(formsCreated[j]);
                        j--;
                    }
                }

                GameObject formsParent = Instantiate(lvlEdit.formParent, lvlEdit.formsToFillTheGrid);

                for (int j = 0; j < newFormCreated.Count; j++)
                {
                    GameObject clone = Instantiate(newFormCreated[j], formsParent.transform);
                    clone.GetComponent<RectTransform>().sizeDelta = new Vector2(125,125);
                }

                i--;
            }

            for (int i = 0; i < gridMana.GetGrid.Count; i++)
            {
                gridMana.GetGrid[i].GetComponent<Image>().sprite = originalCellImg;
                gridMana.GetGrid[i].GetComponent<Image>().color = originalColor;
            }

            Scene actualScene = EditorSceneManager.GetActiveScene();
            sceneGameObjects = actualScene.GetRootGameObjects();

            EditorSceneManager.newSceneCreated += SetLevelInNewScene;

            Scene sceneCreated = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            // Name The Scene and Save the scene
            string path = Application.dataPath + "/Scenes/Level " + sceneLevelNumber++ + ".unity";
            PlayerPrefs.SetInt("SceneLevelNumber", sceneLevelNumber);

            //EditorSceneManager.SaveScene(sceneCreated, path, false);
            //EditorSceneManager.CloseScene(sceneCreated, true);
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
