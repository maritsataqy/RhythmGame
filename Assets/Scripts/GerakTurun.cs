using UnityEngine;

public class GerakTurun : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float kecepatan = 5f;

    [Header("Pengaturan Batas Miss")]
    public float garisMatiY = -4.0f; // Sesuaikan dengan garis merah di scene

    private GameManager gameManager;
    private bool hasMissed = false; // Pengaman biar gak miss berkali-kali

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        // 1. Gerak Turun
        transform.Translate(Vector3.down * kecepatan * Time.deltaTime);

        // 2. Cek Posisi (Apakah lewat garis merah?)
        if (transform.position.y < garisMatiY)
        {
            // --- LOGIKA BARU (PENTING!) ---
            
            // Cek apakah benda ini punya script 'HoldNote'?
            HoldNote holdScript = GetComponent<HoldNote>();

            // Jika INI adalah Hold Note...
            if (holdScript != null)
            {
                // ...dan pemain SEDANG menahannya (lagi dipencet)
                if (holdScript.isBeingHeld)
                {
                    // JANGAN DIHANCURKAN. Biarkan script HoldNote yang ngurus.
                    // Keluar dari fungsi ini (return).
                    return; 
                }
            }
            // -----------------------------

            // Kalau bukan Hold Note, atau Hold Note yang dicuekin (gak dipencet):
            if (!hasMissed)
            {
                Debug.Log("Miss! Lewat Garis Mati.");
                if (gameManager != null)
                {
                    gameManager.NoteMiss();
                }
                hasMissed = true; // Tandai sudah miss
                Destroy(gameObject); // Hancurkan
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-10, garisMatiY, 0), new Vector3(10, garisMatiY, 0));
    }
}