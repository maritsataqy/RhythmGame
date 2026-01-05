using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; 
    // Kita hapus variabel GameplayUI dan LaguGame biar kamu gak bingung ngisinya.

    void Update()
    {
        // Tombol ESC buat nge-test di komputer
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true); // Munculin menu pause
        
        Time.timeScale = 0f; // Waktu game berhenti total
        AudioListener.pause = true; // MATIKAN SEMUA SUARA (Global Mute)
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); // Sembunyikan menu pause
        
        Time.timeScale = 1f; // Waktu jalan lagi
        AudioListener.pause = false; // Nyalakan lagi semua suara
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
        SceneManager.LoadScene("MainMenu"); // Pastikan nama scene menu bener
    }
}