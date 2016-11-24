using System;

namespace VTS.Core.CrossCutting
{

    public class Configuration : IConfiguration
    {
        private const string server = @"http://10.6.25.94"; //@"http://192.168.2.88";
        private const string dbName = "VTS";
        private const int serverTimeOut = 10000;
        private const DefaultLanguage defaultLanguage = DefaultLanguage.System;
        public string RestServerUrl
        {
            get
            {
                return server;
            }           
        }
        public string SqlDatabaseName
        {
            get
            {
                return dbName;
            }
        }
        public DefaultLanguage GetDefaultLanguage
        {
            get
            {
                return defaultLanguage;
            }
        }

        public int ServerTimeOut
        {
            get
            {
                return serverTimeOut;
            }
        }
    }
}