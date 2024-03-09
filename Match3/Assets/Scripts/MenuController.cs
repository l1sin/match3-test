using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
