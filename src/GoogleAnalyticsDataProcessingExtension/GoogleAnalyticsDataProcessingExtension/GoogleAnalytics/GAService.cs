using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Services;
using Google.Apis.Util;
using GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract;


namespace GoogleAnalyticsDataProcessingExtension.GoogleAnalytics
{
    public class GAService : IGAService
    {
        // -- private variables
        private readonly AnalyticsService _gas;

        // -- constructors
        public GAService(string certificateFilePath, string certificatePassword, string serviceAccountEmail)
        {
            _gas = GetAnalyticsService(certificateFilePath, certificatePassword, serviceAccountEmail);
        }

        public GAService(string certificateSubjectDn, string certificateStoreName, string certificateStoreLocation, string serviceAccountEmail)
        {
            _gas = GetAnalyticsService(certificateSubjectDn, certificateStoreName, certificateStoreLocation, serviceAccountEmail);
        }

        public GAService(IGAConnectionParameters gaOAuth2Parameters) 
        {
            var validation = gaOAuth2Parameters.Validate();
            if (validation == GAOAuth2Validations.CertificateInStore)
                _gas = GetAnalyticsService(gaOAuth2Parameters.CertificateSubjectDn, gaOAuth2Parameters.CertificateStoreName, gaOAuth2Parameters.CertificateStoreLocation, gaOAuth2Parameters.ServiceAccountEmail);
            else if (validation == GAOAuth2Validations.CertificateInFile)
                _gas = GetAnalyticsService(gaOAuth2Parameters.CertificateFilePath, gaOAuth2Parameters.CertificatePassword, gaOAuth2Parameters.ServiceAccountEmail);
            else
                throw new Exception("Invalid configuration in connection string. Either path to certificate file or certificate store should be configured.");
        }

        // -- public methods
        public GaData FetchData(IGACommandParameters gaRequestParameters)
        {
            return FetchData(false, gaRequestParameters);
        }

        public GaData FetchHeadersOnly(IGACommandParameters gaRequestParameters)
        {
            return FetchData(true, gaRequestParameters);
        }

        public GaData FetchData(bool headersOnly, string ids, string startDate, string endDate, string metrics,
            string dimensions = null, string sort = null, string filters = null, string segment = null, 
            long? startIndex = null, long? maxResults = null, string fields = null)
        {
            var getRequest = _gas.Data.Ga.Get(ids, startDate, endDate, metrics);

            getRequest.Dimensions = dimensions;
            getRequest.Fields = fields;
            getRequest.Filters = filters;
            getRequest.MaxResults = headersOnly ? 0 : maxResults;
            getRequest.Segment = segment;
            getRequest.Sort = sort;
            getRequest.StartIndex = startIndex;

            return getRequest.Fetch();
        }

        // -- private methods

        private IAuthenticator Authenticate(string certificateFilePath, string certificatePassword, string serviceAccountEmail, AnalyticsService.Scopes scope)
        {
            var certificate = new X509Certificate2(certificateFilePath, certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            return Authenticate(certificate, serviceAccountEmail, scope);
        }

        private IAuthenticator Authenticate(string certificateSubjectDn, string certificateStoreName, string certificateStoreLocation, string serviceAccountEmail, AnalyticsService.Scopes scope)
        {
            var storeLocation = certificateStoreLocation == GAOAuth2Parameters.CertificateStoreLocationLocalMachine ? StoreLocation.LocalMachine : certificateStoreLocation == GAOAuth2Parameters.CertificateStoreLocationCurrentUser ? StoreLocation.CurrentUser : (StoreLocation?)null;
            
            if (!storeLocation.HasValue)
                throw new Exception(string.Format("Invalid Store Location = '{0}'. Valid values: '{1}', '{2}'.", certificateStoreLocation, GAOAuth2Parameters.CertificateStoreLocationLocalMachine, GAOAuth2Parameters.CertificateStoreLocationCurrentUser));
            
            var store = new X509Store(certificateStoreName, storeLocation.Value);
           
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            var results = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificateSubjectDn, false);
            if (results.Count > 1)
                throw new Exception(string.Format("Multiple certificates found with Subject DN = '{0}', Store Name = '{1}', Store Location = '{2}'.", certificateSubjectDn, certificateStoreName, certificateStoreLocation));
            else if (results.Count == 0)
                throw new Exception(string.Format("No certificate found with Subject DN = '{0}', Store Name = '{1}', Store Location = '{2}'.", certificateSubjectDn, certificateStoreName, certificateStoreLocation));

            var certificate = results[0];

            return Authenticate(certificate, serviceAccountEmail, scope);
        }

        private IAuthenticator Authenticate(X509Certificate2 certificate, string serviceAccountEmail, AnalyticsService.Scopes scope)
        {
            var provider = new AssertionFlowClient(GoogleAuthenticationServer.Description, certificate)
            {
                ServiceAccountId = serviceAccountEmail,
                Scope = scope.GetStringValue(),
            };

            return new OAuth2Authenticator<AssertionFlowClient>(provider, AssertionFlowClient.GetState);
        }

        private AnalyticsService GetAnalyticsService(string certificateFilePath, string certificatePassword, string serviceAccountEmail)
        {
            var auth = Authenticate(
                certificateFilePath: certificateFilePath,
                certificatePassword: certificatePassword,
                serviceAccountEmail: serviceAccountEmail,
                scope: AnalyticsService.Scopes.AnalyticsReadonly
            );

            return GetAnalyticsService(auth);
        }

        private AnalyticsService GetAnalyticsService(string certificateSubjectDn, string certificateStoreName, string certificateStoreLocation, string serviceAccountEmail)
        {
            var auth = Authenticate(
                certificateSubjectDn: certificateSubjectDn,
                certificateStoreName: certificateStoreName,
                certificateStoreLocation: certificateStoreLocation,
                serviceAccountEmail: serviceAccountEmail,
                scope: AnalyticsService.Scopes.AnalyticsReadonly
            );

            return GetAnalyticsService(auth);
        }
        private AnalyticsService GetAnalyticsService(IAuthenticator authenticator)
        {
            return new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator, GZipEnabled = true });
        }

        private GaData FetchData(bool headersOnly, IGACommandParameters gaRequestParameters)
        {
            return FetchData(
                headersOnly: headersOnly,
                ids: gaRequestParameters.Ids,
                startDate: gaRequestParameters.StartDate,
                endDate: gaRequestParameters.EndDate,
                metrics: gaRequestParameters.Metrics,
                dimensions: gaRequestParameters.Dimensions,
                sort: gaRequestParameters.Sort,
                filters: gaRequestParameters.Filters,
                segment: gaRequestParameters.Segment,
                startIndex: gaRequestParameters.StartIndex,
                maxResults: gaRequestParameters.MaxResults,
                fields: gaRequestParameters.Fields
            );
        }
    }
}
