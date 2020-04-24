using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model
{
    // The type of projectile to be launched. cube, sphere, capsule, etc... Default is a sphere
    public string ProjectileShape {
        get; set;
    }

    public float ProjectileSpeed {
        get; set;
    }

    public float ProjectileMass {
        get; set;
    }

    public float ProjectileScale {
        get; set;
    }

    /// <summary>
    /// Items must be added in the order of the items in the dropdown box because the index is used to set values
    /// </summary>
    public string[] ProjectileShapesArray {
        get; set;
    }
}
