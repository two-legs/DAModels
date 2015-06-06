using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Metrics
{
  public class EuclideanMetric : ICentroidMetric<double[]>
  {
    public double[] GetCentroid(double[][] a)
    {
      if (a == null)
        throw new ArgumentNullException("Не задано множество для определения центра!");

      double[] centroid = new double[a[0].Length];
      double sum;
      for (int i = 0; i < centroid.Length; i++ )
      {
        sum = 0;
        for (int j = 0; j < a.Length; j++)
        {
          sum += a[j][i];
        }
        centroid[i] = sum / (double)a.Length;
      }

      return centroid;
    }

    public double Calculate(double[] a, double[] b)
    {
      if (a.Length != b.Length)
        throw new ArgumentException("Размерность векторов должна совпадать!");
      
      double s = 0;
      for (int i = 0; i < a.Length; i++ )
        s += (b[i] - a[i]) * (b[i] - a[i]);
      return Math.Sqrt(s);
    }
  }
}
