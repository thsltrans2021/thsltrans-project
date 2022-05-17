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
using SmartDLL;


public class FileManage : MonoBehaviour
{
    public InputField input;
    public SmartFileExplorer fileExplorer = new SmartFileExplorer();
    public void PostDataToApi()
    {
        string initialDir = @"C:\";
        bool restoreDir = true;
        string title = "Open a Text File";
        string defExt = "txt";
        string filter = "txt files (*.txt)|*.txt";

        fileExplorer.OpenExplorer(initialDir, restoreDir, title, defExt, filter);




        //string readFromFilePath = EditorUtility.OpenFilePanel("Overwrite with txt", "", "txt");


        List<string> paragraphList = File.ReadLines(fileExplorer.fileName).ToList();
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

        // List<string> paragraphList = File.ReadLines(readFromFilePath).ToList();


    }

    void ReadText(string path)
    {
        string readFromFilePathi = File.ReadAllText(path);
    }



}
