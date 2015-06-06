using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;

namespace DAModels.Clustering.Algorithms.EM
{
  public class EMClustering : IClusteringAlgorithm<double[]>
  {
    private int _cluster_count = 0;
    private int _dimensions = 0;
    private int _max_iterations;

    private double eps = 0.0000000000001;
    
    private DenseMatrix medians;
    private DenseVector weights;
    private DenseMatrix covariations;
    private DenseMatrix probabilities;
    private DenseMatrix data;
    private double[][] RawData { get; set; }

    public EMResult Result { get; private set; }
    
    public EMClustering(int cluster_count, int max_iterations) 
    {
      _cluster_count = cluster_count;
      _max_iterations = max_iterations;
      
    }
    public EMClustering(EMParams cparams)
      : this(cparams.ClusterCount, cparams.Iterations)
    {
      
    }
    public ClusteringResult MakeClustering(double[][] data)
    {
      this.data = (DenseMatrix)Matrix.Build.DenseOfRowArrays(data);
      RawData = data;

      _dimensions = data[0].Length;
      Init();

      int iteration = 0;
      while (iteration < _max_iterations)
      {
        Expetation();
        Maximization();

        Console.WriteLine(iteration++);
      }

      return GetResult();
    }

    private ClusteringResult GetResult()
    {
      Result = new EMResult(_cluster_count);
      Result.Covariations = covariations.ToRowArrays();
      Result.Medians = medians.ToRowArrays();
      Result.Weights = weights.ToArray();
      Result.Probabilities = probabilities.ToRowArrays();
      Result.Name = "EM-clusterisation result";
      Result.covariations = covariations.ToString();
      Result.medians = medians.ToString();

      return Result;
    }

    private void Expetation()
    {
      for (int i = 0; i < probabilities.RowCount; i++)
      {
        double sum = 0;
        for (int s = 0; s < _cluster_count; s++ )
        {
          sum += weights[s] * Probability((DenseVector)data.Row(i), s);
        }
        for (int j = 0; j < probabilities.ColumnCount; j++)
        {
          double prob = weights[j] * Probability((DenseVector)data.Row(i), j) / sum;
          probabilities[i, j] = prob;
        }
      }
      
    }

    private void Maximization()
    {
      int n = data.RowCount;
      // новый вес
      for (int j = 0; j < weights.Count; j++ )
      {
        double sum = 0;
        for (int i = 0; i < probabilities.RowCount; i++ )
        {
          sum += probabilities[i, j];
        }
        weights[j] = sum / n;
      }

      // новые мат. ожидания
      for (int i = 0; i < _cluster_count; i++ )
      {
        for (int j = 0; j < _dimensions; j++ )
        {
          double sum = 0;
          for (int s = 0; s < n; s++ )
          {
            sum += probabilities[s, i] * data[s, j];
          }

          medians[i, j] = sum / (weights[i] * n);
        }
      }

      // новая матрица ковариаций 
      for (int i = 0; i < _cluster_count; i++)
      {
        for (int j = 0; j < _dimensions; j++)
        {
          double sum = 0;
          for (int s = 0; s < n; s++)
          {
            sum += probabilities[s, i] * (data[s, j] - medians[i, j]) * (data[s, j] - medians[i, j]);
          }
          
          covariations[i, j] = sum / (weights[i] * n);
        }
      }
    }

    private void Init()
    {
      covariations = (DenseMatrix)Matrix.Build.Dense(_cluster_count, _dimensions, 1);
      weights = (DenseVector)Vector.Build.Dense(_cluster_count, (1.0 / (double)_cluster_count));
      medians = (DenseMatrix)Matrix.Build.DenseOfRowArrays(GetRandomItems(RawData, _cluster_count));
      probabilities = new DenseMatrix(RawData.Length, _cluster_count);
    }

    public double[][] GetRandomItems(double[][] source, int count) 
    {
      double[][] items = new double[count][];

      var rnd = new Random();
      List<int> indexes = new List<int>();

      int rnd_index;
      for (int i = 0; i < items.Length; i++)
      {
        do
        {
          rnd_index = rnd.Next(0, source.Length);
        } while (indexes.Contains(rnd_index));
        items[i] = source[rnd_index];

        indexes.Add(rnd_index);
      }

      return items;
    }
    /// <summary>
    /// Вероятность принадлежности кластеру
    /// </summary>
    /// <param name="x">Объект</param>
    /// <param name="cluster">Номер кластера</param>
    /// <returns></returns>
    private double Probability(DenseVector x, int cluster)
    {
      DenseMatrix cov = (DenseMatrix)Matrix.Build.DenseOfDiagonalVector(covariations.Row(cluster));
      double distance = MahalanobisDistance(x, (DenseVector)medians.Row(cluster), cov);

      double determinat = 1;
      for (int i = 0; i < cov.RowCount; i++)
        determinat *= cov[i, i];

      double numerator = Math.Exp(-distance / 2.0);
      double denomenator = Math.Pow((2 * Math.PI), (double)_dimensions / 2.0) * Math.Sqrt(determinat);
      double value = numerator / denomenator;
      if (Double.IsNaN(value) || value < eps)
        value = eps;
      return value;
    }
    /// <summary>
    /// Вероятность принадлежности кластеру
    /// </summary>
    /// <param name="x">Объект</param>
    /// <param name="cluster">Номер кластера</param>
    /// <returns></returns>
    public double Probability(double[] x, int cluster)
    {
      var vx = Vector.Build.DenseOfArray(x);
      return Probability((DenseVector)vx, cluster);
    }

    private double MahalanobisDistance(Vector x, Vector y, Matrix cov)
    {
      DenseMatrix invCov = (DenseMatrix)Matrix.Build.DenseDiagonal(cov.RowCount, cov.ColumnCount, i => 1 / cov[i, i]);
      return (x - y).ToRowMatrix().Multiply(invCov).Multiply((x - y).ToColumnMatrix())[0, 0];
    }
    public double MahalanobisDistance(double[] x, double[] y, double[][] cov)
    {
      var vx = (DenseVector)Vector.Build.DenseOfArray(x);
      var vy = (DenseVector)Vector.Build.DenseOfArray(y);
      var CM = (DenseMatrix)Matrix.Build.DenseOfRowArrays(cov);

      return MahalanobisDistance(vx, vy, CM);
    }

  }
}
