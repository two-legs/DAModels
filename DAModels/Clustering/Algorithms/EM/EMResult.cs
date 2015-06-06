using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms.EM
{
  public class EMResult : ClusteringResult
  {
    public int ClusterCount { get; private set; }
    public double[][] Medians { get; set; }
    public double[] Weights { get; set; }
    public double[][] Covariations { get; set; }
    public double[][] Probabilities { get; set; }
    public double[][] Data { get; set; }

    public string medians;
    public string covariations;
    public string weigths;

    public EMResult(int cluster_count)
    {
      ClusterCount = cluster_count;
    }

    public override string Print()
    {
      StringBuilder s = new StringBuilder("EM-clustering result\n\n");
      s.AppendFormat("Cluster count: {0}\n", ClusterCount);

      s.AppendFormat("Medians:\n{0}", medians);
      s.AppendFormat("Covariations:\n {0}", covariations);
      return s.ToString();
    }
  }
}
