using VTS.Core.CrossCutting;
using VTS.Core.Data;
using VTS.Core.Business;
using Specific.Droid;

namespace VTS.Droid
{
    public class FactorySingleton
    {
        private static Factory _factory;

        private FactorySingleton() { }

        public static Factory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new Factory();
                    _factory.Init(new CrossCuttingConcernsRegistry());
                    _factory.Init(new DataRegistry());
                    _factory.Init(new BusinessRegistry());
                    _factory.Init(new DroidRegistry());
                }
                return _factory;
            }
        }
    }
}

