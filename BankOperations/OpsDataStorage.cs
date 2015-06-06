using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOperations
{
    public class OpsDataStorage
    {
      private SqlConnection _connection;
      private List<string> _dictionared = new List<string>();
      public DataTable Data { get; private set; }
      
      public Dictionary<string, int> Organizations { get; private set; }
      public Dictionary<string, int> BankAccounts { get; private set; }
      public Dictionary<string, int> Currencies { get; private set; }
      public Dictionary<string, int> CFOs { get; private set; }
      public Dictionary<string, int> FuncBlocks { get; private set; }
      public Dictionary<string, int> ActivityLines { get; private set; }
      public Dictionary<string, int> BudgetItems { get; private set; }
      public Dictionary<string, int> Recipients { get; private set; }

      public OpsDataStorage()
      {
        Data = new DataTable("BankOperations");
      }

      public void Load(string database, string user, string password)
      {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        builder.DataSource = database;
        builder.InitialCatalog = "BankOperations";
        builder.UserID = user;
        builder.Password = password;

        using (SqlConnection conn = new SqlConnection())
        {
          conn.ConnectionString = builder.ConnectionString;
          conn.Open();

          SqlCommand command = new SqlCommand();
          command.Connection = conn;
          command.CommandText = "select * from dbo.VOperationsFull where OperationDate >= convert(datetime, '01.01.2014', 104)";

          using (SqlDataReader dr = command.ExecuteReader())
          {
            Data.Load(dr);
          }
          FillDictionaries();
        }
      }

      private void FillDictionaries()
      {
        Organizations = FillDictionary("Organization");
        BankAccounts = FillDictionary("BankAcc");
        Currencies = FillDictionary("Currency");
        CFOs = FillDictionary("CFO");
        FuncBlocks = FillDictionary("FuncBlock");
        ActivityLines = FillDictionary("ActivityLine");
        BudgetItems = FillDictionary("BudgetItem");
        Recipients = FillDictionary("Recipient");
      }

      private Dictionary<string, int> FillDictionary(string field)
      {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        IEnumerable<string> namesList = (from row in Data.AsEnumerable() select row.Field<string>(field)).Distinct();
        int i = 1;
        foreach (string name in namesList)
        {
          if (name != null)
          {
            dict.Add(name, i);
            i++;
          }
        }
        _dictionared.Add(field);
        return dict;
      }

      public double[][] GetConvertedData(IList<string> fields)
      {
        int n = Data.Rows.Count;
        int m = fields.Count;

        double[][] matrix = new double[n][];
        DataRow row;
        for (int i = 0; i < n; i++)
        {
          row = Data.Rows[i];
          double[] array = new double[m];
          int index = 0;
          foreach (string field in fields)
          {
            if (_dictionared.Contains(field))
            {
              try
              {
                double value = (double)GetDictionaryByFieldName(field)[row.Field<string>(field) ?? ""];
                array[index] = value;
              }
              catch
              {
                array[index] = 0;
              }
            }
            else
            {
              array[index] = (double)(row.Field<decimal?>(field) ?? 0) / 10000.0;
            }
            matrix[i] = array;
            index++;
          }
        }

        return matrix;
      }

      public List<string>[] ConvertToTransactionArray(IList<string> fields)
      {
        List<string>[] transactions = new List<string>[Data.Rows.Count];
        for (int i = 0; i < Data.Rows.Count; i++)
        {
          transactions[i] = new List<string>();
          var row = Data.Rows[i];
          foreach (string field in fields)
          {
            transactions[i].Add(row.Field<string>(field) ?? "");
          }
        }

        return transactions;
      }

      public Dictionary<string, int> GetDictionaryByFieldName(string field_name)
      {
        switch (field_name)
        {
          case "Organization":
            return Organizations;
          case "BankAcc":
            return BankAccounts;
          case "Currency":
            return Currencies;
          case "CFO":
            return CFOs;
          case "FuncBlock":
            return FuncBlocks;
          case "ActivityLine":
            return ActivityLines;
          case "BudgetItem":
            return BudgetItems;
          case "Recipient":
            return Recipients;
          default:
            return null;
        }
      }
    }
  }
