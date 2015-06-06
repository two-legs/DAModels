using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms.CLOPE
{
  public class ClopeParams : ClusteringParams
  {
    /// <summary>
    /// Коэффициент отталкивания
    /// </summary>
    public double Repulsion { get; set; }

    public List<string>[] Data;
  }
}
