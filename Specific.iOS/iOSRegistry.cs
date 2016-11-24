using Ninject;
using Specific.iOS.Services;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinIOS;
using VTS.Core.CrossCutting;

namespace Specific.iOS
{
    public class iOSRegistry : IUIRegistry
    {
        public void Register(IKernel kernel)
        {
            kernel.Bind<IFileSystemService>().To<FileSystemService>().InSingletonScope();
            kernel.Bind<ILocalizer>().To<Localizer>().InSingletonScope();
            kernel.Bind<ISQLitePlatform>().To<SQLitePlatformIOS>().InSingletonScope();
            kernel.Bind<IPlatformException>().To<PlatformException>().InSingletonScope();           
        }
    }
}