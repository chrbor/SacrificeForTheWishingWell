using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForReturn());
    }

    IEnumerator WaitForReturn()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        group.alpha = 0;

        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            group.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitWhile(() => Input.anyKey);
        yield return new WaitUntil(() => Input.anyKey);

        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            group.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        SceneManager.LoadScene(0);
        yield break;
    }
}
