using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField input;
    public GameObject FileManager;
    public GameObject Avatar;

    private RainController _avatarController;
    private FileManage _fileManager;
    void Start()
    {
        _avatarController = Avatar.GetComponent<RainController>();
        _fileManager = FileManager.GetComponent<FileManage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ReadInput(){
        Debug.Log(input.text);
    }

    public void PlayTranslationAnimation()
    {
        List<List<List<string>>> paragraphs = _fileManager.Paragraphs;
        if (paragraphs == null)
        {
            Debug.Log("Where is paragraph data?");
        }
        else
        {
            _avatarController.PlayParagraphs(paragraphs);
        }
    }
}




