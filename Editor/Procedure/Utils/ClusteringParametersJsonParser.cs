public class ClusteringParametersJsonParser
{
    public int? nClusters { get; set; } = 100;
    public int? distanceThreshold { get; set; } = 100;
    public string affinity { get; set; } = null;
    public string linkage { get; set; } = null;
    public bool showCharts { get; set; } = false;

    public ClusteringParametersJsonParser(int? nClusters, int? distanceThreshold, string affinity, string linkage, bool showCharts)
    {
        this.nClusters = nClusters;
        this.distanceThreshold = distanceThreshold;
        this.affinity = affinity;
        this.linkage = linkage;
        this.showCharts = showCharts;
    }

    public string GenerateJsonSource()
    {
        string nClustersText = string.Empty;
        string distanceThresholdText = string.Empty;

        if (nClusters != null)
            nClustersText = nClusters.ToString();
        else
            nClustersText = "null";

        if (distanceThreshold != null)
            distanceThresholdText = distanceThreshold.ToString();
        else
            distanceThresholdText = "null";

        return $"{{ \"nClusters\": {nClustersText}, \"distanceThreshold\": {distanceThresholdText}, \"affinity\": \"{affinity}\", \"linkage\": \"{linkage}\", \"showCharts\": {showCharts.ToString().ToLower()} }}";
    }
}
