using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.Build.Content;
using UnityEngine.UI;
using Unity.VisualScripting;
using Lean.Touch;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gm = (GameManager)target;

        // We create buttons to ease the task of any Level Designer
        GUILayout.BeginVertical();

        bool setUpLevel = GUILayout.Button("Set up Level"); // This will take every Forms to fill in the grid in the hierarchy and set them up in the FormToMove list of the GameManager
                                                            // avoiding Level Designer to drag and drop every form in the list
        bool resetLvls = GUILayout.Button("Reset Levels");  // This will reset all levels into level 1

        if (setUpLevel)
        {
            gm.BlocksToMove.Clear();
            gm.BlocksToMove.AddRange(FindObjectsOfType<BlockBehaviour>());

            for(int i = 0; i < gm.BlocksToMove.Count; i++)
            {
                gm.BlocksToMove[i].gameObject.layer = 6;

                LeanSelectableByFinger selFinger = gm.BlocksToMove[i].AddComponent<LeanSelectableByFinger>();

                if(!gm.BlocksToMove[i].TryGetComponent(out LeanDragTranslate translate))
                {
                    translate = gm.BlocksToMove[i].AddComponent<LeanDragTranslate>();
                    translate.Use.RequiredSelectable = selFinger;
                }
            }
        }

        if (resetLvls)
        {
            gm.ResetSavedLvl();
        }


        GUILayout.EndVertical();
    }
}
