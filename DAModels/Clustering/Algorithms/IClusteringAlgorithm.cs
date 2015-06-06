using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms
{
  public interface IClusteringAlgorithm<T>
  {
    ClusteringResult MakeClustering(T[] data);
  }
}
