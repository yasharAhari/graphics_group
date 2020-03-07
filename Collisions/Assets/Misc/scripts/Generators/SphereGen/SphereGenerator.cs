using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : IGenerator
{
    private ArrayList vertexList = new ArrayList();
    private ArrayList faceList = new ArrayList();
    private float radius = 0f;
    private float edgeLength = 0f;
    private double phiRotationDegrees;
    private double thetaRotationDegrees;
    private double southPoleThetaRotationOffsetDegrees;

    public SphereGenerator()
    {
        phiRotationDegrees = RadiansToDegrees(Math.Atan(0.5)); //about 26.5 degrees
        thetaRotationDegrees = 72;
        southPoleThetaRotationOffsetDegrees = thetaRotationDegrees / 2;
    }

    public void SetRadius(float r)
    {
        radius = r;
        edgeLength = (float)(r / 0.9510565163); //icosohedron's circumscribed sphere equation
    }

    public void SetTriangleSize(float size)
    {
        // who cares?
        throw new System.NotImplementedException();
    }

    public UnitSurface[] GetUnitSurfaces()
    {
        //not done yet
        throw new System.NotImplementedException();
    }

    //Written by Yashar
    private Point SToCC(double radius, double phiDegrees, double thetaDegrees)
    {
        float x = (float)(radius * Math.Sin(DegreesToRadians(phiDegrees)) * Math.Cos(DegreesToRadians(thetaDegrees)));
        float y = (float)(radius * Math.Sin(DegreesToRadians(phiDegrees)) * Math.Sin(DegreesToRadians(thetaDegrees)));
        float z = (float)(radius * Math.Cos(DegreesToRadians(phiDegrees)));

        return new Point(x, y, z);
    }

    //Written by Yashar
    private double DegreesToRadians(double degrees)
    {
        return (Math.PI / 180) * degrees;
    }

    private double RadiansToDegrees(double radians)
    {
        return (180 / Math.PI) * radians;
    }

    private void createEdge(Vertex first, Vertex second)
    {
        HalfEdge firstToSecond = new HalfEdge(first, second, false);
        HalfEdge secondToFirst = new HalfEdge(second, first, true);

        first.tryToAddHalfEdge(secondToFirst);
        second.tryToAddHalfEdge(firstToSecond);

        //set some next values of half edges
        foreach (HalfEdge halfEdge in first.halfEdges)
        {
            if (halfEdge.isClockwise == false)
            {
                if (halfEdge.face == null) //edge case
                {
                    halfEdge.next = firstToSecond;
                }
            }
            
        }
        foreach (HalfEdge halfEdge in second.halfEdges)
        {
            if (halfEdge.isClockwise == true)
            {
                if (halfEdge.face == null) //edge case
                {
                    halfEdge.next = secondToFirst;
                }
            }
        }

        //check if a triangle was made
        //first see if there is a common third point
        foreach (HalfEdge halfEdge in first.halfEdges)
        {
            if (halfEdge.isClockwise == false)
            {
                Vertex possibleCommonVertex = halfEdge.startVertex;
                foreach (HalfEdge pHalfEdge in possibleCommonVertex.halfEdges)
                {
                    if (pHalfEdge.isClockwise == false && pHalfEdge.startVertex == second)
                    {
                        //this is a common vertex, so set more next values
                        firstToSecond.next = pHalfEdge;
                        foreach (HalfEdge ppHalfEdge in possibleCommonVertex.halfEdges)
                        {
                            if (ppHalfEdge.startVertex == first)
                            {
                                secondToFirst.next = ppHalfEdge;
                            }
                        }
                    }
                }
                
            }
        }
        // Validate cycles
        if (firstToSecond.next != null
            && firstToSecond.next.next != null
            && firstToSecond.next.next.endVertex == first
            && secondToFirst.next != null
            && secondToFirst.next.next != null
            && secondToFirst.next.next.endVertex == second)
        {
            // THE CLOCKWISE CYCLES SHOULDN't WORK
            // Make face with counter clockwise half edges
            Face validatedFace = new Face(firstToSecond, firstToSecond.next, firstToSecond.next.next);
            // add to data structure
            faceList.Add(validatedFace);
        }
    }

    private Vertex createNorthPole()
    {
        Vertex northPole = new Vertex(SToCC(radius, 0, 0));
        //add to data structure
        vertexList.Add(northPole);
        return northPole;
    }

    private ArrayList createNorthVertices()
    {
        ArrayList vertices = new ArrayList();
        Vertex first = new Vertex(SToCC(radius, phiRotationDegrees, 0));
        Vertex second = new Vertex(SToCC(radius, phiRotationDegrees, thetaRotationDegrees));
        Vertex third = new Vertex(SToCC(radius, phiRotationDegrees, thetaRotationDegrees * 2));
        Vertex fourth = new Vertex(SToCC(radius, phiRotationDegrees, thetaRotationDegrees * 3));
        Vertex fifth = new Vertex(SToCC(radius, phiRotationDegrees, thetaRotationDegrees * 4));
        //add to data structure
        vertexList.Add(first);
        vertexList.Add(second);
        vertexList.Add(third);
        vertexList.Add(fourth);
        vertexList.Add(fifth);

        vertices.Add(first);
        vertices.Add(second);
        vertices.Add(third);
        vertices.Add(fourth);
        vertices.Add(fifth);
        return vertices;
    }

    private void createNorthTriangles()
    {
        Vertex northPole = createNorthPole();
        ArrayList vertices = createNorthVertices();

        createEdge(northPole, (Vertex)vertices[0]);
        createEdge((Vertex)vertices[0], (Vertex)vertices[1]);
        createEdge((Vertex)vertices[1], northPole);
        createEdge((Vertex)vertices[1], (Vertex)vertices[2]);
        createEdge((Vertex)vertices[2], northPole);
        createEdge((Vertex)vertices[2], (Vertex)vertices[3]);
        createEdge((Vertex)vertices[3], northPole);
        createEdge((Vertex)vertices[3], (Vertex)vertices[4]);
        createEdge((Vertex)vertices[4], northPole);
        createEdge((Vertex)vertices[4], (Vertex)vertices[0]); //edge case
    }

    private Vertex createSouthPole()
    {
        Vertex southPole = new Vertex(SToCC(radius, 180, 0));
        //add to data structure
        vertexList.Add(southPole);
        return southPole;
    }
}
