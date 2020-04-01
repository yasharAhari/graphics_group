using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public  class Controller : MonoBehaviour {
    // The model holds all the data relevant to the projectile
    public static Model model;

    public Model model2 = model;
    // These are components we need to control

    // Text UI elements on the panel
    public Text massText;
    public Text speedText;
    public Text scaleText;

    // Sliders on the panel
    public Slider massSlider;
    public Slider speedSlider;
    public Slider scaleSlider;

    // Dropdown box on the panel
    public Dropdown shapeDropdown;

    // Launch button
    public Button launchButton;

    // The main camera
    public Camera mainCamera;

    // Path to prefab resources
    string resourcesPath = "Prefabs/";

    // the projectile object, this is what changes when a new shape is selected
    // a prefeb is loaded into this object depending on what shape the user chooses
    GameObject projectile;

    void Start ( ) {
        
        model = new Model( );
        // Add all possible shapes we want to use to the model, should be added in same order as they appear in the dropdown
        // so they can be accessed in parallel
        model.ProjectileShapesArray = new string [ 5 ];
        model.ProjectileShapesArray [ 0 ] = "sphere";
        model.ProjectileShapesArray [ 1 ] = "cube";
        model.ProjectileShapesArray [ 2 ] = "capsule";
        model.ProjectileShapesArray [ 3 ] = "cylinder";
        // Defaults
        model.ProjectileShape = "sphere";
        model.ProjectileSpeed = 5;
        model.ProjectileScale = 1;
        model.ProjectileMass = 5;

        massText.text = model.ProjectileMass.ToString( );
        speedText.text = model.ProjectileSpeed.ToString( );
        scaleText.text = model.ProjectileScale.ToString( );

        // Set default values on the sliders
        massSlider.value = model.ProjectileMass;
        speedSlider.value = model.ProjectileSpeed;
        scaleSlider.value = model.ProjectileScale;

        // Set the default shape in the dropdown (sphere)
        shapeDropdown.value = 0;

        // Loads the default prefab which is the sphere.. this path is used when launch command is given
        // Path has format Prefabs/<projectileShape>
        resourcesPath += model.ProjectileShape;

    }
    private void Update ( ) {
        massText.text = model.ProjectileMass.ToString( );
        speedText.text = model.ProjectileSpeed.ToString( );
        scaleText.text = model.ProjectileScale.ToString( );

        // Updates the prefab path based on the users shape selection
        resourcesPath = "Prefabs/" + model.ProjectileShape;
            

        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            launchButton.Select( );
        }

    }

    public void launch ( ) {
        // creates the prefab projectile. the movement should be handled instance the script attached to the prefab
        GameObject proj = (GameObject) Resources.Load( resourcesPath );
        proj.GetComponent<Projectile>( ).speed = model.ProjectileSpeed;
        proj.transform.localScale = new Vector3( model.ProjectileScale , model.ProjectileScale , model.ProjectileScale );
        // Eventually use this area to set the Mass of the projectile.
        proj.GetComponent<Rigidbody>().mass = model.ProjectileMass;

        // disable the gravity use 
        proj.GetComponent<Rigidbody>().useGravity = false;
       // Instantiate( proj,new Vector3( mainCamera.transform.position.x , mainCamera.transform.position.y , mainCamera.transform.position.z ) , transform.rotation );
        Instantiate( proj ,  mainCamera.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, mainCamera.nearClipPlane)) , mainCamera.transform.rotation );
    }


    public void setProjectMass ( ) {
        // If the control key is held down then we will round the number to a whole number
        model.ProjectileMass = massSlider.value;
        Debug.Log( "Mass set to: " + model.ProjectileMass );
    }
    public void setRoundedMassValue ( ) {
        model.ProjectileMass = ( float ) Math.Round( ( double ) model.ProjectileMass );
        Debug.Log( "Mass set to: " + model.ProjectileMass );
    }
    public void setProjectileSpeed ( ) {
        model.ProjectileSpeed = speedSlider.value;
        Debug.Log( "Speed set to: " + model.ProjectileSpeed );
    }
    public void setRoundedSpeedValue ( ) {
        model.ProjectileSpeed = ( float ) Math.Round( ( double ) model.ProjectileSpeed );
        Debug.Log( "Speed set to: " + model.ProjectileSpeed );
    }
    public void setProjectileScale ( ) {
        model.ProjectileScale = scaleSlider.value;
        Debug.Log( "Scale set to: " + model.ProjectileScale );
    }
    public void setRoundedScaleValue ( ) {
        model.ProjectileScale = ( float ) Math.Round( ( double ) model.ProjectileScale );
        Debug.Log( "Scale set to: " + model.ProjectileScale );
    }

    public void setProjectileShapefromDropdown ( ) {
        // Each item in the drop down as a value from 0 to n-1 where n is the number of items in the list
        // We will get the selected item and use it to set the current ProjectileShape in the model
        model.ProjectileShape = model.ProjectileShapesArray [ shapeDropdown.value ];
        Debug.Log( "Shape set to: " + model.ProjectileShape );
    }
}
