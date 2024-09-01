using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveChilds : MonoBehaviour
{
    List<Material> materials = new List<Material>();

    void Start()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            materials.AddRange(renderers[i].materials);
        }
    }

    public IEnumerator AnimateDissolve()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            SetValue(value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set to 1 at the end
        SetValue(1f);
    }

    public void SetValue(float value)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetFloat("_Dissolve", value);
        }
    }
}
