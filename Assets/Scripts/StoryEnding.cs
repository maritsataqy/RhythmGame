using UnityEngine;
using TMPro;           
using UnityEngine.SceneManagement; 
using System.Collections;

public class StoryEnding : MonoBehaviour
{
    [Header("--- Konfigurasi Cerita ---")]
    public TextMeshProUGUI textDisplay; 
    public float typingSpeed = 0.04f;   
    public float delayAfterEnding = 5.0f; // Waktu tunggu (5 detik)
    
    [TextArea(3, 10)] 
    public string[] sentences; 

    [Header("--- Tujuan Akhir ---")]
    public string sceneMainMenu = "MainMenu"; 

    private int index = 0;
    private bool isTyping = false; 
    private bool isFinished = false; // Penanda kalau cerita udah tamat

    void Start()
    {
        StartCoroutine(Type());
    }

    void Update()
    {
        // Kalau cerita sudah tamat (lagi nunggu 5 detik), matikan tombol klik
        if (isFinished) return;

        // Kalau pemain klik mouse / tap layar
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Kalau lagi ngetik, langsung selesaikan (SKIP)
                StopAllCoroutines();
                textDisplay.text = sentences[index];
                isTyping = false;

                // Cek: Apakah ini kalimat terakhir yang baru aja di-skip?
                if (index >= sentences.Length - 1)
                {
                    StartCoroutine(WaitAndQuit());
                }
            }
            else
            {
                // Lanjut ke kalimat berikutnya
                NextSentence();
            }
        }
    }

    IEnumerator Type()
    {
        isTyping = true;
        textDisplay.text = ""; 
        
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        
        isTyping = false;

        // --- [LOGIKA BARU] ---
        // Kalau ini adalah kalimat TERAKHIR, langsung mulai hitung mundur
        if (index >= sentences.Length - 1)
        {
            StartCoroutine(WaitAndQuit());
        }
    }

    void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";
            StartCoroutine(Type());
        }
        // Tidak perlu "else" di sini, karena logika tamatnya sudah pindah ke Type() & Update()
    }

    IEnumerator WaitAndQuit()
    {
        // Kunci biar pemain gak bisa klik-klik lagi
        isFinished = true;
        
        Debug.Log("Cerita Tamat. Menunggu " + delayAfterEnding + " detik...");
        
        // Tunggu 5 Detik
        yield return new WaitForSecondsRealtime(delayAfterEnding);

        // Balik ke Menu
        Time.timeScale = 1f; 
        SceneManager.LoadScene(sceneMainMenu);
    }
}