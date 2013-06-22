using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract;
using GoogleAnalyticsDataProcessingExtension.GoogleAnalytics;
using Microsoft.ReportingServices.DataProcessing;


namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension
{
    public class GAConnection : IDbConnection, IDbConnectionExtension
    {
        private const string _localizedName = "Google Analytics";
        private IGAConnectionParameters _connectionParameters;
        private IGAService _gaService;

        #region IDbConnection Members

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            _gaService = null;
        }

        public string ConnectionString
        {
            get { return _connectionParameters.ToString(); }
            set { _connectionParameters = new GAOAuth2Parameters(value); }
        }

        public int ConnectionTimeout
        {
            get { return 0; }
        }

        public IDbCommand CreateCommand()
        {
            return new GACommand(_gaService);
        }

        public void Open()
        {
            _gaService = new GAService(_connectionParameters);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _gaService = null;
            _connectionParameters = null;
        }

        #endregion

        #region IExtension Members

        public string LocalizedName
        {
            get { return _localizedName; }
        }

        public void SetConfiguration(string configuration)
        {
            // empty 
        }

        #endregion

        #region IDbConnectionExtension Members
        public string Impersonate { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        #endregion
    }
}
