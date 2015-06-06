using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAModels.Clustering.Algorithms
{
  public class KMeansResult : ClusteringResult
  {
    public int ClusterCount { get; private set; }
    public double[][] Centriods { get; set; }
    public Dictionary<object, int> Clusterisation { get; set; }

    public KMeansResult(int cluster_count)
    {
      ClusterCount = cluster_count;
    }

    public override string Print()
    {
      StringBuilder s = new StringBuilder("K-Means clustering result\n\n");
      s.AppendFormat("Cluster count: {0}\n", ClusterCount);
      for (int i = 0; i < Centriods.Length; i++ )
      {
        var cluster_elements = Clusterisation.Count(item => item.Value == i);
        string c = "";
        foreach (double v in Centriods[i])
          c += String.Format("{0}\t", v);
        s.AppendFormat("Cluster {0} ({1} elements): centroid ({2}) \n", i, cluster_elements, c);
      }

      return s.ToString();
    }
  }
}
