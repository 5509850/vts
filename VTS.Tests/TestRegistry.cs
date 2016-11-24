using VTS.Core.CrossCutting;
using Ninject;
using SQLite.Net.Interop;
using System.Threading.Tasks;
using VTS.Core.CrossCutting.Helpers;
using System.IO;
using System;
using SQLite.Net.Platform.Win32;
using VTS.Core.Business.ViewModel;
using VTS.Core.Business;

namespace VTS.Tests
{
    public class TestRegistry : IUIRegistry
    {
        public void Register(IKernel kernel)
        {
            //fromBusinessRegistry
            kernel.Bind<ILoginManager>().To<LoginManager>().InSingletonScope();
            kernel.Bind<IVacationManager>().To<VacationManager>().InSingletonScope();
            kernel.Bind<INetworkReachability>().To<TestNetworkReachability>().InSingletonScope();
            kernel.Bind<LoginViewModel>().ToSelf();
            kernel.Bind<VacationsViewModel>().ToSelf();

            //from AndroidRegistry
            kernel.Bind<IFileSystemService>().To<FileSystemService>().InSingletonScope();
            kernel.Bind<ILocalizer>().To<Localizer>().InSingletonScope();
            kernel.Bind<ISQLitePlatform>().To<SQLitePlatformTest>().InSingletonScope();
            kernel.Bind<IPlatformException>().To<PlatformException>().InSingletonScope();
        }
    }

    public class TestRegistryOffline : IUIRegistry
    {
        public void Register(IKernel kernel)
        {
            //fromBusinessRegistry
            kernel.Bind<ILoginManager>().To<LoginManager>().InSingletonScope();
            kernel.Bind<IVacationManager>().To<VacationManager>().InSingletonScope();
            kernel.Bind<INetworkReachability>().To<TestNetworkReachabilityOffline>().InSingletonScope();
            kernel.Bind<LoginViewModel>().ToSelf();
            kernel.Bind<VacationsViewModel>().ToSelf();

            //from AndroidRegistry
            kernel.Bind<IFileSystemService>().To<FileSystemService>().InSingletonScope();
            kernel.Bind<ILocalizer>().To<Localizer>().InSingletonScope();
            kernel.Bind<ISQLitePlatform>().To<SQLitePlatformTest>().InSingletonScope();
            kernel.Bind<IPlatformException>().To<PlatformException>().InSingletonScope();
        }
    }

    public class SQLitePlatformTest : ISQLitePlatform
    {
        private SQLitePlatformWin32 _sqlitePlatformWin32;
        public SQLitePlatformTest()
        {
            _sqlitePlatformWin32 = new SQLitePlatformWin32();
        }

        public IReflectionService ReflectionService { get { return _sqlitePlatformWin32.ReflectionService; } }
        public ISQLiteApi SQLiteApi { get { return _sqlitePlatformWin32.SQLiteApi; } }
        public IStopwatchFactory StopwatchFactory { get { return _sqlitePlatformWin32.StopwatchFactory; } }
        public IVolatileService VolatileService { get { return _sqlitePlatformWin32.VolatileService; } }
    }

    public class FileSystemService : IFileSystemService
    {
        public Task<string> GetPath(string dbName)
        {
            string filename = dbName + ".db3";
            var path = GetFilePath(filename);
            return Helper.Complete(path);
        }
        public Task SaveText(string filename, string text)
        {
            var filePath = GetFilePath(filename);
            System.IO.File.WriteAllText(filePath, text);
            return Helper.Complete();
        }
        public Task<string> LoadText(string filename)
        {
            var filePath = GetFilePath(filename);
            return Helper.Complete(System.IO.File.ReadAllText(filePath));
        }
        public Task<bool> ExistsFile(string filename)
        {
            string filepath = GetFilePath(filename);
            return Helper.Complete(File.Exists(filepath));
        }

        #region private methodes
        private string GetFilePath(string filename)
        {
            string docsPath = "D:\\";
            return Path.Combine(docsPath, filename);
        }
        #endregion

    }
    public class Localizer : ILocalizer
    {
        public string GetCurrentCultureInfo()
        {           
            return "en";
        }
    }

    public class PlatformException : IPlatformException
    {
        public PlatformException() : base() { }
        public Type URISyntaxException()
        {
            return typeof(Java.Net.URISyntaxException);
        }
    }

    public class TestNetworkReachability : INetworkReachability
    {
        private const bool ONLINE = true;
        private const bool OFFLINE = false;
        public bool IsOnlineMode
        {
            get
            {
                return ONLINE;
            }
        }
        public async Task<bool> IsReachableHost(string host, int msTimeout)
        {
            return await Helper.Complete(true);
        }
    }

    public class TestNetworkReachabilityOffline : INetworkReachability
    {
        private const bool ONLINE = true;
        private const bool OFFLINE = false;
        public bool IsOnlineMode
        {
            get
            {
                return OFFLINE;
            }
        }
        public async Task<bool> IsReachableHost(string host, int msTimeout)
        {
            return await Helper.Complete(true);
        }
    }
}