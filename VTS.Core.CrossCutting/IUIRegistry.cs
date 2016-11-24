using Ninject;

namespace VTS.Core.CrossCutting
{
    public interface IUIRegistry
    {
        void Register(IKernel kernel);
    }
}
