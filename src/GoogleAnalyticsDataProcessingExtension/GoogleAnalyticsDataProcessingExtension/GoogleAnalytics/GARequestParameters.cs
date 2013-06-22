using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract;
using Newtonsoft.Json.Linq;

namespace GoogleAnalyticsDataProcessingExtension.GoogleAnalytics
{
    public class GARequestParameters : IGACommandParameters
    {

        public string Ids { get; private set; }
        public string StartDate { get; private set; }
        public string EndDate { get; private set; }
        public string Metrics { get; private set; }
        public string Dimensions { get; private set; }
        public string Sort { get; private set; }
        public string Filters { get; private set; }
        public string Segment { get; private set; }
        public long? StartIndex { get; private set; }
        public long? MaxResults { get; private set; }
        public string Fields { get; private set; }

        private readonly JObject _requestJson;

        public GARequestParameters(string requestString)
            : this(JObject.Parse(requestString))
        { }

        public GARequestParameters(JObject requestJson)
        {
            _requestJson = requestJson;

            Ids = (string)requestJson["Ids"];
            StartDate = (string)requestJson["StartDate"];
            EndDate = (string)requestJson["EndDate"];
            Metrics = (string)requestJson["Metrics"];
            Dimensions = (string)requestJson["Dimensions"];
            Sort = (string)requestJson["Sort"];
            Filters = (string)requestJson["Filters"];
            Segment = (string)requestJson["Segment"];
            StartIndex = (long?)requestJson["StartIndex"];
            MaxResults = (long?)requestJson["MaxResults"];
            Fields = (string)requestJson["Fields"];
        }

        public override string ToString()
        {
            return _requestJson.ToString();
        }
    }
}
