using VTS.Core.CrossCutting;
using VTS.Core.Data;

namespace VTS.Tests.Ninject
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
                    _factory.OnlineMode = true;
                    _factory.Init(new CrossCuttingConcernsRegistry());
                    _factory.Init(new DataRegistry());
                    _factory.Init(new TestRegistry());
                }
                else
                {
                    if (!_factory.OnlineMode)
                    {
                        _factory = new Factory();
                        _factory.OnlineMode = true;
                        _factory.Init(new CrossCuttingConcernsRegistry());
                        _factory.Init(new DataRegistry());
                        _factory.Init(new TestRegistry());
                    }
                }
                return _factory;
            }
        }       

        public static Factory FactoryOffline
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new Factory();
                    _factory.OnlineMode = false;
                    _factory.Init(new CrossCuttingConcernsRegistry());
                    _factory.Init(new DataRegistry());
                    _factory.Init(new TestRegistryOffline());
                }
                else
                {
                    if (_factory.OnlineMode)
                    {
                        _factory = new Factory();
                        _factory.OnlineMode = false;
                        _factory.Init(new CrossCuttingConcernsRegistry());
                        _factory.Init(new DataRegistry());
                        _factory.Init(new TestRegistryOffline());
                    }
                }
                return _factory;
            }
        }
    }   
}

