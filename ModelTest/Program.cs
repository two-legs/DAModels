using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAModels;
using DAModels.Clustering;
using DAModels.Clustering.Algorithms;
using DAModels.Clustering.Algorithms.KMeans;
using DAModels.Clustering.Algorithms.EM;
using System.Data.SqlClient;
using System.Data;
using BankOperations;
using System.IO;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using DAModels.Clustering.Algorithms.CLOPE;

namespace ModelTest
{
  class Program
  {
    static void Main(string[] args)
    {
     // Test();
     // Console.ReadKey();
      OpsDataStorage ops = new OpsDataStorage();
      Console.WriteLine("Start loading... Start time: {0} ", System.DateTime.Now);
      ops.Load("w2012\\sql2014", "sa", "1Qwerty7");
      Console.WriteLine("Loading is complete. End time: {0} ", System.DateTime.Now);

      List<string> fields = new List<string>();
      fields.Add("BankAcc");
    //  fields.Add("Recipient");
      fields.Add("Currency");
      fields.Add("CFO");
      fields.Add("FuncBlock");
      fields.Add("ActivityLine");
      fields.Add("BudgetItem");
    //  fields.Add("Summa");

      Console.WriteLine("Data preprocessing... Start time: {0} ", System.DateTime.Now);
      double[][] data = ops.GetConvertedData(fields);
      Console.WriteLine("Preprocessing is complete. End time: {0} ", System.DateTime.Now);

      //KMeansTest(ops, fields, data);

      EMTest(ops, fields, data);
      Console.ReadKey();

      Console.WriteLine("Starting clustering... Start time: {0} ", System.DateTime.Now);
      ClopeParams clope_params = new ClopeParams();
      clope_params.Data = ops.ConvertToTransactionArray(fields);
      clope_params.Repulsion = 1;

      ClopeClustering clope = new ClopeClustering(clope_params);

      ClusteringModel<List<string>> clopedm = new ClusteringModel<List<string>>(clope_params.Data, clope);
      clopedm.Run();

      Console.WriteLine("Clustering is complete. End time: {0} ", System.DateTime.Now);
      Console.WriteLine(clopedm.Result.Print());

      // SaveToFile((EMResult)emdm.Result, ops, fields);
      SaveToFile((ClopeResult)clopedm.Result, ops, fields);
      Console.ReadKey();
    }

    private static void EMTest(OpsDataStorage ops, List<string> fields, double[][] data)
    {
      Console.WriteLine("Starting clustering... Start time: {0} ", System.DateTime.Now);
      EMParams em_params = new EMParams();
      em_params.Data = data;
      em_params.ClusterCount = 9;
      em_params.Iterations = 300;

      EMClustering em = new EMClustering(em_params);

      ClusteringModel<double[]> emdm = new ClusteringModel<double[]>(data, em);

      emdm.Run();
      Console.WriteLine("Clustering is complete. End time: {0} ", System.DateTime.Now);
      Console.WriteLine(emdm.Result.Print());
      SaveToFile((EMResult)emdm.Result, ops, fields);
      Console.ReadKey();
    }

    private static void KMeansTest(OpsDataStorage ops, List<string> fields, double[][] data)
    {
      Console.WriteLine("Starting clustering... Start time: {0} ", System.DateTime.Now);
      KMeansParams param = new KMeansParams();
      param.ClusterCount = 9;
      param.Data = data;
      param.Metric = new DAModels.Metrics.EuclideanMetric();

      KMeansClustering kmeans = new KMeansClustering(param);

      ClusteringModel<double[]> dm = new ClusteringModel<double[]>(data, kmeans);
      dm.Run();
      Console.WriteLine("Clustering is complete. End time: {0} ", System.DateTime.Now);

      Console.WriteLine(dm.Result.Print());
      SaveToFile((KMeansResult)dm.Result, ops, fields);
      Console.ReadKey();
    }

    private static void Test()
    {
      double[][] points = new double[][] { new double[] {1, 1},   new double[] {2, 1},   new double[] {1, 2},
                                           new double[] {10, 10}, new double[] {12, 10}, new double[] {10, 12},
                                           new double[] {21, 21}, new double[] {22, 21}, new double[] {21, 22}};

      KMeansParams param = new KMeansParams();
      param.ClusterCount = 3;
      param.Data = points;
      param.Metric = new DAModels.Metrics.EuclideanMetric();

      KMeansClustering kmeans = new KMeansClustering(param);

      ClusteringModel<double[]> dm = new ClusteringModel<double[]>(points, kmeans);
      dm.Run();

      Console.WriteLine(dm.Result.Print());
      Console.ReadKey();

      EMParams em_params = new EMParams();
      em_params.Data = points;
      em_params.ClusterCount = 3;
      em_params.Iterations = 10;

      EMClustering em = new EMClustering(em_params);

      ClusteringModel<double[]> emdm = new ClusteringModel<double[]>(points, em);

      emdm.Run();

      Console.WriteLine(emdm.Result.Print());
    }

    private static void SaveToFile(KMeansResult result, OpsDataStorage ops, IList<string> fields) 
    { 
      StringBuilder str = new StringBuilder("K-Means");
      str.AppendLine(result.Print());
      str.AppendLine(PrintDictionaries(ops, fields));

      int num = 1;
      foreach (double[] cluster in result.Centriods)
      {
        str.AppendLine("Cluster " + num);
        int index = 0;
        foreach (string field in fields)
        {
          var dict = ops.GetDictionaryByFieldName(field);
          if (dict != null)
          {
            var value = dict.Where(item => item.Value == (int)Math.Round(cluster[index])).Select(item => item.Key).First();
            str.AppendLine(value);
          }
          else
          {
            str.AppendFormat("{0:0.00}", cluster[index]);
          }
          
          index++;
        }
        
        num++;
      }

      str.AppendLine("Detalisation:");
      for (int i = 0; i < result.Centriods.Length; i++)
      {
        str.AppendLine("Cluster" + i);
        
        //double[] element = 
        int index = 0;
        foreach (string field in fields)
        {
          str.AppendLine(" ---- " + field);
          var dict = ops.GetDictionaryByFieldName(field);
          if (dict != null)
          {
            //var value = dict.Where(item => item.Value == (int)Math.Round(cluster[index])).Select(item => item.Key).First();
            foreach (KeyValuePair<string, int> pair in dict)
            {
              var count = result.Clusterisation.Count(item => item.Value == i && Math.Round(((double[])item.Key)[index]) == pair.Value);
              str.AppendFormat("{0}:\t{1}\n", pair.Key, count);
            }
          }
          else
          {
            //str.AppendFormat("{0:0.00}", cluster[index]);
          }
          
          index++;
        }
      }

      string path = "D:\\Report.txt";
      using (StreamWriter w = File.CreateText(path))
      {
        w.Write(str);
      }
    }

    private static void SaveToFile(EMResult result, OpsDataStorage ops, IList<string> fields)
    {
      StringBuilder str = new StringBuilder("K-Means");
      str.AppendLine(result.Print());
      str.AppendLine(PrintDictionaries(ops, fields));

      str.AppendLine("Detalisation:");
      for (int i = 0; i < result.ClusterCount; i++)
      {
        str.AppendLine("Cluster" + i);

        int index = 0;
        foreach (string field in fields)
        {
          str.AppendLine(" ---- " + field);
          var dict = ops.GetDictionaryByFieldName(field);
          if (dict != null)
          {
            double m = result.Medians[i][index];
            double s = Math.Sqrt(result.Covariations[i][index]);
            foreach (KeyValuePair<string, int> pair in dict)
            {
              double x = (double)pair.Value;


              double prob = Laplace.CDF(m, s, x + 0.5) - Laplace.CDF(m, s, x - 0.5);
              str.AppendFormat("{0}:\t{1}\n", pair.Key, prob);
            }
          }
          else
          {
            //str.AppendFormat("{0:0.00}", cluster[index]);
          }

          index++;
        }
      }

      string path = "D:\\ReportEM.txt";
      using (StreamWriter w = File.CreateText(path))
      {
        w.Write(str);
      }
    }

    private static void SaveToFile(ClopeResult result, OpsDataStorage ops, IList<string> fields)
    {
      
      string path = "D:\\ReportClope.txt";
      using (StreamWriter w = File.CreateText(path))
      {
        w.Write(result.Print());
      }
    }

    private static string PrintDictionaries(OpsDataStorage ops, IList<string> fields)
    {
      StringBuilder str = new StringBuilder("Значения ключей:");
      foreach (string field in fields)
      {
        var dict = ops.GetDictionaryByFieldName("field");
        if (dict != null)
        {
          str.AppendFormat("\n{0}\n", field);
          foreach (KeyValuePair<string, int> pair in dict)
          {
            str.AppendFormat("{0}:\t{1}\n", pair.Key, pair.Value);
          }
        }
      }

      return str.ToString();
    }
  }
}
