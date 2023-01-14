using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Lean.Touch;

public delegate void CheckGridDelegate();
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    private BlockBehaviour blockSelected;

    [Header("Forms To Move/ To Play With")]
    [SerializeField] private List<BlockBehaviour> blocksToMove = new List<BlockBehaviour>();
    [SerializeField] private List<Color> colorsToUse = new List<Color>();

    [Header("UI Buttons")]
    [SerializeField] private List<Image> uiButtons = new List<Image>();

    [Header("Informations UI")]
    [SerializeField] private GameObject lvlIntro;
    [SerializeField] private TextMeshProUGUI lvlBottomInGameTxt;
    [SerializeField] private TextMeshProUGUI lvlNumberIntroTxt;

    [Header("Level Settings")]
    [SerializeField] private int lvlNumber = 0;
    [SerializeField] private int lvlMax = 5;

    public static CheckGridDelegate OnCheckGrid;

    #region Get/Set
    public List<BlockBehaviour> BlocksToMove => blocksToMove;
    public BlockBehaviour BlockSelected { get { return blockSelected; } set { blockSelected = value; } }
    public int LvlNumber { get { return lvlNumber; } set { lvlNumber = value; } }
    #endregion

    private void Start()
    {
        BlocksToMove.AddRange(FindObjectsOfType<BlockBehaviour>());

        for (int i = 0; i < BlocksToMove.Count; i++)
        {
            BlocksToMove[i].gameObject.layer = 6;
            if (!BlocksToMove[i].TryGetComponent(out LeanDragTranslate translate))
            {
                LeanSelectableByFinger selectable = blocksToMove[i].GetComponent<LeanSelectableByFinger>();
                translate.Use.RequiredSelectable = selectable;
                selectable.OnSelected.AddListener(blocksToMove[i].OnSelect);
                selectable.OnDeselected.AddListener(blocksToMove[i].OnDeselect);
            }
        }

        // Get lvlMax to prevent any problem by changing any scene and to always have the same maxLvl
        if (PlayerPrefs.HasKey("LevelMax"))
        {
            lvlMax = PlayerPrefs.GetInt("LevelMax");
        }
        else
        {
            PlayerPrefs.SetInt("LevelMax", lvlMax);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("SceneLevel"))
        {
            PlayerPrefs.SetInt("SceneLevel", lvlNumber);
            PlayerPrefs.Save();
        }

        // Choose different colors at start for each form
        RandomBlocksColorAtStart();

        for (int i = 0; i < uiButtons.Count; i++)
        {
            uiButtons[i].gameObject.SetActive(false);
        }

        OnCheckGrid += CheckBlocksInGrid;

        // Display infos before game
        OnLvlStart();
    }

    public void OnLvlStart()
    {
        // Display the current level to the player

        lvlIntro.SetActive(true);

        lvlBottomInGameTxt.DOFade(0, 1.5f);

        for (int i = 0; i < uiButtons.Count; i++)
        {
            uiButtons[i].DOFade(0, 1.5f);
            uiButtons[i].gameObject.SetActive(false);
        }

        lvlNumber = PlayerPrefs.GetInt("SceneLevel");

        lvlBottomInGameTxt.text = "Level " + lvlNumber.ToString();
        lvlNumberIntroTxt.text = "Level " + lvlNumber.ToString();
    }

    public void ChangeLevel()
    {
        int sceneToContinue = PlayerPrefs.GetInt("SceneLevel");
        lvlMax = PlayerPrefs.GetInt("LevelMax");

        //Debug.Log("Scene number :" + sceneToContinue);
        //Debug.Log("Lvl Max :" + lvlMax);

        // We check if it's the last level and if it is we loop through the levels
        if (sceneToContinue > lvlMax)
        {
            ResetSavedLvl();
            sceneToContinue = PlayerPrefs.GetInt("SceneLevel");

            SceneManager.LoadScene(sceneToContinue - 1);
            return;
        }

        // Load next Level
        SceneManager.LoadScene(sceneToContinue);
    }

    public void OnLvlTapped()
    {
        // When we start a lvl, we tap the screen to display the game
        for (int i = 0; i < uiButtons.Count; i++)
        {
            uiButtons[i].gameObject.SetActive(true);
            uiButtons[i].DOFade(1, 1.5f);
        }
        lvlBottomInGameTxt.DOFade(1, 1.5f);
        lvlIntro.SetActive(false);
    }

    private void CheckBlocksInGrid()
    {
        if (blocksToMove.Count > 0) return;

        Win();
    }

    private void Win()
    {
        UIManager.Instance.ChooseRandomEmojis();
    }

    private void OnDestroy()
    {
        // To prevent the delegate to add CheckBlocksInGrid when we load a new Level
        OnCheckGrid -= CheckBlocksInGrid;
    }

    [ContextMenu("Reset Saved Level to Level 1 + Set Lvl Max")]
    public void ResetSavedLvl()
    {
        PlayerPrefs.SetInt("SceneLevel", 1);
        PlayerPrefs.SetInt("LevelMax", lvlMax);
        PlayerPrefs.Save();
    }

    [ContextMenu("Use Random Color For Each Blocks")]
    public void RandomBlocksColorAtStart()
    {
        //List<Color> colorsUsed = new List<Color>();

        for (int i = 0; i < blocksToMove.Count; i++)
        {
            /*int randomColor = Random.Range(0, colorsToUse.Count);
            Color blocksColor = colorsToUse[randomColor];

            // To prevent blocks to have the same colors
            while (colorsUsed.Contains(blocksColor))
            {
                randomColor = Random.Range(0, colorsToUse.Count);
                blocksColor = colorsToUse[randomColor];
            }

            colorsUsed.Add(blocksColor);
*/
            Color randomColor = Random.ColorHSV(0f,1f,1f,1f);
            for (int j = 0; j < blocksToMove[i].Blocks.Count; j++)
            {
                blocksToMove[i].Blocks[j].GetComponent<Image>().color = randomColor;
            }
        }
    }
}
