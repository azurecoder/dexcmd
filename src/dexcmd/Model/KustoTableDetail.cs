using System;
using System.Data;

namespace dexcmd.Model
{
   public class KustoTableDetail
   {
      private readonly DataRow _reader;

      public KustoTableDetail(DataRow row)
      {
         _reader = row;
      }

      public string TableName => (string) _reader.ItemArray[0];
      public string DatabaseName => (string) _reader.ItemArray[1];
      public string Folder => (string)_reader.ItemArray[2];
      public string DocString => (string)_reader.ItemArray[3];
      public long TotalExtents => (long)_reader.ItemArray[4];
      public double TotalExtentSize => (double)_reader.ItemArray[5];
      public double TotalOriginalSize => (double)_reader.ItemArray[6];
      public long TotalRowCount => (long)_reader.ItemArray[7];
      public long HotExtents => (long)_reader.ItemArray[8];
      public double HotExtentSize => (double)_reader.ItemArray[9];
      public double HotOriginalSize => (double)_reader.ItemArray[10];
      public long HotRowCount => (long)_reader.ItemArray[11];
      public string AuthorizedPrincipals => (string)_reader.ItemArray[12];
      public string RetentionPolicy => (string)_reader.ItemArray[13];
      public string CachingPolicy => (string)_reader.ItemArray[14];
      public string ShardingPolicy => (string)_reader.ItemArray[15];
      public string MergePolicy => (string)_reader.ItemArray[16];
      public string StreamingIngestionPolicy => (string)_reader.ItemArray[17];
      public DateTime MinExtentsCreationTime => DateTime.Parse((string) _reader.ItemArray[18]);
      public DateTime MaxExtentsCreationTime => DateTime.Parse((string)_reader.ItemArray[19]);
      public string RowOrderPolicy => (string)_reader.ItemArray[20];
   }
}
