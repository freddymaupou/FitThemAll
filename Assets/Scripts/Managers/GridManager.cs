using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script used to have references to the forms put in the grid and if it's complete or not
public class GridManager : MonoBehaviour
{
    #region Singleton
    public static GridManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        grid.AddRange(GameObject.FindGameObjectsWithTag("SquareInGrid"));
        if(grid.Count > 0)
        {
            originalGridColor = grid[0].GetComponent<Image>().color;
        }

    }
    #endregion

    [SerializeField] private List<BlockBehaviour> blocksInGrid = new List<BlockBehaviour>();
    [SerializeField] private int gridSize = 10;
    [SerializeField] private GameObject squarePrefab;

    private List<GameObject> grid = new List<GameObject>();
    private Color originalGridColor;

    public List<BlockBehaviour> BlocksInGrid => blocksInGrid;
    public List<GameObject> GetGrid => grid;
    public int GridSize => gridSize;
    public Color GetOriginalGridColor { get { return originalGridColor; } set { originalGridColor = value; } }


    [ContextMenu("Spawn Grid")]
    public void SpawnGrid()
    {
        // Tool for LD to spawn squares in the grid with the gridSize info and the prefab to instantiate
        // We check if the grid already instantiated square, if so, we destroy those GameObjects

        for(int i = 0; i < grid.Count; i++)
        {
            DestroyImmediate(grid[i]);
        }

        grid.Clear();

        for(int i = 0;i < gridSize; i++)
        {
            GameObject squareInGrid = Instantiate(squarePrefab, transform);
            grid.Add(squareInGrid);
        }
        LevelEditorRuntime.Instance.Start();
    }
}
