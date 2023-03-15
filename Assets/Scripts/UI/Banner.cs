using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    [SerializeField] private Text textbox;
    private Color initialColor;
    private Color transparentColor;

    
    private void Start()
    {
        initialColor = textbox.color;
        transparentColor = textbox.color;
        transparentColor.a = 0;
        initialColor.a = 1;
        textbox.color = transparentColor;
    }

    public void Show(string text, float duration)
    {
        StartCoroutine(ShowCoro(text, duration));
    }
    
    private IEnumerator ShowCoro(string text, float duration)
    {
        textbox.color = transparentColor;
        textbox.text = text;
        textbox.DOColor(initialColor, 0.8f);
        yield return new WaitForSeconds(duration);
        textbox.DOColor(transparentColor, 0.5f);
    }
}
