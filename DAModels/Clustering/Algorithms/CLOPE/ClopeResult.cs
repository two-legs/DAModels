using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms.CLOPE
{
  public class ClopeResult : ClusteringResult
  {
    public ClopeCluster[] Clusters;
    public Dictionary<object, int> Clusterization;

    public override string Print()
    {
      StringBuilder str = new StringBuilder("CLOPE Clusterization Result\n");

      int index = 0;
      foreach (ClopeCluster cluster in Clusters)
      {
        str.AppendLine("Cluster #" + index);
        str.AppendLine("Size: " + cluster.Size);
        str.AppendLine("Width: " + cluster.Width);
        str.AppendLine("Square: " + cluster.Square);
          
        foreach (KeyValuePair<string, int> pair in cluster.Historgram)
        {
          str.AppendFormat("{0}:\t{1}\n", pair.Key, pair.Value);
        }

        index++;
      }
      
      return str.ToString();
    }
  }
}
