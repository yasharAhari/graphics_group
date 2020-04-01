

using System.Collections.Generic;

public interface IGenerator
{
    void SetRadius(float r);
    void SetTriangleSize(float size);
    UnitSurface[] GetUnitSurfaces();
    SortedSet<Face> GetFaces();
    SortedSet<Vertex> GetVertices();
    Face getFaceWithId(string faceId);
}
