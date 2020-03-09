using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : IGenerator
{
    private ArrayList vertexList = new ArrayList();
    private ArrayList faceList = new ArrayList();
    private float radius = 4f;
    private float edgeLength = 0f;
    private double phiRotationDegrees;
    private double thetaRotationDegrees;
    private double southPoleThetaRotationOffsetDegrees;

    public SphereGenerator()
    {
        phiRotationDegrees = 60;
        thetaRotationDegrees = 72;
        southPoleThetaRotationOffsetDegrees = thetaRotationDegrees / 2;

        generateIcosahedron();
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

    //"Edge" case when generating icosahedron's second to last edge
    private void createEdgeAndDiamond(Vertex first, Vertex second, Vertex third)
    {
        HalfEdge[] halfEdges = createHalfEdges(first, second);
        tryToAddFace(first, second, halfEdges[0]);
        directChain(halfEdges[1], first, third);
    }

    //"Edge" case when generating icosahedron's last edge
    private void createDoubledEdge(Vertex first, Vertex second) //TODO fix bug
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

    private Vertex[] createNorthVertices()
    {
        Vertex[] vertices = new Vertex[5];

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

        vertices[0] = first;
        vertices[1] = second;
        vertices[2] = third;
        vertices[3] = fourth;
        vertices[4] = fifth;

        return vertices;
    }

    private Vertex createSouthPole()
    {
        Vertex southPole = new Vertex(SToCC(radius, 180, 0));

        //add to data structure
        vertexList.Add(southPole);

        return southPole;
    }

    private Vertex[] createSouthVertices()
    {
        Vertex[] vertices = new Vertex[5];

        Vertex first = new Vertex(SToCC(radius, 180 - phiRotationDegrees, southPoleThetaRotationOffsetDegrees));
        Vertex second = new Vertex(SToCC(radius, 180 - phiRotationDegrees, southPoleThetaRotationOffsetDegrees + thetaRotationDegrees));
        Vertex third = new Vertex(SToCC(radius, 180 - phiRotationDegrees, southPoleThetaRotationOffsetDegrees + thetaRotationDegrees * 2));
        Vertex fourth = new Vertex(SToCC(radius, 180 - phiRotationDegrees, southPoleThetaRotationOffsetDegrees + thetaRotationDegrees * 3));
        Vertex fifth = new Vertex(SToCC(radius, 180 - phiRotationDegrees, southPoleThetaRotationOffsetDegrees + thetaRotationDegrees * 4));

        //add to data structure
        vertexList.Add(first);
        vertexList.Add(second);
        vertexList.Add(third);
        vertexList.Add(fourth);
        vertexList.Add(fifth);

        vertices[0] = first;
        vertices[1] = second;
        vertices[2] = third;
        vertices[3] = fourth;
        vertices[4] = fifth;

        return vertices;

    }

    public void generateIcosahedron()
    {
        Vertex northPole = createNorthPole();
        Vertex[] northVertices = createNorthVertices();
        Vertex[] southVertices = createSouthVertices();
        Vertex southPole = createSouthPole();

        //Phase 1: Create top section, fanning out from north pole
        createEdge(northPole, northVertices[0]);
        createEdge(northVertices[0], northVertices[1]);
        createEdge(northVertices[1], northPole);
        createEdge(northVertices[1], northVertices[2]);
        createEdge(northVertices[2], northPole);
        createEdge(northVertices[2], northVertices[3]);
        createEdge(northVertices[3], northPole);
        createEdge(northVertices[3], northVertices[4]);
        createEdge(northVertices[4], northPole);
        createEdge(northVertices[4], northVertices[0]);

        //Phase 2: Add on downward-facing triangles within middle section
        createEdge(northVertices[0], southVertices[0]);
        createEdge(southVertices[0], northVertices[1]);
        createEdge(northVertices[1], southVertices[1]);
        createEdge(southVertices[1], northVertices[2]);
        createEdge(northVertices[2], southVertices[2]);
        createEdge(southVertices[2], northVertices[3]);
        createEdge(northVertices[3], southVertices[3]);
        createEdge(southVertices[3], northVertices[4]);
        createEdge(northVertices[4], southVertices[4]);
        createEdge(southVertices[4], northVertices[0]);

        //Phase 3: Add on bottom section while also completing the middle section
        createEdge(southVertices[0], southVertices[1]);
        createEdge(southVertices[0], southPole);
        createEdge(southPole, southVertices[1]);
        createEdge(southVertices[1], southVertices[2]);
        createEdge(southPole, southVertices[2]);
        createEdge(southVertices[2], southVertices[3]);
        createEdge(southPole, southVertices[3]);
        createEdge(southVertices[3], southVertices[4]);
        createEdgeAndDiamond(southPole, southVertices[4], southVertices[0]);
        createDoubledEdge(southVertices[4], southVertices[0]);
    }
}
