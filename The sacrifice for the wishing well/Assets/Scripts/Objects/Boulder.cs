using UnityEngine;
using static LoadSave;

public class Boulder : MonoBehaviour
{
    protected Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        savingProgress += SaveObjData;
        clearObjects += DestroyObj;
    }

    private void OnDestroy()
    {
        Debug.Log(name + " destroyed");
        savingProgress -= SaveObjData;
        clearObjects -= DestroyObj;
    }

    virtual protected void SaveObjData() => progress.objects.Add(new ObjectData(gameObject, rb.velocity));
    void DestroyObj() => Destroy(gameObject);
}
