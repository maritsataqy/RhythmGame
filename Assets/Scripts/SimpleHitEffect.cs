using UnityEngine;
using System.Collections;

public class SimpleHitEffect : MonoBehaviour
{
    private SpriteRenderer mySprite;

    void Start()
    {
        // Otomatis cari Sprite Renderer di objek ini
        mySprite = GetComponent<SpriteRenderer>();
    }

    public void FlashRed()
    {
        if (mySprite != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    IEnumerator FlashRoutine()
    {
        // 1. Ubah jadi MERAH
        mySprite.color = Color.red;

        // 2. Tunggu sebentar (0.1 detik)
        yield return new WaitForSeconds(0.1f);

        // 3. Balik jadi PUTIH (Normal)
        mySprite.color = Color.white;
    }
}