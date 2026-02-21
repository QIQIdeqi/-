using UnityEditor;
using UnityEngine;

public class ResetWebGLTemplate
{
    [MenuItem("Tools/Reset WebGL Template")]
    public static void ResetTemplate()
    {
        PlayerSettings.WebGL.template = "APPLICATION:Default";
        Debug.Log("WebGL模板已重置为Default");
    }
}
