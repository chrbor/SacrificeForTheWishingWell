using UnityEngine;
using static LoadSave;

public class Box : MonoBehaviour, IPortable
{
    public float throwStrength;
    public int jumpSteps;

    protected Rigidbody2D rb;

    public float GetStrength() => throwStrength;
    public int GetJumpSteps() => jumpSteps;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        savingProgress += SaveObjData;
        clearObjects += DestroyObj;
    }

    private void OnDestroy()
    {
        savingProgress -= SaveObjData;
        clearObjects -= DestroyObj;
    }

    void DestroyObj() => Destroy(gameObject);
    virtual protected void SaveObjData() => progress.objects.Add(new ObjectData(gameObject, rb.velocity));
}
