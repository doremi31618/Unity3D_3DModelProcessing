using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
public class MeshSimplify_ui : MonoBehaviour
{

    [Header("Single Mesh Simplifier")]
    public InputField srcPath;
    public InputField dstPath;
    public Slider quality;
    public InputField quality_inputField;
    public Button startButton;
    public Text infoText;

    [Header("Folder Mesh Simplifier")]
    public InputField folderPath;
    public InputField destinationFolder;
    public Slider foldermode_quality;
    public InputField foldermode_quality_inputField;
    public Button startFolderSimplifyButton;

    #region private attribute
    bool isFinish = true;
    string _info;
    ProcessMeshSimplify meshDesimator;
    #endregion

    void Start()
    {
        startButton.onClick.AddListener(StartSimplify);
        quality.onValueChanged.AddListener(OnQualitySliderValuedChanged);
        quality_inputField.onEndEdit.AddListener(OnQualityTextValuedChanged);

        startFolderSimplifyButton.onClick.AddListener(StartFolderSimplify);
        foldermode_quality.onValueChanged.AddListener(OnFolderModeQualitySliderValuedChanged);
        foldermode_quality_inputField.onEndEdit.AddListener(OnFolderModeQualityTextValuedChanged);

        meshDesimator = GetComponent<ProcessMeshSimplify>();
        meshDesimator.simplifyInfo += UpdateInfoText;
    }

    public void StartSimplify()
    {
        float _quality = quality.normalizedValue;
        string sourcePath = srcPath.text;
        string destPath = dstPath.text;
        _info = "Start Simplify \n";
        isFinish = false;
        StartCoroutine(UpdateInfoUI());
        meshDesimator.SimplifyObj(sourcePath, destPath, _quality);
    }

    public void StartFolderSimplify()
    {
        StartCoroutine(SimplifyAllObjFiles(folderPath.text));
    }
    //on single mode
    public void OnQualitySliderValuedChanged(float value)
    {
        quality_inputField.text = value.ToString();
    }

    public void OnQualityTextValuedChanged(string value)
    {
        quality.value = Mathf.Clamp(float.Parse(value), 0, 1);
    }

    //on folder mode
    public void OnFolderModeQualitySliderValuedChanged(float value)
    {
        foldermode_quality_inputField.text = value.ToString();
    }

    public void OnFolderModeQualityTextValuedChanged(string value)
    {
        foldermode_quality.value = Mathf.Clamp(float.Parse(value), 0, 1);
    }

    public void UpdateInfoText(string info)
    {
        if (info != "finish")
            _info += "\n" + info;
        else
            isFinish = true;
    }
    IEnumerator SimplifyAllObjFiles(string path)
    {
        string sourceFolderPath = path;
        string destinationFolderPath = destinationFolder.text;
        DirectoryInfo di = new DirectoryInfo(sourceFolderPath);
        var objFiles = di.GetFiles("*.obj");
        float _quality = foldermode_quality.normalizedValue;
        int index=1; 
        int length=objFiles.Length;
        isFinish = false;
        
        if (length == 0){

            yield break;
        }
        foreach (var obj in objFiles)
        {
            
            string objName = Path.GetFileNameWithoutExtension(obj.FullName) ;
            string sourcePath = obj.FullName;
            string destPath = destinationFolderPath + objName + "_simplify.obj";

            print("sourcePath : " + sourcePath);
            print("destPath : " + destPath);

            string uiInfo = string.Format(
                "current process {0}/{1} \nStart simplify \n source path : {2}\n destination : {3}\n",
                index, length, sourcePath, destPath);
            _info = uiInfo;
            // var sb = new System.Text.StringBuilder("");
            meshDesimator.SimplifyObj(sourcePath, destPath, _quality);
            while (!isFinish)
            {
                _info += "=";
                infoText.text = _info;
                yield return new WaitForSeconds(1);
                
            }
            // meshDesimator.DisposeTask();
            infoText.text = _info;
            yield return new WaitForSeconds(3);
            index += 1;
            isFinish = false;
        }
    }

    IEnumerator UpdateInfoUI()
    {
        while (!isFinish)
        {
            _info += "=";
            infoText.text = _info;
            yield return new WaitForSeconds(1);
        }
        infoText.text = _info;
    }

}
