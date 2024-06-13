﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.XtraGrid.Columns;
using StarLaiPortal.Module.BusinessObjects.Advanced_Shipment_Notice;
using StarLaiPortal.Module.BusinessObjects.Credit_Notes_Cancellation;
using StarLaiPortal.Module.BusinessObjects.Dashboard;
using StarLaiPortal.Module.BusinessObjects.Delivery_Order;
using StarLaiPortal.Module.BusinessObjects.GRN;
using StarLaiPortal.Module.BusinessObjects.Inquiry_View;
using StarLaiPortal.Module.BusinessObjects.Item_Inquiry;
using StarLaiPortal.Module.BusinessObjects.Load;
using StarLaiPortal.Module.BusinessObjects.Pack_List;
using StarLaiPortal.Module.BusinessObjects.Pick_List;
using StarLaiPortal.Module.BusinessObjects.Purchase_Order;
using StarLaiPortal.Module.BusinessObjects.Purchase_Return;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using StarLaiPortal.Module.BusinessObjects.Sales_Order_Collection;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using StarLaiPortal.Module.BusinessObjects.Sales_Refund;
using StarLaiPortal.Module.BusinessObjects.Sales_Return;
using StarLaiPortal.Module.BusinessObjects.Stock_Adjustment;
using StarLaiPortal.Module.BusinessObjects.Stock_Count;
using StarLaiPortal.Module.BusinessObjects.Warehouse_Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

// 2023-07-28 add AR Downpayment cancalletion ver 1.0.7
// 2023-09-11 add dashboard sales/purchase/warehouse ver 1.0.9
// 2023-09-19 add disable detail view ver 1.0.9
// 2023-10-20 add stock count ver 1.0.11

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ListViewControllers : ViewController<ListView>
    {
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        ListViewController listViewController;
        public ListViewControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View.ObjectTypeInfo.Type == typeof(Dashboards))
            {
                if (View.Id == "Dashboards_ListView")
                {
                    processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem +=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            // Start ver 1.0.9
            if (View.ObjectTypeInfo.Type == typeof(DashboardsSales))
            {
                if (View.Id == "DashboardsSales_ListView")
                {
                    processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem +=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(DashboardsPurchase))
            {
                if (View.Id == "DashboardsPurchase_ListView")
                {
                    processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem +=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(DashboardsWarehouse))
            {
                if (View.Id == "DashboardsWarehouse_ListView")
                {
                    processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem +=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(vwInquirySalesOrder))
            {
                if (View.Id == "vwInquirySalesOrder_ListView")
                {
                    processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem +=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(vwInquiryPickList))
            {
                if (View.Id == "vwInquiryPickList_ListView")
                {
                    processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem +=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }
            // End ver 1.0.9

            if (View.ObjectTypeInfo.Type == typeof(SalesQuotation) || View.ObjectTypeInfo.Type == typeof(SalesOrder) ||
                View.ObjectTypeInfo.Type == typeof(SalesOrderCollection) || View.ObjectTypeInfo.Type == typeof(PickList) ||
                 View.ObjectTypeInfo.Type == typeof(PackList) ||
                 View.ObjectTypeInfo.Type == typeof(Load) || View.ObjectTypeInfo.Type == typeof(PurchaseOrders) ||
                 View.ObjectTypeInfo.Type == typeof(ASN) || View.ObjectTypeInfo.Type == typeof(GRN) ||
                 View.ObjectTypeInfo.Type == typeof(PurchaseReturnRequests) || View.ObjectTypeInfo.Type == typeof(SalesReturnRequests) ||
                 View.ObjectTypeInfo.Type == typeof(SalesReturns) || View.ObjectTypeInfo.Type == typeof(PurchaseReturns) ||
                 View.ObjectTypeInfo.Type == typeof(WarehouseTransferReq) || View.ObjectTypeInfo.Type == typeof(WarehouseTransfers) ||
                 View.ObjectTypeInfo.Type == typeof(StockAdjustmentRequests) || View.ObjectTypeInfo.Type == typeof(StockAdjustments) ||
                 View.ObjectTypeInfo.Type == typeof(SalesRefundRequests) || View.ObjectTypeInfo.Type == typeof(SalesRefunds) ||
                 View.ObjectTypeInfo.Type == typeof(DeliveryOrder) ||
                 // Start ver 1.0.7
                 View.ObjectTypeInfo.Type == typeof(ARDownpaymentCancel) ||
                 // End ver 1.0.7
                 // Start ver 1.0.12
                 View.ObjectTypeInfo.Type == typeof(StockCountSheet) ||
                 View.ObjectTypeInfo.Type == typeof(StockCountConfirm)
                 // Start ver 1.0.12
                 )
            {
                listViewController = Frame.GetController<ListViewController>();
                if (listViewController != null)
                {
                    listViewController.EditAction.Active["123"] = false;
                }
            }

            //// Start ver 1.0.15
            //if (DateTime.Now.Minute.ToString("00").Substring(1, 1) == "0" || 
            //    DateTime.Now.Minute.ToString("00").Substring(1, 1) == "3" ||
            //    DateTime.Now.Minute.ToString("00").Substring(1, 1) == "6")
            //{
            //    MemoryManagement.FlushGCMemory();
            //}
            //// End ver 1.0.15
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null)
            {
                ASPxGridView gridView = gridListEditor.Grid;
                //gridView.Settings.UseFixedTableLayout = true;
                //gridView.Width = Unit.Percentage(128);
                //gridView.SettingsResizing.ColumnResizeMode = ColumnResizeMode.Control;
                gridListEditor.Grid.Styles.Cell.Wrap = DevExpress.Utils.DefaultBoolean.False;
            }
        }
        protected override void OnDeactivated()
        {
            if (View.ObjectTypeInfo.Type == typeof(Dashboards))
            {
                if (View.Id == "Dashboards_ListView")
                {
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem -=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            // Start ver 1.0.9
            if (View.ObjectTypeInfo.Type == typeof(DashboardsSales))
            {
                if (View.Id == "DashboardsSales_ListView")
                {
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem -=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(DashboardsPurchase))
            {
                if (View.Id == "DashboardsPurchase_ListView")
                {
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem -=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(DashboardsWarehouse))
            {
                if (View.Id == "DashboardsWarehouse_ListView")
                {
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem -=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(vwInquirySalesOrder))
            {
                if (View.Id == "vwInquirySalesOrder_ListView")
                {
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem -=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(vwInquiryPickList))
            {
                if (View.Id == "vwInquiryPickList_ListView")
                {
                    if (processCurrentObjectController != null)
                    {
                        processCurrentObjectController.CustomProcessSelectedItem -=
                            processCurrentObjectController_CustomProcessSelectedItem;
                    }
                }
            }
            // End ver 1.0.9

            if (View.ObjectTypeInfo.Type == typeof(SalesQuotation) || View.ObjectTypeInfo.Type == typeof(SalesOrder) ||
                View.ObjectTypeInfo.Type == typeof(SalesOrderCollection) || View.ObjectTypeInfo.Type == typeof(PickList) ||
                View.ObjectTypeInfo.Type == typeof(PackList) ||
                 View.ObjectTypeInfo.Type == typeof(Load) || View.ObjectTypeInfo.Type == typeof(PurchaseOrders) ||
                 View.ObjectTypeInfo.Type == typeof(ASN) || View.ObjectTypeInfo.Type == typeof(GRN) ||
                 View.ObjectTypeInfo.Type == typeof(PurchaseReturnRequests) || View.ObjectTypeInfo.Type == typeof(SalesReturnRequests) ||
                 View.ObjectTypeInfo.Type == typeof(SalesReturns) || View.ObjectTypeInfo.Type == typeof(PurchaseReturns) ||
                 View.ObjectTypeInfo.Type == typeof(WarehouseTransferReq) || View.ObjectTypeInfo.Type == typeof(WarehouseTransfers) ||
                 View.ObjectTypeInfo.Type == typeof(StockAdjustmentRequests) || View.ObjectTypeInfo.Type == typeof(StockAdjustments) ||
                 View.ObjectTypeInfo.Type == typeof(SalesRefundRequests) || View.ObjectTypeInfo.Type == typeof(SalesRefunds) ||
                 View.ObjectTypeInfo.Type == typeof(DeliveryOrder) ||
                 // Start ver 1.0.7
                 View.ObjectTypeInfo.Type == typeof(ARDownpaymentCancel) ||
                 // End ver 1.0.7
                 // Start ver 1.0.12
                 View.ObjectTypeInfo.Type == typeof(StockCountSheet) ||
                 View.ObjectTypeInfo.Type == typeof(StockCountConfirm)
                 // Start ver 1.0.12
                 )
            {
                if (listViewController != null)
                {
                    listViewController.EditAction.Active.RemoveItem("123");
                    listViewController = null;
                }
            }

            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void processCurrentObjectController_CustomProcessSelectedItem(
            object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;
        }
    }
}
