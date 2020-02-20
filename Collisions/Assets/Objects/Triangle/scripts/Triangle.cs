using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{

    public Triangle(UnitSurface surface)
    {
        this.setSurface(surface);
    }

    private UnitSurface surface; 

    public void setSurface(UnitSurface surface)
    {
        this.surface = surface;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    /**
     * <summary>Updates the objects mesh</summary>
     *  
     */ 
    public void UpdateShape()
    {
        this.GetComponent<Mesh>().vertices = this.surface.getVertices();
    }
}
