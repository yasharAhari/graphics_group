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
        UnitSurface[] unitSurfaceArray = new UnitSurface[faceList.Count];

        for (int i = 0; i < unitSurfaceArray.Length; i++)
        {
            Face face = (Face)faceList[i];
            Vertex point1 = face.halfEdges[0].endVertex;
            Vertex point2 = face.halfEdges[1].endVertex;
            Vertex point3 = face.halfEdges[2].endVertex;
            UnitSurface triangle = new UnitSurface(point1, point2, point3);
            unitSurfaceArray[i] = triangle;
        }

        return unitSurfaceArray;
    }

    //Written by Yashar
    private Point SToCC(double radius, double phiDegrees, double thetaDegrees)
    {
        float x = (float)(radius * Math.Sin(DegreesToRadians(phiDegrees)) * Math.Cos(DegreesToRadians(thetaDegrees)));
        float y = (float)(radius * Math.Sin(DegreesToRadians(phiDegrees)) * Math.Sin(DegreesToRadians(thetaDegrees)));
        float z = (float)(radius * Math.Cos(DegreesToRadians(phiDegrees)));

        return new Point(x, y, z);
    }

    private Point CCToS(float x, float y, float z)
    {
        throw new NotImplementedException();
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
        HalfEdge[] halfEdges = createHalfEdges(first, second);
        tryToChain(first, second);
        tryToAddFace(first, second, halfEdges[0]);
    }

    //Returns firstToSecond, secondToFirst in that order
    //It is common to call tryToChain() after this
    private HalfEdge[] createHalfEdges(Vertex first, Vertex second)
    {
        HalfEdge[] halfEdges = new HalfEdge[2];

        HalfEdge firstToSecond = new HalfEdge(first, second);
        HalfEdge secondToFirst = new HalfEdge(second, first);

        firstToSecond.opposite = secondToFirst;
        secondToFirst.opposite = firstToSecond;

        first.tryToAddHalfEdge(secondToFirst);
        second.tryToAddHalfEdge(firstToSecond);

        halfEdges[0] = firstToSecond;
        halfEdges[1] = secondToFirst;

        return halfEdges;
    }

    //This should only be called immediately after calling createHalfEdges()
    private void tryToChain(Vertex first, Vertex second)
    {
        if (first.halfEdges.Count >= 2 && second.halfEdges.Count == 1)
        {
            foreach (HalfEdge hE in first.halfEdges)
            {
                //This logic requires that triangles be formed iteratively
                if (hE.face == null)
                {
                    directChain(hE, first, second);
                    return;
                }
            }
        }
    }

    private void directChain(HalfEdge firstEdge, Vertex begin, Vertex end)
    {
        foreach (HalfEdge hE in end.halfEdges)
        {
            if (hE.startVertex == begin)
            {
                directChain(firstEdge, hE);
                return;
            }
        }
    }

    private void directChain(HalfEdge firstEdge, HalfEdge secondEdge)
    {
        firstEdge.next = secondEdge;
    }

    private void tryToAddFace(Vertex first, Vertex second, HalfEdge firstToSecond)
    {
        HalfEdge secondToCommon = null;
        HalfEdge commonToFirst = null;

        foreach (HalfEdge hFirst in first.halfEdges)
        {
            foreach (HalfEdge hSecond in second.halfEdges)
            {
                if (hFirst.startVertex == hSecond.opposite.endVertex)
                {
                    commonToFirst = hFirst;
                    secondToCommon = hSecond.opposite;
                }
            }
        }

        if (secondToCommon != null && commonToFirst != null)
        {
            directChain(commonToFirst, firstToSecond);
            directChain(firstToSecond, secondToCommon);

            Face triangleFace = new Face(commonToFirst, firstToSecond, secondToCommon);
            commonToFirst.face = triangleFace;
            firstToSecond.face = triangleFace;
            secondToCommon.face = triangleFace;

            //add to data structure
            faceList.Add(triangleFace);
        }
    }

    //"Edge" case when generating icosohedron's second to last edge
    private void createEdgeAndDiamond(Vertex first, Vertex second, Vertex third)
    {
        HalfEdge[] halfEdges = createHalfEdges(first, second);
        tryToAddFace(first, second, halfEdges[0]);
        directChain(halfEdges[1], first, third);
    }

    //"Edge" case when generating icosohedron's last edge
    private void createDoubledEdge(Vertex first, Vertex second)
    {
        HalfEdge[] halfEdges = createHalfEdges(first, second);
        tryToAddFace(first, second, halfEdges[0]);
        tryToAddFace(second, first, halfEdges[1]);
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
