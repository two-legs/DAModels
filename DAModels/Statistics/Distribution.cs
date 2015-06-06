using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Statistics
{
  public abstract class Distribution
  {
    public abstract double GetProbability(double value);
  }
}
