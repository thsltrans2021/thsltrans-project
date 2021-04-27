using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FileManage : MonoBehaviour
{
    string path;
    string namePDF = "test";

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
