using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class StagesDB : ScriptableObject
{
    public List<StagesDBEntity> Entities; // Replace 'EntityType' to an actual type that is serializable.
}
