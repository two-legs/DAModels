using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels
{
  public abstract class ModelResult
  {
    public string Name { get; set; }
    /// <summary>
    /// Вывести результаты кластеризации
    /// </summary>
    /// <returns></returns>
    public virtual string Print()
    {
      return "Model result";
    }
  }
}
