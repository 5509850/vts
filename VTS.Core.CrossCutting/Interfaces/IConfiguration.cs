namespace VTS.Core.CrossCutting
{
    public  interface IConfiguration
    {
        string RestServerUrl { get; }
        string SqlDatabaseName { get; }

        int ServerTimeOut { get; }

        DefaultLanguage GetDefaultLanguage { get; }
    }
}
