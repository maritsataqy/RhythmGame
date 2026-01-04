using UnityEngine;
using UnityEngine.EventSystems; // Wajib ada buat deteksi pointer mouse

// Script ini mendeteksi kalau mouse masuk/keluar dari area tombol
public class ButtonScaleAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleBesar = 1.1f; // Seberapa besar (1.1 = 110%)
    private Vector3 scaleAwal;

    void Start()
    {
        // Simpan ukuran aslinya
        scaleAwal = transform.localScale;
    }

    // Pas Mouse Masuk (Hover)
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = scaleAwal * scaleBesar;
    }

    // Pas Mouse Keluar
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = scaleAwal; // Balik ke ukuran normal
    }
}