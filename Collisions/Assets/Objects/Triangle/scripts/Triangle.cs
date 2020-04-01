using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{

    private UnitSurface surface;
    // neighbors
    private string[] neighborIds = new string[3];

    // this objects id
    private string id;

    // all about energy
    private double currentEnergy = 0;
    private double frameEnergyCost = 5f;
    private double cutoffEnergy = 0.5f;
    private double maxEnergyCanHold = 250f;

    private Color initialColor;

    private bool eneryEntered = false;
    private string propogateId = null;

    public void SetSurface(UnitSurface surface)
    {
        this.surface = surface;
        this.id = surface.id;
    }

    // Energy Propagation
    public void PropagateEnergy(double energy, string id)
    {
        this.currentEnergy += energy;
        this.propogateId = id;
        this.eneryEntered = true;
    }

    // get the surface and neighbors 
    public void InsertFace(Face face)
    {
        // record all neighbor data 
        this.neighborIds[0] = face.halfEdges[0].opposite.face.id;
        this.neighborIds[1] = face.halfEdges[1].opposite.face.id;
        this.neighborIds[2] = face.halfEdges[2].opposite.face.id;
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
        this.initialColor = UnityEngine.Random.ColorHSV();
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", initialColor);
    }

    // Update is called once per frame
    void Update()
    {
        if(this.currentEnergy >= cutoffEnergy)
        {
            if(this.currentEnergy > maxEnergyCanHold)
            {
                this.Propagate();
            }
            this.updateColorByEnergy();
            this.currentEnergy -= frameEnergyCost;
            if(currentEnergy <= cutoffEnergy)
            {
                this.GetComponent<MeshRenderer>().material.SetColor("_Color", initialColor);
            }
        }
    }

    // propagate extra energy to thy neighbors 
    private void Propagate()
    {
        double leftOverEnergy = currentEnergy - maxEnergyCanHold;
        this.currentEnergy = maxEnergyCanHold;
        foreach(string nid in this.neighborIds)
        {
            if(this.eneryEntered && nid.Equals(this.propogateId))
            {
                // not this neighbor
                this.eneryEntered = false;
                continue;
            }
            
            GameObject neighbor = GameObject.Find(nid);
            neighbor.GetComponent<Triangle>().PropagateEnergy(leftOverEnergy / 3, this.id);
        }
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
       
        // add a current mesh for the collider too 
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    // this is called when an object collides with this triangle 
    public void OnCollisionEnter(Collision collision)
    {
        GameObject enteredObject = collision.gameObject;
        float mass = enteredObject.GetComponent<Rigidbody>().mass;
        float speed = enteredObject.GetComponent<Projectile>().speed;
        this.currentEnergy = 1 * mass * speed * speed / 2;
        
    }

    // change color based on how much energy we have. 
    private void updateColorByEnergy()
    {
        Color color = this.GetComponent<MeshRenderer>().material.color;
        double ratio = currentEnergy / this.maxEnergyCanHold;
        Color newCol = new Color((float)(initialColor.r*ratio+ratio),(float)(initialColor.g-ratio),(float)(initialColor.b-ratio));
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", newCol);
    }
}
