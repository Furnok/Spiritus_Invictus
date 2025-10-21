using Sirenix.OdinInspector;
using UnityEngine;

public class S_SaveConversion : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Save")]
    [SerializeField, S_SaveName] private string saveName;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ContentSaved rsoContentSaved;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnResetCursor rseOnResetCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSaveData rseOnSaveData;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnLoadData rseOnLoadData;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnDeleteData rseOnDeleteData;

    public void Setup(string name)
    {
        saveName = name;
    }

    public void ButtonPressSaveData()
    {
        rseOnSaveData.Call(saveName, false);
    }

    public void ButtonPressLoadData()
    {
        rseOnLoadData.Call(saveName, false);
    }

    public void ButtonPressDeleteData()
    {
        rseOnDeleteData.Call(saveName);
        rseOnResetCursor.Call();
    }
}