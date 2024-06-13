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
using StarLaiPortal.Module.BusinessObjects.Stock_Count;
using StarLaiPortal.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.ExpressApp.Web;
using System.Configuration;
using System.Web;
using Admiral.ImportData;

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class StockCountConfirmControllers : ViewController
    {
        GeneralControllers genCon;
        public StockCountConfirmControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.SubmitSCC.Active.SetItemValue("Enabled", false);
            this.CancelSCC.Active.SetItemValue("Enabled", false);
            this.ExportConfirmCountItems.Active.SetItemValue("Enabled", false);
            this.ImportConfirmCountItems.Active.SetItemValue("Enabled", false);
            this.PrintStockConfirm.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "StockCountConfirm_DetailView")
            {
                //this.BackToInquiry.Active.SetItemValue("Enabled", true);
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitSCC.Active.SetItemValue("Enabled", true);
                    this.CancelSCC.Active.SetItemValue("Enabled", true);
                    this.PrintStockConfirm.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitSCC.Active.SetItemValue("Enabled", false);
                    this.CancelSCC.Active.SetItemValue("Enabled", false);
                    this.PrintStockConfirm.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    this.ExportConfirmCountItems.Active.SetItemValue("Enabled", false);
                    this.ImportConfirmCountItems.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.ExportConfirmCountItems.Active.SetItemValue("Enabled", false);
                    this.ImportConfirmCountItems.Active.SetItemValue("Enabled", false);
                }
            }
            else
            {
                this.SubmitSCC.Active.SetItemValue("Enabled", false);
                this.CancelSCC.Active.SetItemValue("Enabled", false);
                this.ExportConfirmCountItems.Active.SetItemValue("Enabled", false);
                this.ImportConfirmCountItems.Active.SetItemValue("Enabled", false);
                this.PrintStockConfirm.Active.SetItemValue("Enabled", false);
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
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

        private void SubmitSCC_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            StockCountConfirm selectedObject = (StockCountConfirm)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;
            SqlConnection conn = new SqlConnection(genCon.getConnectionString());

            if (selectedObject.IsValid == false)
            {
                showMsg("Error", "No Content.", InformationType.Error);
                return;
            }

            selectedObject.Status = DocStatus.Submitted;
            StockCountConfirmDocTrail ds = ObjectSpace.CreateObject<StockCountConfirmDocTrail>();
            ds.DocStatus = DocStatus.Submitted;
            ds.DocRemarks = p.ParamString;
            selectedObject.StockCountConfirmDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            StockCountConfirm trx = os.FindObject<StockCountConfirm>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Submit Done.", InformationType.Success);

            MemoryManagement.FlushMemory();
        }

        private void SubmitSCC_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelSCC_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            StockCountConfirm selectedObject = (StockCountConfirm)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            StockCountConfirmDocTrail ds = ObjectSpace.CreateObject<StockCountConfirmDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.StockCountConfirmDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            StockCountConfirm trx = os.FindObject<StockCountConfirm>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);

            MemoryManagement.FlushMemory();
        }

        private void CancelSCC_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void ExportConfirmCountItems_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            StockCountConfirm counted = (StockCountConfirm)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\ConfirmCountedImportFormat.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("DocNum", counted.DocNum);
                doc.SetParameterValue("Type", "StarLaiPortal.Module.BusinessObjects.Stock_Count.StockCountConfirmDetails");

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + counted.DocNum + "_" + user.UserName + "_ConfirmCountedImport_" + ".xls";

                doc.ExportToDisk(ExportFormatType.Excel, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + counted.DocNum + "_" + user.UserName + "_ConfirmCountedImport_" + ".xls";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }

            MemoryManagement.FlushMemory();
        }

        private void ImportConfirmCountItems_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            MemoryManagement.FlushMemory();
        }

        private void ImportConfirmCountItems_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            StockCountConfirm trx = (StockCountConfirm)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var solution = os.CreateObject<ImportData>();
            solution.Option = new ImportOption();

            solution.Option.UpdateProgress = (x) => solution.Progress = x;
            solution.Option.DocNum = trx.DocNum;
            solution.Option.ConnectionString = genCon.getConnectionString();
            solution.Option.Type = "StockCountConfirm";

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

        private void PrintStockConfirm_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            StockCountConfirm stockconfirm = (StockCountConfirm)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\StockCountConfirm.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("dockey@", stockconfirm.Oid);
                doc.SetParameterValue("dbName@", conn.Database);

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + stockconfirm.Oid + "_" + user.UserName + "_StockConfirm_"
                    + DateTime.Parse(stockconfirm.StockCountDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + stockconfirm.Oid + "_" + user.UserName + "_StockConfirm_"
                    + DateTime.Parse(stockconfirm.StockCountDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }

            MemoryManagement.FlushMemory();
        }
    }
}
