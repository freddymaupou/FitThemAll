using DG.Tweening;
using Lean.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script allows us to select the gameobject and place it to the grid or move out from it
public class FormBehaviour : MonoBehaviour
{
    private RectTransform formsToMoveParent;
    private RectTransform gridParent;
    private RectTransform rectTransform;

    private GameManager gameManager;
    private GridManager gridManager;

    private List<BlockBehaviour> blocksFromForm = new List<BlockBehaviour>();

    private bool inGrid;

    private Vector3 originalFormPos;

    public bool InGrid { get { return inGrid; } set { inGrid = value; } }
    public Vector3 OriginalFormPos => originalFormPos;
    public RectTransform FormsToMoveParent => formsToMoveParent;
    public List<BlockBehaviour> BlocksFromForm => blocksFromForm;

    private void Start()
    {
        // Get RectTransforms
        rectTransform = GetComponent<RectTransform>();
        gridParent = GameObject.FindObjectOfType<GridManager>().GetComponent<RectTransform>();
        formsToMoveParent = GameObject.FindGameObjectWithTag("GridForms").GetComponent<RectTransform>();

        // Get the blocks reference for the script to be aware of what the form is created from
        blocksFromForm.AddRange(transform.GetChild(0).GetComponentsInChildren<BlockBehaviour>());

        // Get the original pos
        originalFormPos = GetComponent<RectTransform>().GetChild(0).transform.localPosition;

        // Get the singleton's Instance
        gameManager = GameManager.Instance;
        gridManager = GridManager.Instance;
    }

    public bool CheckIfBlocksInGrid()
    {
        for(int i = 0; i < blocksFromForm.Count; i++)
        {
            // If not every block of the form can be placed (released) in the grid then return
            // else we return true and can place the entire form
            if (!blocksFromForm[i].CanRelease) return false;
        }
        inGrid = true;
        return true;
    }

    public void OnFormSelected(LeanSelect select)
    {
        // Touch vibration

        // Check if the form is in the grid
        // then reset default settings
        if (inGrid)
        {
           // gridManager.FormInGrid.Remove(this);
            for(int i = 0; i < blocksFromForm.Count; i++)
            {
                blocksFromForm[i].transform.SetParent(blocksFromForm[i].BlockParent);         // reseting the parent to this gameobject transform to be able to reset the position out of the grid if we deselect
                //blocksFromForm[i].transform.parent.SetParent(rectTransform);         // reseting the parent to this gameobject transform to be able to reset the position out of the grid if we deselect
                //blocksFromForm[i].transform.localScale = Vector3.one;     // resize the blocks to have the originalSize
                
                blocksFromForm[i].gameObject.layer = 6;                   // layer = "SquareToFill" => when a block is over this one, we can't place the block
            }
            inGrid = false;                                               // form not in the Grid anymore
        }
        else 
        {
            transform.DOScale(1.3f, 0.1f);
            //gameManager.FormsToMove.Remove(this);
        }

        AudioManager.Instance.PlaySound("SelectForm");

        if (transform.childCount > 0)
        {
            Debug.Log(transform.GetChild(0).name);
            Debug.Log(transform.GetChild(0).childCount);
        }

        for (int i = 0; i < blocksFromForm.Count; i++)
        {
            blocksFromForm[i].GetComponent<Outline>().enabled = true; // Activate Outline for Gamefeel 
        }
        //gameManager.Player.FormSelected = this;                           // we have a reference of what the player selected + it's activate the BlockBehaviour Update method and the Player's one 
    }

    public void OnFormDeselected(LeanSelect select)
    {
        AudioManager.Instance.PlaySound("DeselectForm");

        for (int i = 0; i < blocksFromForm.Count; i++)
        {
            blocksFromForm[i].GetComponent<Outline>().enabled = false;
        }

        // check if the entire form is in the grid to finally place it
        if (CheckIfBlocksInGrid())
        {
            float x = 0f;
            float y = 0f;

            for (int i = 0; i < transform.GetChild(0).childCount;)
            {
                // We can place all the blocks in the anchoredPos ogf the place that he found with the raycast
                // and we change its layer to 3 = SquareToFill to avoid stacking the same place in the grid
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().anchoredPosition = transform.GetChild(0).GetChild(i).GetComponent<BlockBehaviour>().GridPlace.anchoredPosition;
                transform.GetChild(0).GetChild(i).GetComponent<RectTransform>().localScale = transform.GetChild(0).GetChild(i).GetComponent<BlockBehaviour>().GridPlace.localScale;
                transform.GetChild(0).GetChild(i).gameObject.layer = 3;

                // cf (1)
                x += transform.GetChild(0).GetChild(i).GetComponent<BlockBehaviour>().GridPlace.anchoredPosition.x;
                y += transform.GetChild(0).GetChild(i).GetComponent<BlockBehaviour>().GridPlace.anchoredPosition.y;
                transform.GetChild(0).GetChild(i).transform.SetParent(gridParent, false);
            }

            // (1)
            // we place the form parent at the center of the places found to be able to take the blocks out of the grid
            // we set the parent to the grid to be correctly placed
            float centerX = x / blocksFromForm.Count;
            float centerY = y / blocksFromForm.Count;

            //GetComponent<LayoutElement>().enabled = true;
            //GetComponent<LayoutElement>().ignoreLayout = true;

            rectTransform.SetParent(gridParent);
            rectTransform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            rectTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(centerX, centerY, 0);

            //if (!gridManager.FormInGrid.Contains(this))
            /*{
                gridManager.FormInGrid.Add(this);
                GameManager.OnCheckGrid?.Invoke();
            }*/
        }
        else
        {
            // if the form not in the grid (not every block are touching a place in the grid)
            // we place the form back to its orignal pos
            /*Debug.Log(blocksFromForm[0].transform.parent);
            blocksFromForm[0].transform.parent.SetParent(rectTransform);
            Debug.Log(blocksFromForm[0].transform.parent.parent);*/

            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                transform.GetChild(0).GetChild(i).localPosition = transform.GetChild(0).GetChild(i).GetComponent<BlockBehaviour>().GetOriginalLocalPos;
                transform.GetChild(0).GetChild(i).localScale = Vector3.one;
            }

            //gameManager.FormsToMove.Add(this);
            //rectTransform.GetChild(0).SetParent(formsToMoveParent);
            rectTransform.SetParent(FormsToMoveParent);
            transform.localPosition = OriginalFormPos;
            transform.DOScale(1f, 0.1f);

            //GetComponent<LayoutElement>().enabled = false;
            //GetComponent<LayoutElement>().ignoreLayout = false;
        }

        // we setting the form selected to null to avoid the Player and the BlockBehaviour script to use their Update Method
        //gameManager.Player.FormSelected = null;
    }
}
