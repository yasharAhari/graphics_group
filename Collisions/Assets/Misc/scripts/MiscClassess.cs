using System;
/**
 * <summary>
 * The Point.
 * Point object holds three coordinate for a point with two extra floats if needs to be used.
 * </summary>
 * 
 */ 
public class Point
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
