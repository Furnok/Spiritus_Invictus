using UnityEngine;

public class S_SaveConversion : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_SaveName] private string saveName;

    [Header("Output")]
    [SerializeField] private RSO_ContentSaved rsoContentSaved;
    [SerializeField] private RSE_OnResetCursor rseOnResetCursor;
    [SerializeField] private RSE_OnSaveData rseSaveData;
    [SerializeField] private RSE_OnLoadData rseLoadData;
    [SerializeField] private RSE_OnDeleteData rseDeleteData;

    public void Setup(string name)
    {
        saveName = name;
    }

    public void ButtonPressSaveData()
    {
        rseSaveData.Call(saveName, false);
    }

    public void ButtonPressLoadData()
    {
        rseLoadData.Call(saveName, false);
    }

    public void ButtonPressDeleteData()
    {
        rseDeleteData.Call(saveName);
        rseOnResetCursor.Call();
    }
}