using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavbar : MonoBehaviour
{
    public void ChangeScene(string sceenName)
    {
        SceneManager.LoadScene(sceenName);
    }

    public void Signout()
    {
        // TODO
        print("Please implement this method");
    }
}
