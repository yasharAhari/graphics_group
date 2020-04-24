using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : IGenerator
{
    private class FaceVertices
    {
        public Vertex oneTwoMid;
        public Vertex twoThreeMid;
        public Vertex threeOneMid;

        public FaceVertices(Vertex first, Vertex second, Vertex third)
        {
            oneTwoMid = first;
            twoThreeMid = second;
            threeOneMid = third;
        }
    }

    public SortedSet<Vertex> vertexSet = new SortedSet<Vertex>();
    public SortedSet<Face> faceSet = new SortedSet<Face>();
    private float radius;
    private float edgeLength;
    private double phiRotationDegrees;
    private double thetaRotationDegrees;
    private double southPoleThetaRotationOffsetDegrees;
    private int numberOfTriangulations;

    public SphereGenerator()
    {
        radius = 1f;
        phiRotationDegrees = 60;
        thetaRotationDegrees = 72;
        southPoleThetaRotationOffsetDegrees = thetaRotationDegrees / 2;
        numberOfTriangulations = 3; //Max of 3, above 3 gets buggy
    }

    public SortedSet<Face> GetFaces()
    {
        return this.faceSet;
    }

    public Face getFaceWithId(string faceId)
    {
        foreach(Face thisFace in this.faceSet)
        {
            if(thisFace.id.Equals(faceId))
            {
                return thisFace;
            }
        }
        return null;
    }

    public SortedSet<Vertex> GetVertices()
    {
        return this.vertexSet;
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
        generateSphere();

        UnitSurface[] unitSurfaceArray = new UnitSurface[faceSet.Count];

        ArrayList faceList = new ArrayList();
        foreach (Face face in faceSet)
        {
            faceList.Add(face);
        }

        for (int i = 0; i < unitSurfaceArray.Length; i++)
        {
            Face face = (Face) faceList[i];
            Vertex point1 = face.halfEdges[0].endVertex;
            Vertex point2 = face.halfEdges[1].endVertex;
            Vertex point3 = face.halfEdges[2].endVertex;
            UnitSurface triangle = new UnitSurface(point1, point2, point3, face.id);
            unitSurfaceArray[i] = triangle;
        }

        return unitSurfaceArray;
    }

    private void generateSphere()
    {
        generateIcosahedron();
        triangulateIcosahedron(numberOfTriangulations);
    }

    private void generateIcosahedron()
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
        createEdge(southPole, southVertices[4]);
        createDoubledEdge(southVertices[4], southVertices[0]);
    }

    private void triangulateIcosahedron(int counter)
    {
        for (int i = 0; i < counter; i++)
        {
            //Create set of "edges" in icosahedron
            SortedSet<Edge> edgeSet = createEdgeSet();

            //Split each edge
            foreach (Edge edge in edgeSet)
            {
                Vertex first = edge.hE1.startVertex;
                Vertex second = edge.hE1.endVertex;
                HalfEdge firstToSecond = edge.hE1;
                HalfEdge secondToFirst = edge.hE2;
                HalfEdge previousToFirst = findPreviousHalfEdge(firstToSecond);
                HalfEdge previousToSecond = findPreviousHalfEdge(secondToFirst);

                splitEdge(first, second, firstToSecond, secondToFirst, previousToFirst, previousToSecond);
            }
            //Verify splits
            ArrayList verifiedFaceList = new ArrayList();
            foreach (Face face in faceSet)
            {
                HalfEdge start = face.halfEdges[0];
                if (start.next != null
                && start.next.next != null
                && start.next.next.next != null
                && start.next.next.next.next != null
                && start.next.next.next.next.next != null
                && start.next.next.next.next.next.next != null
                && start.next.next.next.next.next.next == start)
                {
                    verifiedFaceList.Add(face);
                }
            }

            //Triangulate each face
            ArrayList triangulationVerticesList = new ArrayList();

            foreach (Face face in faceSet)
            {
                triangulationVerticesList.Add(getTriangulationVertices(face));
                dereferenceFace(face);
            }

            faceSet.Clear();

            foreach (FaceVertices vertices in triangulationVerticesList)
            {
                createEdge(vertices.oneTwoMid, vertices.threeOneMid);
                createEdge(vertices.twoThreeMid, vertices.oneTwoMid);
                directChain(vertices.threeOneMid, vertices.oneTwoMid, vertices.twoThreeMid);
                createEdge(vertices.threeOneMid, vertices.twoThreeMid);
            }

            foreach (FaceVertices vertices in triangulationVerticesList)
            {
                HalfEdge oneTwoMidToTwoThreeMid = null;
                foreach (HalfEdge hTwoThreeMid in vertices.twoThreeMid.halfEdges)
                {
                    if (hTwoThreeMid.startVertex.Equals(vertices.oneTwoMid))
                    {
                        oneTwoMidToTwoThreeMid = hTwoThreeMid;
                    }
                }
                tryToAddFace(vertices.oneTwoMid, vertices.twoThreeMid, oneTwoMidToTwoThreeMid);
            }
        }

        //Reset all vertex coordinates to correct spherical radius length
        foreach (Vertex vertex in vertexSet)
        {
            //double radius = Math.Sqrt(Math.Pow(vertex.x, 2) + Math.Pow(vertex.y, 2) + Math.Pow(vertex.z, 2));
            double phiRadians = getPhiRadians(vertex.x, vertex.y, vertex.z);
            double thetaRadians = getThetaRadians(vertex.x, vertex.y, vertex.z);

            Point coords = SToCCUsingRadians(this.radius, phiRadians, thetaRadians);

            vertex.x = coords.x;
            if (vertex.y < 0 && coords.y > 0)
            {
                vertex.y = -coords.y;
            }
            else
            {
                vertex.y = coords.y;
            }
            vertex.z = coords.z;
        }

    }

    SortedSet<Edge> createEdgeSet()
    {
        SortedSet<Edge> edgeSet = new SortedSet<Edge>();
        foreach (Face face in faceSet)
        {
            foreach (HalfEdge hE in face.halfEdges)
            {
                Edge edge = new Edge(hE, hE.opposite);
                edgeSet.Add(edge);
            }
        }

        return edgeSet;
    }

    HalfEdge findPreviousHalfEdge(HalfEdge originalHalfEdge)
    {
        Face originalFace = originalHalfEdge.face;
        HalfEdge previousHalfEdge = originalHalfEdge.next;

        while (previousHalfEdge.next != originalHalfEdge)
        {
            previousHalfEdge = previousHalfEdge.next;
        }

        return previousHalfEdge;
    }

    void splitEdge(Vertex first, Vertex second, HalfEdge firstToSecond, HalfEdge secondToFirst, 
        HalfEdge previousToFirst, HalfEdge previousToSecond)
    {
        Vertex midVertex = createMidVertex(first, second);
        firstToSecond.startVertex = midVertex;
        secondToFirst.startVertex = midVertex;

        HalfEdge firstToMid = new HalfEdge(first, midVertex);
        HalfEdge secondToMid = new HalfEdge(second, midVertex);

        firstToMid.next = firstToSecond;
        secondToMid.next = secondToFirst;
        firstToMid.opposite = secondToFirst;
        secondToFirst.opposite = firstToMid;
        secondToMid.opposite = firstToSecond;
        firstToSecond.opposite = secondToMid;
        previousToSecond.next = secondToMid;
        previousToFirst.next = firstToMid;
        midVertex.tryToAddHalfEdge(secondToMid);
        midVertex.tryToAddHalfEdge(firstToMid);

        //add to data structure
        vertexSet.Add(midVertex);
    }

    Vertex createMidVertex(Vertex first, Vertex second)
    {
        float midX = (first.x + second.x) / 2.0f;
        float midY = (first.y + second.y) / 2.0f;
        float midZ = (first.z + second.z) / 2.0f;

        return new Vertex(midX, midY, midZ);
    }

    FaceVertices getTriangulationVertices(Face currentFace)
    {
        HalfEdge threeOneMidToFirst = currentFace.halfEdges[0];
        HalfEdge oneTwoMidToSecond = currentFace.halfEdges[1];
        HalfEdge twoThreeMidToThird = currentFace.halfEdges[2];

        Vertex threeOneMid = threeOneMidToFirst.startVertex;
        //Vertex first = threeOneMidToFirst.endVertex;
        Vertex oneTwoMid = oneTwoMidToSecond.startVertex;
        //Vertex second = oneTwoMidToSecond.endVertex;
        Vertex twoThreeMid = twoThreeMidToThird.startVertex;
        //Vertex third = twoThreeMidToThird.endVertex;
        FaceVertices vertices = new FaceVertices(oneTwoMid, twoThreeMid, threeOneMid);

        return vertices;
    }

    void dereferenceFace(Face currentFace)
    {
        foreach (HalfEdge hE in currentFace.halfEdges)
        {
            if (hE.next == null)
            {
                throw new System.ArgumentNullException("Face " + currentFace.id + " has corrupted half edges");
            }

            hE.next.next = null;
            hE.face = null;
        }
    }

    //Written by Yashar
    private Point SToCC(double radius, double phiDegrees, double thetaDegrees)
    {
        float x = (float)(radius * Math.Sin(DegreesToRadians(phiDegrees)) * Math.Cos(DegreesToRadians(thetaDegrees)));
        float y = (float)(radius * Math.Sin(DegreesToRadians(phiDegrees)) * Math.Sin(DegreesToRadians(thetaDegrees)));
        float z = (float)(radius * Math.Cos(DegreesToRadians(phiDegrees)));

        return new Point(x, y, z);
    }

    private Point SToCCUsingRadians(double radius, double phiRadians, double thetaRadians)
    {
        float x = (float)(radius * Math.Sin(phiRadians) * Math.Cos(thetaRadians));
        float y = (float)(radius * Math.Sin(phiRadians) * Math.Sin(thetaRadians));
        float z = (float)(radius * Math.Cos(phiRadians));

        return new Point(x, y, z);
    }

    private double getThetaRadians(float x, float y, float z)
    {
        double phiRadians = getPhiRadians(x, y, z);
        double radius = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        double acosArg;
        if (x == 0)
        {
            acosArg = 0;
        }
        else
        {
            acosArg = x / (radius * Math.Sin(phiRadians));
        }
        
        if (acosArg < -1)
        {
            acosArg = -1;
        }
        if (acosArg > 1)
        {
            acosArg = 1;
        }
        double thetaRadians = Math.Acos(acosArg);
        return thetaRadians;
    }

    private double getPhiRadians(float x, float y, float z)
    {
        double radius = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        double acosArg = z / radius;
        if (acosArg < -1)
        {
            acosArg = -1;
        }
        if (acosArg > 1)
        {
            acosArg = 1;
        }
        double phiRadians = Math.Acos(acosArg);
        return phiRadians;
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

    private void directChain(Vertex begin, Vertex middle, Vertex end)
    {
        HalfEdge middleToEnd = null;
        HalfEdge beginToMiddle = null;

        foreach (HalfEdge hE in end.halfEdges)
        {
            if (hE.startVertex == middle)
            {
                middleToEnd = hE;
                break;
            }
            if (middleToEnd != null)
            {
                break;
            }
        }

        foreach (HalfEdge hE in middle.halfEdges)
        {
            if (hE.startVertex == begin)
            {
                beginToMiddle = hE;
                break;
            }
            if (beginToMiddle != null)
            {
                break;
            }
        }

        if (middleToEnd != null && beginToMiddle != null)
        {
            directChain(beginToMiddle, middleToEnd);
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

    private bool tryToAddFace(Vertex first, Vertex second, HalfEdge firstToSecond)
    {
        HalfEdge secondToCommon = null;
        HalfEdge commonToFirst = null;

        foreach (HalfEdge hFirst in first.halfEdges)
        {
            foreach (HalfEdge hSecond in second.halfEdges)
            {
                if (hFirst.face == null && hFirst.startVertex == hSecond.opposite.endVertex)
                {
                    commonToFirst = hFirst;
                    secondToCommon = hSecond.opposite;
                    break;
                }
            }
            if (secondToCommon != null && commonToFirst != null)
            {
                break;
            }
        }

        if (secondToCommon != null && commonToFirst != null)
        {
            directChain(commonToFirst, firstToSecond);
            directChain(firstToSecond, secondToCommon);
            directChain(secondToCommon, commonToFirst); //May be redundent in some cases

            //Ensure cycle is valid
            if (firstToSecond.next != null
                && firstToSecond.next.next != null
                && firstToSecond.next.next.next == firstToSecond)
            {
                Face triangleFace = new Face(commonToFirst, firstToSecond, secondToCommon);
                commonToFirst.face = triangleFace;
                firstToSecond.face = triangleFace;
                secondToCommon.face = triangleFace;

                //add to data structure
                faceSet.Add(triangleFace);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //"Edge" case when generating icosahedron's last edge
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
        vertexSet.Add(northPole);

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
        vertexSet.Add(first);
        vertexSet.Add(second);
        vertexSet.Add(third);
        vertexSet.Add(fourth);
        vertexSet.Add(fifth);

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
        vertexSet.Add(southPole);

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
        vertexSet.Add(first);
        vertexSet.Add(second);
        vertexSet.Add(third);
        vertexSet.Add(fourth);
        vertexSet.Add(fifth);

        vertices[0] = first;
        vertices[1] = second;
        vertices[2] = third;
        vertices[3] = fourth;
        vertices[4] = fifth;

        return vertices;

    }

    
}
