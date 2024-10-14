using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;
using MetaBIM;

public class BuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }


    // Called before the build is started.
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Build Processor Start: " + callbackOrder);
        Debug.Log("Product: " + Config.ProductionName);
        Debug.Log("OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
        Debug.Log("Call Back Order: " + callbackOrder);
    }



    // Called after the build is finished.
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("Build Processor Complete.");
    }
}