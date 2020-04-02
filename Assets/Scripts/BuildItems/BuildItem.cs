﻿using System;
using UnityEngine;

// The base object for the Monobehaviours that are used on the map
public class BuildItem : MonoBehaviour
{
    public string Id = "";

    public void Initialise()
    {
        Id = Guid.NewGuid().ToString();
    }
}
