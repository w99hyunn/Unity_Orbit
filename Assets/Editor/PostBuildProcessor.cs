using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PostBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        GameObject[] objectsToExclude = GameObject.FindGameObjectsWithTag("ExcludeFromBuild");
        foreach (GameObject obj in objectsToExclude)
        {
            obj.SetActive(true);
        }
    }
}
