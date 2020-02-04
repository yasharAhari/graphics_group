using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AsteroidGeneratorV1 : AsteroidGenerator
{
    //The size of the Asteroid
    private double size;

    //vertexes after generation
    private List<Vector4> _vertexes;

    //The triangles after configuration 
    private List<int> _triangles;

    //The amount of noise that Asteroid will inherit
    private float noiseLevel;

    //The amount of point that asteroid will have drastic changes
    private long varietyPoints;

    //Amount of angle change in each iteration 
    private float detailLevel;

    //Number of iteration from north to south.
    private int thetaSwipeCount;

    //Number of iteration from east to west.
    private int phiSwipesCount;

    

    private List<Vector3> normals;

    public AsteroidGeneratorV1(double size, float noiseLevel, long varietyPoints, float detailLevel)    
    {
        this.size = size;
        this.noiseLevel = noiseLevel;
        this.varietyPoints = varietyPoints;
        this.detailLevel = detailLevel;
    }


    /**
     * Gets the Cartesian coordinates from the spherical coordinate variables and returns a Vertex object. 
     * The radius of the vertex stays same as input.
     */
    private Vector4 getCoordinates(double theta, double phi, double radius)
    {
        theta = theta * Mathf.PI / 180;
        phi = phi * Mathf.PI / 180;
        double x = radius * Mathf.Sin((float)theta) * Mathf.Cos((float)phi);
        double y = radius * Mathf.Sin((float)theta) * Mathf.Sin((float)phi);
        double z = radius * Mathf.Cos((float)theta);
        Vector4 vector = new Vector4((float)x,(float)y,(float)z,(float)radius);
        return vector;

    }


    public void Generate()
    {
        // clear off the old vertexes in case this is a regeneration
        _vertexes = new List<Vector4>();
        phiSwipesCount = 0;
        thetaSwipeCount = 0;
        List < Vector4 > firstThetaSwipe = new List<Vector4>();

        /*
         * The random generator here is the Random generator that comes with UnityEngine
         */


        // first create and populate the variety array
        float[] vartiety = new float[varietyPoints];
        for(int index = 0; index < varietyPoints; ++index)
        {
            vartiety[index] = Random.Range(-0.5f, 0.5f);
        }


        // Determine the initial Radius 
        // to prevent Irregularities, the minimum radius will be 3/4 of the intended size
        float radius = (float) this.size;

        // The random will change later to it need to be kept at an-other variable 
        float initial_radius = radius;

        // The North polar radius will be the initial radius because it is the place where generator start crating points
        // it need to be saved thus next north pole point assignments will be same and won't create polar wall.
        float north_polar_radius = radius;

        // Again, The same concept as the North_polar_r but the value will determined after first theta swipe.
        // if you wondering what is the theta swipe is, refer to comments before the generation loop.
        float south_polar_radius = 0;

        // Flags if it is the theta swipes first run
        bool first_run_flag_theta = true;

        // Flags if it is the phi swipes first run
        bool first_run_flag_phi = true;

        // The radius of the side vertex (the vertex generated before) in theta swipe
        float radius_side_theta = 0;

        // The radius of the neighbor vertex on the other theta swipe (along the phi swipe)
        float radius_side_phi = 0;

        // The total amount of the Vertex inserted in theta swipe 
        int theta_vertex_total_count = 0;

        // Total amount of the vertex inserted so far.
        int total_vertex_inserted = 0;

        /*
         * So there is two variable here that represents the spherical coordinates. As you know, in the spherical coordinates 
         * there are 3 variables, 2 angles and radius. The idea here is that for each angle combinations a controlled random radius will be generated.
         * The "controlled" here means that the randomly generated in near proximity of the vertex generated one before and one in the neighbor.
         * There is terms of south and north pole, because although it is a asteroid, it can be seen as a celestial body. 
         * Of course, The poles are the places where the axis of rotation is, I know, but to make things easier here, The point where the theta 
         * swipe starts is north pole, and the place it ends is south pole. 
         * To make it easier, the theta swipe is same as the latitude and the phi swipe with longitude and the first theta swipe can be named as
         * the Greenwich meridian.
         * 
         * 
         * 
         */
        for (float phi = 0; phi <= 360-detailLevel; phi = phi + detailLevel)
        {
            // Number of Vertex Generated in each theta swipe 
            int theta_vertex_count = 0;
            for(float theta = 0; theta <=180; theta = theta + detailLevel)
            {
                // Control blocks where the decision made about value of the 
                if(!first_run_flag_theta)
                {
                    // something cool about C# that is different than Java that is [] override exist. 
                    Vector4 side_vertex_theta = _vertexes[_vertexes.Count - 1];
                    radius_side_theta = side_vertex_theta.w;

                    if(!first_run_flag_phi)
                    {
                        Vector4 side_vertex_phi = _vertexes[total_vertex_inserted - theta_vertex_total_count];
                        radius_side_phi = side_vertex_phi.w;
                    }
                    else
                    {
                        radius_side_phi = initial_radius;
                    }

                    float median_radius = radius_side_phi;
                    //***!!!!
                    if(theta > 100f && !first_run_flag_phi)
                    {
                        median_radius = (median_radius + south_polar_radius) / 2;
                    }
                    if(phi > 0)
                    {
                        Vector4 vert = _vertexes[theta_vertex_count];
                        median_radius = (median_radius + vert.w) / 2f;
                    }

                    if(theta_vertex_count == 0)
                    {
                        radius = north_polar_radius;
                    }
                    else
                    {
                        if(!first_run_flag_phi)
                        {
                            if(theta_vertex_count >= theta_vertex_total_count -1)
                            {
                                radius = south_polar_radius;
                            }
                            else
                            {
                                radius = Random.Range(median_radius - noiseLevel, median_radius + noiseLevel);
                            }
                        }
                        else
                        {
                            //(value - from1) / (to1 - from1) * (to2 - from2) + from2 the mapping
                            int current_variety_index = (int)((theta - 0) / (180 - 0) * (varietyPoints-1 - 0) + 0);
                            radius = radius + vartiety[current_variety_index] * Random.Range(0, noiseLevel);
                        }
                    }

                }


                // The Assignment
                Vector4 v = getCoordinates(theta, phi, radius);
                _vertexes.Add(v);
                ++theta_vertex_count;
                ++total_vertex_inserted;

                if(first_run_flag_phi)
                {
                    firstThetaSwipe.Add(v);
                }

                first_run_flag_theta = false;
            }
            if(first_run_flag_phi)
            {
                south_polar_radius = radius;
                first_run_flag_phi = false;
                theta_vertex_total_count = theta_vertex_count;
                this.thetaSwipeCount = theta_vertex_count;
            }

            ++phiSwipesCount;

        }


        //Fix the last Swipe which is replacing the last Swipe with first swipe
        int index_f = 0;
        for(int index = _vertexes.Count - theta_vertex_total_count; index < _vertexes.Count; ++index)
        {
            _vertexes[index] = firstThetaSwipe[index_f];
            ++index_f;
        }

        generateTriangles();


    }



    public List<int> getTriangle()
    {
        return this._triangles;
    }


    public List<Vector3> getNormals()
    {
        this.normals = new List<Vector3>();
        HashSet<int> indexes = new HashSet<int>();
        for(int index = 0; index < this._triangles.Count;index +=3 )
        {
            Vector4 v1 = _vertexes[_triangles[index]];
            Vector4 v2 = _vertexes[_triangles[index+1]];
            Vector4 v3 = _vertexes[_triangles[index+2]];
            Vector3 normal = GetNormal(v1, v2, v3);
            if(!indexes.Contains(_triangles[index]))
            {
                indexes.Add(_triangles[index]);
                this.normals.Add(GetNormal(v1, v2, v3));
            }
            if (!indexes.Contains(_triangles[index+1]))
            {
                indexes.Add(_triangles[index+1]);
                this.normals.Add(GetNormal(v1, v2, v3));
            }
            if (!indexes.Contains(_triangles[index+2]))
            {
                indexes.Add(_triangles[index+2]);
                this.normals.Add(GetNormal(v1, v2, v3));
            }
        }

       
        return this.normals;
    }

    public List<Vector3> getVertexes()
    {
        // have to create the V3 from V4 so...
        List<Vector3> _meshVertexes = new List<Vector3>(_vertexes.Count);
        for(int index = 0; index < _vertexes.Count; ++index)
        {
            Vector3 vertex = new Vector3(_vertexes[index].x, _vertexes[index].y, _vertexes[index].z);
            _meshVertexes.Add(vertex);
        }
        return _meshVertexes;
    }



    private void generateTriangles()
    {
        _triangles = new List<int>();

        for(int phi_index = 0; phi_index < phiSwipesCount -1;++phi_index)
        {
            for (int theta_index = 0; theta_index < thetaSwipeCount - 1; ++theta_index)
            {
                //on every iteration we deal with two triangles

                // adding triangle one:
                /*

                 1*------*2
                  |     /
                  |    /
                  |   / 
                  |  /
                  | /
                  |/ 
                  *3
                */


                _triangles.Add(theta_index + (phi_index * thetaSwipeCount));
                _triangles.Add(theta_index + (phi_index * thetaSwipeCount) + 1);
                _triangles.Add(theta_index + ((phi_index + 1) * thetaSwipeCount));


                //adding triangle 2
                /*
                         *1
                        /|
                       / | 
                      /  |
                     /   |
                    /    |
                   /     |
                 3*------*2       
                */
                _triangles.Add(theta_index + (phi_index * thetaSwipeCount) + 1);
                _triangles.Add(theta_index + ((phi_index + 1) * thetaSwipeCount) + 1);
                _triangles.Add(theta_index + ((phi_index + 1) * thetaSwipeCount));
            }
        }
    }


    private Vector3 GetNormal(Vector4 _a, Vector4 _b, Vector4 _c)
    {
        Vector3 a = new Vector3(_a.x, _a.y, _a.z);
        Vector3 b = new Vector3(_b.x, _b.y, _c.z);
        Vector3 c = new Vector3(_c.x, _c.y, _c.z);
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        // Cross the vectors to get a perpendicular vector, then normalize it.
        return Vector3.Cross(side1, side2).normalized;
    }

}
