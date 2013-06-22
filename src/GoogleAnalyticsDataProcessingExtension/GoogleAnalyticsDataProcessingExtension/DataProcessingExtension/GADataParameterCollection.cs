using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ReportingServices.DataProcessing;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension
{
    public class GADataParameterCollection : ArrayList, IDataParameterCollection
    {
        #region IDataParameterCollection Members

        public int Add(IDataParameter parameter)
        {
            return base.Add(parameter);
        }

        #endregion
    }
}
