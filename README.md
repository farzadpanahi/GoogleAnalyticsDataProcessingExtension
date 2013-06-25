## Google Analytics Data Processing Extension

You can use this custom [data processing extension](http://msdn.microsoft.com/en-us/library/ms152816.aspx) to connect to [Google Analytics API](https://developers.google.com/analytics/devguides/) and fetch data into a Data Set in Microsoft SSRS to create your report based on data from Google Analytics.

After you copy the dlls and apply the neccessary changes to config files, you will see Google Analytics as a new Data Source type added to the list of available types drop-down list. You can then create a Google Analytics Data Source under your report project.  

### Google Analytics Authorization
I am using OAuth 2 to authorize requests to Google Core Reporting API (v3). You need to follow the instructions [here](https://developers.google.com/analytics/devguides/reporting/core/v3/gdataAuthorization) to create a Service Account access to Google Reporting APIs. You need to download the generated certificate which also includes a private key.

### Installation

Unfortunatelly I didn't get a chance to write a installer for this project. *I encourage anyone interested to write one* :> to help reduce the installation headache.

Basically to deploy a data processing extension we need to copy the required dlls to specific folders and modify couple of configuration files. You can read more about the deployment [here](http://msdn.microsoft.com/en-us/library/ms155104.aspx).

The path to these files are different depending on whether you want to use this extension just in Visual Studio for development purposes or you want to use it in your SSRS server. I have the following placeholders to make the instructions useable for both Visual Studio and SSRS deployment.

Visual Studio Paths:
* ``<BIN DIR>`` = ``<VISUAL STUDIO DIR>\Common7\IDE\PrivateAssemblies`` e.g. ``C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\PrivateAssemblies``.
* ``<CONF DIR>`` = ``<VISUAL STUDIO DIR>\Common7\IDE\PrivateAssemblies`` e.g. ``C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\PrivateAssemblies``

SSRS Paths:
* ``<BIN DIR>`` = ``<RERPORT SERVER DIR>\Reporting Services\ReportServer\bin`` e.g. ``C:\Program Files\Microsoft SQL Server\MSRS11.MSSQLSERVER\Reporting Services\ReportServer\bin``.
* ``<CONF DIR>`` = ``<RERPORT SERVER DIR>\Reporting Services\ReportServer`` e.g. ``C:\Program Files\Microsoft SQL Server\MSRS11.MSSQLSERVER\Reporting Services\ReportServer``


1. Copy all dll files from [build](https://github.com/farzadpanahi/GoogleAnalyticsDataProcessingExtension/tree/master/build) folder (or compile from [src](https://github.com/farzadpanahi/GoogleAnalyticsDataProcessingExtension/tree/master/src)) to ``<BIN DIR>``.

2. Modify ``<CONF DIR>\RSReportDesigner.config``. 

  2.1. Add the following line in ``<Data>`` element:

    ```xml
<Extension Name="Google_Analytics" Type="GoogleAnalyticsDataProcessingExtension.DataProcessingExtension.GAConnection,  GoogleAnalyticsDataProcessingExtension" />
```

  2.2. [Only for Visual Studio] Add the following line in ``<Designer>`` element:

    ```xml
<Extension Name="Google_Analytics" Type="Microsoft.ReportingServices.QueryDesigners.GenericQueryDesigner, Microsoft.ReportingServices.QueryDesigners"/>
```

3. Modify ``<CONF DIR>\RSPreviewPolicy.config``. Add the following CodeGroups in the inner ``<CodeGroup>`` element that has ``<IMembershipCondition class="ZoneMembershipCondition" version="1" Zone="MyComputer" />`` condition. Make sure you replace ``<BIN DIR>`` in the URL attribute for the following CodeGroup elements. This is to give proper access permission to the newly added dlls.

    ```xml
<CodeGroup 
  class="UnionCodeGroup" 
	version="1" 
	PermissionSetName="FullTrust" 
	Name="Google_AnalyticsCodeGroup1"
		Description="Code group for my Google Analytics data processing extension">
		<IMembershipCondition 
		class="UrlMembershipCondition" 
		version="1" 
		Url="<BIN DIR>\DotNetOpenAuth.dll"
		/> 
</CodeGroup>		
<CodeGroup 
	class="UnionCodeGroup" 
	version="1" 
	PermissionSetName="FullTrust" 
	Name="Google_AnalyticsCodeGroup2"
		Description="Code group for my Google Analytics data processing extension">
		<IMembershipCondition 
		class="UrlMembershipCondition" 
		version="1" 
		Url="<BIN DIR>\Google.Apis.Analytics.v3.dll"
		/> 
</CodeGroup>	
<CodeGroup 
	class="UnionCodeGroup" 
	version="1" 
	PermissionSetName="FullTrust" 
	Name="Google_AnalyticsCodeGroup3"
		Description="Code group for my Google Analytics data processing extension">
		<IMembershipCondition 
		class="UrlMembershipCondition" 
		version="1" 
		Url="<BIN DIR>\Google.Apis.Authentication.OAuth2.dll"
		/> 
</CodeGroup>	
<CodeGroup 
	class="UnionCodeGroup" 
	version="1" 
	PermissionSetName="FullTrust" 
	Name="Google_AnalyticsCodeGroup4"
		Description="Code group for my Google Analytics data processing extension">
		<IMembershipCondition 
		class="UrlMembershipCondition" 
		version="1" 
		Url="<BIN DIR>\Google.Apis.dll"
		/> 
</CodeGroup>	
<CodeGroup 
	class="UnionCodeGroup" 
	version="1" 
	PermissionSetName="FullTrust" 
	Name="Google_AnalyticsCodeGroup5"
		Description="Code group for my Google Analytics data processing extension">
		<IMembershipCondition 
		class="UrlMembershipCondition" 
		version="1" 
		Url="<BIN DIR>\GoogleAnalyticsDataProcessingExtension.dll"
		/> 
</CodeGroup>	
<CodeGroup 
	class="UnionCodeGroup" 
	version="1" 
	PermissionSetName="FullTrust" 
	Name="Google_AnalyticsCodeGroup6"
		Description="Code group for my Google Analytics data processing extension">
		<IMembershipCondition 
		class="UrlMembershipCondition" 
		version="1" 
		Url="<BIN DIR>\Newtonsoft.Json.Net35.dll"
		/> 
</CodeGroup>				  
```

4. Now after you restart SSRS service or your Visual Studio, you should see Google Analytics as a Data Source type when creating a new Data Source.

### Configuration
1. Import the certificate you downloaded from Google APIs to Personal certificate store under Computer Account (not Current User). You can do this by opening a MMC and adding Certificates snap-in. Find the Personal store, right-click -> All Tasks -> Import. _Make sure you import the certificate as exportable._
2. You need to create a new Google Analytics Data Source for your report project. The Data Source Connection String syntax is JSON as follows:

    ```json
{ "CertificateSubjectDn": "<subject-dn>[REQUIRED]", "ServiceAccountEmail": "<service-account>[REQUIRED]", "CertificateStoreName": "<store-name>[default=My]", "CertificateStoreLocation": "<store-location>[default=LocalMachine]" }
```

  You can find the subject DN from certificate. Make sure you eliminate any whitespace in the string. Also you will get the service account when you create access in Google APIs. Example:

    ```json
{ "CertificateSubjectDn": "CN=123456789012.apps.googleusercontent.com", "ServiceAccountEmail": "123456789012@developer.gserviceaccount.com" }
```

  There is an alternate syntax for connection string if you do not want to store the certificate in a certificate store and you want to directly reference it from filesystem. _I do not recommend this configuration_ for security reasons, but I implemented it for testing and debug purposes only. Alternate Connection String syntax:
  
    ```json
{ "CertificateFilePath": "<file-path>[REQUIRED]", "CertificatePassword": "<password>[REQUIRED]", "ServiceAccountEmail": "<service-account>[REQUIRED]" }
```

  Make sure to escape the backslash in path. Example:
  
    ```json
{ "CertificateFilePath": "c:\\private_keys\\1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a-privatekey.p12", "CertificatePassword": "notasecret", "ServiceAccountEmail": "123456789012@developer.gserviceaccount.com" }
```
  
3. Explicitly set the credentials for the Data Source to the credentials of the SSRS service. This is important. If you don't do this SSRS will use "execution account" to access the private key and most probebly it will fail.
4. Give read access to this user for the private key of the certificate. You can do this by opening a MMC and adding Certificates snap-in. Find your certificate: Right-click -> All Tasks -> Manage Private Keys.
5. The query syntax is JSON. You just need to createt a JSON with the parameters you need and paste it into the Query box of the Data Set in your report project. JSON query syntax:

    ```json
{ "Ids": "<ids>[REQUIRED]", "StartDate": "<start-date>[REQUIRED]", "EndDate": "<end-date>[REQUIRED]", "Metrics": "<metric>[REQUIRED]", "Dimensions": "<dimensions>", "Filters": "<filters>", "Sort": "<sort>", "Segment": "<segment>", "StartIndex": "start-index", "MaxResults": "<max-results>", "Fields": "<fields>" }
```

  Example:

    ```json
{ "Ids": "ga:12345678", "StartDate": "2013-01-01", "EndDate": "2013-02-01", "Metrics": "ga:visits, ga:timeOnSite", "Dimensions": "ga:year,ga:month", "Sort": "-ga:year,-ga:month,-ga:visits" }
```
  
  You can read more about the parameters [here](https://developers.google.com/analytics/devguides/reporting/core/v3/reference).
  
  If you are using report parameters and you want to include them in your JSON query, you should use Query expressions and escape the double quotations. Example:
  
    ```
= "{ ""Ids"": ""ga:12345678"", ""StartDate"": """ & Format(Parameters!StartDate.Value, "yyyy-MM-dd") & """, ""EndDate"": """ & Format(Parameters!EndDate.Value, "yyyy-MM-dd") & """, ""Metrics"": ""ga:visits, ga:timeOnSite"", ""Dimensions"": ""ga:year,ga:month"", ""Sort"": ""-ga:year,-ga:month,-ga:visits"" }"
```

### Last Word
Installation and configuration looks a bit tedious but it works when done properly : >
