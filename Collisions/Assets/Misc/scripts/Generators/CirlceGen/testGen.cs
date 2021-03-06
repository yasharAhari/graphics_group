﻿using System.Collections.Generic;

public class testGen : IGenerator
{
    public SortedSet<Face> GetFaces()
    {
        throw new System.NotImplementedException();
    }

    public Face getFaceWithId(string faceId)
    {
        throw new System.NotImplementedException();
    }

    public UnitSurface[] GetUnitSurfaces()
    {
        // make a pyramid by hand for testing 
        UnitSurface[] surfaces = new UnitSurface[6];
        Point commonAppex = new Point(0, 0, 10f);

        surfaces[0] = new UnitSurface(new Point(5,-5,0), new Point(5,5,0), commonAppex, "Triangle");
        surfaces[1] = new UnitSurface(new Point(5,5,0), new Point(-5,5,0), commonAppex, "Triangle");
        surfaces[2] = new UnitSurface(new Point(-5,5,0), new Point(-5,-5,0), commonAppex, "Triangle");
        surfaces[3] = new UnitSurface(new Point(-5,-5,0), new Point(5,-5,0), commonAppex, "Triangle");
        surfaces[4] = new UnitSurface(new Point(-5,-5,0), new Point(5,5,0), new Point(5,-5,0), "Triangle");
        surfaces[5] = new UnitSurface(new Point(5, 5, 0), new Point(-5, -5, 0), new Point(-5, 5, 0), "Triangle");

        return surfaces;
    }

    public SortedSet<Vertex> GetVertices()
    {
        throw new System.NotImplementedException();
    }

    public void SetRadius(float r)
    {
        // no need
        throw new System.NotImplementedException();
    }

    public void SetTriangleSize(float size)
    {
        // who cares?
        throw new System.NotImplementedException();
    }

    
}
