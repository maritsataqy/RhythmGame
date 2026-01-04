using UnityEngine;

public class HoldNoteVisual : MonoBehaviour
{
    public SpriteRenderer bodySprite;
    public SpriteRenderer headSprite;
    public BoxCollider2D myCollider;

    public float minVisualLength = 1.5f; 

    // KITA TAMBAH PARAMETER BARU: "totalDuration"
    public void SetDuration(float currentDuration, float speed, float totalDuration)
    {
        // 1. Hitung Panjang Asli Fisika
        float panjangAsli = speed * currentDuration;

        // 2. Hitung Persentase Sisa (0.0 sampai 1.0)
        // Kalau totalDuration 0, kita anggap 0 biar gak error dibagi 0
        float persentase = (totalDuration > 0) ? (currentDuration / totalDuration) : 0;

        // 3. Hitung "Min Visual" yang dinamis
        // Pas awal (100%), dia 1.5. Pas mau abis (0%), dia jadi 0.
        float dynamicMinLength = minVisualLength * persentase;

        // 4. Pilih yang paling aman
        float panjangVisual = Mathf.Max(panjangAsli, dynamicMinLength);

        // --- UPDATE GAMBAR ---
        if (bodySprite != null)
        {
            Vector2 ukuranBaru = bodySprite.size;
            ukuranBaru.y = panjangVisual; 
            bodySprite.size = ukuranBaru;
        }

        // --- UPDATE COLLIDER (Tetap Jujur) ---
        if (myCollider != null)
        {
            Vector2 colliderSize = myCollider.size;
            colliderSize.y = panjangAsli; 
            myCollider.size = colliderSize;

            Vector2 colliderOffset = myCollider.offset;
            colliderOffset.y = panjangAsli / 2f; 
            myCollider.offset = colliderOffset;
        }
    }
}