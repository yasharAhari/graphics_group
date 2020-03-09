using System.Collections;
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
        // get the surface triangles and get it to instantiate as triangles 
        UnitSurface[] surfaces = this.generator.GetUnitSurfaces();
        foreach(UnitSurface surface in surfaces)
        {
            GameObject tri = new GameObject("Triangle");
            tri.AddComponent<Triangle>();
            tri.GetComponent<Triangle>().SetSurface(surface);
            tri.transform.parent = this.transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
