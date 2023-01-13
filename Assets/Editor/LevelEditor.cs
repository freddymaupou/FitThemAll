using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LevelEditor : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private int width;
    private int height;

    private bool test;
    private bool gridCreated;

    private Color colorChosen;

    private List<Texture2D> gridText = new List<Texture2D>();

    private enum Way
    {
        Horizontal,
        Vertical,
    };

    private Way way;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Level Editor/Level Editor")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelEditor window = (LevelEditor)GetWindow(typeof(LevelEditor));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        way = (Way)EditorGUILayout.EnumPopup("Display in which way ?", way);
        bool createGrid = GUILayout.Button("Create Grid");
        if (createGrid)
        {
            gridText.Clear();
            gridCreated = true;
            for(int i = 0; i < width*height; i++)
            {
                Texture2D LogoTex = Resources.Load("SquareToMove") as Texture2D; //don't put png
                gridText.Add(LogoTex);
            }
        }

        GUILayout.EndHorizontal();

        colorChosen = EditorGUILayout.ColorField("Choose Block Color", colorChosen);

        if (gridCreated)
        {
            //Debug.Log(gridText.Count);
            if(way == Way.Horizontal)
            {
                for (int k = 0; k < gridText.Count; k++)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Box(gridText[k], GUILayout.MaxHeight(64), GUILayout.MaxWidth(64));

                    Rect clickArea = new Rect(0, 0, gridText[k].width, gridText[k].height);

                    Event current = Event.current;

                    if(clickArea.Contains(current.mousePosition) && current.type == EventType.MouseDown)
                    {
                        Color[] newCol = new Color[gridText[k].width * gridText[k].height];
                        for(int i =0; i<newCol.Length; i++)
                        {
                            newCol[i] = colorChosen;
                        }

                        gridText[k].SetPixels(newCol);
                        gridText[k].Apply();
                    }

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                for (int i = 0; i < height; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < width; j++)
                    {
                        bool newSquare = GUILayout.Button("B", GUILayout.Width(30), GUILayout.Height(30));
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        GUIStyle s = new GUIStyle(GUI.skin.box);



        if (test)
        {
            s.normal.background = MakeTex(30, 30, colorChosen);
        }
        else
        {
            s.normal.background = MakeTex(30, 30, Color.green);
        }

        /*if (GUI.Button(new Rect(10, 130, 170, 30), "Click", s))
        {
            Debug.Log("Clicked the button with text");
            test = !test;
        }*/

        



        myString = EditorGUILayout.TextField("Text Field", myString);
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();


    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        col = new Color(col.r, col.g, col.b, 1f);
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
