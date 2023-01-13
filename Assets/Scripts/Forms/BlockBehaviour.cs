using DG.Tweening;
using Lean.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// This script is used to check if the gameobject is touching a image (UI) to its RectTransform to snap the blocks when we can
public class BlockBehaviour : MonoBehaviour
{
    private GridManager gridManager;

    private List<BlockBehaviour> blocks = new List<BlockBehaviour>();

    private RectTransform thisRectTr;
    private RectTransform gridPlace;
    private RectTransform gridToFillParent;
    private Transform blockParent;

    private Vector3 originalLocalPos;
    private Vector3 originalLocalScale;

    private bool canRelease;
    private bool inGrid;

    private string formPlaced;

    private Tween scaleUp;

    #region Get/Set
    public RectTransform GridPlace { get { return gridPlace; } set { gridPlace = value; } }
    public Transform BlockParent => blockParent;
    public List<BlockBehaviour> Blocks => blocks;
    public Vector3 GetOriginalLocalPos => originalLocalPos;
    public Vector3 GetOriginalLocalScale => originalLocalScale;

    public bool CanRelease { get { return canRelease; } set { canRelease = value; } }
    public bool InGrid { get { return inGrid; } set { inGrid = value; } }
    #endregion

    private void Start()
    {
        gridManager = GridManager.Instance;

        blockParent = transform.parent;
        for(int i = 0; i < blockParent.childCount; i++)
        {
            blocks.Add(blockParent.GetChild(i).GetComponent<BlockBehaviour>());
        }

        // Get RectTransforms
        thisRectTr = GetComponent<RectTransform>();
        gridToFillParent = FindObjectOfType<GridManager>().GetComponent<RectTransform>();

        // Get the original pos and scale
        originalLocalPos = transform.localPosition;
        originalLocalScale = transform.localScale;

        formPlaced = "FormPlaced";                  // if all the blocks are in the grid, we tell the parent that its form has been placed
                                                    // this will avoid blocks overlap each other in the grid
    }

    private void Update()
    {
        if(GameManager.Instance.BlockSelected != null)
        {
            #region Raycast behaviour
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = transform.position;

            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (RaycastResult result in raycastResults)
                {
                    if (result.gameObject.TryGetComponent(out BlockBehaviour block))
                    {
                        // Check if there's already a block placed
                        // then break to avoid placement
                        if (block.BlockParent.tag == formPlaced)
                        {
                            break;
                        }
                    }

                    // 3 = SquareToFill => squares in the grid
                    if (result.gameObject.layer == 3)
                    {
                        canRelease = true;
                        gridPlace = result.gameObject.GetComponent<RectTransform>();
                    }
                    else
                    {
                        canRelease = false;
                    }
                }
            }
            #endregion

            #region Outline Effect
            // If the all the blocks can't be released then the outline will be black
            for (int i = 0; i < blocks.Count; i++)
            {
                if (!blocks[i].CanRelease)
                {
                    for (int j = 0; j < blocks.Count; j++)
                    {
                        blocks[j].GetComponent<Outline>().effectColor = Color.black;
                    }
                    return;
                }
            }

            // Else the all the blocks can be released then the outline will be green
            for (int j = 0; j < blocks.Count; j++)
            {
                blocks[j].GetComponent<Outline>().effectColor = Color.green;
            }
            #endregion
        }
    }

    public void OnSelect(LeanSelect select)
    {
        #region Effects
        Vibration.Init();
        Vibration.Vibrate(50);
        AudioManager.Instance.PlaySound("SelectForm");
        for (int j = 0; j < blocks.Count; j++)
        {
            blocks[j].GetComponent<Outline>().enabled = true;
        }
        #endregion

        GameManager.Instance.BlockSelected = this;

        // if the block is in the grid, we remove the block from it
        if (inGrid)
        {
            // Reset children parent
            for(int i = 0; i < blocks.Count; i++)
            {
                if(blocks[i] != thisRectTr)
                {
                    blocks[i].transform.SetParent(transform);
                    blocks[i].GetComponent<BlockBehaviour>().inGrid = false;

                    gridManager.BlocksInGrid.Remove(blocks[i]);
                    GameManager.Instance.BlocksToMove.Add(blocks[i]);
                }
            }

            transform.SetParent(blockParent); // To avoid layers problem

            inGrid = false;
            blockParent.tag = "Untagged";

            gridManager.BlocksInGrid.Remove(this);
            GameManager.Instance.BlocksToMove.Add(this);

        }
        else
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Transform child = transform.parent.GetChild(i);

                if (child.gameObject != gameObject)
                {
                    child.SetParent(transform);
                    i--;
                }
            }
            scaleUp = transform.DOScale(1.3f, 0.1f);
        }
    }

    public bool CheckIfBlocksInGrid()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            // If not every block of the form can be placed (released) in the grid then return false
            // else we return true and can place all the blocks
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            BlockBehaviour block = child.GetComponent<BlockBehaviour>();
            if (!block.CanRelease) return false;
        }

        if (!canRelease) return false;

        inGrid = true;
        return true;
    }

    public void OnDeselect(LeanSelect select)
    {
        #region Effects
        for (int j = 0; j < blocks.Count; j++)
        {
            blocks[j].GetComponent<Outline>().enabled = false;
        }
        AudioManager.Instance.PlaySound("DeselectForm");

        if (scaleUp != null && scaleUp.IsPlaying())
        {
            scaleUp.Kill();
        }
        #endregion

        if (CheckIfBlocksInGrid())
        {
            blockParent.tag = formPlaced;

            #region Set Blocks position in the grid
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
                BlockBehaviour block = child.GetComponent<BlockBehaviour>();

                block.inGrid = true;

                child.SetParent(gridToFillParent);
                child.localPosition = block.gridPlace.localPosition;
                child.localScale = block.gridPlace.localScale;
                i--;
            }

            inGrid = true;

            transform.SetParent(gridToFillParent);
            transform.localPosition = gridPlace.localPosition;
            transform.localScale = gridPlace.localScale;
            #endregion

            if (!gridManager.BlocksInGrid.Contains(this))
            {
                GameManager.Instance.BlocksToMove.Remove(this);
                gridManager.BlocksInGrid.AddRange(blocks);
                GameManager.OnCheckGrid?.Invoke();
            }
        }
        else
        {
            #region Reset Blocks position
            transform.SetParent(blockParent);
            transform.localPosition = originalLocalPos;
            transform.localScale = originalLocalScale;

            for (int i = 0; i < transform.childCount;)
            {
                RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
                BlockBehaviour block = child.GetComponent<BlockBehaviour>();
                child.SetParent(blockParent);
                child.localPosition = block.originalLocalPos;
                child.localScale = block.originalLocalScale;
            }
            #endregion
        }

        // we setting the block selected to null to avoid the BlockBehaviour scripts to use their Update Method
        GameManager.Instance.BlockSelected = null;
    }
}