using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;



public class FileManage : MonoBehaviour
{
    public InputField input;

    public void PostDataToApi()
    {
        #if UNITY_EDITOR
            string readFromFilePath = EditorUtility.OpenFilePanel("Overwrite with txt", "", "txt");
            List<string> paragraphList = File.ReadLines(readFromFilePath).ToList();
            for (int i = 0; i < paragraphList.Count; i++)
            {
                if (paragraphList[i] == "")
                {
                    paragraphList.RemoveAt(i--);
                }
                else
                {
                    paragraphList[i] = paragraphList[i].Trim();
                    Debug.Log(paragraphList[i]);
                    input.text += paragraphList[i] + "\n";

                }
            }
        #endif
        // List<string> paragraphList = File.ReadLines(readFromFilePath).ToList();


    }

}
