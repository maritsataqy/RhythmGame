using UnityEngine;

public class ButtonController : MonoBehaviour
{
    // --- KOMPONEN VISUAL ---
    private SpriteRenderer sr; 
    public KeyCode keyToPress; 
    
    public Color defaultColor = new Color(1, 1, 1); 
    public Color pressedColor = new Color(0.7f, 0.7f, 0.7f);

    private NoteObject targetNote = null; 
    private GameManager gm;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = defaultColor;

        // Cari GameManager pakai cara baru yang lebih cepat
        gm = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        // ================================================================
        // 1. SAAT TOMBOL DITEKAN (TAP)
        // ================================================================
        if (Input.GetKeyDown(keyToPress))
        {
            sr.color = pressedColor;

            if (targetNote != null)
            {
                // KASUS A: NOTE BIASA
                if (!targetNote.isHoldNote)
                {
                    // --- LOGIKA JARAK (PERFECT vs GOOD) ---
                    // Hitung selisih jarak antara tombol dan note
                    float jarak = Mathf.Abs(transform.position.y - targetNote.transform.position.y);

                    // Kalau jaraknya dekat sekali (misal < 0.25 unit) -> PERFECT
                    if (jarak <= 0.25f)
                    {
                        if (gm != null) gm.NoteHitPerfect(); 
                        Debug.Log("PERFECT! Jarak: " + jarak);
                    }
                    else
                    {
                        // Kalau jaraknya agak jauh tapi masih kena -> GOOD
                        if (gm != null) gm.NoteHitGood(); 
                        Debug.Log("GOOD. Jarak: " + jarak);
                    }
                    // --------------------------------------

                    Destroy(targetNote.gameObject);
                    targetNote = null;
                }
                // KASUS B: HOLD NOTE (AWAL TEKAN)
                else
                {
                    targetNote.sedangDitekan = true;
                    targetNote.GetComponent<SpriteRenderer>().enabled = false;
                    
                    // PERBAIKAN ERROR DISINI:
                    // Dulu: gm.NoteHit();
                    // Sekarang: Kita kasih Perfect aja buat bonus awal
                    if (gm != null) gm.NoteHitPerfect(); 
                }
            }
        }

        // ================================================================
        // 2. SAAT TOMBOL DILEPAS (RELEASE)
        // ================================================================
        if (Input.GetKeyUp(keyToPress))
        {
            sr.color = defaultColor;

            if (targetNote != null && targetNote.isHoldNote)
            {
                targetNote.sedangDitekan = false;
                if (gm != null) gm.NoteMiss(); // Gagal nahan -> Miss
            }
        }
    }

    // --- FUNGSI DETEKSI TABRAKAN ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Note")
        {
            targetNote = other.GetComponent<NoteObject>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Note" && targetNote != null && other.gameObject == targetNote.gameObject)
        {
            // Khusus Hold Note: Sukses menahan sampai ekornya lewat
            if (targetNote.isHoldNote && targetNote.sedangDitekan)
            {
                // PERBAIKAN ERROR DISINI JUGA:
                // Dulu: gm.NoteHit();
                // Sekarang: Kita kasih Perfect karena berhasil nahan full
                if (gm != null) gm.NoteHitPerfect(); 
            }
            
            targetNote = null;
        }
    }
}