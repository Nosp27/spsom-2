using System;
using System.IO;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    [SerializeField] private String dirPath = "Assets";
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [ContextMenu("Screenshot")]
    void TakeScreenshot()
    {
        Camera cam = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(width, height);
        tex.ReadPixels(new Rect(0,0,width,height), 0, 0);

        RenderTexture.active = null;
        cam.targetTexture = null;
        DestroyImmediate(rt);

        print("W");
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes($"{dirPath}/img.png", bytes);
    }
}
