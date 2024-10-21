using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Controller : MonoBehaviour
{
    public float speed = 5f; // ความเร็วที่สามารถปรับได้ใน Inspector
    public int maxJumpCount = 2; // จำนวนครั้งที่อนุญาตให้กระโดด
    private int jumpCount; // ตัวแปรนับจำนวนการกระโดด
    float x, sx;
    Animator am;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sx = transform.localScale.x; // บันทึกขนาดเริ่มต้นของตัวละคร
        jumpCount = 0; // เริ่มต้นนับจำนวนการกระโดดที่ 0
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal"); // รับค่าการควบคุมจากปุ่มซ้ายขวา
        am.SetBool("isWalking", x != 0); // ตั้งค่าพารามิเตอร์ isWalking

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount) // เช็คจำนวนการกระโดด
        {
            am.SetBool("jump", true);
            rb.velocity = new Vector2(rb.velocity.x, 8f); // กระโดด
            jumpCount++; // เพิ่มจำนวนการกระโดด
        }

        rb.velocity = new Vector2(x * speed, rb.velocity.y); // เคลื่อนที่ตามแนวนอน

        // เปลี่ยนทิศทางของตัวละคร
        if (x > 0)
        {
            transform.localScale = new Vector3(sx, transform.localScale.y, transform.localScale.z); // หันไปทางขวา
        }
        else if (x < 0)
        {
            transform.localScale = new Vector3(-sx, transform.localScale.y, transform.localScale.z); // หันไปทางซ้าย
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        am.SetBool("jump", false); // หยุดการกระโดดเมื่อแตะพื้น
        jumpCount = 0; // รีเซ็ตจำนวนการกระโดดเมื่อแตะพื้น
    }
}
