using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels.Clustering.Algorithms.CLOPE
{
  public class ClopeCluster
  {
    /// <summary>
    /// Гистограмма (признак - частота)
    /// </summary>
    public Dictionary<string, int> Historgram { get; private set; }
    /// <summary>
    /// Площадь гистограммы
    /// </summary>
    public int Square { get; private set; }
    /// <summary>
    /// Средняя высота гистограммы
    /// </summary>
    public double Height { get; private set; }
    public int Size { get; private set; }
    public int Width { get; private set; }

    public ClopeCluster()
    {
      Historgram = new Dictionary<string, int>();
    }

    public void AddTransaction(List<string> transaction)
    {
      Square += transaction.Count; 
      foreach (string key in transaction)
      {
        if (!Historgram.ContainsKey(key))
          Historgram.Add(key, 0);
        Historgram[key]++;
      }
      Size++;
      Width = Historgram.Count;
    }

    public void RemoveTransaction(List<string> transaction)
    {
      Square -= transaction.Count;
      foreach (string key in transaction)
      {
        Historgram[key]--;
        if (Historgram[key] == 0)
          Historgram.Remove(key);
      }
      Size--;
      Width = Historgram.Count;
    }
  }
}
