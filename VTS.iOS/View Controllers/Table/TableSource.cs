using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using VTS.Core.Data.Models;
using VTS.iOS.Helpers;
using VTS.iOS.Ninject;
using VTS.iOS.Navigation;

namespace VTS.iOS.View_Controllers.Table
{
    public class TableSource : UITableViewSource
    {
        List<VTSModel> tableItems;
        string cellIdentifier = "TableCell";
        ViewControllerBase owner;
        string trash = "Trash";

        public TableSource(List<VTSModel> items, ViewControllerBase owner)
        {
            tableItems = items;
            this.owner = owner;
            trash = owner.Localize("trash");
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }
               
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {            
            var editController = new EditVacationViewController();
            editController.ID = tableItems[indexPath.Row].Id;
            FactorySingleton.Factory.Get<NavigationService>()
                .Navigate(editController);            
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {           
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);           
            var cellStyle = UITableViewCellStyle.Value1;
            if (cell == null)
            {
                cell = new UITableViewCell(cellStyle, cellIdentifier);
            }           
            cell.TextLabel.Text = tableItems[indexPath.Row].VacationType;
            if (cellStyle == UITableViewCellStyle.Subtitle
               || cellStyle == UITableViewCellStyle.Value1
               || cellStyle == UITableViewCellStyle.Value2)
            {
                cell.DetailTextLabel.Text = tableItems[indexPath.Row].Date;
                if (tableItems[indexPath.Row].Status.Equals(VacationStatus.Approved.ToString()))
                {
                    cell.DetailTextLabel.TextColor = UIColor.Green;
                }
                else {
                    if (tableItems[indexPath.Row].Status.Equals(VacationStatus.Rejected.ToString()))
                    {
                        cell.DetailTextLabel.TextColor = UIColor.Red;
                    }
                    else
                    {
                        cell.DetailTextLabel.TextColor = UIColor.Gray;   
                    }
                }
            }
            if (cellStyle != UITableViewCellStyle.Value2)
                cell.ImageView.Image = UIImage.FromFile("Images/person.png");
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            cell.DetailTextLabel.Font = FontLoader.GetFontSmall();
            cell.TextLabel.Font = FontLoader.GetFontBold();
            return cell;
        }

        #region -= editing methods =-
        
        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    // remove the item from the underlying data source
                    owner.DeleteItem(tableItems[indexPath.Row]);
                    tableItems.RemoveAt(indexPath.Row);
                    // delete the row from the table
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;

                case UITableViewCellEditingStyle.Insert:
                    //---- create a new item and add it to our underlying data
                    var newItem = new VTSModel();
                    newItem.VacationType = "inserted";
                    tableItems.Insert(indexPath.Row, newItem);
                    //---- insert a new row in the table
                    tableView.InsertRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;

                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }
        }
       
        /// <summary>
        /// Called by the table view to determine whether or not the row is editable
        /// </summary>
        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true; // return false if you wish to disable editing for a specific indexPath or for all rows
        }

        /// <summary>
        /// Called by the table view to determine whether or not the row is moveable
        /// </summary>
        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return indexPath.Row < tableView.NumberOfRowsInSection(0) - 1;
        }

        /// <summary>
        /// Custom text for delete button
        /// </summary>
        public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
        {
            return trash + "(" + tableItems[indexPath.Row].VacationType + ")";
        }
        #region Edit and create example
        /// <summary>
        /// Called by the table view to determine whether the editing control should be an insert
        /// or a delete.
        /// </summary>
        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView.Editing)
            {
                if (indexPath.Row == tableView.NumberOfRowsInSection(0) - 1)
                    return UITableViewCellEditingStyle.Insert;
                else
                    return UITableViewCellEditingStyle.Delete;
            }
            else  // not in editing mode, enable swipe-to-delete for all rows
                return UITableViewCellEditingStyle.Delete;
        }
        public override NSIndexPath CustomizeMoveTarget(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
        {
            var numRows = tableView.NumberOfRowsInSection(0) - 1; // less the (add new) one
            Console.WriteLine(proposedIndexPath.Row + " " + numRows);
            if (proposedIndexPath.Row >= numRows)
                return NSIndexPath.FromRowSection(numRows - 1, 0);
            else
                return proposedIndexPath;
        }
        /// <summary>
        /// called by the table view when a row is moved.
        /// </summary>
        public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
        {
            //---- get a reference to the item
            var item = tableItems[sourceIndexPath.Row];
            var deleteAt = sourceIndexPath.Row;
            var insertAt = destinationIndexPath.Row;

            //---- if we're moving within the same section, and we're inserting it before
            if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row))
            {
                //---- add one to where we delete, because we're increasing the index by inserting
                deleteAt += 1;
            }
            else {
                insertAt += 1;
            }

            //---- copy the item to the new location
            tableItems.Insert(destinationIndexPath.Row, item);

            //---- remove from the old
            tableItems.RemoveAt(deleteAt);
        }
        /// <summary>
        /// Called manually when the table goes into edit mode
        /// </summary>
        public void WillBeginTableEditing(UITableView tableView)
        {
            //---- start animations
            tableView.BeginUpdates();

            //---- insert a new row in the table
            tableView.InsertRows(new NSIndexPath[] {
                    NSIndexPath.FromRowSection (tableView.NumberOfRowsInSection (0), 0)
                }, UITableViewRowAnimation.Fade);
            //---- create a new item and add it to our underlying data
            var newItem = new VTSModel();
            newItem.VacationType = "(add new)";
            tableItems.Add(newItem);

            //---- end animations
            tableView.EndUpdates();
        }

        /// <summary>
        /// Called manually when the table leaves edit mode
        /// </summary>
        public void DidFinishTableEditing(UITableView tableView)
        {
            //---- start animations
            tableView.BeginUpdates();
            //---- remove our row from the underlying data
            tableItems.RemoveAt((int)tableView.NumberOfRowsInSection(0) - 1); // zero based :)
                                                                              //---- remove the row from the table
            tableView.DeleteRows(new NSIndexPath[] { NSIndexPath.FromRowSection(tableView.NumberOfRowsInSection(0) - 1, 0) }, UITableViewRowAnimation.Fade);
            //---- finish animations
            tableView.EndUpdates();
        }
        #endregion

        #endregion

    }
}