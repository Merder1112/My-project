using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // เพิ่มบรรทัดนี้

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
 