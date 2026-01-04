using UnityEngine;
using System.Collections;

public class CharacterVisuals : MonoBehaviour
{
    public Animator myAnimator;       
    public SpriteRenderer mySprite;   

    // 1. Dipanggil saat Game Mulai (Start) 
    // atau saat Pemain berhasil nge-hit lagi setelah Miss.
    public void PlaySwing()
    {
        if (myAnimator != null)
        {
            // Pastikan kita HAPUS trigger miss dulu biar gak nyangkut
            myAnimator.ResetTrigger("trigMiss");
            
            // Nyalakan trigger Swing buat masuk ke mode tempur (Looping)
            myAnimator.SetTrigger("trigSwing");
        }
    }

    // 2. Dipanggil saat kena Note (Perfect/Good)
    public void PlayAttack()
    {
        // LOGIKA BARU:
        // Karena animasi Attack <-> PreAttack sudah otomatis looping di Animator,
        // kita sebenernya gak perlu nyuruh dia 'Attack' lagi.
        
        // TAPI: Kalau sebelumnya pemain habis Miss (animasi lagi diam/Finish),
        // kita harus paksa dia balik ke mode tempur.
        // Jadi kita panggil fungsi PlaySwing() lagi.
        
        PlaySwing();
    }

    // 3. Dipanggil saat MISS (PlayHurt)
    public void PlayHurt()
    {
        if (myAnimator != null)
        {
            // INI PENTING: Matikan trigger swing, Nyalakan trigger Miss
            // Biar dia keluar dari Loop dan pindah ke Finish
            myAnimator.ResetTrigger("trigSwing");
            myAnimator.SetTrigger("trigMiss");
        }

        // Efek Merah (Tetap sama)
        if (mySprite != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        mySprite.color = Color.red; 
        yield return new WaitForSeconds(0.1f); 
        mySprite.color = Color.white; 
    }
}