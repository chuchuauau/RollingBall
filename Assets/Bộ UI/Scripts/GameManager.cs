using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Home + Score")]
    public GameObject mainMenuPanel;
    public GameObject textScore;
    public Slider scoreBar;              // Thanh bar điểm số
    public int maxScore = 25;            // Mốc điểm chiến thắng

    [Header("Setting + About")]
    public GameObject settingsPanel;
    public GameObject aboutImagePopup;

    [Header("Sound Settings")]
    public Slider soundSlider;
    public Button soundButton;
    public Image soundButtonImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public VideoPlayer bgVideo;

    [Header("Game Over & Victory")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public AudioSource gameOverSound;
    public AudioSource victorySound;

    [Header("Nut Pause")]
    public GameObject inGamePauseButton;
    public GameObject pausePanel;

    [Header("Clicker Sound")]
    public AudioSource clickSound;

    private bool isSoundOn = true;
    public static bool isRetrying = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

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
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (textScore != null) textScore.SetActive(false);
            if (scoreBar != null) scoreBar.gameObject.SetActive(false); // Ẩn thanh bar ở Menu chính
            if (inGamePauseButton != null) inGamePauseButton.SetActive(false);
        }

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (aboutImagePopup != null) aboutImagePopup.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (soundSlider != null)
        {
            soundSlider.value = AudioListener.volume;
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        }

        if (soundButton != null)
        {
            soundButton.onClick.AddListener(ToggleSoundButton);
        }

        if (scoreBar != null)
        {
            scoreBar.minValue = 0;
            scoreBar.maxValue = maxScore;
            scoreBar.value = 0;
        }

        UpdateSoundUI();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (textScore != null) textScore.SetActive(true);
        if (scoreBar != null) scoreBar.gameObject.SetActive(true); // Hiện thanh bar khi bắt đầu chơi
        if (inGamePauseButton != null) inGamePauseButton.SetActive(true);

        UpdateScoreUI(0);
    }

    public void PlayClickSound()
    {
        if (clickSound != null && isSoundOn)
        {
            clickSound.Play();
        }
    }


public void OpenPause()
    {
        Time.timeScale = 0;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (inGamePauseButton != null) inGamePauseButton.SetActive(false);

        if (textScore != null) textScore.SetActive(false);
        if (scoreBar != null) scoreBar.gameObject.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (inGamePauseButton != null) inGamePauseButton.SetActive(true);

        if (textScore != null) textScore.SetActive(true);
        if (scoreBar != null) scoreBar.gameObject.SetActive(true);
    }

    public void UpdateScoreUI(int score)
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null && player.scoreTxt != null)
        {
            player.scoreTxt.text = score.ToString();
        }

        if (scoreBar != null)
        {
            scoreBar.value = score;
        }

        if (score >= maxScore)
        {
            ShowVictory();
        }
    }

    public void ShowVictory()
    {
        StartCoroutine(WaitAndShowVictory());
    }

    private IEnumerator WaitAndShowVictory()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 0;
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (textScore != null) textScore.SetActive(false);
        if (scoreBar != null) scoreBar.gameObject.SetActive(false); // Ẩn thanh bar khi thắng
        if (inGamePauseButton != null) inGamePauseButton.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (victorySound != null && isSoundOn)
        {
            victorySound.Play();
        }
    }

    public void OnSoundSliderChanged(float value)
    {
        AudioListener.volume = value;
        isSoundOn = value > 0;
        UpdateSoundUI();
    }

    public void ToggleSoundButton()
    {
        isSoundOn = !isSoundOn;
        float targetVolume = isSoundOn ? 1f : 0f;

        AudioListener.volume = targetVolume;
        if (soundSlider != null)
        {
            soundSlider.value = targetVolume;
        }

        UpdateSoundUI();
    }

    private void UpdateSoundUI()
    {
        if (soundButtonImage != null)
        {
            soundButtonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
        }

        if (bgVideo != null)
        {
            for (ushort i = 0; i < (ushort)bgVideo.audioTrackCount; i++)
            {
                bgVideo.SetDirectAudioMute(i, !isSoundOn);
                if (isSoundOn)
                {
                    bgVideo.SetDirectAudioVolume(i, AudioListener.volume);
                }
            }
        }
    }

    public void OpenSettings() { if (settingsPanel != null) settingsPanel.SetActive(true); if (pausePanel != null) pausePanel.SetActive(false); }
    public void CloseSettings() { if (settingsPanel != null) settingsPanel.SetActive(false); if (pausePanel != null && mainMenuPanel != null && !mainMenuPanel.activeSelf) pausePanel.SetActive(true); }

    public void OpenAbout() { if (aboutImagePopup != null) aboutImagePopup.SetActive(true); }
    public void CloseAbout() { if (aboutImagePopup != null) aboutImagePopup.SetActive(false); }

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
        if (scoreBar != null) scoreBar.gameObject.SetActive(false); // Ẩn thanh bar khi thua
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