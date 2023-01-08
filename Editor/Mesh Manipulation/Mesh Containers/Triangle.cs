public class Triangle
{
    public int[] data;

    public Triangle(int v0, int v1, int v2)
    {
        data = new int[] { v0, v1, v2 };
    }

    public int Contains(int index)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == index) { return i; }
        }
        return -1;
    }
}