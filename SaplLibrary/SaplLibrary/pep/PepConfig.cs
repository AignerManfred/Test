using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Sapl.Pdp.Api;
using Sapl.Pdp.Remote;

namespace Sapl.Pep.Config
{
    public class PepConfig
    {
        private static PepConfig _instance;

        private PepConfig()
        {
        }

        public static PepConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PepConfig();

                return _instance;
            }
        }

        public IPolicyDecisionPoint PolicyDecisionPoint
        {
            get
            {
                // Create an HttpClientHandler object and set to use default credentials
                HttpClientHandler handler = new HttpClientHandler();

                // Set custom server validation callback
                handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;

                return new RemotePolicyDecisionPoint(new Uri("https://localhost:8443"), "YJidgyT2mfdkbmL", "Fa4zvYQdiwHZVXh", handler); 
            }
        }

        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            // It is possible to inspect the certificate provided by the server.
            Console.WriteLine($"Requested URI: {requestMessage.RequestUri}");
            Console.WriteLine($"Effective date: {certificate.GetEffectiveDateString()}");
            Console.WriteLine($"Exp date: {certificate.GetExpirationDateString()}");
            Console.WriteLine($"Issuer: {certificate.Issuer}");
            Console.WriteLine($"Subject: {certificate.Subject}");

            // Based on the custom logic it is possible to decide whether the client considers certificate valid or not
            // Console.WriteLine($"Errors: {sslErrors}");

            // return sslErrors == SslPolicyErrors.None;
            return true;
        }

    }
}
