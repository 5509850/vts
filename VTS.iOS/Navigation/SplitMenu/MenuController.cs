using UIKit;
using MonoTouch.Dialog;
using System;
using System.Collections.Generic;
using VTS.Core.CrossCutting;
using VTS.iOS.Ninject;
using VTS.iOS.Helpers;

namespace VTS.iOS.Navigation.SplitMenu
{
    public class MenuController : DialogViewController
    {
        private List<StringElement> menuItems;
        private ILocalizeService _localizeService;

        public event EventHandler<RowClickedEventArgs> RowClicked;
        public class RowClickedEventArgs : EventArgs
        {
            public int Item { get; set; }
            public RowClickedEventArgs(int item) : base()
            { this.Item = item; }
        }

        public MenuController() : base(UITableViewStyle.Plain, null)
        {
            _localizeService = FactorySingleton.Factory.Get<LocalizeService>();            
            InitMenuItem();            
            var section = new Section();
            section.AddAll(menuItems);            
            Root = new RootElement("....") { section };                
        }

        private void InitMenuItem()
        {
            menuItems = new List<StringElement>();            
            menuItems.Add(new StringElement(_localizeService.Localize("Requests"),
            delegate
            {
                if (RowClicked != null)
                {
                    RowClicked(this, new RowClickedEventArgs(Utils.LIST_VACATION));
                }
            }));

            menuItems.Add(new StringElement(_localizeService.Localize("addVacation"),
                delegate
                {
                    if (RowClicked != null)
                    {
                        RowClicked(this, new RowClickedEventArgs(Utils.VACATION_REQUEST));
                    }
                }));

            menuItems.Add(new StringElement(_localizeService.Localize("SickToday"),
                delegate
                {
                    if (RowClicked != null)
                    {
                        RowClicked(this, new RowClickedEventArgs(Utils.SICK_REQUEST));
                    }
                }));
            menuItems.Add(new StringElement(_localizeService.Localize("Overtime"),
              delegate
              {
                  if (RowClicked != null)
                  {
                      RowClicked(this, new RowClickedEventArgs(Utils.OVERTIME_REQUEST));
                  }
              }));
            menuItems.Add(new StringElement(_localizeService.Localize("LeaveWOPay"),
              delegate
              {
                  if (RowClicked != null)
                  {
                      RowClicked(this, new RowClickedEventArgs(Utils.LIVEWOP_REQUEST));
                  }
              }));
            menuItems.Add(new StringElement(_localizeService.Localize("ExceptionalLeave"),
             delegate
             {
                 if (RowClicked != null)
                 {
                     RowClicked(this, new RowClickedEventArgs(Utils.EXCEPTIONAL_REQUEST));
                 }
             }));
            menuItems.Add(new StringElement(_localizeService.Localize("Exit"),
            delegate
            {
                if (RowClicked != null)
                {
                    RowClicked(this, new RowClickedEventArgs(Utils.EXIT_REQUEST));
                }
            }));
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }
}