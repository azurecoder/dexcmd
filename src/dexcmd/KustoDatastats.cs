using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace KustoIngest
{
   public class KustoDatastats
   {
      private readonly IDataReader _reader;

      public KustoDatastats(IDataReader reader)
      {
         _reader = reader;
         _reader.Read();
      }

      public string DatabaseName => _reader.GetString(0);
      public string PersistentStorage => _reader.GetString(1);
      public string Version => _reader.GetString(2);
      public bool IsCurrent => _reader.GetBoolean(3);
      public string DatabaseAccessMode => _reader.GetString(4);
      public string PrettyName => _reader.GetString(5);
      public Guid DatabaseId => _reader.GetGuid(6);
      public double OriginalSize => _reader.GetDouble(7);
      public double ExtentSize => _reader.GetDouble(8);
      public double CompressedSize => _reader.GetDouble(9);
      public double IndexSize => _reader.GetDouble(10);
      public double RowCount => _reader.GetDouble(11);
      public double HotOriginalSize => _reader.GetDouble(12);
      public double HotExtentsSize => _reader.GetDouble(13);
      public double HotCompressedSize => _reader.GetDouble(14) / 1000000000;
      public double HotIndexSize => _reader.GetDouble(15);
      public long HotRowCount => _reader.GetInt64(16);
   }
}
