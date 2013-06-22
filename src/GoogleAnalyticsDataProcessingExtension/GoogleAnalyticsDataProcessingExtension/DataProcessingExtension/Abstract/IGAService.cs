using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Analytics.v3.Data;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract
{
    public interface IGAService
    {
        GaData FetchData(IGACommandParameters gaCommandParameters);
        GaData FetchHeadersOnly(IGACommandParameters gaCommandParameters);
        GaData FetchData(bool headersOnly, string ids, string startDate, string endDate, string metrics,
            string dimensions = null, string sort = null, string filters = null, string segment = null,
            long? startIndex = null, long? maxResults = null, string fields = null);
    }
}
