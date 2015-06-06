using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAModels.Metrics;

namespace DAModels.Clustering.Algorithms.KMeans
{
  public class KMeansParams : ClusteringParams
  {
    public ICentroidMetric<double[]> Metric { get; set; }
    public double[][] Data { get; set; }
  }
}
