using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;


public class FileManage : MonoBehaviour
{
    string path;
    string namePDF;
    string message;

public void stratText()
    {

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
            else
            {
                fineLines[i] = fineLines[i].Trim();
                // Instantiate(recallTextObject, contentWindow);
                // recallTextObject.GetComponent<Text>().text += (fineLines[i] + "\n" );
                Debug.Log(fineLines[i]);
            }
        }

        Debug.Log(fineLines.Count());

    }

    public void GetData() => StartCoroutine(PostData());
    public IEnumerator PostData()
    {

        WWWForm form = new WWWForm();
        string readFromFilePath = "Assets/Resources/test2.txt";
        List<string> paragraphList = File.ReadLines(readFromFilePath).ToList();
        var myList = readFromFilePath.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
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

            }
        }

        // paragraphs = paragraphList;
        // data.Add("p" , fineLines);

        Paragraph dataToPost = new Paragraph()
        {
            paragraphs = paragraphList,
            lang = "en"
        }; 

        string url = "https://thsltrans-api.herokuapp.com/api/trans/translate";

        var dataObject = new DataObject() { data = dataToPost};
   
        string toJson = JsonUtility.ToJson(dataObject);
        Debug.Log(toJson);
        // using (UnityWebRequest request = UnityWebRequest.Get(uri))

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(toJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        var responseData = JsonUtility.FromJson<JsonResponseOut>(request.downloadHandler.text);
        Debug.Log(request.downloadHandler.text);
        Debug.Log(responseData.data[0].thsl_translation[0].myList.Count);
        /*foreach (List<string> element in responseData.data[0].thsl_translation)
        {
            Debug.Log(element.Count);
        }*/
        // Debug.Log("Status Code: " + request.downloadHandler.text);

    }

    // public void BrowseFile()
    // {
    //     path = EditorUtility.OpenFilePanel("Show","", "pdf");

    //     // StartCoroutine(openPDF());
    // }


    void openPDF()
    {
        namePDF = "test";
        TextAsset pdfTem = Resources.Load("PDFs/" + namePDF, typeof(TextAsset)) as TextAsset;
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + namePDF + ".pdf", pdfTem.bytes);
        Application.OpenURL(Application.persistentDataPath + "/" + namePDF + ".pdf");
    }
}
[System.Serializable]
public class Paragraph
{
    public List<string> paragraphs;
    public string lang;
}


[System.Serializable]
public class DataObject
{
    public Paragraph data;

}

[System.Serializable]
public class JsonResponseIn
{
    public string original;
    public int p_number;
    public List<ListWrapper> thsl_translation;

}

[System.Serializable]
public class JsonResponseOut
{
    public List<JsonResponseIn> data;
    public string message;
}


[System.Serializable]
public class ListWrapper
{
    public List<string> myList;
}

