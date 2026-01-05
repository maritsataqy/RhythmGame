using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada buat pindah scene!

public class MainMenuManager : MonoBehaviour
{
    [Header("Daftar Panel")]
    public GameObject panelMainMenu;  // Wadah Judul & Tombol Play awal
    public GameObject panelRules;     // Wadah Info Cara Main
    public GameObject panelSelect;    // Wadah Pilih Karakter

    [Header("Pengaturan Audio")]
    public AudioSource musicSource;

    void Start()
    {
        // Pas game mulai, kalau ada lagunya, langsung play!
        if (musicSource != null)
        {
            musicSource.Play();
        }
    }
   

    public void KeluarGame()
    {
        Application.Quit();
        Debug.Log("Keluar Game");
    }

    // --- BAGIAN 1: TOMBOL DI MENU UTAMA ---

    public void TekanTombolPlay()
    {
        // Tutup menu utama, Buka menu pilih karakter
        panelMainMenu.SetActive(false);
        panelSelect.SetActive(true);
    }

    public void TekanTombolRules()
    {
        // Buka panel rules
        panelRules.SetActive(true);
        
        // UPDATE BARU: Matikan menu utama biar GAK NUMPUK!
        panelMainMenu.SetActive(false);
    }

    // --- BAGIAN 2: TOMBOL BACK / KEMBALI ---

    public void TekanBackDariRules()
    {
        // Tutup panel rules
        panelRules.SetActive(false);
        
        // UPDATE BARU: Nyalakan lagi menu utama
        panelMainMenu.SetActive(true);
    }

    public void TekanBackDariSelect()
    {
        // Tutup pilih karakter, Balik ke menu utama
        panelSelect.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    // --- BAGIAN 3: PILIH HERO & MULAI GAME ---

    public void PilihLevel(string namaScene)
    {
        SceneManager.LoadScene(namaScene);
    }
}