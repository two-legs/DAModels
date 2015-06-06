using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Metrics
{
  public interface ICentroidMetric<T> : IMetric<T>
  {
    T GetCentroid(T[] a);
  }
}
