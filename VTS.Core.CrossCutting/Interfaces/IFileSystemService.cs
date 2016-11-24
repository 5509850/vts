using System.Threading.Tasks;

namespace VTS.Core.CrossCutting
{
    public interface IFileSystemService
    {
        Task<string> GetPath(string dbName);
        Task SaveText(string filename, string text);
        Task<string> LoadText(string filename);
        Task<bool> ExistsFile(string filename);       
    }
}
