using System;
using System.IO;
using System.Text.Json;

namespace proknow_sdk
{
    public class ProKnow
    {
        public Requestor Requestor { get; private set; }

        public ProKnow(string baseUrl, string credentialsFile, int lockRenewalBuffer = 30)
        {
            using (StreamReader sr = new StreamReader(credentialsFile))
            {
                var proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
                Requestor = new Requestor(baseUrl, proKnowCredentials.Id, proKnowCredentials.Secret);
            }
        }

        public ProKnow(string baseUrl, string credentialsId, string credentialsSecret, int lockRenewalBuffer = 30)
        {
            throw new NotImplementedException();
        }
    }
}
