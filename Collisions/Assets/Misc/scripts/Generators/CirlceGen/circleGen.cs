using System;
using System.Collections.Generic;
public class CircleGen : IGenerator
{
    private double Radius;
    private double TriangleSize;
    // number of circles 
    private int NumberOfCircles;

    private List<UnitSurface> surfaces;

    public CircleGen()
    {
        this.surfaces = new List<UnitSurface>();
    }

    public UnitSurface[] GetUnitSurfaces()
    {

        throw new System.NotImplementedException();
    }

    public void SetRadius(float r)
    {
        this.Radius = r;
    }

    public void SetTriangleSize(float size)
    {
        this.TriangleSize = size;
    }


    /// private methods
    /// 
    private int CalulateNumberOfCircles()
    {
        //find approximate height of the equilateral triangle
        // c^2 = a^2 + b^2 
        double height = Math.Sqrt(Math.Pow(this.TriangleSize, 2f) - Math.Pow(this.TriangleSize / 2, 2));

        // find the perimeter of the equator 
        double p = 2 * Math.PI * this.Radius;

        // find minimum how many triangle fits in the specified sphere
        int minT = (int)Math.Floor(p / height);
        return minT;
    }

    // calculate how many triangle fit to the latitude circle of given angle
    private int CalulateNumberOfTriangles(double latitudeAngle)
    {
        // calculate the radius of the circle 
        double radiusPrime = Math.Abs(Math.Cos((Math.PI / 180) * latitudeAngle))*this.Radius;

        // find the perimeter for the circle 
        double p = 2 * Math.PI * radiusPrime;

        // find min amount of triangles can fit in to said circle 
        int minT = (int)Math.Floor(p / this.TriangleSize);
        
        return minT;
    }


    private void Generate()
    {
        // There are two vertexes that we know their location 
        Point northPole = new Point(0, 0, (float)this.Radius);
        Point southPole = new Point(0, 0, -(float)this.Radius);

        // How many circle we need
        int numberOfCircles = this.CalulateNumberOfCircles();

        
    }
}
