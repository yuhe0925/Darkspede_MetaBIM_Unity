using MetaBIM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShareLinkPackage
{
    public string workspaceGuid;
    public string projectGuid;
    public string versionGuid;
    public int update = 0;
    
    public Workspace workspace;
    public Project project;
    public MetaBIM.Version version;

    public bool shared = false;
    public Action<bool> CompleteCallback;
    public Action<float> LoadingProgress;
    
    public string location = "EN";
    
}





