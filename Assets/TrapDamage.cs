using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    // ปริมาณความเสียหายที่กับดักจะทำ
    public int damageAmount = 10; // เปลี่ยนเป็น int

    // เมื่อผู้เล่นชนกับดัก
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            // เข้าถึงคอมโพเนนต์ PlayerControl ของผู้เล่นเพื่อทำความเสียหาย
            PlayerControl player = other.GetComponent<PlayerControl>();

            if (player != null)
            {
                player.TakeDamage(damageAmount); // ส่งค่าเป็น int
            }
        }
    }
}
