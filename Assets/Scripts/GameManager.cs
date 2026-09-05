using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Home + Score")]
    public GameObject mainMenuPanel;
    public GameObject textScore;

    [Header("Setting + About")]
    public GameObject settingsPanel;
    public GameObject aboutImagePopup;

    [Header("Sound")]
    public Image soundButtonImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public VideoPlayer bgVideo;
    private bool isSoundOn = true;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public AudioSource gameOverSound;

    [Header("Nut Pause")]
    public GameObject inGamePauseButton;
    public GameObject pausePanel;

    [Header("Clicker Sound")]
    public AudioSource clickSound;

    public static bool isRetrying = false;

    void Start()
    {
        if (isRetrying)
        {
            isRetrying = false;
            StartGame();
        }
        else
        {
            Time.timeScale = 0;
            mainMenuPanel.SetActive(true);
            if (textScore != null) textScore.SetActive(false);
            if (inGamePauseButton != null) inGamePauseButton.SetActive(false);
        }

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (aboutImagePopup != null) aboutImagePopup.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        mainMenuPanel.SetActive(false);
        if (textScore != null) textScore.SetActive(true);
        if (inGamePauseButton != null) inGamePauseButton.SetActive(true);

        UpdateScoreUI();
    }

    // --- LOGIC AM THANH NUT BAM ---
    public void PlayClickSound()
    {
        if (clickSound != null && isSoundOn)
        {
            clickSound.Play();
        }
    }

    // --- LOGIC MỞ/ĐÓNG PAUSE ---
    public void OpenPause()
    {
        Time.timeScale = 0;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (inGamePauseButton != null) inGamePauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (inGamePauseButton != null) inGamePauseButton.SetActive(true);
    }

    public void UpdateScoreUI()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null && player.scoreTxt != null)
        {
            player.scoreTxt.text = player.score.ToString();
        }
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1f : 0f;
        if (bgVideo != null)
        {
            for (ushort i = 0; i < (ushort)bgVideo.audioTrackCount; i++)
            {
                bgVideo.SetDirectAudioMute(i, !isSoundOn);
            }
        }
        if (soundButtonImage != null)
        {
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }
    }

    public void OpenSettings() { settingsPanel.SetActive(true); if (pausePanel != null) pausePanel.SetActive(false); }
    public void CloseSettings() { settingsPanel.SetActive(false); if (pausePanel != null && !mainMenuPanel.activeSelf) pausePanel.SetActive(true); }

    public void OpenAbout() { aboutImagePopup.SetActive(true); }
    public void CloseAbout() { aboutImagePopup.SetActive(false); }

    public void ShowGameOver()
    {
        StartCoroutine(WaitAndShowGameOver());
    }
    private IEnumerator WaitAndShowGameOver()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 0;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (textScore != null) textScore.SetActive(false);
        if (inGamePauseButton != null) inGamePauseButton.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (gameOverSound != null && isSoundOn)
        {
            gameOverSound.Play();
        }
    }

    public void RestartGame()
    {
        isRetrying = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToHome()
    {
        isRetrying = false;
        Time.timeScale = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}