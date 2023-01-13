using Lean.Common;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorRuntime : MonoBehaviour
{
    public static LevelEditorRuntime Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public FlexibleColorPicker colorPicker;
    private List<LeanSelectable> matSelected = new List<LeanSelectable>();

    public void Start()
    {
        for(int i = 0; i < GridManager.Instance.GetGrid.Count; i++)
        {
            LeanSelectableByFinger obj = GridManager.Instance.GetGrid[i].GetComponent<LeanSelectableByFinger>(); ;
            obj.OnSelected.AddListener(OnSelect);
            obj.OnDeselected.AddListener(OnDeselect);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(matSelected.Count > 0)
        {
            for(int i = 0; i < matSelected.Count; i++)
            {
                matSelected[i].GetComponent<Image>().color = colorPicker.color;
            }
        }
    }

    public void OnSelect(LeanSelect select)
    {
        matSelected.AddRange(select.Selectables);
    }

    public void OnDeselect(LeanSelect select)
    {
        matSelected.Clear();
    }
}
