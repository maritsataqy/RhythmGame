using UnityEngine;
using System.Collections; // Wajib ada untuk fitur jeda waktu (Coroutine)

public class HitEffect : MonoBehaviour
{
    [Header("Pengaturan Efek")]
    public float lifeTime = 0.5f;      // Berapa lama tulisan ini hidup (jangan lama-lama)
    public float kedipSpeed = 0.05f;   // Seberapa cepat kedipnya (makin kecil makin ngebut)

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // 1. Perintah bunuh diri setelah 'lifeTime' detik
        Destroy(gameObject, lifeTime);

        // 2. Mulai efek korslet (kedip-kedip)
        StartCoroutine(EfekKorslet());
    }

    // Ini fungsi khusus (Coroutine) biar bisa mainin waktu jeda
    IEnumerator EfekKorslet()
    {
        // Ulangi terus selama objek ini masih hidup
        while (true)
        {
            // Matikan gambar
            sr.enabled = false;
            // Tunggu sebentar banget
            yield return new WaitForSeconds(kedipSpeed);

            // Nyalakan gambar
            sr.enabled = true;
            // Tunggu sebentar lagi
            yield return new WaitForSeconds(kedipSpeed);
        }
    }

    // Kita HAPUS void Update() biar dia diam di tempat dan gak naik ke atas
}