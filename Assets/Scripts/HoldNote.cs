using UnityEngine;

public class HoldNote : MonoBehaviour
{
    [Header("Status")]
    public bool canBePressed;
    public bool isBeingHeld;
    public bool isFinished;
    
    [Header("Setup")]
    public KeyCode keyToPress; 

    [Header("Data Note")]
    public float noteSpeed;     
    public float totalDuration; 
    private float currentDuration; 

    // Referensi
    private GameManager gameManager;
    private HoldNoteVisual visualScript; 
    private MonoBehaviour gerakScript;   // Referensi ke script GerakTurun
    private Rigidbody2D rb; 

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>(); 
        visualScript = GetComponent<HoldNoteVisual>();
        
        // Ambil script GerakTurun yang nempel di benda yang sama
        gerakScript = GetComponent<GerakTurun>(); 
        rb = GetComponent<Rigidbody2D>();

        // Ambil kecepatan dari GerakTurun biar sinkron
        if (gerakScript != null)
        {
            // Pastikan casting ke tipe yang benar (GerakTurun)
            noteSpeed = GetComponent<GerakTurun>().kecepatan; 
        }
    }

    public void SetupNote(float duration)
    {
        totalDuration = duration;
        currentDuration = duration;
    }

    void Update()
    {
        // --- CATATAN PENTING ---
        // Kita TIDAK mengecek posisi Y (Miss Manual) di sini.
        // Biarkan script 'GerakTurun.cs' yang mengecek apakah not ini jatuh ke jurang.
        // Script ini FOKUS hanya pada INPUT TOMBOL.
        // -----------------------

        // 1. INPUT DITEKAN (MULAI)
        if (Input.GetKeyDown(keyToPress))
        {
            if (canBePressed)
            {
                isBeingHeld = true;
                isFinished = false;
                
                // --- LOGIKA FREEZE (BIAR GAK JATUH SAAT DITAHAN) ---
                // Matikan script gerak supaya not 'nempel' di tombol saat ditahan
                if (gerakScript != null) gerakScript.enabled = false;
                
                // Stop physics juga kalau pake Rigidbody
                if (rb != null) 
                {
                    // Gunakan 'velocity' untuk Unity versi lama, 'linearVelocity' untuk Unity 6
                    // Kalau error merah disini, ganti jadi rb.velocity = Vector2.zero;
                    rb.linearVelocity = Vector2.zero; 
                    rb.bodyType = RigidbodyType2D.Kinematic; 
                }

                // Panggil Score Awal (Hit Pertama)
                if (gameManager != null) gameManager.NoteHitPerfect(); 
            }
        }

        // 2. INPUT DITAHAN (PROSES)
        if (Input.GetKey(keyToPress) && isBeingHeld)
        {
            currentDuration -= Time.deltaTime;

            // Update gambar batang yang memendek
            if (visualScript != null)
                visualScript.SetDuration(currentDuration, noteSpeed, totalDuration);

            // Cek apakah durasi habis (SUKSES FULL)
            if (currentDuration <= 0)
            {
                isFinished = true;
                isBeingHeld = false;
                
                // Score Akhir (Selesai Tahan)
                if (gameManager != null) gameManager.NoteHitPerfect(); 
                
                Destroy(gameObject); // Hapus not karena sudah selesai dimainkan
            }
        }

        // 3. INPUT DILEPAS (GAGAL DI TENGAH)
        if (Input.GetKeyUp(keyToPress))
        {
            if (isBeingHeld && !isFinished)
            {
                // Lepas sebelum durasi habis = MISS
                Debug.Log("Yah lepas tombol... Miss!");
                isBeingHeld = false;
                
                // Nyalakan lagi geraknya (opsional, biar jatuh ke bawah lalu destroyed)
                if (gerakScript != null) gerakScript.enabled = true;

                if (gameManager != null) gameManager.NoteMiss(); 
                Destroy(gameObject); 
            }
        }
    }

    // Deteksi masuk area tombol
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Activator")) canBePressed = true;
    }

    // Deteksi keluar area tombol
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Activator")) canBePressed = false;
    }
}