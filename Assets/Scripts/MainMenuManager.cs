using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada buat pindah scene!

public class MainMenuManager : MonoBehaviour
{
    [Header("Daftar Panel")]
    public GameObject panelMainMenu;  // Wadah Judul & Tombol Play awal
    public GameObject panelRules;     // Wadah Info Cara Main
    public GameObject panelSelect;    // Wadah Pilih Karakter

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
        // (Menu utama gak usah dimatiin gpp, biar ketumpuk aja)
    }

    // --- BAGIAN 2: TOMBOL BACK / KEMBALI ---

    public void TekanBackDariRules()
    {
        // Tutup panel rules
        panelRules.SetActive(false);
    }

    public void TekanBackDariSelect()
    {
        // Tutup pilih karakter, Balik ke menu utama
        panelSelect.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    // --- BAGIAN 3: PILIH HERO & MULAI GAME ---

    public void PilihHero(int nomerHero)
    {
        // Simpan pilihan hero ke memori (0=Easy, 1=Normal, 2=Hard)
        PlayerPrefs.SetInt("PilihanKarakter", nomerHero);
        
        // Pindah ke Scene Game (PASTIKAN NAMA SCENE SAMA PERSIS)
        // Ganti "SampleScene" dengan nama scene game kamu yang ada not baloknya
        SceneManager.LoadScene("SampleScene"); 
    }
}