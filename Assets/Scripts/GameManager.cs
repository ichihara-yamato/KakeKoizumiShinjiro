using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game flow logic: score tracking, energy UI updates, and ultimate effect handling.
/// Attach this to a persistent manager GameObject in your scene.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private GameObject restartButton;

    [Header("Ultimate Feedback")]
    [SerializeField] private ParticleSystem ultimateEffectPrefab;
    [SerializeField] private CanvasGroup screenFlashCanvas;
    [SerializeField] private float flashDuration = 0.25f;
    [SerializeField] private AudioClip ultimateSfx;

    [Header("Scoring")]
    [SerializeField] private float scoreRate = 1f;

    private AudioSource audioSource;
    private float score;
    private bool gameOver;
    private Coroutine flashRoutine;

    public bool IsGameOver => gameOver;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        if (screenFlashCanvas != null)
        {
            screenFlashCanvas.alpha = 0f;
        }
    }

    private void Start()
    {
        score = 0f;
        UpdateScoreText();
        UpdateEnergy(0f);
    }

    private void Update()
    {
        if (gameOver) return;

        score += Time.deltaTime * scoreRate;
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        if (gameOver) return;

        score += points;
        UpdateScoreText();
    }

    public void UpdateEnergy(float energy)
    {
        if (energyText == null) return;

        energyText.text = $"エナジー: {Mathf.Clamp(Mathf.FloorToInt(energy), 0, 100)}%";
    }

    public void TriggerUltimate(Vector3 playerPosition)
    {
        if (gameOver) return;

        ClearAllObstacles();
        PlayUltimateFeedback(playerPosition);
    }

    private void ClearAllObstacles()
    {
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle);
        }
    }

    private void PlayUltimateFeedback(Vector3 origin)
    {
        if (ultimateEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(ultimateEffectPrefab, origin, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetimeMultiplier);
        }

        if (screenFlashCanvas != null)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine());
        }

        if (ultimateSfx != null)
        {
            audioSource.PlayOneShot(ultimateSfx);
        }
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;
        screenFlashCanvas.alpha = 1f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float normalized = timer / flashDuration;
            screenFlashCanvas.alpha = Mathf.Lerp(1f, 0f, normalized);
            yield return null;
        }

        screenFlashCanvas.alpha = 0f;
    }

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        if (restartButton != null)
        {
            restartButton.SetActive(true);
        }

        if (scoreText != null)
        {
            int scoreInt = Mathf.FloorToInt(score);
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (scoreInt > highScore)
            {
                PlayerPrefs.SetInt("HighScore", scoreInt);
                highScore = scoreInt;
            }

            scoreText.text = $"小泉、信頼を勝ち取れ！\n国民の評価: {scoreInt}\nHighScore: {highScore}";
        }
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void UpdateScoreText()
    {
        if (scoreText == null) return;
        scoreText.text = $"国民の評価: {Mathf.FloorToInt(score)}";
    }
}
