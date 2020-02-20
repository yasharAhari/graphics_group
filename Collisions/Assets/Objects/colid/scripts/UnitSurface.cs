using UnityEngine;

/**
 * <summary>
 * The UnitSurface
 * <para>
 * The UnitSurface object holds the information about a single triangle surface
 * </para>
 * </summary> 
 * 
 * 
 * 
 * */
public class UnitSurface
{
    public Point a;
    public Point b;
    public Point c;

    /**
     * <summary>Constructor</summary>
     * <param name="a">first point</param>
     * <param name="b">second Point</param>
     * <param name="c">third point</param>
     * */
    public UnitSurface(Point a, Point b, Point c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    /**
     * <summary>Gets a array of all vertexes as Unity Vector3</summary>
     * <returns>An array of the vertexes as Unity Vector3</returns>
     * */
    public Vector3[] getVertices()
    {
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(a.x, a.y, a.z);
        vertices[1] = new Vector3(b.x, b.y, b.z);
        vertices[2] = new Vector3(c.x, c.y, c.z);

        return vertices;
    }

    /**
     * <summary>
     * Get triangles
     * <para>Returns default triangles</para>
     * </summary>
     * 
     * <returns>triangles array</returns>
     * 
     * 
     * */
    public int[] getTriangle()
    {
        int[] triangles = { 0, 1, 2 };
        return triangles;
    }
}
