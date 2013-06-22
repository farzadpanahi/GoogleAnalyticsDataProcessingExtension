using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ReportingServices.DataProcessing;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension
{
    public class GADataParameter : IDataParameter
    {
        #region IDataParameter Members

        public string ParameterName { get; set; }
        public object Value { get; set; }

        #endregion
    }
}
