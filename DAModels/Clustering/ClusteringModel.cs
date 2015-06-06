using DAModels.Clustering;
using DAModels.Clustering.Algorithms;
using DAModels.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels
{
  public class ClusteringModel<T> : IntelligentModel<T>
  {
    public IMetric<T> Metric { get; set; }
    public IClusteringAlgorithm<T> Clusterizer { get; set; }
    public ClusteringResult Result { get; private set; }
   
    public ClusteringModel(T[] data, IClusteringAlgorithm<T> algorithm)
    {
      Data = data;
      Clusterizer = algorithm;
    }
    
    public override void Run()
    {
      if (Clusterizer == null)
        throw new NullReferenceException("Не задан алгоритм кластеризации");
      Result = Clusterizer.MakeClustering(Data);
    }

    public override void Initialize()
    {
      throw new NotImplementedException();
    }

    public override ModelTestResult Test()
    {
      throw new NotImplementedException();
    }

    public override void Save()
    {
      throw new NotImplementedException();
    }

    public override void Load()
    {
      throw new NotImplementedException();
    }
  }
}
