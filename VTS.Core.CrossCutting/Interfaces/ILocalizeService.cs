namespace VTS.Core.CrossCutting
{
    public interface ILocalizeService
    {
        void LoadLocalization(string language = null);

        string Localize(string key);
    }
}
