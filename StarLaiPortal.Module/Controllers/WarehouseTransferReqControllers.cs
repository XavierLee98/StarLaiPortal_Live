﻿using Admiral.ImportData;
using CrystalDecisions.CrystalReports.Engine;
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
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraPrinting.Export.Pdf;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Item_Inquiry;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using StarLaiPortal.Module.BusinessObjects.Setup;
using StarLaiPortal.Module.BusinessObjects.View;
using StarLaiPortal.Module.BusinessObjects.Warehouse_Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

// 2023-08-16 - add stock 3 and stock 4 - ver 1.0.8
// 2023-08-25 - export and import function - ver 1.0.9
// 2023-09-12 - add warehouse transfer req no ver 1.0.9
// 2023-09-25 - add stock balance checking - ver 1.0.10
// 2024-03-15 - do not check stock balance if same warehouse - ver 1.0.14
// 2024-04-04 - Update available qty ver 1.0.15
// 2024-06-11 - fixed disable edit and delete button ver 1.0.17

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class WarehouseTransferReqControllers : ViewController
    {
        GeneralControllers genCon;
        public WarehouseTransferReqControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.WTRInquiryItem.Active.SetItemValue("Enabled", false);
            this.SubmitWTR.Active.SetItemValue("Enabled", false);
            this.CancelWTR.Active.SetItemValue("Enabled", false);
            this.PreviewWTR.Active.SetItemValue("Enabled", false);
            this.ReviewAppWTR.Active.SetItemValue("Enabled", false);
            this.ApproveAppWTR.Active.SetItemValue("Enabled", false);
            this.RejectAppWTR.Active.SetItemValue("Enabled", false);
            this.WTRCopyToWT.Active.SetItemValue("Enabled", false);
            this.ApproveAppWTR_Pop.Active.SetItemValue("Enabled", false);
            // Start ver 1.0.9
            this.ExportWHReq.Active.SetItemValue("Enabled", false);
            this.ImportWHReq.Active.SetItemValue("Enabled", false);
            // End ver 1.0.9

            // Start ver 1.0.17
            if (View.ObjectTypeInfo.Type == typeof(WarehouseTransferReq))
            {
                if (View is DetailView)
                {
                    BusinessObjects.Warehouse_Transfer.WarehouseTransferReq warehousereq = View.CurrentObject as BusinessObjects.Warehouse_Transfer.WarehouseTransferReq;

                    foreach (WarehouseTransferReqDetails dtl in warehousereq.WarehouseTransferReqDetails)
                    {
                        if (dtl.FromWarehouse != null)
                        {
                            dtl.Available = genCon.GenerateInstock(ObjectSpace, dtl.ItemCode.ItemCode, dtl.FromWarehouse.WarehouseCode);
                        }
                        else
                        {
                            dtl.Available = 0;
                        }
                    }

                    if (warehousereq.IsNew == false)
                    {
                        try
                        {
                            ObjectSpace.CommitChanges();
                            ObjectSpace.Refresh();
                        }
                        catch { }
                    }
                }
            }
            // End ver 1.0.17
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "WarehouseTransferReq_DetailView")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitWTR.Active.SetItemValue("Enabled", true);
                    this.CancelWTR.Active.SetItemValue("Enabled", true);
                    this.PreviewWTR.Active.SetItemValue("Enabled", true);
                    this.WTRCopyToWT.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitWTR.Active.SetItemValue("Enabled", false);
                    this.CancelWTR.Active.SetItemValue("Enabled", false);
                    this.PreviewWTR.Active.SetItemValue("Enabled", false);
                    this.WTRCopyToWT.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    this.WTRInquiryItem.Active.SetItemValue("Enabled", true);
                    // Start ver 1.0.9
                    this.ExportWHReq.Active.SetItemValue("Enabled", true);
                    this.ImportWHReq.Active.SetItemValue("Enabled", true);
                    // End ver 1.0.9
                }
                else
                {
                    this.WTRInquiryItem.Active.SetItemValue("Enabled", false);
                    // Start ver 1.0.9
                    this.ExportWHReq.Active.SetItemValue("Enabled", false);
                    this.ImportWHReq.Active.SetItemValue("Enabled", false);
                    // End ver 1.0.9
                }

                // Start ver 1.0.15
                if (View.ObjectTypeInfo.Type == typeof(WarehouseTransferReq))
                {
                    // Start ver 1.0.17
                    //    if (View is DetailView)
                    //    {
                    //        BusinessObjects.Warehouse_Transfer.WarehouseTransferReq warehousereq = View.CurrentObject as BusinessObjects.Warehouse_Transfer.WarehouseTransferReq;

                    //        foreach (WarehouseTransferReqDetails dtl in warehousereq.WarehouseTransferReqDetails)
                    //        {
                    //            if (dtl.FromWarehouse != null)
                    //            {
                    //                dtl.Available = genCon.GenerateInstock(ObjectSpace, dtl.ItemCode.ItemCode, dtl.FromWarehouse.WarehouseCode);
                    //            }
                    //            else
                    //            {
                    //                dtl.Available = 0;
                    //            }
                    //        }

                    //        if (warehousereq.IsNew == false)
                    //        {
                    //            ObjectSpace.CommitChanges();
                    //            ObjectSpace.Refresh();
                    //        }
                    //    }
                    // End ver 1.0.17
                }
                // End ver 1.0.15
            }
            else if (View.Id == "WarehouseTransferReq_ListView_Approval")
            {
                //this.ReviewAppWTR.Active.SetItemValue("Enabled", true);
                //this.ReviewAppWTR.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                //this.ApproveAppWTR.Active.SetItemValue("Enabled", true);
                //this.ApproveAppWTR.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                //this.RejectAppWTR.Active.SetItemValue("Enabled", true);
                //this.RejectAppWTR.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
                this.ApproveAppWTR_Pop.Active.SetItemValue("Enabled", true);
            }
            else if (View.Id == "WarehouseTransferReq_DetailView_Approval")
            {
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.ApproveAppWTR_Pop.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.ApproveAppWTR_Pop.Active.SetItemValue("Enabled", false);
                }
            }
            else
            {
                this.WTRInquiryItem.Active.SetItemValue("Enabled", false);
                this.SubmitWTR.Active.SetItemValue("Enabled", false);
                this.CancelWTR.Active.SetItemValue("Enabled", false);
                this.PreviewWTR.Active.SetItemValue("Enabled", false);
                this.ReviewAppWTR.Active.SetItemValue("Enabled", false);
                this.ApproveAppWTR.Active.SetItemValue("Enabled", false);
                this.RejectAppWTR.Active.SetItemValue("Enabled", false);
                this.WTRCopyToWT.Active.SetItemValue("Enabled", false);
                this.ApproveAppWTR_Pop.Active.SetItemValue("Enabled", false);
                // Start ver 1.0.9
                this.ExportWHReq.Active.SetItemValue("Enabled", false);
                this.ImportWHReq.Active.SetItemValue("Enabled", false);
                // End ver 1.0.9
            }

            // Start ver 1.0.15
            if (View.Id == "WarehouseTransferReq_WarehouseTransferReqDetails_ListView")
            {
                ((ASPxGridListEditor)((ListView)View).Editor).Grid.RowUpdating += new DevExpress.Web.Data.ASPxDataUpdatingEventHandler(Grid_RowUpdating);
            }
            // End ver 1.0.15
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        // Start ver 1.0.15
        private void Grid_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            ASPxGridListEditor listEditor = ((ListView)View).Editor as ASPxGridListEditor;
            if (listEditor != null)
            {
                object currentObject = listEditor.Grid.GetRow(listEditor.Grid.EditingRowVisibleIndex);
                if (currentObject != null)
                {
                    object warehouse = currentObject.GetType().GetProperty("FromWarehouse").GetValue(currentObject);
                    object itemcode = currentObject.GetType().GetProperty("ItemCode").GetValue(currentObject);

                    if (warehouse != null)
                    {
                        currentObject.GetType().GetProperty("Available").SetValue(currentObject, genCon.GenerateInstock(ObjectSpace,
                            (itemcode as vwItemMasters).ItemCode, (warehouse as vwWarehouse).WarehouseCode));
                    }
                }
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

        private void WTRInquiryItem_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void WTRInquiryItem_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            WarehouseTransferReq trx = (WarehouseTransferReq)View.CurrentObject;

            if (trx.DocNum == null)
            {
                string docprefix = genCon.GetDocPrefix();
                trx.DocNum = genCon.GenerateDocNum(DocTypeList.WTR, ObjectSpace, TransferType.NA, 0, docprefix);
            }

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            WarehouseTransferReq wtr = os.FindObject<WarehouseTransferReq>(new BinaryOperator("Oid", trx.Oid));

            IObjectSpace inqos = Application.CreateObjectSpace();
            ItemInquiry addnew = inqos.CreateObject<ItemInquiry>();

            DetailView dv = Application.CreateDetailView(inqos, addnew, true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((ItemInquiry)dv.CurrentObject).Cart = wtr.DocNum;
            ((ItemInquiry)dv.CurrentObject).DocType = DocTypeList.WTR;
            if (trx.Supplier != null)
            {
                ((ItemInquiry)dv.CurrentObject).CardCode = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwBusniessPartner>
                    (trx.Supplier.BPCode);
            }

            ItemInquiryDefault defaultdata = inqos.FindObject<ItemInquiryDefault>(CriteriaOperator.Parse("DocType = ? and IsActive= ?",
                DocTypeList.WTR, "True"));

            if (defaultdata != null)
            {
                if (defaultdata.PriceList1 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).PriceList1 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwPriceList>
                        (defaultdata.PriceList1.ListNum);
                }
                if (defaultdata.PriceList2 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).PriceList2 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwPriceList>
                        (defaultdata.PriceList2.ListNum);
                }
                if (defaultdata.PriceList3 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).PriceList3 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwPriceList>
                        (defaultdata.PriceList3.ListNum);
                }
                if (defaultdata.PriceList4 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).PriceList4 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwPriceList>
                        (defaultdata.PriceList4.ListNum);
                }
                if (defaultdata.Stock1 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).Stock1 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                        (defaultdata.Stock1.WarehouseCode);
                }
                if (defaultdata.Stock2 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).Stock2 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                        (defaultdata.Stock2.WarehouseCode);
                }
                // Start ver 1.0.8
                if (defaultdata.Stock3 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).Stock3 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                        (defaultdata.Stock3.WarehouseCode);
                }
                if (defaultdata.Stock4 != null)
                {
                    ((ItemInquiry)dv.CurrentObject).Stock4 = ((ItemInquiry)dv.CurrentObject).Session.GetObjectByKey<vwWarehouse>
                        (defaultdata.Stock4.WarehouseCode);
                }
                // End ver 1.0.8
            }


            inqos.CommitChanges();
            inqos.Refresh();

            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            e.DialogController.AcceptAction.ActionMeaning = ActionMeaning.Unknown;
            e.Maximized = true;

            e.View = dv;
        }

        private void SubmitWTR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            WarehouseTransferReq selectedObject = (WarehouseTransferReq)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;
            SqlConnection conn = new SqlConnection(genCon.getConnectionString());

            // Start ver 1.0.10
            foreach(WarehouseTransferReqDetails dtl in selectedObject.WarehouseTransferReqDetails)
            {
                // Start ver 1.0.14
                if (selectedObject.FromWarehouse.WarehouseCode != selectedObject.ToWarehouse.WarehouseCode)
                {
                // End ver 1.0.14
                    vwStockBalance available = ObjectSpace.FindObject<vwStockBalance>(CriteriaOperator.Parse("ItemCode = ? and WhsCode = ?",
                        dtl.ItemCode, selectedObject.FromWarehouse.WarehouseCode));

                    if (available != null)
                    {
                        if (available.InStock < (double)dtl.Quantity)
                        {
                            showMsg("Error", "Insufficient onhand quantity.", InformationType.Error);
                            return;
                        }
                    }
                    else
                    {
                        showMsg("Error", "Insufficient onhand quantity.", InformationType.Error);
                        return;
                    }
                // Start ver 1.0.14
                }
                // End ver 1.0.14

                if (dtl.FromBin != null)
                {
                    if (dtl.FromBin.InStock < dtl.Quantity)
                    {
                        showMsg("Error", "Bin not enough stock.", InformationType.Error);
                        return;
                    }
                }
            }
            // End ver 1.0.10

            if (selectedObject.IsValid == true)
            {
                selectedObject.Status = DocStatus.Submitted;
                WarehouseTransferReqDocTrail ds = ObjectSpace.CreateObject<WarehouseTransferReqDocTrail>();
                ds.DocStatus = DocStatus.Submitted;
                ds.DocRemarks = p.ParamString;
                selectedObject.WarehouseTransferReqDocTrail.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                #region Get approval
                List<string> ToEmails = new List<string>();
                string emailbody = "";
                string emailsubject = "";
                string emailaddress = "";
                Guid emailuser;
                DateTime emailtime = DateTime.Now;

                string getapproval = "EXEC sp_GetApproval '" + selectedObject.CreateUser.Oid + "', '" + selectedObject.Oid + "', 'WarehouseTransferRequest'";
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();
                SqlCommand cmd = new SqlCommand(getapproval, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(1) != "")
                    {
                        emailbody = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                               reader.GetString(3) + System.Environment.NewLine + GeneralSettings.appurl + reader.GetString(2) +
                               System.Environment.NewLine + System.Environment.NewLine;

                        emailsubject = "Warehouse Transfer Request Approval";
                        emailaddress = reader.GetString(1);
                        emailuser = reader.GetGuid(0);

                        ToEmails.Add(emailaddress);
                    }
                }
                cmd.Dispose();
                conn.Close();

                if (ToEmails.Count > 0)
                {
                    if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                    {
                    }
                }

                #endregion

                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("Oid", selectedObject.Oid));
                openNewView(os, trx, ViewEditMode.View);
                showMsg("Successful", "Submit Done.", InformationType.Success);
            }
            else
            {
                showMsg("Error", "No Content.", InformationType.Error);
            }
        }

        private void SubmitWTR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelWTR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            WarehouseTransferReq selectedObject = (WarehouseTransferReq)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            WarehouseTransferReqDocTrail ds = ObjectSpace.CreateObject<WarehouseTransferReqDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.WarehouseTransferReqDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            WarehouseTransferReq trx = os.FindObject<WarehouseTransferReq>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelWTR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void PreviewWTR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            WarehouseTransferReq print = (WarehouseTransferReq)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\WarehouseTransferRequest.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("dockey@", print.Oid);
                doc.SetParameterValue("dbName@", conn.Database);

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + print.Oid + "_" + user.UserName + "_WTR_"
                    + DateTime.Parse(DateTime.Now.ToString()).ToString("yyyyMMdd") + ".pdf";

                doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + print.Oid + "_" + user.UserName + "_WTR_"
                    + DateTime.Parse(DateTime.Now.ToString()).ToString("yyyyMMdd") + ".pdf";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }
        }

        private void ReviewAppWTR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }

        private void ApproveAppWTR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            WarehouseTransferReq selectedObject = (WarehouseTransferReq)e.CurrentObject;

            selectedObject.AppStatus = ApprovalStatusType.Approved;

            WarehouseTransferReqDocTrail ds = ObjectSpace.CreateObject<WarehouseTransferReqDocTrail>();
            ds.DocStatus = DocStatus.Submitted;
            ds.DocRemarks = "Approved";
            selectedObject.WarehouseTransferReqDocTrail.Add(ds);

            WarehouseTransferReqAppStatus apps = ObjectSpace.CreateObject<WarehouseTransferReqAppStatus>();
            apps.AppStatus = ApprovalStatusType.Approved;
            apps.AppRemarks = "Approved";
            selectedObject.WarehouseTransferReqAppStatus.Add(apps);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            showMsg("Successful", "Approve Done.", InformationType.Success);
        }

        private void RejectAppWTR_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            WarehouseTransferReq selectedObject = (WarehouseTransferReq)e.CurrentObject;

            selectedObject.AppStatus = ApprovalStatusType.Rejected;

            WarehouseTransferReqDocTrail ds = ObjectSpace.CreateObject<WarehouseTransferReqDocTrail>();
            ds.DocStatus = DocStatus.Submitted;
            ds.DocRemarks = "Rejected";
            selectedObject.WarehouseTransferReqDocTrail.Add(ds);

            WarehouseTransferReqAppStatus apps = ObjectSpace.CreateObject<WarehouseTransferReqAppStatus>();
            apps.AppStatus = ApprovalStatusType.Rejected;
            apps.AppRemarks = "Rejected";
            selectedObject.WarehouseTransferReqAppStatus.Add(apps);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            showMsg("Successful", "Reject Done.", InformationType.Success);
        }

        private void WTRCopyToWT_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                WarehouseTransferReq wtr = (WarehouseTransferReq)View.CurrentObject;
                IObjectSpace os = Application.CreateObjectSpace();
                WarehouseTransfers newwt = os.CreateObject<WarehouseTransfers>();

                if (wtr.Supplier != null)
                {
                    newwt.Supplier = newwt.Session.GetObjectByKey<vwBusniessPartner>(wtr.Supplier.BPCode);
                }
                newwt.Picker = wtr.Picker;
                if (wtr.FromWarehouse != null)
                {
                    newwt.FromWarehouse = newwt.Session.GetObjectByKey<vwWarehouse>(wtr.FromWarehouse.WarehouseCode);
                }
                if (wtr.ToWarehouse != null)
                {
                    newwt.ToWarehouse = newwt.Session.GetObjectByKey<vwWarehouse>(wtr.ToWarehouse.WarehouseCode);
                }
                newwt.Remarks = wtr.Remarks;
                newwt.TransferType = TransferType.WT;
                // Start ver 1.0.9
                newwt.WarehouseTransferReqNo = wtr.DocNum;
                // End ver 1.0.9

                foreach (WarehouseTransferReqDetails dtl in wtr.WarehouseTransferReqDetails)
                {
                    WarehouseTransferDetails newwtdetails = os.CreateObject<WarehouseTransferDetails>();

                    newwtdetails.ItemCode = newwtdetails.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode.ItemCode);
                    newwtdetails.ItemDesc = dtl.ItemDesc;
                    newwtdetails.UOM = dtl.UOM;
                    newwtdetails.CatalogNo = dtl.CatalogNo;
                    if (dtl.FromWarehouse != null)
                    {
                        newwtdetails.FromWarehouse = newwtdetails.Session.GetObjectByKey<vwWarehouse>(dtl.FromWarehouse.WarehouseCode);
                    }
                    if (dtl.ToWarehouse != null)
                    {
                        newwtdetails.ToWarehouse = newwtdetails.Session.GetObjectByKey<vwWarehouse>(dtl.ToWarehouse.WarehouseCode);
                    }
                    if (dtl.FromBin != null)
                    {
                        newwtdetails.FromBin = newwtdetails.Session.GetObjectByKey<vwBin>(dtl.FromBin.BinCode);
                    }
                    if (dtl.ToBin != null)
                    {
                        newwtdetails.ToBin = newwtdetails.Session.GetObjectByKey<vwBin>(dtl.ToBin.BinCode);
                    }
                    newwtdetails.Quantity = dtl.Quantity;
                    newwtdetails.BaseDoc = wtr.DocNum;
                    newwtdetails.BaseId = dtl.Oid.ToString();
                    newwt.WarehouseTransferDetails.Add(newwtdetails);
                }

                foreach (WarehouseTransferReqAttachment dtl in wtr.WarehouseTransferReqAttachment)
                {
                    WarehouseTransferAttachment newwtatt = os.CreateObject<WarehouseTransferAttachment>();

                    newwtatt.File = newwtatt.Session.GetObjectByKey<FileData>(dtl.File.Oid);
                    newwtatt.Remarks = dtl.Remarks;

                    newwt.WarehouseTransferAttachment.Add(newwtatt);
                }

                ShowViewParameters svp = new ShowViewParameters();
                DetailView dv = Application.CreateDetailView(os, newwt);
                dv.ViewEditMode = ViewEditMode.Edit;
                dv.IsRoot = true;
                svp.CreatedView = dv;

                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                showMsg("Success", "Copy Success.", InformationType.Success);
            }
            catch (Exception)
            {
                showMsg("Fail", "Copy Fail.", InformationType.Error);
            }
        }

        private void ApproveAppWTR_Pop_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count > 1)
            {
                int totaldoc = 0;

                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                if (p.IsValid == false)
                {
                    try
                    {
                        foreach (WarehouseTransferReq dtl in e.SelectedObjects)
                        {
                            IObjectSpace wos = Application.CreateObjectSpace();
                            WarehouseTransferReq wtr = wos.FindObject<WarehouseTransferReq>(new BinaryOperator("Oid", dtl.Oid));

                            ApprovalStatusType appstatus = ApprovalStatusType.Required_Approval;

                            if (appstatus == ApprovalStatusType.Not_Applicable)
                                appstatus = ApprovalStatusType.Required_Approval;

                            if (p.IsErr) return;
                            if (appstatus == ApprovalStatusType.Required_Approval && p.AppStatus == ApprovalActions.NA)
                            {
                                showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                                return;
                            }
                            else if (appstatus == ApprovalStatusType.Approved && p.AppStatus == ApprovalActions.Yes)
                            {
                                showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                                return;
                            }
                            else if (appstatus == ApprovalStatusType.Rejected && p.AppStatus == ApprovalActions.No)
                            {
                                showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                                return;
                            }
                            if (p.AppStatus == ApprovalActions.NA)
                            {
                                appstatus = ApprovalStatusType.Required_Approval;
                            }
                            if (p.AppStatus == ApprovalActions.Yes)
                            {
                                appstatus = ApprovalStatusType.Approved;
                            }
                            if (p.AppStatus == ApprovalActions.No)
                            {
                                appstatus = ApprovalStatusType.Rejected;
                            }

                            WarehouseTransferReqAppStatus ds = wos.CreateObject<WarehouseTransferReqAppStatus>();
                            ds.WarehouseTransferReq = wos.GetObjectByKey<WarehouseTransferReq>(wtr.Oid);
                            ds.AppStatus = appstatus;
                            if (appstatus == ApprovalStatusType.Rejected)
                            {
                                //sq.Status = DocStatus.New;
                                ds.AppRemarks = p.ParamString +
                                    System.Environment.NewLine + "(Reject User: " + user.Staff.StaffName + ")" +
                                    System.Environment.NewLine + "(Reason: Approval Rejected) - " + p.ParamString;
                                ds.CreateUser = wos.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                            }
                            else
                            {
                                ds.AppRemarks = p.ParamString +
                                    System.Environment.NewLine + "(Approved User: " + user.Staff.StaffName + ") - " + p.ParamString;
                            }
                            wtr.WarehouseTransferReqAppStatus.Add(ds);

                            wos.CommitChanges();
                            wos.Refresh();

                            totaldoc++;

                            #region approval

                            List<string> ToEmails = new List<string>();
                            string emailbody = "";
                            string emailsubject = "";
                            string emailaddress = "";
                            Guid emailuser;
                            DateTime emailtime = DateTime.Now;

                            string getapproval = "EXEC sp_Approval '" + user.UserName + "', '" + wtr.Oid + "', 'WarehouseTransferReq', " + ((int)appstatus);
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(getapproval, conn);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                if (reader.GetString(1) != "")
                                {
                                    emailbody = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                                              reader.GetString(3) + System.Environment.NewLine + GeneralSettings.appurl + reader.GetString(2) +
                                              System.Environment.NewLine + System.Environment.NewLine;

                                    if (appstatus == ApprovalStatusType.Approved)
                                        emailsubject = "Warehouse Transfer Request Approval";
                                    else if (appstatus == ApprovalStatusType.Rejected)
                                        emailsubject = "Warehouse Transfer Request Approval Rejected";

                                    emailaddress = reader.GetString(1);
                                    emailuser = reader.GetGuid(0);

                                    ToEmails.Add(emailaddress);
                                }
                            }
                            cmd.Dispose();
                            conn.Close();

                            if (ToEmails.Count > 0)
                            {
                                if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                                {
                                }
                            }
                            #endregion

                            //ObjectSpace.CommitChanges(); //This line persists created object(s).
                            //ObjectSpace.Refresh();
                        }

                        showMsg("Info", "Total Document : " + totaldoc + " Approval Done.", InformationType.Info);

                        ((DevExpress.ExpressApp.ListView)Frame.View).ObjectSpace.Refresh();
                    }
                    catch (Exception ex)
                    {
                        showMsg("Error", ex.Message, InformationType.Error);
                    }
                }
            }
            else if (e.SelectedObjects.Count == 1)
            {
                SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;
                ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;

                if (p.IsValid == false)
                {
                    foreach (WarehouseTransferReq dtl in e.SelectedObjects)
                    {
                        IObjectSpace wos = Application.CreateObjectSpace();
                        WarehouseTransferReq wtr = wos.FindObject<WarehouseTransferReq>(new BinaryOperator("Oid", dtl.Oid));

                        ApprovalStatusType appstatus = ApprovalStatusType.Required_Approval;

                        if (appstatus == ApprovalStatusType.Not_Applicable)
                            appstatus = ApprovalStatusType.Required_Approval;


                        if (p.IsErr) return;
                        if (appstatus == ApprovalStatusType.Required_Approval && p.AppStatus == ApprovalActions.NA)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatusType.Approved && p.AppStatus == ApprovalActions.Yes)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        else if (appstatus == ApprovalStatusType.Rejected && p.AppStatus == ApprovalActions.No)
                        {
                            showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                            return;
                        }
                        if (p.AppStatus == ApprovalActions.NA)
                        {
                            appstatus = ApprovalStatusType.Required_Approval;
                        }
                        if (p.AppStatus == ApprovalActions.Yes)
                        {
                            appstatus = ApprovalStatusType.Approved;
                        }
                        if (p.AppStatus == ApprovalActions.No)
                        {
                            appstatus = ApprovalStatusType.Rejected;
                        }

                        WarehouseTransferReqAppStatus ds = wos.CreateObject<WarehouseTransferReqAppStatus>();
                        ds.WarehouseTransferReq = wos.GetObjectByKey<WarehouseTransferReq>(wtr.Oid);
                        ds.AppStatus = appstatus;
                        if (appstatus == ApprovalStatusType.Rejected)
                        {
                            ds.AppRemarks = 
                                System.Environment.NewLine + "(Reject User: " + user.Staff.StaffName + ")" +
                                System.Environment.NewLine + "(Reason: Approval Rejected) - " + p.ParamString;
                            ds.CreateUser = wos.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                        }
                        else
                        {
                            ds.AppRemarks = System.Environment.NewLine + "(Approved User: " + user.Staff.StaffName + ") - " + p.ParamString;
                        }
                        wtr.WarehouseTransferReqAppStatus.Add(ds);

                        wos.CommitChanges();
                        wos.Refresh();

                        #region approval

                        List<string> ToEmails = new List<string>();
                        string emailbody = "";
                        string emailsubject = "";
                        string emailaddress = "";
                        Guid emailuser;
                        DateTime emailtime = DateTime.Now;

                        string getapproval = "EXEC sp_Approval '" + user.UserName + "', '" + wtr.Oid + "', 'WarehouseTransferReq', " + ((int)appstatus);
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(getapproval, conn);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetString(1) != "")
                            {
                                emailbody = "Dear Sir/Madam, " + System.Environment.NewLine + System.Environment.NewLine +
                                      reader.GetString(3) + System.Environment.NewLine + GeneralSettings.appurl + reader.GetString(2) +
                                      System.Environment.NewLine + System.Environment.NewLine;

                                if (appstatus == ApprovalStatusType.Approved)
                                    emailsubject = "Warehouse Transfer Request Approval";
                                else if (appstatus == ApprovalStatusType.Rejected)
                                    emailsubject = "Warehouse Transfer Request Approval Rejected";

                                emailaddress = reader.GetString(1);
                                emailuser = reader.GetGuid(0);

                                ToEmails.Add(emailaddress);
                            }
                        }
                        cmd.Dispose();
                        conn.Close();

                        if (ToEmails.Count > 0)
                        {
                            if (genCon.SendEmail(emailsubject, emailbody, ToEmails) == 1)
                            {
                            }
                        }
                        #endregion

                        IObjectSpace tos = Application.CreateObjectSpace();
                        WarehouseTransferReq trx = tos.FindObject<WarehouseTransferReq>(new BinaryOperator("Oid", wtr.Oid));
                        openNewView(tos, trx, ViewEditMode.View);
                        showMsg("Successful", "Approve Done.", InformationType.Success);
                    }
                }
            }
            else
            {
                showMsg("Fail", "No Warehouse Transfer Request selected.", InformationType.Error);
            }
        }

        private void ApproveAppWTR_Pop_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;

            ApprovalStatusType appstatus = ApprovalStatusType.Required_Approval;

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            switch (appstatus)
            {
                case ApprovalStatusType.Required_Approval:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.NA;
                    break;
                case ApprovalStatusType.Approved:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.Yes;
                    break;
                case ApprovalStatusType.Rejected:
                    ((ApprovalParameters)dv.CurrentObject).AppStatus = ApprovalActions.No;
                    break;
            }
            ((ApprovalParameters)dv.CurrentObject).IsErr = err;
            ((ApprovalParameters)dv.CurrentObject).ActionMessage = "Press choose from approval status 'Yes or No' and press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        // Start ver 1.0.9
        private void ExportWHReq_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            WarehouseTransferReq whr = (WarehouseTransferReq)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\WHRImportFormat.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("DocNum", whr.DocNum);
                doc.SetParameterValue("Type", "StarLaiPortal.Module.BusinessObjects.Warehouse_Transfer.WarehouseTransferReq");

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + whr.DocNum + "_" + user.UserName + "_WHRImport_" + ".xls";

                doc.ExportToDisk(ExportFormatType.Excel, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + whr.DocNum + "_" + user.UserName + "_WHRImport_" + ".xls";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }
        }

        private void ImportWHReq_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void ImportWHReq_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            WarehouseTransferReq trx = (WarehouseTransferReq)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var solution = os.CreateObject<ImportData>();
            solution.Option = new ImportOption();

            solution.Option.UpdateProgress = (x) => solution.Progress = x;
            solution.Option.DocNum = trx.DocNum;
            solution.Option.ConnectionString = genCon.getConnectionString();
            solution.Option.Type = "WarehouseTransferReq";

            solution.Option.MainTypeInfo = (this.View as DetailView).Model.ModelClass;
            var view = Application.CreateDetailView(os, solution, false);

            view.Closed += (sss, eee) =>
            {
                this.Frame.GetController<RefreshController>().RefreshAction.DoExecute();
            };

            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            e.DialogController.AcceptAction.ActionMeaning = ActionMeaning.Unknown;
            //e.Maximized = true;

            e.View = view;
        }
        // End ver 1.0.9
    }
}
