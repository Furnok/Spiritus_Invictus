using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_UISave : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Image")]
    [SerializeField] private Image circle;

    [TabGroup("References")]
    [Title("Text")]
    [SerializeField] private TextMeshProUGUI text;

    private float timer = 0;

    private void OnEnable()
    {
        timer = 0;
        circle.transform.rotation = Quaternion.identity;
    }

    private void OnDisable()
    {
        timer = 0;
        circle.transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        circle.transform.Rotate(0f, 0f, -180f * Time.deltaTime);

        timer += Time.deltaTime;
        int dots = (int)(timer % 4);
        text.text = "Saving" + new string('.', dots);
    }
}