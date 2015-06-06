using System;
namespace DAModels
{
  interface IModelComputable<T>
  {
    ModelResult Compute(T item);
  }
}
