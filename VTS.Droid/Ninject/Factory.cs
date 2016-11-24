using Ninject;
using System;
using System.Diagnostics;
using VTS.Core.CrossCutting;

namespace VTS.Droid
{
    public class Factory
    {
        private IKernel _kernel = new StandardKernel();
        public void Init(IUIRegistry registry)
        {
            registry.Register(_kernel);
        }
        public T Get<T>()
        {        
            try
            {
                return _kernel.Get<T>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error {0} with Exception {1}", typeof(T).Name, ex));
                throw;
            }
        }
    }
}
