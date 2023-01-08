public class ClusterNode
{
    public ClusterNode[] children;
    public int index;

    public ClusterNode(ClusterNode[] children, int index)
    {
        this.children = children;
        this.index = index;
    }

    public ClusterNode(int index)
    {
        this.children = null;
        this.index = index;
    }
}