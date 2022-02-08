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

public class TextInput : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField input;
/*    public GameObject FileManager;*/
    public GameObject Avatar;

    private RainController _avatarController;
    private FileManage _fileManager;
    void Start()
    {
        _avatarController = Avatar.GetComponent<RainController>();
 /*       _fileManager = FileManager.GetComponent<FileManage>();*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<List<List<string>>> Paragraphs;

    public void PostInput() => StartCoroutine(ReadInput());


    public IEnumerator ReadInput() {
        List<string> listInput = new List<string>();
        var lines = input.text.Split(char.Parse("\n"));
        /*        var lines = input.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);*/

        for (int i = 0; i < lines.Length; i++)
        {


            if (lines[i] != "")
            {
                lines[i] = lines[i].Trim();
                listInput.Add(lines[i]);
            }

            Debug.Log(lines[i]);
        }
        Debug.Log(listInput.Count);

        Paragraph dataToPost = new Paragraph()
        {
            paragraphs = listInput,
            lang = "en"
        };


        string url = "https://thsltrans-api.herokuapp.com/api/trans/translate";
        var dataObject = new DataObject() { data = dataToPost };
        string toJson = JsonUtility.ToJson(dataObject);
        Debug.Log(toJson);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(toJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        var responseData = JsonUtility.FromJson<JsonResponseOut>(request.downloadHandler.text);
        Debug.Log(request.downloadHandler.text);

        List<List<List<string>>> paragraphs = new List<List<List<string>>>();
        foreach (var dataResponse in responseData.data)
        {
            List<List<string>> thsl_trans = new List<List<string>>();
            foreach (var thsl in dataResponse.thsl_translation)
            {
                List<string> gross = new List<string>();
                foreach (var word in thsl.Split(','))
                {
                    gross.Add(word);

                }
                thsl_trans.Add(gross);
            }
            paragraphs.Add(thsl_trans);
        }

        Debug.Log($"Retrieved data, p length: {paragraphs.Count}");
        for (int k = 0; k < paragraphs.Count; k++)
        {
            for (int i = 0; i < paragraphs[k].Count; i++)
            {
                for (int j = 0; j < paragraphs[k][i].Count; j++)
                {
                    Debug.Log($"p {k}, s {i}, w {j}, {paragraphs[k][i][j]}");
                }
            }
        }
        Paragraphs = paragraphs;

        List<List<List<string>>> _paragraphs= Paragraphs;
        if (_paragraphs == null)
        {
            Debug.Log("Where is paragraph data?");
        }
        else
        {
            _avatarController.PlayParagraphs(_paragraphs);
        }

    }

/*    public void PlayTranslationAnimation()
    {
        List<List<List<string>>> paragraphs = Paragraphs;
        if (paragraphs == null)
        {
            Debug.Log("Where is paragraph data?");
        }
        else
        {
            _avatarController.PlayParagraphs(paragraphs);
        }
    }*/
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
    public List<string> thsl_translation;

}

[System.Serializable]
public class JsonResponseOut
{
    public List<JsonResponseIn> data;
    public string message;
}




