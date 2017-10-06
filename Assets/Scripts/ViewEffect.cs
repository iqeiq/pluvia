using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewEffect : MonoBehaviour {

    public float amount = 3.0f;
    [SerializeField] Shader shader;
    Material material;
    　
    
    void Start()
    {
        material = new Material(shader) {
            hideFlags = HideFlags.DontSave
        };
        
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //material.SetFloat("_Amount", amount);
        material.SetFloat("_Power", amount);
        Graphics.Blit(src, dest, material);
    }

}
