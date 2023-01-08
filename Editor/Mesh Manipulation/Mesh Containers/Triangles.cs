using System.Collections.Generic;

static public class Triangles
{
    static public List<Triangle> TransformIntToVec3(int[] triangles)
    {
        List<Triangle> result = new List<Triangle>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Triangle triangle = new Triangle(triangles[i], triangles[i + 1], triangles[i + 2]);
            result.Add(triangle);
        }
        return result;
    }

    static public int[] TransformVec3ToInt(List<Triangle> triangles)
    {
        List<int> result = new List<int>();
        foreach (Triangle triangle in triangles)
        {
            result.Add((int)triangle.data[0]);
            result.Add((int)triangle.data[1]);
            result.Add((int)triangle.data[2]);
        }
        return result.ToArray();
    }
}
