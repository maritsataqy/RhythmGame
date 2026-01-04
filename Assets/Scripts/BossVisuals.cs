using UnityEngine;
using System.Collections;

public class BossVisuals : MonoBehaviour
{
    [Header("Komponen Boss")]
    public Animator myAnimator;
    public SpriteRenderer mySprite;

    [Header("Settingan Efek")]
    public Color warnaKenaHit = Color.red;

    // --- FUNGSI YANG DIPANGGIL GAME MANAGER ---

    // Dipanggil pas Hero berhasil nyerang (Perfect/Good)
    // Demon akan memainkan animasi "Attack" (sebagai reaksinya)
    public void PlayReaction()
    {
        if (myAnimator != null)
        {
            // Reset dulu jaga-jaga kalau kena spam hit cepet banget
            myAnimator.ResetTrigger("trigHit"); 
            myAnimator.SetTrigger("trigHit");
        }
        
        // Sekalian efek kedip merah
        PlayFlashRed();
    }

    // Fungsi tambahan buat efek kedip merah
    public void PlayFlashRed()
    {
        if (mySprite != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashRedRoutine());
        }
    }

    IEnumerator FlashRedRoutine()
    {
        mySprite.color = warnaKenaHit; 
        yield return new WaitForSeconds(0.1f); // Kedip cepet
        mySprite.color = Color.white; 
    }
}