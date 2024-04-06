using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Dashboard;
using StarLaiPortal.Module.BusinessObjects.Inquiry_View;
using StarLaiPortal.Module.BusinessObjects.Pick_List;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DevExpress.Xpo.DB;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using DevExpress.ExpressApp.Xpo;
using StarLaiPortal.Module.BusinessObjects.Item_Inquiry;

// 2023-09-14 - add filter into inquiry - ver 1.0.9
// 2023-10-16 - sales order inquiry add "All" option for filter and view button - ver 1.0.11
// 2024-01-30 - add inventory movement search button - ver 1.0.14
// 2024-04-05 - add inquiry search button - ver 1.0.15

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class InquiryViewControllers : ViewController
    {
        // Start ver 1.0.9
        private DateTime Fromdate;
        private DateTime Todate;
        // End ver 1.0.9
        public InquiryViewControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.ViewOpenPickList.Active.SetItemValue("Enabled", false);
            this.ViewPickListDetailInquiry.Active.SetItemValue("Enabled", false);
            this.ViewPickListInquiry.Active.SetItemValue("Enabled", false);
            // Start ver 1.0.9
            this.InquiryStatus.Active.SetItemValue("Enabled", false);
            this.InquiryDateFrom.Active.SetItemValue("Enabled", false);
            this.InquiryDateTo.Active.SetItemValue("Enabled", false);
            this.InquiryFilter.Active.SetItemValue("Enabled", false);
            // End ver 1.0.9
            // Start ver 1.0.11
            this.ViewSalesOrderInquiry.Active.SetItemValue("Enabled", false);
            // End ver 1.0.11
            // Start ver 1.0.14
            this.StockMovementSPSearch.Active.SetItemValue("Enabled", false);
            // End ver 1.0.14
            // Start ver 1.0.15
            this.InquirySearch.Active.SetItemValue("Enabled", false);
            // End ver 1.0.15

            if (typeof(vwInquiryOpenPickList).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(vwInquiryOpenPickList))
                {
                    this.ViewOpenPickList.Active.SetItemValue("Enabled", true);
                    this.ViewOpenPickList.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }

            if (typeof(vwInquiryPickListDetails).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(vwInquiryPickListDetails))
                {
                    this.ViewPickListDetailInquiry.Active.SetItemValue("Enabled", true);
                    this.ViewPickListDetailInquiry.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }

            if (typeof(vwInquiryPickList).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(vwInquiryPickList))
                {
                    this.ViewPickListInquiry.Active.SetItemValue("Enabled", true);
                    this.ViewPickListInquiry.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                }
            }

            // Start ver 1.0.9
            if (typeof(vwInquirySalesOrder).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(vwInquirySalesOrder))
                {
                    this.ViewSalesOrderInquiry.Active.SetItemValue("Enabled", true);
                    this.ViewSalesOrderInquiry.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;

                    if (View.Id == "vwInquirySalesOrder_ListView")
                    {
                        InquiryStatus.Items.Clear();

                        // Start ver 1.0.11
                        InquiryStatus.Items.Add(new ChoiceActionItem("All", "All"));
                        // End ver 1.0.11
                        InquiryStatus.Items.Add(new ChoiceActionItem("Open", "Open"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Draft", "Draft"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Submitted", "Submitted"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Cancelled", "Cancelled"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Closed", "Closed"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Posted", "Posted"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Pending Post", "Pending Post"));

                        InquiryStatus.SelectedIndex = 1;

                        this.InquiryStatus.Active.SetItemValue("Enabled", true);
                        InquiryStatus.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        InquiryStatus.CustomizeControl += action_CustomizeControl;

                        this.InquiryDateFrom.Active.SetItemValue("Enabled", true);
                        this.InquiryDateFrom.Value = DateTime.Today.AddDays(-7);
                        InquiryDateFrom.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.InquiryDateFrom.CustomizeControl += DateActionFrom_CustomizeControl;
                        this.InquiryDateTo.Active.SetItemValue("Enabled", true);
                        this.InquiryDateTo.Value = DateTime.Today.AddDays(1);
                        InquiryDateTo.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.InquiryDateTo.CustomizeControl += DateActionTo_CustomizeControl;
                        this.InquiryFilter.Active.SetItemValue("Enabled", true);

                        // Start ver 1.0.11
                        if (InquiryStatus.SelectedItem.Id != "All")
                        {
                            // End ver 1.0.11
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                            "and DocDate >= ? and DocDate <= ?",
                            InquiryStatus.SelectedItem.Id, InquiryDateFrom.Value, InquiryDateTo.Value);
                        // Start ver 1.0.11
                        }
                        else
                        {
                            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?",
                                InquiryDateFrom.Value, InquiryDateTo.Value);
                        }
                        // End ver 1.0.11
                    }
                }
            }

            if (typeof(vwInquiryPickList).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(vwInquiryPickList))
                {
                    if (View.Id == "vwInquiryPickList_ListView")
                    {
                        InquiryStatus.Items.Clear();

                        InquiryStatus.Items.Add(new ChoiceActionItem("Open", "Open"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Draft", "Draft"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Submitted", "Submitted"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Cancelled", "Cancelled"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Closed", "Closed"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Posted", "Posted"));
                        InquiryStatus.Items.Add(new ChoiceActionItem("Pending Post", "Pending Post"));

                        InquiryStatus.SelectedIndex = 1;

                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ?",
                            InquiryStatus.SelectedItem.Id);

                        this.InquiryStatus.Active.SetItemValue("Enabled", true);
                        InquiryStatus.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        InquiryStatus.CustomizeControl += action_CustomizeControl;

                        this.InquiryDateFrom.Active.SetItemValue("Enabled", true);
                        this.InquiryDateFrom.Value = DateTime.Today.AddDays(-7);
                        InquiryDateFrom.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.InquiryDateFrom.CustomizeControl += DateActionFrom_CustomizeControl;
                        this.InquiryDateTo.Active.SetItemValue("Enabled", true);
                        this.InquiryDateTo.Value = DateTime.Today.AddDays(1);
                        InquiryDateTo.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.InquiryDateTo.CustomizeControl += DateActionTo_CustomizeControl;
                        this.InquiryFilter.Active.SetItemValue("Enabled", true);

                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                        "and DocDate >= ? and DocDate <= ?",
                        InquiryStatus.SelectedItem.Id, InquiryDateFrom.Value, InquiryDateTo.Value);
                    }
                }
            }
            // End ver 1.0.9

            // Start ver 1.0.14
            if (typeof(StockMovement).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(StockMovement))
                {
                    this.StockMovementSPSearch.Active.SetItemValue("Enabled", true);
                }
            }
            // End ver 1.0.14

            // Start ver 1.0.15
            if (typeof(SalesQuotationInquiry).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(SalesQuotationInquiry))
                {
                    this.InquirySearch.Active.SetItemValue("Enabled", true);
                }
            }
            // End ver 1.0.15
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control. 
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        void action_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            SingleChoiceActionAsModeMenuActionItem actionItem = e.Control as SingleChoiceActionAsModeMenuActionItem;
            if (actionItem != null && actionItem.Action.PaintStyle == DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption)
            {
                DropDownSingleChoiceActionControlBase control = (DropDownSingleChoiceActionControlBase)actionItem.Control;
                control.Label.Text = actionItem.Action.Caption;
                control.Label.Style["padding-right"] = "5px";
                control.ComboBox.Width = 100;
            }
        }

        private void DateActionFrom_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            ParametrizedActionMenuActionItem actionItem = e.Control as ParametrizedActionMenuActionItem;

            if (actionItem != null)
            {
                ASPxDateEdit dateEdit = actionItem.Control.Editor as ASPxDateEdit;
                if (dateEdit != null)
                {
                    dateEdit.Width = 110;
                    dateEdit.Buttons.Clear();
                    if (dateEdit.Text != "")
                    {
                        Fromdate = Convert.ToDateTime(dateEdit.Text);
                    }
                }
            }
        }

        private void DateActionTo_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            ParametrizedActionMenuActionItem actionItem = e.Control as ParametrizedActionMenuActionItem;

            if (actionItem != null)
            {
                ASPxDateEdit dateEdit = actionItem.Control.Editor as ASPxDateEdit;
                if (dateEdit != null)
                {
                    dateEdit.Width = 110;
                    dateEdit.Buttons.Clear();
                    if (dateEdit.Text != "")
                    {
                        Todate = Convert.ToDateTime(dateEdit.Text);
                    }
                }
            }
        }

        public void openNewView(IObjectSpace os, object target, ViewEditMode viewmode)
        {
            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(os, target);
            dv.ViewEditMode = viewmode;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

        }

        public void showMsg(string caption, string msg, InformationType msgtype)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            //options.Message = string.Format("{0} task(s) have been successfully updated!", e.SelectedObjects.Count);
            options.Message = string.Format("{0}", msg);
            options.Type = msgtype;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            Application.ShowViewStrategy.ShowMessage(options);
        }

        private void ViewOpenPickList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            vwInquiryOpenPickList selectedObject = (vwInquiryOpenPickList)e.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.PortalNo));
            openNewView(os, trx, ViewEditMode.View);
        }

        private void ViewOpenPickList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            vwInquiryOpenPickList selectedObject = (vwInquiryOpenPickList)View.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.PortalNo));

            DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
            e.View = detailView;
            e.DialogController.AcceptAction.Caption = "Go To Document";
            e.Maximized = true;
            //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
        }

        private void ViewPickListDetailInquiry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            vwInquiryPickListDetails selectedObject = (vwInquiryPickListDetails)e.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.PortalNo));
            openNewView(os, trx, ViewEditMode.View);
        }

        private void ViewPickListDetailInquiry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            vwInquiryPickListDetails selectedObject = (vwInquiryPickListDetails)View.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.PortalNo));

            DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
            e.View = detailView;
            e.DialogController.AcceptAction.Caption = "Go To Document";
            e.Maximized = true;
            //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
        }

        private void ViewPickListInquiry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            vwInquiryPickList selectedObject = (vwInquiryPickList)e.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.PortalNo));
            openNewView(os, trx, ViewEditMode.View);
        }

        private void ViewPickListInquiry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            vwInquiryPickList selectedObject = (vwInquiryPickList)View.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            PickList trx = os.FindObject<PickList>(new BinaryOperator("DocNum", selectedObject.PortalNo));

            DetailView detailView = Application.CreateDetailView(os, "PickList_DetailView_Dashboard", true, trx);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
            e.View = detailView;
            e.DialogController.AcceptAction.Caption = "Go To Document";
            e.Maximized = true;
            //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
        }

        // Start ver 1.0.9
        private void InquiryStatus_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            // Start ver 1.0.11
            if (InquiryStatus.SelectedItem.Id != "All")
            {
            // End ver 1.0.11
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                    "and DocDate >= ? and DocDate <= ?",
                    InquiryStatus.SelectedItem.Id, Fromdate, Todate);
            // Start ver 1.0.11
            }
            else
            {
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?",
                    Fromdate, Todate);
            }
            // End ver 1.0.11
        }

        private void InquiryDateFrom_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                "and DocDate >= ? and DocDate <= ?",
                InquiryStatus.SelectedItem.Id, Fromdate, Todate);
        }

        private void InquiryDateTo_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                "and DocDate >= ? and DocDate <= ?",
                InquiryStatus.SelectedItem.Id, Fromdate, Todate);
        }

        private void InquiryFilter_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            // Start ver 1.0.11
            if (InquiryStatus.SelectedItem.Id != "All")
            {
            // End ver 1.0.11
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                "and DocDate >= ? and DocDate <= ?",
                InquiryStatus.SelectedItem.Id, Fromdate, Todate);
            // Start ver 1.0.11
            }
            else
            {
                ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?",
                    Fromdate, Todate);
            }
            // End ver 1.0.11
        }
        // End ver 1.0.9

        // Start ver 1.0.11
        private void ViewSalesOrderInquiry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            vwInquirySalesOrder selectedObject = (vwInquirySalesOrder)e.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.PortalNo));
            openNewView(os, trx, ViewEditMode.View);
        }

        private void ViewSalesOrderInquiry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            vwInquirySalesOrder selectedObject = (vwInquirySalesOrder)View.CurrentObject;

            IObjectSpace os = Application.CreateObjectSpace();
            SalesOrder trx = os.FindObject<SalesOrder>(new BinaryOperator("DocNum", selectedObject.PortalNo));

            DetailView detailView = Application.CreateDetailView(os, "SalesOrder_DetailView_Dashboard", true, trx);
            detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.View;
            e.View = detailView;
            e.DialogController.AcceptAction.Caption = "Go To Document";
            e.Maximized = true;
            //e.DialogController.CancelAction.Active["NothingToCancel"] = false;
        }
        // End ver 1.0.11

        // Start ver 1.0.14
        private void StockMovementSPSearch_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string itemcode = "";
            string portalnum = "";
            StockMovement selectedObject = (StockMovement)e.CurrentObject;

            if (selectedObject.ItemCode != null)
            {
                itemcode = selectedObject.ItemCode.ItemCode;
            }

            if (selectedObject.PortalDocNum != null)
            {
                portalnum = selectedObject.PortalDocNum;
            }

            int cnt = 0;
            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetStockMovement", 
                new OperandValue(selectedObject.DateFrom.Date),
                new OperandValue(selectedObject.DateTo.Date),
                new OperandValue(itemcode), new OperandValue(portalnum));

            if (sprocData.ResultSet.Count() > 0)
            {
                if (sprocData.ResultSet[0].Rows.Count() > 0)
                {
                    selectedObject.Results.Clear();
                    foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                    {
                        StockMovementResult stockmovement = new StockMovementResult();

                        stockmovement.Oid = ++cnt;
                        stockmovement.TransDate = row.Values[0].ToString();
                        stockmovement.PortalNo = row.Values[1].ToString();
                        stockmovement.SAPNo = row.Values[2].ToString();
                        stockmovement.CardCode = row.Values[3].ToString();
                        stockmovement.CardName = row.Values[4].ToString();
                        stockmovement.ItemCode = row.Values[5].ToString();
                        stockmovement.ItemName = row.Values[6].ToString();
                        stockmovement.LegacyItemCode = row.Values[7].ToString();
                        stockmovement.CatalogNo = row.Values[8].ToString();
                        stockmovement.Model = row.Values[9].ToString();
                        stockmovement.Quantity = Convert.ToDecimal(row.Values[10].ToString());
                        stockmovement.UOM = row.Values[11].ToString();
                        stockmovement.Warehouse = row.Values[12].ToString();
                        stockmovement.BinLocation = row.Values[13].ToString();
                        stockmovement.TransType = row.Values[14].ToString();

                        selectedObject.Results.Add(stockmovement);
                    }
                }
            }

            ObjectSpace.Refresh();
            View.Refresh();

            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();
        }
        // End ver 1.0.14

        // Start ver 1.0.15
        private void InquirySearch_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.ObjectTypeInfo.Type == typeof(SalesQuotationInquiry))
            {
                SalesQuotationInquiry currObject = (SalesQuotationInquiry)e.CurrentObject;
                currObject.Results.Clear();

                XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();
                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetInquiryView",
                    new OperandValue(currObject.DateFrom.Date),
                    new OperandValue(currObject.DateTo.Date), new OperandValue(currObject.Status), new OperandValue("SalesQuotationInquiry"));

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            SalesQuotationInquiryResult result = new SalesQuotationInquiryResult();

                            result.PriKey = row.Values[0].ToString();
                            result.PortalNo = row.Values[1].ToString();
                            result.DocDate = DateTime.Parse(row.Values[2].ToString());
                            result.DueDate = DateTime.Parse(row.Values[3].ToString());
                            result.Status = row.Values[4].ToString();
                            result.HitCreditLimit = row.Values[5].ToString();
                            result.HitCreditTerm = row.Values[6].ToString();
                            result.HitPriceChange = row.Values[7].ToString();
                            result.CardGroup = row.Values[8].ToString();
                            result.CardCode = row.Values[9].ToString();
                            result.CardName = row.Values[10].ToString();
                            result.ContactNo = row.Values[11].ToString();
                            result.Transporter = row.Values[12].ToString();
                            result.Salesperson = row.Values[13].ToString();
                            result.Priority = row.Values[14].ToString();
                            result.Series = row.Values[15].ToString();
                            result.Amount = decimal.Parse(row.Values[16].ToString());
                            result.Remarks = row.Values[17].ToString();
                            result.PortalSONo = row.Values[18].ToString();
                            result.SONo = row.Values[19].ToString();
                            result.PickListNo = row.Values[20].ToString();
                            result.PackListNo = row.Values[21].ToString();
                            result.LoadingNo = row.Values[22].ToString();
                            result.PortalDONo = row.Values[23].ToString();
                            result.SAPDONo = row.Values[24].ToString();
                            result.SAPInvNo = row.Values[25].ToString();
                            result.CreateDate = DateTime.Parse(row.Values[26].ToString());
                            result.PriceChange = bool.Parse(row.Values[27].ToString());
                            result.ExceedPrice = bool.Parse(row.Values[28].ToString());
                            result.ExceedCreditControl = bool.Parse(row.Values[29].ToString());

                            currObject.Results.Add(result);
                        }
                    }
                }

                ObjectSpace.Refresh();
                View.Refresh();

                persistentObjectSpace.Session.DropIdentityMap();
                persistentObjectSpace.Dispose();
            }
        }
        // End ver 1.0.15
    }
}
