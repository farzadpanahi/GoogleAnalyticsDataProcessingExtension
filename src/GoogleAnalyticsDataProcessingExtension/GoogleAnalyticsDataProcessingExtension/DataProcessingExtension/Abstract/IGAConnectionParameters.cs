using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.Abstract
{
    public enum GAOAuth2Validations
    {
        Invalid = 0,
        CertificateInStore = 1,
        CertificateInFile = 2,
    }

    public interface IGAConnectionParameters
    {
        string CertificateFilePath { get; }
        string CertificatePassword { get; }
        string ServiceAccountEmail { get; }
        string CertificateSubjectDn { get; }
        string CertificateStoreName { get; }
        string CertificateStoreLocation { get; }

        string ToString();
        GAOAuth2Validations Validate();
    }


}
