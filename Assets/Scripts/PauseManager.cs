using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <--- WAJIB ADA BIAR BISA BACA TEKS
using System.Collections; // <--- WAJIB ADA BIAR BISA HITUNG MUNDUR

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public TMP_Text countdownText; // <--- Kotak baru buat masukin teks 3-2-1 tadi

    private bool isPaused = false; // Biar gak bug kalau dipencet-pencet

    void Update()
    {
        // Tombol ESC buat nge-test
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        
        Time.timeScale = 0f; // Bekukan waktu
        AudioListener.pause = true; // Matikan suara (Global Mute)
    }

    // Fungsi ini dipanggil tombol Resume
    public void ResumeGame()
    {
        // Jangan langsung unpause! Kita mulai hitung mundur dulu.
        StartCoroutine(ProsesHitungMundur());
    }

    // Ini Logika Hitung Mundurnya
    IEnumerator ProsesHitungMundur()
    {
        // 1. Sembunyikan Panel Pause biar layar bersih
        pausePanel.SetActive(false);

        // 2. Munculkan Teks Angka
        countdownText.gameObject.SetActive(true);

        // 3. Mulai Hitung (Pakai WaitForSecondsRealtime karena TimeScale lagi 0)
        
        countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f); // Tunggu 1 detik asli

        countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f); // Setengah detik aja buat GO

        // 4. KEMBALI KE GAME
        countdownText.gameObject.SetActive(false); // Sembunyikan angka
        Time.timeScale = 1f; // Waktu jalan lagi
        AudioListener.pause = false; // Suara nyala lagi
        isPaused = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void KeMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}