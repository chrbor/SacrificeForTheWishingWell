using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrassCamScript : MonoBehaviour
{
    public static GrassCamScript grasScript;

    [Header("Setup:")]
    [Tooltip("material zum update der rtex, welche die verschiebung angibt")]
    public Material mat;

    public static CustomRenderTexture rTex;
    [Tooltip("Bildauflösung definiert durch Pixel-Höhe. Weite wird durch (höhe * aspect) bestimmt")]
    public int resolution = 720;
    [Tooltip("Gibt an wie groß das Fenster gemessen an der y-höhe für das interaktive Gras ist")]
    public float windowScale = 10;
    public static Vector2 texScale;

    [Tooltip("Anzahl der Objekte, die mit dem Gras interagieren können")]
    public int objectCapacity = 20;
    [HideInInspector]
    public List<Vector4> objData;//xy: globale position, zw: velocity
    [HideInInspector]
    public List<Vector4> objScale;//einflussbereich in form einem abgerundeten Rechteck: x=breite, y=höhe, z=Eck-Kreisumfang, w=verschliierung
    //objScale.xyz muss in globalscale angegeben werden 
    [HideInInspector]
    public List<Vector4> objStrength;//x: drück-Stärke, y: abstoß-stärke, z: rotations-stärke

    [Header("Eigenschaften:")]
    [Tooltip("Startfarbe, mit der die Verschiebe-Textur initiiert wird")]
    public Color defaultColor = new Color(.5f, .5f, .5f, .5f);

    [Tooltip("gibt an wie stark das gras zu seiner ruheposition gezogen wird ")]
    public float stiffness = .8f;
    [Tooltip("gibt an wie stark die schwingung des grases gedämpft wird ")]
    public float damping = .75f;
    [Tooltip("gibt an um wieviel der schwing-vorgang verlangsamt wird ")]
    public float speedDown = 100;


    [Header("Wind:"), Tooltip("x: kraft, y: winkel, z: windfrequenz, w: noise-scale")]
    public Vector4 wind = new Vector4(0, 0, 1, 5);
    [Tooltip("Gibt an wie stark das Gras entgegen der Windrichtung wankt"), Range(0, .5f)]
    public float windOff = .5f;

    [Header("Rotation:"), Tooltip("Gibt an wie stark die rotation des Gerätes das Gras beeinflusst")]
    public float rotStrength = 1;

    int CamDataID, objDataID, objScaleID, objLengthID;


    [Header("Test-Input:")]
    public float strength = .1f;
    [Tooltip("einflussbereich in form eines abgerundeten Rechtecks: x=breite, y=höhe, z=Eck-Radius, w=verschlierung")]
    public Vector4 effectField;

    // Start is called before the first frame update
    void Awake()
    {
        grasScript = this;

        //Erstelle neue Instanz des Materials:
        mat = new Material(mat);
        CamDataID = mat.shader.GetPropertyNameId(mat.shader.FindPropertyIndex("_CamData"));
        //objDataID = mat.shader.GetPropertyNameId(mat.shader.FindPropertyIndex("_ObjData"));
        //objScaleID = mat.shader.GetPropertyNameId(mat.shader.FindPropertyIndex("_ObjScale"));
        objLengthID = mat.shader.GetPropertyNameId(mat.shader.FindPropertyIndex("_ObjLength"));

        //Erstelle neue Instanz des Materials des Spriterenderers:
        texScale = new Vector2(Camera.main.aspect, 1) * windowScale * 2;
        mat.SetFloat("_Scale", windowScale);
        mat.SetFloat("_SpeedDown", speedDown);
        mat.SetFloat("_Damping", damping);
        mat.SetFloat("_Stiffness", stiffness);

        //Initialisiere Vector-Array:
        objData = new List<Vector4>(objectCapacity);
        objScale = new List<Vector4>(objectCapacity);
        objStrength = new List<Vector4>(objectCapacity);
        mat.SetInt(objLengthID, objectCapacity);
        Vector4[] vec4Init = new Vector4[objectCapacity];
        mat.SetVectorArray("_ObjData", vec4Init);
        mat.SetVectorArray("_ObjScale", vec4Init);
        mat.SetVectorArray("_ObjStrength", vec4Init);


        //Erstelle rendertexture
        rTex = new CustomRenderTexture((int)(resolution * Camera.main.aspect), resolution, RenderTextureFormat.ARGBHalf);
        rTex.material = mat;
        rTex.initializationMode = CustomRenderTextureUpdateMode.OnLoad;
        rTex.initializationSource = CustomRenderTextureInitializationSource.TextureAndColor;
        rTex.initializationColor = defaultColor;
        rTex.updateMode = CustomRenderTextureUpdateMode.Realtime;
        //rTex.updateMode = CustomRenderTextureUpdateMode.OnLoad;
        rTex.wrapMode = TextureWrapMode.Clamp;

        rTex.doubleBuffered = true;
        rTex.Create();

        Input.gyro.enabled = true;
        prevAngle = Mathf.Atan2(Input.gyro.gravity.y, Input.gyro.gravity.x);

        myWind = wind;
    }

    private void OnDestroy()
    {
        if(rTex != null) rTex.Release();
    }


    Vector3 prevPos, prevMousePos;
    float prevAngle;
    Vector4 myWind;
    void Update()
    {
        if (Input.GetKey(KeyCode.U)) Camera.main.orthographicSize -= .1f;
        if (Input.GetKey(KeyCode.I)) Camera.main.orthographicSize += .1f;

        //*
        //Mouse:
        Vector3 mouseDiff = Input.mousePosition - prevMousePos;
        Vector2 worldPos = TouchAndScreen.PixelToWorld(Input.mousePosition);

        objData.Add(new Vector4(worldPos.x, worldPos.y, mouseDiff.x, mouseDiff.y));
        objScale.Add(effectField);
        objStrength.Add(new Vector3(strength,0));
        //*/

        //Update der Objektdaten:
        mat.SetInt(objLengthID, objData.Count);
        mat.SetVectorArray("_ObjData", objData);
        mat.SetVectorArray("_ObjScale", objScale);
        mat.SetVectorArray("_ObjStrength", objStrength);
        objData.Clear();
        objScale.Clear();
        objStrength.Clear();


        //Update der Rotationsverschiebung:
        float angle_current = Mathf.Atan2(Input.gyro.gravity.y, Input.gyro.gravity.x);
        float angle_diff = angle_current - prevAngle;
        if (Mathf.Abs(angle_diff) > Mathf.PI) angle_diff -= Mathf.Sign(angle_diff) * 2 * Mathf.PI;

        mat.SetFloat("_RotSpeed", angle_diff * rotStrength);
        mat.SetVector(CamDataID, transform.position - prevPos);


        //Update von Wind:
        myWind.y = wind.y + angle_current * Mathf.Rad2Deg + 90;
        mat.SetVector("_Wind", myWind);
        mat.SetFloat("_WindOff", windOff);


        prevPos = transform.position;
        prevMousePos = Input.mousePosition;
        prevAngle = angle_current;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * windowScale * 2);
    }
}
