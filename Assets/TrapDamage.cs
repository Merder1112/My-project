using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    // ����ҳ����������·��Ѻ�ѡ�з�
    public int damageAmount = 10; // ����¹�� int

    // ����ͼ����蹪��Ѻ�ѡ
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ��Ǩ�ͺ����繼������������
        if (other.CompareTag("Player"))
        {
            // ��Ҷ֧����๹�� PlayerControl �ͧ���������ͷӤ����������
            PlayerControl player = other.GetComponent<PlayerControl>();

            if (player != null)
            {
                player.TakeDamage(damageAmount); // �觤���� int
            }
        }
    }
}
