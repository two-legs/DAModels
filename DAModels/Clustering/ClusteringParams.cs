using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering
{
  public class ClusteringParams
  {
    private int _clusterCount; 
    
    public int ClusterCount 
    {
      get { return _clusterCount; }
      set {
        if (value > 0)
          _clusterCount = value;
        else
          throw new ArgumentException("Неверное число кластеров!");
      } 
    }
  }
}
