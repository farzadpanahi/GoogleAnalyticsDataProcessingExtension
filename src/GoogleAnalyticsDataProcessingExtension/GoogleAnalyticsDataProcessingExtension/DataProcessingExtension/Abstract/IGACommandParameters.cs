using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract
{
    public interface IGACommandParameters
    {
        string Ids { get; }
        string StartDate { get; }
        string EndDate { get; }
        string Metrics { get; }
        string Dimensions { get; }
        string Sort { get; }
        string Filters { get; }
        string Segment { get; }
        long? StartIndex { get; }
        long? MaxResults { get; }
        string Fields { get; }
    }
}
