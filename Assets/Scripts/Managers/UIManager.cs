using DG.Tweening;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    # region Singleton
    public static UIManager Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    #endregion

    [SerializeField] private Sprite spriteAudioVolumeOn;
    [SerializeField] private Sprite spriteAudioVolumeOff;
    [SerializeField] private Image imgAudioUI;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private List<Sprite> winEmojis = new List<Sprite>();
    [SerializeField] private Image imgEmoji;

    private GridManager gridManager;
    private Tween audioTween;

    private bool audioMuted;

    private void Start()
    {
        // Get the GridManager
        gridManager = GridManager.Instance;

        // Deactivate winScreen
        winScreen.SetActive(false);

        // Check if the player muted the audio in a previous game
        if (PlayerPrefs.GetFloat("AudioMuted") == 0)
        {
            AudioManager.Instance.AudioSource.mute = false;
            imgAudioUI.sprite = spriteAudioVolumeOn;
            audioMuted = false;
        }
        else
        {
            AudioManager.Instance.AudioSource.mute = true;
            imgAudioUI.sprite = spriteAudioVolumeOff;
            audioMuted = true;
        }
    }

    public void ResetFormsPostion()
    {
        // Reset Block Positions to the bottom of the grid like it used to be before the blocks were in the grid
        for(int i = 0; i < gridManager.BlocksInGrid.Count; i++)
        {
            BlockBehaviour block = gridManager.BlocksInGrid[i];
            // For each block, we reset its parent to the original parent
            // Then we reset their localPos to be placed as game Start
            // We reset scale
            // We remove tag to avoid any problems
            block.transform.SetParent(block.BlockParent);
            block.transform.localPosition =block.GetOriginalLocalPos;
            block.transform.localScale = block.GetOriginalLocalScale;
            block.BlockParent.tag = "Untagged";

            // The form isn't in the grid anymore
            block.InGrid = false;

            GameManager.Instance.BlocksToMove.Add(block);
        }

        // There are no longer blocks in the grid
        gridManager.BlocksInGrid.Clear();
    }

    public void RestartFromLevelIntro()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MuteAudio()
    {
        // have to keep the audio muted if we reload the game
        if (audioTween == null)
        {
            audioTween = imgAudioUI.transform.DOPunchScale(imgAudioUI.transform.localScale / 3, 0.1f, 3);
        }
        else if (audioTween != null && !audioTween.IsPlaying())
        {
            audioTween = imgAudioUI.transform.DOPunchScale(imgAudioUI.transform.localScale / 3, 0.1f, 3);
        }

        if (!audioMuted)
        {
            AudioManager.Instance.AudioSource.mute = true;
            PlayerPrefs.SetFloat("AudioMuted", 1);
            imgAudioUI.sprite = spriteAudioVolumeOff;
            audioMuted = true;
        }
        else
        {
            AudioManager.Instance.AudioSource.mute = false;
            PlayerPrefs.SetFloat("AudioMuted", 0);
            imgAudioUI.sprite = spriteAudioVolumeOn;
            audioMuted = false;
        }
        PlayerPrefs.Save();
    }

    public void ChooseRandomEmojis()
    {
        int randomEmoji = Random.Range(0, winEmojis.Count);
        imgEmoji.sprite = winEmojis[randomEmoji];
        winScreen.SetActive(true);
    }

    public void OnTapToContinue(LeanFinger finger)
    {
        // We change scene to the next level and we save the current progression
        winScreen.SetActive(false);
        PlayerPrefs.SetInt("SceneLevel", ++GameManager.Instance.LvlNumber);
        PlayerPrefs.Save();
        GameManager.Instance.ChangeLevel();
    }
}
