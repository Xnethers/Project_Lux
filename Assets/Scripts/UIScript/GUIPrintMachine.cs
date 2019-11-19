using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIPrintMachine : MonoBehaviour
{
    /// <summary>
    [Header("間隔時間")]
    /// </summary>
    public float letterPause = 0.2f;

    public AudioClip clip;

    private AudioSource source;
    /// <summary>
    /// 暫存中間值
    /// </summary>
    private string word;
    private Text tb;
    /// <summary>
    [Header("要顯示的內容")]
    /// </summary>
    public string text = "請輸入你的ID";
    public

    void Start()
    {
        tb = GetComponent<Text>();
        source = GetComponent<AudioSource>();
        word = text;
        text = "";
        StartCoroutine(TypeText());
    }

    // void OnGUI()
    // {
    //     GUI.Label(new Rect(100, 100, 200, 200), "text show");
    //     GUI.Label(new Rect(50, 50, 250, 250), text);
    // }

    /// <summary>
    /// 打字機效果
    /// </summary>
    /// <returns></returns>
    private IEnumerator TypeText()
    {
        foreach (char letter in word.ToCharArray())
        {
            text += letter;
            if (clip)
            {
                source.PlayOneShot(clip);
            }
            tb.text = text;
            yield return new WaitForSeconds(letterPause);
        }
    }
}
