using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PreBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        GameObject[] objectsToExclude = GameObject.FindGameObjectsWithTag("ExcludeFromBuild");
        foreach (GameObject obj in objectsToExclude)
        {
            obj.SetActive(false);
        }
    }
}
