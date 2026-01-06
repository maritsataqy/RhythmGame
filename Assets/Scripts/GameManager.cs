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

    [Header("--- Character Visuals ---")]
    // [BARU] Script untuk efek kedip merah (Karakter Diam)
    public SimpleHitEffect heroFlash;  

    // Script Boss tetap pakai yang lama (karena Boss masih ada animasi)
    public BossVisuals bossVisuals;     

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

    [Header("Sound Effects")]
    public AudioSource sfxPlayer;  // Speaker buat muter suara
    public AudioClip hitSound;     // Kaset suara Pukulan (Hit)
    public AudioClip missSound;    // Kaset suara Salah (Miss)

    [Header("Karakter (Logic)")]
    // heroChar kita biarkan null/kosong jika tidak pakai animasi
    public RhyrhmCharacter heroChar;
    public RhyrhmCharacter bossChar;
    
    // Private variables
    private float currentHeroHealth;
    private float currentBossHealth;
    
    private List<NoteData> beatmap = new List<NoteData>();      
    private int urutanNote = 0;

    private bool isGameActive = false; 


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
        // --- [CHEAT] Tekan W untuk Menang Instan ---
        if (Input.GetKeyDown(KeyCode.W))
        {
            LevelCleared(); // Panggil fungsi menang
            if (musicSource != null) musicSource.Stop(); // Matikan musik biar hening
        }
        // -------------------------------------------


        
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

            // --- [LOGIKA SWING INI SAYA MATIKAN] ---
            // Karena heroVisuals sudah dihapus (ganti jadi heroFlash)
            /*
            if (!sudahSwing && waktuSwing > 0 && songPosition >= waktuSwing)
            {
                 // Kalau mau pakai efek suara persiapan, bisa taruh sini
                Debug.Log("Game Mulai!");
                sudahSwing = true;
            }
            */
            // -------------------------------------

            // Logika Spawn Note
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

        // Efek Visual Partikel
        if (perfectEffect != null) Instantiate(perfectEffect, effectSpawnPos.position, Quaternion.identity);

        // Efek Suara (Pilih satu sumber audio saja biar rapi, di sini saya pakai sfxPlayer)
        if (sfxPlayer != null && hitSound != null) sfxPlayer.PlayOneShot(hitSound);
        else if (sfxSource != null && hitSound != null) sfxSource.PlayOneShot(hitSound); // Backup

        // [LOGIKA BARU] Hero diam saja, Boss yang bereaksi sakit
        if (bossVisuals != null) bossVisuals.PlayReaction();

        UpdateUI();
    }

    public void NoteHitGood()
    {
        currentBossHealth -= 5f; 
        currentScore += 50;      
        TambahCombo(); 
        
        // Efek Visual Partikel
        if (goodEffect != null) Instantiate(goodEffect, effectSpawnPos.position, Quaternion.identity);

        // Efek Suara
        if (sfxPlayer != null && hitSound != null) sfxPlayer.PlayOneShot(hitSound);
        else if (sfxSource != null && hitSound != null) sfxSource.PlayOneShot(hitSound);

        // Boss bereaksi
        if (bossVisuals != null) bossVisuals.PlayReaction();

        UpdateUI();
    }

    public void NoteMiss()
    {
        currentHeroHealth -= 10f; 
        if (currentHeroHealth < 0) currentHeroHealth = 0;
        currentCombo = 0;
        
        // Efek Partikel Miss
        if (missEffect != null) Instantiate(missEffect, effectSpawnPos.position, Quaternion.identity);

        // --- [UBAH BAGIAN INI: PANGGIL EFEK KEDIP MERAH] ---
        if (heroFlash != null) 
        {
            heroFlash.FlashRed();
        }
        // ----------------------------------------------------

        // Efek Suara Miss
        if (sfxPlayer != null && missSound != null)
        {
            sfxPlayer.PlayOneShot(missSound);
        }

        // Kalau Boss mukul balik pas pemain salah (Jika Boss masih punya animasi attack)
        if (bossChar != null) bossChar.PlayAttack();
        
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
                comboText.text = currentCombo + "\nCOMBO"; 
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