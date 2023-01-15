using Lean.Common;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEditorRuntime : MonoBehaviour
{
    #region Singleton
    public static LevelEditorRuntime Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    public GridManager gridManager;
    public Transform formsToFillTheGrid;
    public GameObject formParent;

    private List<LeanSelectable> matSelected = new List<LeanSelectable>();

    public void Start()
    {
        if(GridManager.Instance.GetGrid.Count > 0)
        {
            GridManager.Instance.GetOriginalGridColor = GridManager.Instance.GetGrid[0].GetComponent<Image>().color;
        }

        for (int i = 0; i < GridManager.Instance.GetGrid.Count; i++)
        {
            LeanSelectableByFinger obj = GridManager.Instance.GetGrid[i].GetComponent<LeanSelectableByFinger>(); ;
            obj.OnSelected.AddListener(OnSelect);
            obj.OnDeselected.AddListener(OnDeselect);
        }
    }

    void Update()
    {
        // Check if every blocks are colored different as white
        // then activate validate Btn
        if(matSelected.Count > 0)
        {
            for(int i = 0; i < matSelected.Count; i++)
            {
                //matSelected[i].GetComponent<Image>().color = colorPicker.color;
            }
        }
    }

    public void OnSelect(LeanSelect select)
    {
        matSelected.Clear();
        matSelected.AddRange(select.Selectables);
        for(int i = 0; i < matSelected.Count; i++)
        {
            matSelected[i].GetComponent<Outline>().enabled = true;
        }
    }

    public void OnDeselect(LeanSelect select)
    {
        for (int i = 0; i < matSelected.Count; i++)
        {
            matSelected[i].GetComponent<Outline>().enabled = false;
        }
        select.Selectables.Clear();
        matSelected.Clear();
    }

    public void OnValidateForm()
    {
        if(CheckIfBlocksAreFormed())
        {
            for(int i = 0; i < GridManager.Instance.GetGrid.Count; i++)
            {
                Instantiate(GridManager.Instance.GetGrid[i], GridManager.Instance.GetGrid[i].transform.localPosition, Quaternion.identity);
            }
            Debug.Log("Form validated and created.");
        }
        else
        {
            Debug.Log("You have to select at least 1 square to create a form.");
        }
    }

    private bool CheckIfBlocksAreFormed()
    {
        for (int i = 0; i < GridManager.Instance.GetGrid.Count; i++)
        {
            GameObject block = GridManager.Instance.GetGrid[i];
            if(block.GetComponent<Image>().color == GridManager.Instance.GetOriginalGridColor)
            {
                return false;
            }
        }
        return true;
    }

    public void OnCancelForms()
    {
        for(int i = 0;i < GridManager.Instance.GetGrid.Count; i++)
        {
            GridManager.Instance.GetGrid[i].GetComponent<Image>().color = GridManager.Instance.GetOriginalGridColor;
        }
    }

    public void RestartEverything()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
