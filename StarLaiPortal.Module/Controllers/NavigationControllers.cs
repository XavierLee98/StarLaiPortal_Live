﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Delivery_Order;
using StarLaiPortal.Module.BusinessObjects.Item_Inquiry;
using StarLaiPortal.Module.BusinessObjects.Print_Module;
using StarLaiPortal.Module.BusinessObjects.Reports;
using StarLaiPortal.Module.BusinessObjects.Sales_Order_Collection;
using StarLaiPortal.Module.BusinessObjects.Setup;
using StarLaiPortal.Module.BusinessObjects.Stock_Count_Inquiry;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 2023-10-23 add stock count ver 1.0.12
// 2023-12-04 add daily delivery summary ver 1.0.13

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class NavigationControllers : WindowController
    {
        private ShowNavigationItemController showNavigationItemController;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public NavigationControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            showNavigationItemController.CustomShowNavigationItem += showNavigationItemController_CustomShowNavigationItem;
        }
        //protected override void OnViewControlsCreated()
        //{
        //    base.OnViewControlsCreated();
        //    // Access and customize the target View control.
        //}
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            showNavigationItemController.CustomShowNavigationItem -= showNavigationItemController_CustomShowNavigationItem;
        }

        void showNavigationItemController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Id == "ItemInquiry_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(ItemInquiry));
                ItemInquiry iteminquiry = objectSpace.FindObject<ItemInquiry>(new BinaryOperator("Oid", 1));

                DocTypes number = objectSpace.FindObject<DocTypes>(new BinaryOperator("BoCode", DocTypeList.SO));

                iteminquiry.Search = null;
                iteminquiry.Cart = number.NextDocNum.ToString();
                iteminquiry.PriceList1 = null;
                iteminquiry.PriceList2 = null;
                iteminquiry.Stock1 = null;
                iteminquiry.Stock2 = null;

                for (int i = 0; iteminquiry.ItemInquiryDetails.Count > i;)
                {
                    iteminquiry.ItemInquiryDetails.Remove(iteminquiry.ItemInquiryDetails[i]);
                }

                DetailView detailView = Application.CreateDetailView(objectSpace, iteminquiry);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;

                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "PrintLabel_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                PrintLabel newprintlabel = objectSpace.CreateObject<PrintLabel>();

                DetailView detailView = Application.CreateDetailView(objectSpace, newprintlabel);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;

                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "SalesOrderCollectionReport_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                SalesOrderCollectionReport newsalescollectionreport = objectSpace.CreateObject<SalesOrderCollectionReport>();

                DetailView detailView = Application.CreateDetailView(objectSpace, newsalescollectionreport);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;

                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "StockReorderingReport_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                StockReorderingReport newstockreorder = objectSpace.CreateObject<StockReorderingReport>();

                DetailView detailView = Application.CreateDetailView(objectSpace, newstockreorder);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
                e.ActionArguments.ShowViewParameters.CreatedView = detailView;

                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "ItemInquiry_ListView_Report")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                ItemInquiry newiteminquiry = objectSpace.CreateObject<ItemInquiry>();

                DetailView detailView = Application.CreateDetailView(objectSpace, "ItemInquiry_DetailView_Report", true, newiteminquiry);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

                ItemInquiryDefault defaultdata = objectSpace.FindObject<ItemInquiryDefault>(CriteriaOperator.Parse("DocType = ? and IsActive= ?",
                    DocTypeList.Reports, "True"));

                if (defaultdata != null)
                {
                    if (defaultdata.PriceList1 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).PriceList1 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwPriceList>
                            (defaultdata.PriceList1.ListNum);
                    }
                    if (defaultdata.PriceList2 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).PriceList2 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwPriceList>
                            (defaultdata.PriceList2.ListNum);
                    }
                    if (defaultdata.PriceList3 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).PriceList3 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwPriceList>
                            (defaultdata.PriceList3.ListNum);
                    }
                    if (defaultdata.PriceList4 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).PriceList4 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwPriceList>
                            (defaultdata.PriceList4.ListNum);
                    }
                    if (defaultdata.Stock1 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).Stock1 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                            (defaultdata.Stock1.WarehouseCode);
                    }
                    if (defaultdata.Stock2 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).Stock2 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                            (defaultdata.Stock2.WarehouseCode);
                    }
                    if (defaultdata.Stock3 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).Stock3 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                            (defaultdata.Stock3.WarehouseCode);
                    }
                    if (defaultdata.Stock4 != null)
                    {
                        ((ItemInquiry)detailView.CurrentObject).Stock4 = ((ItemInquiry)detailView.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                            (defaultdata.Stock4.WarehouseCode);
                    }
                }

                objectSpace.CommitChanges();
                objectSpace.Refresh();

                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            // Start ver 1.0.12
            if (e.ActionArguments.SelectedChoiceActionItem.Id == "StockCountBinInquiry_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                StockCountBinInquiry newstockcountbin = objectSpace.CreateObject<StockCountBinInquiry>();

                DetailView detailView = Application.CreateDetailView(objectSpace, "StockCountBinInquiry_DetailView", true, newstockcountbin);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

                newstockcountbin.FromDate = DateTime.Today;
                newstockcountbin.ToDate = DateTime.Today;
                newstockcountbin.Round = 1;

                objectSpace.CommitChanges();
                objectSpace.Refresh();

                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "StockCountItemInquiry_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                StockCountItemInquiry newstockcountitem = objectSpace.CreateObject<StockCountItemInquiry>();

                DetailView detailView = Application.CreateDetailView(objectSpace, "StockCountItemInquiry_DetailView", true, newstockcountitem);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

                newstockcountitem.FromDate = DateTime.Today;
                newstockcountitem.ToDate = DateTime.Today;
                newstockcountitem.Round = 1;

                objectSpace.CommitChanges();
                objectSpace.Refresh();

                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }

            if (e.ActionArguments.SelectedChoiceActionItem.Id == "StockCountVarianceInquiry_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                StockCountVarianceInquiry newstockcountvariance = objectSpace.CreateObject<StockCountVarianceInquiry>();

                DetailView detailView = Application.CreateDetailView(objectSpace, "StockCountVarianceInquiry_DetailView", true, newstockcountvariance);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

                newstockcountvariance.StockCountDate = DateTime.Today;

                objectSpace.CommitChanges();
                objectSpace.Refresh();

                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }
            // End ver 1.0.12

            // Start ver 1.0.13
            if (e.ActionArguments.SelectedChoiceActionItem.Id == "DailyDeliveryOrders_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                DailyDeliveryOrders newdaily = objectSpace.CreateObject<DailyDeliveryOrders>();

                DetailView detailView = Application.CreateDetailView(objectSpace, "DailyDeliveryOrders_DetailView", true, newdaily);
                detailView.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

                newdaily.DateFr = DateTime.Today;
                newdaily.DateTo = DateTime.Today;

                objectSpace.CommitChanges();
                objectSpace.Refresh();

                e.ActionArguments.ShowViewParameters.CreatedView = detailView;
                e.Handled = true;
            }
            // End ver 1.0.13
        }
    }
}
