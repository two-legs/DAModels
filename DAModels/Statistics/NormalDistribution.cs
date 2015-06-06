using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Statistics
{
  public class NormalDistribution : Distribution 
  {
    private double m;
    private double s;
    
    public NormalDistribution(double median, double sigma)
    {
      m = median;
      s = sigma;
    }

    public override double GetProbability(double x)
    {
      return Math.Exp(-(x - m) * (x - m) / (2 * s * s)) / Math.Sqrt(2 * Math.PI * s * s);
    }
  }
}
