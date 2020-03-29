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

    public bool Equals(Point other)
    {
        if (Object.ReferenceEquals(other, null))
        {
            return false;
        }

        if (Object.ReferenceEquals(this, other))
        {
            return true;
        }

        if (this.GetType() != other.GetType())
        {
            return false;
        }

        //compare x, y, and z values
        return this.GetHashCode() == other.GetHashCode();
        //return ((double)x.CompareTo(other.x) == 0) 
        //    && ((double)y.CompareTo(other.y) == 0)
        //    && ((double)z.CompareTo(other.z) == 0);
    }

    //must be unique enough to differentiate points by coordinates
    public override int GetHashCode()
    {
        float hashCode = 17.0f; //some prime number
        hashCode = hashCode * 31f + x;
        hashCode = hashCode * 31f + y;
        hashCode = hashCode * 31f + z;

        return (int)hashCode;
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

public class Vertex : Point, IComparable<Vertex>
{
    //There should be a maximum of 3 half edges in this ArrayList
    public ArrayList halfEdges = new ArrayList();

    public Vertex(float x, float y, float z) : base(x, y, z)
    {
    }

    public Vertex(Point p) : base(p.x, p.y, p.z)
    {
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

    public int CompareTo(Vertex other)
    {
        return (int)(this.GetHashCode() - other.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Vertex);
    }

    public bool Equals(Vertex other)
    {
        return other != null
            && this.GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        float hashCode = 23f;

        hashCode = hashCode * 37 + this.x;
        hashCode = hashCode * 37 + this.y;
        hashCode = hashCode * 37 + this.z;

        return (int) hashCode;
    }

}

public class HalfEdge
{
    public Vertex startVertex;
    public Vertex endVertex;
    public Face face = null;
    public HalfEdge opposite = null;
    public HalfEdge next = null;

    public HalfEdge(Vertex start, Vertex end)
    {
        startVertex = start;
        endVertex = end;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as HalfEdge);
    }

    public bool Equals(HalfEdge other)
    {
        return other != null
            && this.GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        Point start = startVertex as Point;
        Point end = endVertex as Point;

        var hashCode = 23;
        hashCode = hashCode * 37 + start.GetHashCode();
        hashCode = hashCode * 37 + end.GetHashCode();

        return hashCode;
    }
}

public class Face : IComparable<Face>
{
    public HalfEdge[] halfEdges = new HalfEdge[3];
    public string id = Guid.NewGuid().ToString();

    public Face(HalfEdge one, HalfEdge two, HalfEdge three)
    {
        halfEdges[0] = one;
        halfEdges[1] = two;
        halfEdges[2] = three;
    }

    public int CompareTo(Face other)
    {
        return (int)(this.id.GetHashCode() - other.id.GetHashCode());
    }
}

public class Edge : IComparable<Edge>
{
    public HalfEdge hE1;
    public HalfEdge hE2;

    public Edge(HalfEdge hE1, HalfEdge hE2)
    {
        this.hE1 = hE1;
        this.hE2 = hE2;
    }

    public int CompareTo(Edge other)
    {
        return (int)(this.GetHashCode() - other.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Edge);
    }

    public bool Equals(Edge other)
    {
        return other != null
            && ((hE1.GetHashCode() == other.hE1.GetHashCode() && hE2.GetHashCode() == other.hE2.GetHashCode())
            || (hE1.GetHashCode() == other.hE2.GetHashCode() && hE2.GetHashCode() == other.hE1.GetHashCode()));
    }

    public override int GetHashCode()
    {
        //var hashCode = 19;
        //hashCode = hashCode * 31 + hE1.GetHashCode();
        //hashCode = hashCode * 31 + hE2.GetHashCode();
        
        return hE1.GetHashCode() + hE2.GetHashCode();
    }
}