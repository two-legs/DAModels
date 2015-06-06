using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Metrics
{
  public interface IMetric<T>
  {
    double Calculate(T a, T b);
  }
}
