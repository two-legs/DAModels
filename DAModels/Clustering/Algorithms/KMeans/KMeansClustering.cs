using DAModels.Clustering.Algorithms.KMeans;
using DAModels.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms
{
  public class KMeansClustering : IClusteringAlgorithm<double[]>
  {
    private ICentroidMetric<double[]> _metric;
    private int _cluster_count = 0;
    
    private double[][] _centroids;
    private Dictionary<object, int> _clusterisation = new Dictionary<object,int>();

    public double[][] Data { get; set; }

    public KMeansClustering(ICentroidMetric<double[]> metric, int cluster_count) 
    {
      if (metric == null)
        throw new ArgumentNullException();
      
      _cluster_count = cluster_count;
      _metric = metric;
      

      _centroids = new double[_cluster_count][];
      
    }
    public KMeansClustering(KMeansParams cparams)
      : this((ICentroidMetric<double[]>)cparams.Metric, cparams.ClusterCount)
    {
      
    }

    public ClusteringResult MakeClustering(double[][] data)
    {
      if (data == null)
        throw new ArgumentNullException();
      Data = data;
      MapData(Data);
      Init();
      
      KMeansResult result = new KMeansResult(_cluster_count);

      double[][] prev_centroids;
      do
      {
        prev_centroids = (double[][])_centroids.Clone();
        ExpetationStep();
        MaximizationStep();

        Console.WriteLine(GetMaxDistance(_centroids, prev_centroids));
      } while (GetMaxDistance(_centroids, prev_centroids) > 0);

      result.Centriods = _centroids;
      result.Clusterisation = _clusterisation;
      result.Name = "K-Means clusterisation";

      return result;
    }

    /// <summary>
    /// Отнести каждый элемент к кластеру
    /// </summary>
    private void ExpetationStep()
    {
      double[] distances;
      int cluster;
      foreach (double[] item in Data)
      {
        distances = new double[_cluster_count];

        for (int i = 0; i < distances.Length; i++)
          distances[i] = _metric.Calculate(item, _centroids[i]);

        cluster = Array.IndexOf(distances, distances.Min());

        _clusterisation[item] = cluster;
      }
    }
    /// <summary>
    /// Вычисление новых центров
    /// </summary>
    private void MaximizationStep()
    {
      // Все элементы кластера
      for (int i = 0; i < _centroids.Length; i++ )
      {
        var cluster_elements = _clusterisation.Where(item => item.Value == i)
                                              .Select(item => (double[])item.Key);
        
        if (cluster_elements.Any())
          _centroids[i] = _metric.GetCentroid(cluster_elements.ToArray());
      }
    }

    public void Init()
    {
      var rnd = new Random();
      List<int> indexes = new List<int>();

      int rnd_index;
      for (int i = 0; i < _centroids.Length; i++)
      {
        do
        {
          rnd_index = rnd.Next(0, Data.Length);
        } while (indexes.Contains(rnd_index));
        _centroids[i] = Data[rnd_index];

        indexes.Add(rnd_index);
      }
    }
    /// <summary>
    /// Заполняет словарь данными
    /// </summary>
    /// <param name="data"></param>
    public void MapData(double[][] data)
    {
      foreach (object o in data)
        _clusterisation.Add(o, 0);
    }

    public double GetMaxDistance(double[][] a, double[][] b)
    {
      if (a.Length != b.Length)
        throw new Exception("Не совпадает размерность векторов!");
      double max = 0;
      double distance;
      for (int i = 0; i < a.Length; i++)
      {
        distance = _metric.Calculate(a[i], b[i]);
        if (distance > max)
          max = distance;
      }

      return max;
    }
  }
}
