using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; 
    
    [Header("Komponen UI")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI comboText; 
    public Slider healthBarHero; 
    public Slider healthBarBoss; 

    [Header("Komponen Audio")]
    public AudioSource musicSource; 

    [Header("Settingan Hero")]
    public int maxHealthHero = 100;    
    public int damagePerMiss = 10;
    public int healPerHit = 2; // <-- BARU: Kalau kena, sembuh 2 poin
    
    [Header("Settingan Boss")]
    public int maxHealthBoss = 1000; 
    public int damageKeBoss = 10;    

    [Header("Settingan Lain")]
    public int hukumanCombo = 10;

    // Variabel Privat
    int currentScore = 0;
    int currentCombo = 0; 
    int currentHealthHero; 
    int currentHealthBoss; 

    void Awake()
    {
        instance = this; 
    }

    void Start()
    {
        currentScore = 0;
        currentCombo = 0;
        currentHealthHero = maxHealthHero; 
        currentHealthBoss = maxHealthBoss; 
        
        Time.timeScale = 1; 
        UpdateUI();
    }

    public void AddScore(int nilai)
    {
        currentScore += nilai;
        UpdateUI();
    }

    // Fungsi Kalau Kena (Combo Nambah + Boss Sakit + HERO SEMBUH)
    public void Hit()
    {
        currentCombo++;

        // 1. Serang Boss
        currentHealthBoss -= damageKeBoss;
        if (currentHealthBoss <= 0)
        {
            currentHealthBoss = 0;
            Victory(); 
        }

        // 2. HERO SEMBUH (REGEN) <-- TAMBAHAN BARU
        currentHealthHero += healPerHit;
        
        // PENTING: Jangan sampai nyawa lebih dari 100%
        if (currentHealthHero > maxHealthHero)
        {
            currentHealthHero = maxHealthHero;
        }

        UpdateUI();
    }

    public void Miss()
    {
        currentCombo -= hukumanCombo;
        if (currentCombo < 0) currentCombo = 0;

        // Hero Sakit
        currentHealthHero -= damagePerMiss;

        if (currentHealthHero <= 0)
        {
            currentHealthHero = 0;
            GameOver(); 
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
        if (comboText != null) comboText.text = currentCombo.ToString();

        if (healthBarHero != null)
        {
            healthBarHero.maxValue = maxHealthHero; 
            healthBarHero.value = currentHealthHero; 
        }

        if (healthBarBoss != null)
        {
            healthBarBoss.maxValue = maxHealthBoss;
            healthBarBoss.value = currentHealthBoss;
        }
    }

    void GameOver()
    {
        Debug.Log("KALAH! HERO MATI!");
        if (musicSource != null) musicSource.Stop();
        Time.timeScale = 0; 
    }

    void Victory()
    {
        Debug.Log("MENANG! BOSS KALAH!");
        if (musicSource != null) musicSource.Stop();
        Time.timeScale = 0; 
    }
}