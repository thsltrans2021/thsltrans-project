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
    // public Transform contentWindow;
    // public GameObject recallTextObject;

    public void stratText() {

        // string readFromFilePath = Application.streamingAssetsPath + "test" + ".txt";
        string readFromFilePath = "Assets/Resources/test.txt";

        // // string[] readText = File.ReadAllLines(path);

        List<string> fineLines = File.ReadLines(readFromFilePath).ToList();
        // List<string> fineLines = File.ReadAllLines(readFromFilePath).stream().filter(str -> !str.trim().isEmpty()).collect(Collectors.toList());
        // List<string> fineLines = File.ReadAllLines(readFromFilePath).Where(s => s.Trim() != string.Empty).ToArray();
        // string inLine = reader.ReadToEnd(readFromFilePath);
        var myList = readFromFilePath.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
        // myList = myList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

        // var doc = readFromFilePath.Split(new string[] { "\r\n" } , System.StringSplitOptions.None);
        // foreach (string s in readText)
        // {
        //     Console.WriteLine(s);
        // }

        Debug.Log(fineLines.Count());
     
        for (int i = 0; i < fineLines.Count; i++)
            {
                if (fineLines[i] == "")
                {
                    fineLines.RemoveAt(i--);
                }
                else {
                    fineLines[i] = fineLines[i].Trim();
                     // Instantiate(recallTextObject, contentWindow);
                    // recallTextObject.GetComponent<Text>().text += (fineLines[i] + "\n" );
                    Debug.Log( fineLines[i]);
                }
            }

        Debug.Log(fineLines.Count());


    }

    // public void BrowseFile()
    // {
    //     path = EditorUtility.OpenFilePanel("Show","", "pdf");
    
    //     // StartCoroutine(openPDF());
    // }


    void openPDF()
    {
        TextAsset pdfTem = Resources.Load("PDFs/"+namePDF, typeof(TextAsset)) as TextAsset;
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/"+namePDF+".pdf", pdfTem.bytes);
        Application.OpenURL(Application.persistentDataPath+"/"+namePDF+".pdf");
    }
}

