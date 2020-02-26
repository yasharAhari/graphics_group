public class testGen : IGenerator
{
    public UnitSurface[] GetUnitSurfaces()
    {
        // make a pyramid by hand for testing 
        UnitSurface[] surfaces = new UnitSurface[6];
        Point commonAppex = new Point(0, 0, 10f);

        surfaces[0] = new UnitSurface(new Point(5,-5,0), new Point(5,5,0), commonAppex);
        surfaces[1] = new UnitSurface(new Point(5,5,0), new Point(-5,5,0), commonAppex);
        surfaces[2] = new UnitSurface(new Point(-5,5,0), new Point(-5,-5,0), commonAppex);
        surfaces[3] = new UnitSurface(new Point(-5,-5,0), new Point(5,-5,0), commonAppex);
        surfaces[4] = new UnitSurface(new Point(-5,-5,0), new Point(5,5,0), new Point(5,-5,0));
        surfaces[5] = new UnitSurface(new Point(5, 5, 0), new Point(-5, -5, 0), new Point(-5, 5, 0));

        return surfaces;
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
