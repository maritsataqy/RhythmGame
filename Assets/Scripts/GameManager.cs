using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 
using TMPro;          
using UnityEngine.SceneManagement;

[System.Serializable]
public class NoteData
{
    public float detik;     
    public int arah;        
    public int tipe; 
    public float durasi;    
}

public class GameManager : MonoBehaviour
{
    [Header("--- Audio & Beatmap ---")]
    public AudioSource musicSource;   
    public TextAsset fileBeatmap;
    
    public AudioSource sfxSource;     
    public AudioClip hitSound;        
    public AudioClip missSound;       

    [Header("--- Spawning Setup ---")]
    public GameObject[] cetakanPanah;     
    public GameObject[] cetakanHoldNote; 
    public Transform[] titikMuncul;       
    public float noteTravelTime = 1.4f; 
    
    [Range(-5f, 5f)]
    public float songOffset = 0f;

    [Header("--- Game Stats ---")]
    public float maxHeroHealth = 500f;
    public float maxBossHealth = 5000f; 

    [Header("--- Visual Effects ---")]
    public GameObject perfectEffect; 
    public GameObject missEffect;    
    public GameObject goodEffect;
    public Transform effectSpawnPos; 

    [Header("--- Character Animations ---")]
    // [EDIT] Kita ganti HeroAnim lama dengan Script Visual baru
    public CharacterVisuals heroVisuals;  
    public CharacterReactor bossAnim;     

    [Header("--- UI Panels ---")]
    public GameObject gameOverPanel; 
    public GameObject victoryPanel;

    [Header("--- Score & Combo ---")]
    public float currentScore = 0;
    public int currentCombo = 0; 
    
    [Header("--- UI References ---")]
    public Image heroHealthBar;   
    public Image bossHealthBar;   
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI comboText; 
    
    // Private variables
    private float currentHeroHealth;
    private float currentBossHealth;
    
    private List<NoteData> beatmap = new List<NoteData>();      
    private int urutanNote = 0;

    private bool isGameActive = false; 

    // --- [BARU: VARIABLE BUAT SWING OTOMATIS] ---
    [Header("--- Auto Swing Logic ---")]
    public float leadInTime = 2.0f; // Berapa detik sebelum not pertama swing?
    private float waktuSwing = -1f;
    private bool sudahSwing = false;
    // --------------------------------------------

    void Start()
    {
        currentHeroHealth = maxHeroHealth;
        currentBossHealth = maxBossHealth;
        currentScore = 0;
        currentCombo = 0;
        UpdateUI(); 

        BacaFileBeatmap();

        if (musicSource != null)
        {
            musicSource.Play();
            isGameActive = true; 
        }
    }

    void Update()
    {
        // --- LOGIKA UI ---
        if (comboText != null)
        {
            comboText.transform.localScale = Vector3.Lerp(
                comboText.transform.localScale, Vector3.one, Time.deltaTime * 10f
            );
        }

        if (musicSource != null)
        {
            float songPosition = musicSource.time;

            // --- [BARU: LOGIKA EKSEKUSI SWING] ---
            if (!sudahSwing && waktuSwing > 0 && songPosition >= waktuSwing)
            {
                if (heroVisuals != null) 
                {
                    heroVisuals.PlaySwing();
                    Debug.Log("SWING NOW! Persiapan Tempur.");
                }
                sudahSwing = true;
            }
            // -------------------------------------

            // Logika Spawn Note (PUNYA LO TETAP SAMA)
            if (urutanNote < beatmap.Count)
            {
                float waktuTarget = beatmap[urutanNote].detik + songOffset;
                float waktuMuncul = waktuTarget - noteTravelTime;

                if (songPosition >= waktuMuncul)
                {
                    MunculkanPanah(beatmap[urutanNote]); 
                    urutanNote++;
                }
            }
        
            // LOGIKA MENANG
            if (isGameActive && !musicSource.isPlaying && urutanNote >= beatmap.Count)
            {
                if (currentHeroHealth > 0) LevelCleared();
            }
        }
    }

    // --- LOGIKA GAMEPLAY ---

    public void NoteHitPerfect()
    {
        currentBossHealth -= 10f; 
        currentScore += 100;  
        TambahCombo(); 

        if (sfxSource != null && hitSound != null) sfxSource.PlayOneShot(hitSound);
        if (perfectEffect != null) Instantiate(perfectEffect, effectSpawnPos.position, Quaternion.identity);

        // [EDIT] Panggil visual baru
        if (heroVisuals != null) heroVisuals.PlayAttack();
        if (bossAnim != null) bossAnim.PlayHurt();

        UpdateUI();
    }

    public void NoteHitGood()
    {
        currentBossHealth -= 5f; 
        currentScore += 50;      
        TambahCombo(); 
        
        if (sfxSource != null && hitSound != null) sfxSource.PlayOneShot(hitSound);
        if (goodEffect != null) Instantiate(goodEffect, effectSpawnPos.position, Quaternion.identity);

        // [EDIT] Kalau Good mau nyerang juga, panggil ini:
        if (heroVisuals != null) heroVisuals.PlayAttack();

        UpdateUI();
    }

    public void NoteMiss()
    {
        currentHeroHealth -= 10f; 
        if (currentHeroHealth < 0) currentHeroHealth = 0;
        currentCombo = 0;
        
        if (sfxSource != null && missSound != null) sfxSource.PlayOneShot(missSound);
        if (missEffect != null) Instantiate(missEffect, effectSpawnPos.position, Quaternion.identity);

        // [EDIT] Panggil visual hurt baru
        if (heroVisuals != null) heroVisuals.PlayHurt();

        UpdateUI();

        if (currentHeroHealth <= 0) GameOver(); 
    }

    // --- SYSTEM UTILS ---

    void TambahCombo()
    {
        currentCombo++;
        if (comboText != null) comboText.transform.localScale = Vector3.one * 1.5f; 
    }

    void UpdateUI()
    {
        if (heroHealthBar != null) heroHealthBar.fillAmount = currentHeroHealth / maxHeroHealth;
        if (bossHealthBar != null) bossHealthBar.fillAmount = currentBossHealth / maxBossHealth;
        if (scoreText != null) scoreText.text = currentScore.ToString();

        if (comboText != null)
        {
            if (currentCombo > 0)
            {
                comboText.text = "COMBO\n" + currentCombo; 
                comboText.gameObject.SetActive(true); 
            }
            else comboText.gameObject.SetActive(false); 
        }
    }

    void BacaFileBeatmap()
    {
        if (fileBeatmap == null) return; 

        string[] baris = fileBeatmap.text.Split('\n');
        bool mulaiBaca = false; 
        float waktuNotPertama = -1f; // [BARU] Penampung

        foreach (string b in baris)
        {
            string line = b.Trim(); 
            if (line == "[HitObjects]") { mulaiBaca = true; continue; }

            if (mulaiBaca && line != "")
            {
                string[] data = line.Split(',');

                if (data.Length > 2)
                {
                    float waktu = int.Parse(data[2]) / 1000f;
                    
                    // --- [BARU: DETEKSI NOT PERTAMA] ---
                    if (waktuNotPertama == -1f)
                    {
                        waktuNotPertama = waktu;
                        waktuSwing = waktuNotPertama - leadInTime; // Set jadwal swing
                    }
                    // -----------------------------------

                    int x = int.Parse(data[0]);
                    int tipeNote = int.Parse(data[3]); 
                    int arah = 0;

                    if (x < 128) arah = 0;      
                    else if (x < 256) arah = 1; 
                    else if (x < 384) arah = 2; 
                    else arah = 3;              

                    float panjangHold = 0f;
                    if ((tipeNote & 128) != 0) 
                    {
                        string[] extra = data[5].Split(':');
                        float waktuAkhir = float.Parse(extra[0]) / 1000f;
                        panjangHold = waktuAkhir - waktu; 
                    }

                    beatmap.Add(new NoteData { detik = waktu, arah = arah, tipe = tipeNote, durasi = panjangHold });
                }
            }
        }
    }

    void MunculkanPanah(NoteData note)
    {
        GameObject panahBaru;
        if (note.durasi > 0)
        {
            panahBaru = Instantiate(cetakanHoldNote[note.arah]);
            float speed = panahBaru.GetComponent<GerakTurun>().kecepatan;
            panahBaru.GetComponent<HoldNoteVisual>().SetDuration(note.durasi, speed, note.durasi);
            HoldNote logicScript = panahBaru.GetComponent<HoldNote>();
            if (logicScript != null) logicScript.SetupNote(note.durasi);
        }
        else
        {
            panahBaru = Instantiate(cetakanPanah[note.arah]);
        }
        panahBaru.transform.position = titikMuncul[note.arah].position;
    }

    public void LevelCleared()
    {
        isGameActive = false; 
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        isGameActive = false; 
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; 
        if (musicSource != null) musicSource.Stop(); 
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}