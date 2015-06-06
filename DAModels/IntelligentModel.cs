using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAModels
{
    public abstract class IntelligentModel<T> 
    {
      public T[] Data { get; set; }
      
      public abstract void Run();
      public abstract void Initialize();
      public abstract ModelTestResult Test();

      public abstract void Save();
      public abstract void Load();
    }
}
