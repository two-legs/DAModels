using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms.EM
{
  public class EMParams : ClusteringParams
  {
    public int Iterations { get; set; }
    public double[][] Data { get; set; }
  }
}
