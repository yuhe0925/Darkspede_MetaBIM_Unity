using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BimLevel
{
    public string LevelName;
    public string LevelIfcID;
    public string LevelID;
    public GameObject LevelObject;

    public float LevelHeightMax;
    public float LevelHeightMin;


    public float LevelCurrentHeight;
    public float LevelOffset;

    public Bounds levelBounds;

    // 

    public BimLevel(string levelName, string levelIfcID, GameObject levelObject)
    {
        LevelName = levelName;
        LevelIfcID = levelIfcID;
        LevelObject = levelObject;

    }

    public void SetLevelHeight(float _min, float _max)
    {

    }


    /*
      ||         ||           ||
     --------------------------------------  Max Height
     --------------------------------------  
      ||         ||           ||
      ||         ||           ||
      ||  this   ||   Level   ||
      ||         ||           ||
      ||         ||           ||
      -------------------------------------- Min Height
      -------------------------------------- 
      ||         ||           || 
      ||         ||           || 
      ||         ||           || 
      ||         ||           || 
      ||         ||           || 
        
     */
}
