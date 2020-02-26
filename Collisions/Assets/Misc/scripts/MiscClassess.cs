using System;
using System.Collections;
/**
 * <summary>
 * The Point.
 * Point object holds three coordinate for a point with two extra floats if needs to be used.
 * </summary>
 * 
 */ 
public class Point : IEquatable<Point>
{
    public float x;
    public float y;
    public float z;
    public float w;
    public float k;

    /**
     * <summary>
     * <para>
     * Constructor 
     * </para>
     * </summary>
     * <param name="x"> The x variable</param>
     * <param name="y"> The y variable</param>
     * <param name="z"> The z variable</param>
     * */
    public Point(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /**
     * <summary>
     * <para>
     * Constructor 
     * </para>
     * </summary>
     * <param name="x"> The x variable </param>
     * <param name="y"> The y variable </param>
     * <param name="z"> The z variable </param>
     * <param name="w"> The w variable </param>
     * <param name="k"> The k variable</param>
     * */
    public Point(float x, float y, float z,float w, float k)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
        this.k = k;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Point);
    }

    public bool Equals(Point point)
    {
        if (Object.ReferenceEquals(point, null))
        {
            return false;
        }

        if (Object.ReferenceEquals(this, point))
        {
            return true;
        }

        if (this.GetType() != point.GetType())
        {
            return false;
        }

        //compare x, y, and z values
        return (x == point.x) && (y == point.y) && (z == point.z);
    }

    //must be unique enough to differentiate points by coordinates
    public override int GetHashCode()
    {
        float seed = 17.0f; //some prime number
        return (int)((x * seed) + (y * seed) + (z * seed));
    }

    public static bool operator ==(Point left, Point right)
    {
        //null checks
        if (Object.ReferenceEquals(left, null))
        {
            if (Object.ReferenceEquals(right, null))
            {
                return true;
            }

            return false;
        }

        return left.Equals(right);
    }

    public static bool operator!=(Point left, Point right)
    {
        return !(left == right);
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w, a.k + b.k);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w, a.k - b.k);
    }

    // cross product for only x,y,z
    public static Point operator *(Point a, Point b)
    {
        return new Point((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x));
    }
}

public class Vertex : Point
{
    //There should be a maximum of 3 half edges in this ArrayList
    private ArrayList halfEdges = new ArrayList();

    public Vertex(float x, float y, float z) : base(x, y, z)
    {
        halfEdges.Capacity = 3;
    }

    public void tryToAddHalfEdge(HalfEdge halfEdge)
    {
        Point endPoint = halfEdge.endVertex;
        Point thisPoint = this;
        if (endPoint == thisPoint)
        {
            halfEdges.Add(halfEdge);
        }
    }
}

public class HalfEdge
{
    public Vertex startVertex;
    public Vertex endVertex;
    public Face face;
    public HalfEdge opposite;
    public HalfEdge next;
    public Boolean isClockwise;
}

public class Face
{
    public HalfEdge[] halfEdges = new HalfEdge[3];

    public Face(HalfEdge one, HalfEdge two, HalfEdge three)
    {
        halfEdges[0] = one;
        halfEdges[1] = two;
        halfEdges[2] = three;
    }
}