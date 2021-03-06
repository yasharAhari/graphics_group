﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colid : MonoBehaviour
{
    public GameObject triangle;

    // add your generator here, No need for any import
    public IGenerator generator = new SphereGenerator();
    

    // Start is called before the first frame update
    void Start()
    {
        // Set radius
        generator.SetRadius(20f);

        // get the surface triangles and get it to instantiate as triangles 
        UnitSurface[] surfaces = this.generator.GetUnitSurfaces();
        foreach(UnitSurface surface in surfaces)
        {
            GameObject tri = new GameObject(surface.id);
            tri.AddComponent<Triangle>();
            tri.GetComponent<Triangle>().SetSurface(surface);
            Face currentFace = generator.getFaceWithId(surface.id);
            tri.GetComponent<Triangle>().InsertFace(currentFace);
            tri.transform.parent = this.transform;

           
           
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
