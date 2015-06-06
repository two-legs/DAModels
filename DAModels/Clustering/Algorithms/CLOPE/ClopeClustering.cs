using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms.CLOPE
{
  public class ClopeClustering : IClusteringAlgorithm<List<string>>
  {
    private List<string>[] _data;
    private int _cluster_count;
    private double _repulsion;
    /// <summary>
    /// Массив из словаря признак - частота
    /// </summary>
    private Dictionary<int, ClopeCluster> _clusters = new Dictionary<int, ClopeCluster>();
    private Dictionary<object, int> _clusterization = new Dictionary<object, int>();

    public ClopeClustering(ClopeParams param)
    {
      _data = param.Data;
      _cluster_count = param.ClusterCount;
      _repulsion = param.Repulsion;
    }

    public double Profit(ClopeCluster[] clusters, double repulsion)
    {
      double sum1 = 0;
      double sum2 = 0;
      foreach (ClopeCluster cf in clusters)
      {
        sum1 += cf.Square * cf.Width / Math.Pow(cf.Height, repulsion);
        sum2 += cf.Width;
      }
      return sum1 / sum2;
    }

    private double DeltaAdd(ClopeCluster cluster, List<string> transaction, double repulsion)
    {
      if (cluster == null)
        return transaction.Count / Math.Pow(transaction.Count, repulsion);

      int squareNew = cluster.Square + transaction.Count;
      int widthNew = cluster.Width;

      foreach (string key in transaction)
        if (!cluster.Historgram.ContainsKey(key))
          widthNew++;
      
      return squareNew * (cluster.Size + 1) / Math.Pow(widthNew, repulsion) - cluster.Square * cluster.Size / Math.Pow(cluster.Width, repulsion);
    }

    private double DeltaRemove(ClopeCluster cluster, List<string> transaction, double repulsion)
    {
      int squareNew = cluster.Square - transaction.Count;
      int widthNew = cluster.Width;

      foreach (string key in transaction)
        if (cluster.Historgram[key] == 1)
          widthNew--;
      
      return squareNew * (cluster.Size - 1) / Math.Pow(widthNew, repulsion) - cluster.Square * cluster.Size / Math.Pow(cluster.Width, repulsion);
    }

    public ClusteringResult MakeClustering(List<string>[] data)
    {
      // Подготовить данные для кластеризации: инициировать словарь с делением объектоа по кластерам
      foreach (object item in _data)
        _clusterization.Add(item, 0);
      
      bool moved = true;
      while (moved)
      {
        moved = false;
        
        foreach(List<string> transaction in data)
        {
          double removeDelta = _clusterization[transaction] == 0 ? 0 : DeltaRemove(_clusters[_clusterization[transaction]], transaction, _repulsion);
          double maxDelta = 0;
          double delta = 0;
          int newClusterNumber = _clusterization[transaction];

          foreach (KeyValuePair<int, ClopeCluster> pair in _clusters)
          {
            delta = DeltaAdd(pair.Value, transaction, _repulsion);
            if (delta + removeDelta > maxDelta)
            {
              maxDelta = delta;
              newClusterNumber = pair.Key;
            }
          }

          delta = DeltaAdd(null, transaction, _repulsion);
          if (delta + removeDelta > maxDelta)
          {
            if (_clusters.Count == 0)
              newClusterNumber = 1;
            else
              newClusterNumber = _clusters.Keys.Max() + 1;

            _clusters.Add(newClusterNumber, new ClopeCluster());
          }

          if (newClusterNumber != _clusterization[transaction])
          {
            if (_clusterization[transaction] != 0)
              _clusters[_clusterization[transaction]].RemoveTransaction(transaction);
            
            _clusters[newClusterNumber].AddTransaction(transaction);
            
            moved = true;
            _clusterization[transaction] = newClusterNumber;
            //data.WriteTransaction(transaction);
          }
        }
      }

      List<int> keysForDeleting = new List<int>();
      foreach (int key in _clusters.Keys)
        if (_clusters[key].Size == 0)
          keysForDeleting.Add(key);
      foreach (int key in keysForDeleting)
        _clusters.Remove(key);

      ClopeResult result = new ClopeResult();
      result.Clusters = _clusters.Values.ToArray();
      result.Clusterization = _clusterization;

      return result;
    }
  }
}
