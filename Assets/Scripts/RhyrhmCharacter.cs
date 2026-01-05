using UnityEngine;

public class RhyrhmCharacter : MonoBehaviour
{
    private Animator myAnim;

    void Start()
    {
        // Ambil Animator yang nempel di badan sendiri
        myAnim = GetComponent<Animator>();
    }

    // Fungsi ini akan dipanggil oleh GameSystem kalau not kena
    public void PlayAttack()
    {
        if(myAnim != null)
        {
            // Reset dulu biar kalau spam tombol tetep jalan
            myAnim.ResetTrigger("Hit"); 
            
            // Panggil Trigger yang tadi kita bikin di Fase 2
            myAnim.SetTrigger("Hit");
        }
    }
}