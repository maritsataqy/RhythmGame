using UnityEngine;
using System.Collections;

public class CharacterReactor : MonoBehaviour
{
    private Vector3 posisiAwal;
    private SpriteRenderer sr;

    [Header("Settingan Gerak")]
    public float jarakMaju = 0.5f; // Seberapa jauh dia maju pas nyerang
    public float durasiGerak = 0.1f; // Seberapa cepat geraknya

    void Start()
    {
        posisiAwal = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    // --- FUNGSI 1: SERANGAN (MAJU - MUNDUR) ---
    public void PlayAttack()
    {
        StopAllCoroutines(); // Reset gerak sebelumnya biar gak tabrakan
        StartCoroutine(AnimasiSerang());
    }

    IEnumerator AnimasiSerang()
    {
        // 1. Maju ke depan (Kanan)
        // Kalau ini script buat Boss, ganti Vector3.right jadi Vector3.left
        Vector3 target = posisiAwal + (Vector3.right * jarakMaju); 
        
        float timer = 0;
        while (timer < durasiGerak)
        {
            transform.position = Vector3.Lerp(posisiAwal, target, timer / durasiGerak);
            timer += Time.deltaTime;
            yield return null;
        }

        // 2. Balik ke posisi awal
        timer = 0;
        while (timer < durasiGerak)
        {
            transform.position = Vector3.Lerp(target, posisiAwal, timer / durasiGerak);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = posisiAwal;
    }

    // --- FUNGSI 2: KENA DAMAGE (GENTAR/SHAKE) ---
    public void PlayHurt()
    {
        StopAllCoroutines();
        StartCoroutine(AnimasiGetar());
    }

    IEnumerator AnimasiGetar()
    {
        // Ubah warna jadi merah sebentar
        sr.color = Color.red;

        // Getar kiri-kanan
        Vector3 randomPos;
        for (int i = 0; i < 5; i++)
        {
            randomPos = posisiAwal + (Random.insideUnitSphere * 0.05f); // Angka 0.2 = kekuatan getar
            randomPos.z = posisiAwal.z; // Biar z gak berubah
            transform.position = randomPos;
            yield return new WaitForSeconds(0.05f);
        }

        // Kembalikan posisi & warna
        transform.position = posisiAwal;
        sr.color = Color.white;
    }
}