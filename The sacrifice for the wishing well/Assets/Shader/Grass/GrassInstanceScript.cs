using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GrassCamScript;

[ExecuteInEditMode]
public class GrassInstanceScript : MonoBehaviour
{
    [Tooltip("wenn das objekt sich nicht rotiert, dann kann die subroutine mit diesem boolean eingespart werden")]
    public bool isStatic;
    [Tooltip("faktor, mit dem die Verschiebung multipliziert wird")]
    public float strength = 1;
    [Tooltip("gibt an, wie stark das Sprite sich bei einer x-verschiebung verdreht")]
    public float curve = 1;
    [Tooltip("x-pos, ab dem der sprite anfängt beeinflusst werden zu können"), Range(0, .75f)]
    public float baseOffset = 0;
    [Tooltip("zufällige verschiebung der verschiebe-textur in worldspace- koordinaten")]
    public Vector2 variance;

    [Tooltip("gibt an, wie stark das Sprite leuchten soll")]
    public float hdrFactor = 1;

    private Material mat;

    void Start()
    {
        mat = new Material(GetComponent<SpriteRenderer>().sharedMaterial);
        SpriteRenderer srenderer = GetComponent<SpriteRenderer>();
        srenderer.material = mat;
        mat.SetTexture("_DisplTex", rTex);
        mat.SetVector("_TexScale", texScale);

        mat.SetFloat("_Strength", strength);
        mat.SetFloat("_curve", curve);
        mat.SetFloat("_base_off", baseOffset);
        mat.SetVector("_PosOff", new Vector2(Random.Range(-variance.x, variance.x), Random.Range(-variance.y, variance.y)));
        mat.SetFloat("_hdrFactor", hdrFactor);

        mat.SetFloat("_spriteAspect", srenderer.sprite.rect.width / srenderer.sprite.rect.height);
        mat.SetVector("_ScaleFactor", new Vector2(srenderer.sprite.texture.width, srenderer.sprite.texture.height) / srenderer.sprite.pixelsPerUnit);
        //mat.SetVector("_ScaleFactor", new Vector2(srenderer.sprite.textureRect.width, srenderer.sprite.textureRect.height) / srenderer.sprite.pixelsPerUnit);
        //mat.SetVector("_ObjOff", new Vector2(srenderer.sprite.textureRect.width, srenderer.sprite.textureRect.height) / srenderer.sprite.pixelsPerUnit);

        if (isStatic) mat.SetFloat("_Rotation", transform.eulerAngles.z);
        else StartCoroutine(UpdateRotation());
    }

    IEnumerator UpdateRotation()
    {
        int rotID = mat.shader.GetPropertyNameId(mat.shader.FindPropertyIndex("_Rotation"));
        while (true)
        {
            yield return new WaitForEndOfFrame();
            mat.SetFloat(rotID, transform.eulerAngles.z);
        }
    }
}
