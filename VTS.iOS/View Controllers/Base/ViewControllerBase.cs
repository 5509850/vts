using Cirrious.FluentLayouts.Touch;
using UIKit;
using VTS.Core.CrossCutting;
using VTS.iOS.Ninject;

namespace VTS.iOS.View_Controllers
{
    public class ViewControllerBase : UIViewController                
    {
        private ILocalizeService _localizeService;
        public ViewControllerBase()
        {
            _localizeService = FactorySingleton.Factory.Get<LocalizeService>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Initialize();           
        }

        public override void ViewDidAppear(bool animate)
        {
            base.ViewDidAppear(animate);
            ApplyEvents();          
        }

        public override void ViewDidDisappear(bool animate)
        {
            base.ViewDidDisappear(animate);
            DisposeResource();
        }       

        private void Initialize()
        {          
            CreateLayout();
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();                    
            ApplyConstraints();                     
            FetchDataToControl();
        }

        protected virtual void CreateLayout() { }
        protected virtual void ApplyConstraints() { }      
        protected virtual void FetchDataToControl() { }
        protected virtual void ApplyEvents() { }
        protected virtual void DisposeResource() { }
        public async virtual void DeleteItem(object item) { }
        public string Localize(string key)
        {
            return _localizeService.Localize(key);
        }
    }
}
