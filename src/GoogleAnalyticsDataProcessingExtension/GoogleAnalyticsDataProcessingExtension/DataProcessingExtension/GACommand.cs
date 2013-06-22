using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract;
using GoogleAnalyticsDataProcessingExtension.GoogleAnalytics;
using Microsoft.ReportingServices.DataProcessing;
using Newtonsoft.Json.Linq;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension
{
    public class GACommand : IDbCommand
    {
        private readonly IGAService _gaService;
        private GADataParameterCollection _parameters;
        private IGACommandParameters _commandParameters;

        public GACommand(IGAService gaService)
        {
            _gaService = gaService;
            _parameters = new GADataParameterCollection();
        }

        #region IDbCommand Members

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public string CommandText 
        { 
            get { return _commandParameters.ToString(); } 
            set { _commandParameters = new GARequestParameters(value); } 
        }
        public int CommandTimeout { get; set; }

        public CommandType CommandType
        {
            get { return Microsoft.ReportingServices.DataProcessing.CommandType.Text; }
            set { if (value != CommandType.Text) throw (new NotSupportedException("Only command type Text is supported.")); }
        }

        public IDataParameter CreateParameter()
        {
            return new GADataParameter();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            if (behavior == CommandBehavior.SchemaOnly)
                return new GADataReader(_gaService.FetchHeadersOnly(_commandParameters));

            return new GADataReader(_gaService.FetchData(_commandParameters));
        }

        public IDataParameterCollection Parameters
        {
            get { return _parameters; }
        }

        public IDbTransaction Transaction
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _commandParameters = null;
        }

        #endregion
    }
}
