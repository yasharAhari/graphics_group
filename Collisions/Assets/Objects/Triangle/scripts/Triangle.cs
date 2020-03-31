using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{

    private UnitSurface surface;

    public void SetSurface(UnitSurface surface)
    {
        
        this.surface = surface;
    }
        
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.AddComponent(typeof(MeshFilter));
        this.gameObject.AddComponent(typeof(MeshCollider));
        this.UpdateShape();
    }

    void Awake()
    {
        this.gameObject.AddComponent(typeof(MeshRenderer));
        
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
        this.GetComponent<MeshFilter>().mesh.vertices = this.surface.getVertices();
        this.GetComponent<MeshFilter>().mesh.triangles = this.surface.getTriangle();
        this.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", UnityEngine.Random.ColorHSV());
        // add a current mesh for the collider too 
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;

    }
}
