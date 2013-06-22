using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract;
using Newtonsoft.Json.Linq;

namespace GoogleAnalyticsDataProcessingExtension.GoogleAnalytics
{
    public class GAOAuth2Parameters : IGAConnectionParameters
    {
        // -- public constants
        public const string CertificateStoreLocationLocalMachine = "LocalMachine";
        public const string CertificateStoreLocationCurrentUser = "CurrentUser";
        public const string CertiricateStoreNameMy = "My";

        // -- public properties
        public string CertificateFilePath { get; private set; }
        public string CertificatePassword { get; private set; }
        public string ServiceAccountEmail { get; private set; }
        public string CertificateSubjectDn { get; private set; }
        public string CertificateStoreName { get; private set; }
        public string CertificateStoreLocation { get; private set; }

        private readonly JObject _oauth2ParametersJson;

        public GAOAuth2Parameters(string oauth2ParametersString)
            : this(JObject.Parse(oauth2ParametersString))
        { }

        public GAOAuth2Parameters(JObject oauth2ParametersJson)

        {
            _oauth2ParametersJson = oauth2ParametersJson;
            CertificateFilePath = (string)oauth2ParametersJson["CertificateFilePath"];
            CertificatePassword = (string)oauth2ParametersJson["CertificatePassword"];
            ServiceAccountEmail = (string)oauth2ParametersJson["ServiceAccountEmail"];
            CertificateSubjectDn = (string)oauth2ParametersJson["CertificateSubjectDn"];
            CertificateStoreName = (string)oauth2ParametersJson["CertificateStoreName"];
            CertificateStoreLocation = (string)oauth2ParametersJson["CertificateStoreLocation"];

            // set default store name and location if empty
            if (!string.IsNullOrEmpty(CertificateSubjectDn))
            {
                if (string.IsNullOrEmpty(CertificateStoreName)) CertificateStoreName = CertiricateStoreNameMy;
                if (string.IsNullOrEmpty(CertificateStoreLocation)) CertificateStoreLocation = CertificateStoreLocationLocalMachine;
            }
        }

        public override string ToString()
        {
            return _oauth2ParametersJson.ToString();
        }

        public GAOAuth2Validations Validate()
        {
            if (!string.IsNullOrEmpty(CertificateSubjectDn) &&
                !string.IsNullOrEmpty(ServiceAccountEmail))
                return GAOAuth2Validations.CertificateInStore;

            else if (!string.IsNullOrEmpty(CertificateFilePath) &&
                !string.IsNullOrEmpty(CertificatePassword) &&
                !string.IsNullOrEmpty(ServiceAccountEmail))
                return GAOAuth2Validations.CertificateInFile;

            else
                return GAOAuth2Validations.Invalid;
        }
    }
}
