using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class FileManage : MonoBehaviour
{
    string path;
    string namePDF = "test";
    public Transform contentWindow;
    public GameObject recallTextObject;

    public void stratText() {
        Console.WriteLine(" Enter Your Name:"); 

        // string readFromFilePath = Application.streamingAssetsPath + "test" + ".txt";
        string readFromFilePath = "Assets/Resources/test.txt";

        // string[] readText = File.ReadAllLines(path);

        List<string> fineLines = File.ReadAllLines(readFromFilePath).ToList();
        // foreach (string s in readText)
        // {
        //     Console.WriteLine(s);
        // }

        foreach (string line in fineLines){
            Instantiate(recallTextObject, contentWindow);
            recallTextObject.GetComponent<Text>().text += (line + "\n" );

            Debug.Log(line);
        }

    }

    public void BrowseFile()
    {
        path = EditorUtility.OpenFilePanel("Show","", "pdf");
    
        // StartCoroutine(openPDF());
    }


    void openPDF()
    {
        TextAsset pdfTem = Resources.Load("PDFs/"+namePDF, typeof(TextAsset)) as TextAsset;
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/"+namePDF+".pdf", pdfTem.bytes);
        Application.OpenURL(Application.persistentDataPath+"/"+namePDF+".pdf");
    }
}

