using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Analytics.v3.Data;
using Microsoft.ReportingServices.DataProcessing;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension
{
    public class GADataReader : IDataReader
    {
        private readonly GaData _data;
        private IEnumerator<IList<string>> _rowsEnumerator;

        public GADataReader(GaData data)
        {
            _data = data;
            _rowsEnumerator = data.Rows.GetEnumerator();
            System.Diagnostics.Debug.WriteLine("data row count = " + data.Rows.Count);
        }

        #region IDataReader Members

        public int FieldCount
        {
            get { return _data.ColumnHeaders.Count; }
        }

        public Type GetFieldType(int fieldIndex)
        {
            return _rowsEnumerator.Current[fieldIndex].GetType();
        }

        public string GetName(int fieldIndex)
        {
            return _data.ColumnHeaders[fieldIndex].Name;
        }

        public int GetOrdinal(string fieldName)
        {
            return _data.ColumnHeaders.Select(x => x.Name).ToList().IndexOf(fieldName);
        }

        public object GetValue(int fieldIndex)
        {
            return _rowsEnumerator.Current[fieldIndex];
        }

        public bool Read()
        {
            return _rowsEnumerator.MoveNext();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
           
        }

        #endregion
    }
}
