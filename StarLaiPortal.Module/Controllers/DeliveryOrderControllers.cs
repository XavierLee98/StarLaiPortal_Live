﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Delivery_Order;
using StarLaiPortal.Module.BusinessObjects.Pick_List;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;

// 2023-07-28 add print button and do not add count in preview ver 0.1
// 2023-12-04 add daily delivery summary ver 1.0.13

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DeliveryOrderControllers : ViewController
    {
        GeneralControllers genCon;
        public DeliveryOrderControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.

            // Start ver 1.0.15
            ChoiceActionItem NA = new ChoiceActionItem("NA", "Print Option", null);
            ChoiceActionItem ViewDO = new ChoiceActionItem("ViewDO", "View DO", null);
            ChoiceActionItem PrintDO = new ChoiceActionItem("PrintDO", "Print DO", null);
            ChoiceActionItem Invoice = new ChoiceActionItem("Invoice", "Invoice", null);
            ChoiceActionItem BundleDO = new ChoiceActionItem("BundleDO", "Bundle DO", null);
            ChoiceActionItem DMBundle = new ChoiceActionItem("DMBundle", "DM Bundle", null);

            ChoicePrintDelivery.Items.Add(NA);
            ChoicePrintDelivery.Items.Add(ViewDO);
            ChoicePrintDelivery.Items.Add(PrintDO);
            ChoicePrintDelivery.Items.Add(Invoice);
            ChoicePrintDelivery.Items.Add(BundleDO);
            ChoicePrintDelivery.Items.Add(DMBundle);
            // End ver 1.0.15
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.DOCopyFromLoading.Active.SetItemValue("Enabled", false);
            this.SubmitDO.Active.SetItemValue("Enabled", false);
            this.CancelDO.Active.SetItemValue("Enabled", false);
            this.PreviewDO.Active.SetItemValue("Enabled", false);
            this.PreviewInv.Active.SetItemValue("Enabled", false);
            this.PreviewBundleDO.Active.SetItemValue("Enabled", false);
            // Start ver 0.1
            this.PrintDO.Active.SetItemValue("Enabled", false);
            // End ver 0.1
            this.PrintDMBundleDO.Active.SetItemValue("Enabled", false);
            // Start ver 1.0.13
            this.PrintDailyDeliveryS.Active.SetItemValue("Enabled", false);
            // End ver 1.0.13
            // Start ver 1.0.15
            this.ChoicePrintDelivery.Active.SetItemValue("Enabled", false);
            // End ver 1.0.15

            // Start ver 1.0.15
            if (typeof(DeliveryOrder).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(DeliveryOrder))
                {
                    if (View.Id == "DeliveryOrder_ListView_ByDate")
                    {
                        this.ChoicePrintDelivery.Active.SetItemValue("Enabled", true);
                        ChoicePrintDelivery.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        ChoicePrintDelivery.CustomizeControl += action_CustomizeControl;

                        ChoicePrintDelivery.SelectedIndex = 0;
                    }
                }
            }
            // End ver 1.0.15
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "DeliveryOrder_DetailView")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitDO.Active.SetItemValue("Enabled", true);
                    this.CancelDO.Active.SetItemValue("Enabled", true);
                    this.PreviewDO.Active.SetItemValue("Enabled", true);
                    this.PreviewInv.Active.SetItemValue("Enabled", true);
                    this.PreviewBundleDO.Active.SetItemValue("Enabled", true);
                    // Start ver 0.1
                    this.PrintDO.Active.SetItemValue("Enabled", true);
                    // End ver 0.1
                    this.PrintDMBundleDO.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitDO.Active.SetItemValue("Enabled", false);
                    this.CancelDO.Active.SetItemValue("Enabled", false);
                    this.PreviewDO.Active.SetItemValue("Enabled", false);
                    this.PreviewInv.Active.SetItemValue("Enabled", false);
                    this.PreviewBundleDO.Active.SetItemValue("Enabled", false);
                    // Start ver 0.1
                    this.PrintDO.Active.SetItemValue("Enabled", false);
                    // End ver 0.1
                    this.PrintDMBundleDO.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    //this.DOCopyFromLoading.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.DOCopyFromLoading.Active.SetItemValue("Enabled", false);
                }
            }
            else if (View.Id == "DeliveryOrder_ListView")
            {
                this.PreviewDO.Active.SetItemValue("Enabled", true);
                this.PreviewInv.Active.SetItemValue("Enabled", true);
                this.PreviewBundleDO.Active.SetItemValue("Enabled", true);
                // Start ver 0.1
                this.PrintDO.Active.SetItemValue("Enabled", true);
                // End ver 0.1
                this.PrintDMBundleDO.Active.SetItemValue("Enabled", true);
            }
            // Start ver 1.0.13
            else if (View.Id == "DailyDeliveryOrders_DetailView")
            {
                this.PrintDailyDeliveryS.Active.SetItemValue("Enabled", true);
            }
            // End ver 1.0.13
            else
            {
                this.DOCopyFromLoading.Active.SetItemValue("Enabled", false);
                this.SubmitDO.Active.SetItemValue("Enabled", false);
                this.CancelDO.Active.SetItemValue("Enabled", false);
                this.PreviewDO.Active.SetItemValue("Enabled", false);
                this.PreviewInv.Active.SetItemValue("Enabled", false);
                this.PreviewBundleDO.Active.SetItemValue("Enabled", false);
                // Start ver 0.1
                this.PrintDO.Active.SetItemValue("Enabled", false);
                // End ver 0.1
                this.PrintDMBundleDO.Active.SetItemValue("Enabled", false);
                // Start ver 1.0.13
                this.PrintDailyDeliveryS.Active.SetItemValue("Enabled", false);
                // End ver 1.0.13
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        // Start ver 1.0.15
        void action_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            SingleChoiceActionAsModeMenuActionItem actionItem = e.Control as SingleChoiceActionAsModeMenuActionItem;
            if (actionItem != null && actionItem.Action.PaintStyle == DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption)
            {
                DropDownSingleChoiceActionControlBase control = (DropDownSingleChoiceActionControlBase)actionItem.Control;
                //control.Label.Text = actionItem.Action.Caption;
                //control.Label.Style["padding-right"] = "5px";
                control.ComboBox.Width = 120;
            }
        }
        // End ver 1.0.15

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

        private void DOCopyFromLoading_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count == 1)
            {
                try
                {
                    DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;

                    foreach (vwLoadingSO dtl in e.PopupWindowViewSelectedObjects)
                    {
                        foreach(PickListDetails dtlpick in dtl.PickListDocNum.PickListDetails)
                        {
                            SalesOrder so = ObjectSpace.FindObject<SalesOrder>(CriteriaOperator.Parse("DocNum = ?", dtl.SODocNum));

                            if (so != null)
                            {
                                foreach(SalesOrderDetails dtlsales in so.SalesOrderDetails)
                                {
                                    if (dtlsales.ItemCode.ItemCode == dtlpick.ItemCode.ItemCode && dtlpick.SOBaseDoc == so.DocNum)
                                    {
                                        DeliveryOrderDetails newdeliveryitem = ObjectSpace.CreateObject<DeliveryOrderDetails>();

                                        newdeliveryitem.ItemCode = newdeliveryitem.Session.GetObjectByKey<vwItemMasters>(dtlsales.ItemCode.ItemCode);
                                        if (dtlsales.Location != null)
                                        {
                                            newdeliveryitem.Warehouse = newdeliveryitem.Session.GetObjectByKey<vwWarehouse>(dtlsales.Location.WarehouseCode);
                                        }
                                        newdeliveryitem.Quantity = dtlpick.PickQty;
                                        newdeliveryitem.Price = dtlsales.Price;
                                        newdeliveryitem.BaseDoc = dtl.LoadingDocNum.ToString();
                                        newdeliveryitem.SODocNum = dtl.SODocNum;
                                        newdeliveryitem.PickListDocNum = dtl.PickListDocNum.DocNum;

                                        delivery.DeliveryOrderDetails.Add(newdeliveryitem);
                                    }
                                }
                            }
                        }

                        showMsg("Success", "Copy Success.", InformationType.Success);
                    }
                    //}
                }
                catch (Exception)
                {
                    showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
            else if (e.PopupWindowViewSelectedObjects.Count > 1)
            {
                showMsg("Error", "One DO only allow one SO.", InformationType.Error);
            }
        }

        private void DOCopyFromLoading_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vwLoadingSO));
            var cs = Application.CreateCollectionSource(os, typeof(vwLoadingSO), viewId);
            if (delivery.Customer != null)
            {
                cs.Criteria["Customer"] = new BinaryOperator("Customer", delivery.Customer.BPCode);
            }
            else
            {
                cs.Criteria["Customer"] = new BinaryOperator("Customer", "");
            }

            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void SubmitDO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DeliveryOrder selectedObject = (DeliveryOrder)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Submitted;
            DeliveryOrderDocTrail ds = ObjectSpace.CreateObject<DeliveryOrderDocTrail>();
            ds.DocStatus = DocStatus.Submitted;
            ds.DocRemarks = p.ParamString;
            selectedObject.DeliveryOrderDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Submit Done.", InformationType.Success);
        }

        private void SubmitDO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelDO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DeliveryOrder selectedObject = (DeliveryOrder)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            DeliveryOrderDocTrail ds = ObjectSpace.CreateObject<DeliveryOrderDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.DeliveryOrderDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelDO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void PreviewDO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\DeliveryOrder.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", delivery.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                    // Start ver 0.1
                    //delivery.DOPrintCount = delivery.DOPrintCount + 1;
                    //delivery.DOPrintDate = DateTime.Now;

                    //ObjectSpace.CommitChanges();
                    //ObjectSpace.Refresh();
                    // End ver 0.1
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select one DO only.", InformationType.Error);
            }
        }

        private void PreviewInv_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                if (delivery.SAPDocNum != null)
                {
                    try
                    {
                        ReportDocument doc = new ReportDocument();
                        strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\Invoice.rpt"));
                        strDatabase = conn.Database;
                        strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                        strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.Refresh();

                        doc.SetParameterValue("dockey@", delivery.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);

                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_Inv_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_Inv_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                        IObjectSpace os = Application.CreateObjectSpace();
                        DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                        trx.INVPrintCount = trx.INVPrintCount + 1;
                        trx.INVPrintDate = DateTime.Now;

                        os.CommitChanges();
                        os.Refresh();
                    }
                    catch (Exception ex)
                    {
                        showMsg("Fail", ex.Message, InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Invoice not found." , InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select one DO only.", InformationType.Error);
            }
        }

        private void PreviewBundleDO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\BundleDO.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", delivery.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);
                    doc.SetParameterValue("@printedBy", user.Staff.StaffName);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_BundleDO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_BundleDO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                    IObjectSpace os = Application.CreateObjectSpace();
                    DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                    trx.BundleDOPrintCount = trx.BundleDOPrintCount + 1;
                    trx.BundleDOPrintDate = DateTime.Now;

                    os.CommitChanges();
                    os.Refresh();
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select one DO only.", InformationType.Error);
            }
        }

        // Start ver 0.1
        private void PrintDO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\DeliveryOrder.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", delivery.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                    IObjectSpace os = Application.CreateObjectSpace();
                    DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                    trx.DOPrintCount = trx.DOPrintCount + 1;
                    trx.DOPrintDate = DateTime.Now;

                    os.CommitChanges();
                    os.Refresh();
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select one DO only.", InformationType.Error);
            }
        }
        // End ver 0.1

        private void PrintDMBundleDO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {
                string strServer;
                string strDatabase;
                string strUserID;
                string strPwd;
                string filename;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\BundleDOHalfLetter.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("dockey@", delivery.Oid);
                    doc.SetParameterValue("dbName@", conn.Database);
                    doc.SetParameterValue("@printedBy", user.Staff.StaffName);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_DMBundleDO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + delivery.Oid + "_" + user.UserName + "_DMBundleDO_"
                        + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                    IObjectSpace os = Application.CreateObjectSpace();
                    DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                    trx.BundleDOPrintCount = trx.BundleDOPrintCount + 1;
                    trx.BundleDOPrintDate = DateTime.Now;

                    os.CommitChanges();
                    os.Refresh();
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select one DO only.", InformationType.Error);
            }
        }

        // Start ver 1.0.13
        private void PrintDailyDeliveryS_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            DailyDeliveryOrders deliverysummary = (DailyDeliveryOrders)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            if (deliverysummary.CardCode != null)
            {
                try
                {
                    ReportDocument doc = new ReportDocument();
                    strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                    doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\DailyDeliverySummary.rpt"));
                    strDatabase = conn.Database;
                    strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                    strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                    doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    doc.Refresh();

                    doc.SetParameterValue("DateFr", deliverysummary.DateFr.Date);
                    doc.SetParameterValue("DateTo", deliverysummary.DateTo.Date);
                    doc.SetParameterValue("CardCode", deliverysummary.CardCode.BPCode);

                    filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                        + "_" + deliverysummary.Oid + "_" + user.UserName + "_DeliverySum_"
                        + DateTime.Parse(deliverysummary.DateFr.ToString()).ToString("yyyyMMdd") + ".pdf";

                    doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                    doc.Close();
                    doc.Dispose();

                    string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                        + "_" + deliverysummary.Oid + "_" + user.UserName + "_DeliverySum_"
                        + DateTime.Parse(deliverysummary.DateFr.ToString()).ToString("yyyyMMdd") + ".pdf";
                    var script = "window.open('" + url + "');";

                    WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
                }
                catch (Exception ex)
                {
                    showMsg("Fail", ex.Message, InformationType.Error);
                }
            }
            else
            {
                showMsg("Fail", "Please select cardcode.", InformationType.Error);
            }
        }
        // End ver 1.0.13

        // Start ver 1.0.15
        private void ChoicePrintDelivery_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (e.SelectedChoiceActionItem.Id == "ViewDO")
            {
                if (e.SelectedObjects.Count == 1)
                {
                    string strServer;
                    string strDatabase;
                    string strUserID;
                    string strPwd;
                    string filename;

                    SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                    DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                    ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                    try
                    {
                        ReportDocument doc = new ReportDocument();
                        strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\DeliveryOrder.rpt"));
                        strDatabase = conn.Database;
                        strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                        strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.Refresh();

                        doc.SetParameterValue("dockey@", delivery.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);

                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                        // Start ver 0.1
                        //delivery.DOPrintCount = delivery.DOPrintCount + 1;
                        //delivery.DOPrintDate = DateTime.Now;

                        //ObjectSpace.CommitChanges();
                        //ObjectSpace.Refresh();
                        // End ver 0.1
                    }
                    catch (Exception ex)
                    {
                        showMsg("Fail", ex.Message, InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Please select one DO only.", InformationType.Error);
                }
            }

            if (e.SelectedChoiceActionItem.Id == "PrintDO")
            {
                if (e.SelectedObjects.Count == 1)
                {
                    string strServer;
                    string strDatabase;
                    string strUserID;
                    string strPwd;
                    string filename;

                    SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                    DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                    ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                    try
                    {
                        ReportDocument doc = new ReportDocument();
                        strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\DeliveryOrder.rpt"));
                        strDatabase = conn.Database;
                        strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                        strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.Refresh();

                        doc.SetParameterValue("dockey@", delivery.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);

                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_DO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                        IObjectSpace os = Application.CreateObjectSpace();
                        DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                        trx.DOPrintCount = trx.DOPrintCount + 1;
                        trx.DOPrintDate = DateTime.Now;

                        os.CommitChanges();
                        os.Refresh();
                    }
                    catch (Exception ex)
                    {
                        showMsg("Fail", ex.Message, InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Please select one DO only.", InformationType.Error);
                }
            }

            if (e.SelectedChoiceActionItem.Id == "Invoice")
            {
                if (e.SelectedObjects.Count == 1)
                {
                    string strServer;
                    string strDatabase;
                    string strUserID;
                    string strPwd;
                    string filename;

                    SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                    DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                    ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                    if (delivery.SAPDocNum != null)
                    {
                        try
                        {
                            ReportDocument doc = new ReportDocument();
                            strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                            doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\Invoice.rpt"));
                            strDatabase = conn.Database;
                            strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                            strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                            doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                            doc.Refresh();

                            doc.SetParameterValue("dockey@", delivery.Oid);
                            doc.SetParameterValue("dbName@", conn.Database);

                            filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                                + "_" + delivery.Oid + "_" + user.UserName + "_Inv_"
                                + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                            doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                            doc.Close();
                            doc.Dispose();

                            string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                                ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                                + "_" + delivery.Oid + "_" + user.UserName + "_Inv_"
                                + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                            var script = "window.open('" + url + "');";

                            WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                            IObjectSpace os = Application.CreateObjectSpace();
                            DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                            trx.INVPrintCount = trx.INVPrintCount + 1;
                            trx.INVPrintDate = DateTime.Now;

                            os.CommitChanges();
                            os.Refresh();
                        }
                        catch (Exception ex)
                        {
                            showMsg("Fail", ex.Message, InformationType.Error);
                        }
                    }
                    else
                    {
                        showMsg("Fail", "Invoice not found.", InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Please select one DO only.", InformationType.Error);
                }
            }

            if (e.SelectedChoiceActionItem.Id == "BundleDO")
            {
                if (e.SelectedObjects.Count == 1)
                {
                    string strServer;
                    string strDatabase;
                    string strUserID;
                    string strPwd;
                    string filename;

                    SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                    DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                    ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                    try
                    {
                        ReportDocument doc = new ReportDocument();
                        strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\BundleDO.rpt"));
                        strDatabase = conn.Database;
                        strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                        strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.Refresh();

                        doc.SetParameterValue("dockey@", delivery.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);
                        doc.SetParameterValue("@printedBy", user.Staff.StaffName);

                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_BundleDO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_BundleDO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                        IObjectSpace os = Application.CreateObjectSpace();
                        DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                        trx.BundleDOPrintCount = trx.BundleDOPrintCount + 1;
                        trx.BundleDOPrintDate = DateTime.Now;

                        os.CommitChanges();
                        os.Refresh();
                    }
                    catch (Exception ex)
                    {
                        showMsg("Fail", ex.Message, InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Please select one DO only.", InformationType.Error);
                }
            }

            if (e.SelectedChoiceActionItem.Id == "DMBundle")
            {
                if (e.SelectedObjects.Count == 1)
                {
                    string strServer;
                    string strDatabase;
                    string strUserID;
                    string strPwd;
                    string filename;

                    SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                    DeliveryOrder delivery = (DeliveryOrder)View.CurrentObject;
                    ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

                    try
                    {
                        ReportDocument doc = new ReportDocument();
                        strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                        doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\BundleDOHalfLetter.rpt"));
                        strDatabase = conn.Database;
                        strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                        strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                        doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                        doc.Refresh();

                        doc.SetParameterValue("dockey@", delivery.Oid);
                        doc.SetParameterValue("dbName@", conn.Database);
                        doc.SetParameterValue("@printedBy", user.Staff.StaffName);

                        filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_DMBundleDO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                        doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                        doc.Close();
                        doc.Dispose();

                        string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                            ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                            + "_" + delivery.Oid + "_" + user.UserName + "_DMBundleDO_"
                            + DateTime.Parse(delivery.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                        var script = "window.open('" + url + "');";

                        WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                        IObjectSpace os = Application.CreateObjectSpace();
                        DeliveryOrder trx = os.FindObject<DeliveryOrder>(new BinaryOperator("Oid", delivery.Oid));

                        trx.BundleDOPrintCount = trx.BundleDOPrintCount + 1;
                        trx.BundleDOPrintDate = DateTime.Now;

                        os.CommitChanges();
                        os.Refresh();
                    }
                    catch (Exception ex)
                    {
                        showMsg("Fail", ex.Message, InformationType.Error);
                    }
                }
                else
                {
                    showMsg("Fail", "Please select one DO only.", InformationType.Error);
                }
            }
        }
        // End ver 1.0.15
    }
}
