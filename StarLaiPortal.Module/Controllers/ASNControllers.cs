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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Advanced_Shipment_Notice;
using StarLaiPortal.Module.BusinessObjects.GRN;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

// 2023-08-25 add validation for qty when submit ver 1.0.9
// 2023-09-25 add copyto qty ver 1.0.10
// 2023-11-02 pass print user ver 1.0.12
// 2023-12-04 avoid copy same asn ver 1.0.13

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ASNControllers : ViewController
    {
        GeneralControllers genCon;
        public ASNControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.ASNCopyFromPO.Active.SetItemValue("Enabled", false);
            this.ASNCopyFromPODetails.Active.SetItemValue("Enabled", false);
            this.SubmitASN.Active.SetItemValue("Enabled", false);
            this.CancelASN.Active.SetItemValue("Enabled", false);
            this.PreviewASN.Active.SetItemValue("Enabled", false);
            this.PrintLabelASN.Active.SetItemValue("Enabled", false);
            this.ASNCopyToGRN.Active.SetItemValue("Enabled", false);
            this.CloseASN.Active.SetItemValue("Enabled", false);
            this.ExportASN.Active.SetItemValue("Enabled", false);
            this.ImportASN.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "ASN_DetailView")
            {
                //this.BackToInquiry.Active.SetItemValue("Enabled", true);
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitASN.Active.SetItemValue("Enabled", true);
                    this.CancelASN.Active.SetItemValue("Enabled", true);
                    this.PreviewASN.Active.SetItemValue("Enabled", true);
                    //this.PrintLabelASN.Active.SetItemValue("Enabled", true);
                    this.ASNCopyToGRN.Active.SetItemValue("Enabled", true);
                    this.CloseASN.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitASN.Active.SetItemValue("Enabled", false);
                    this.CancelASN.Active.SetItemValue("Enabled", false);
                    this.PreviewASN.Active.SetItemValue("Enabled", false);
                    this.PrintLabelASN.Active.SetItemValue("Enabled", false);
                    this.ASNCopyToGRN.Active.SetItemValue("Enabled", false);
                    this.CloseASN.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    this.ASNCopyFromPO.Active.SetItemValue("Enabled", true);
                    this.ASNCopyFromPODetails.Active.SetItemValue("Enabled", true);
                    this.ExportASN.Active.SetItemValue("Enabled", true);
                    this.ImportASN.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.ASNCopyFromPO.Active.SetItemValue("Enabled", false);
                    this.ASNCopyFromPODetails.Active.SetItemValue("Enabled", false);
                    this.ExportASN.Active.SetItemValue("Enabled", false);
                    this.ImportASN.Active.SetItemValue("Enabled", false);
                }
            }
            else
            {
                this.ASNCopyFromPO.Active.SetItemValue("Enabled", false);
                this.ASNCopyFromPODetails.Active.SetItemValue("Enabled", false);
                this.SubmitASN.Active.SetItemValue("Enabled", false);
                this.CancelASN.Active.SetItemValue("Enabled", false);
                this.PreviewASN.Active.SetItemValue("Enabled", false);
                this.PrintLabelASN.Active.SetItemValue("Enabled", false);
                this.ASNCopyToGRN.Active.SetItemValue("Enabled", false);
                this.CloseASN.Active.SetItemValue("Enabled", false);
                this.ExportASN.Active.SetItemValue("Enabled", false);
                this.ImportASN.Active.SetItemValue("Enabled", false);
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

        private void ASNCopyFromPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    ASN asn = (ASN)View.CurrentObject;

                    //if (asn.IsNew == true)
                    //{
                    //    IObjectSpace os = Application.CreateObjectSpace();
                    //    ASN newasn = os.CreateObject<ASN>();

                    //    foreach (vwPO po in e.PopupWindowViewSelectedObjects)
                    //    {
                    //        IList<vwPODetails> polist = ObjectSpace.GetObjects<vwPODetails>
                    //            (CriteriaOperator.Parse("SAPDocNum = ?", po.SAPDocNum));

                    //        foreach (vwPODetails dtl in polist)
                    //        {
                    //            if (dtl.CardCode != null)
                    //            {
                    //                newasn.Supplier = newasn.Session.GetObjectByKey<vwBusniessPartner>(dtl.CardCode);
                    //            }

                    //            ASNDetails newasnitem = os.CreateObject<ASNDetails>();

                    //            newasnitem.PONo = dtl.SAPDocNum;
                    //            newasnitem.ItemCode = dtl.ItemCode;
                    //            newasnitem.ItemDesc = dtl.ItemDescrip;
                    //            newasnitem.CatalogNo = dtl.CatalogNum;
                    //            newasnitem.UOM = dtl.UOM;
                    //            if (dtl.WhsCode != null)
                    //            {
                    //                newasnitem.Location = newasnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                    //            }
                    //            newasnitem.DefBin = dtl.DefBin;
                    //            newasnitem.Quantity = dtl.OpenQty;
                    //            newasnitem.UnloadQty = dtl.OpenQty;
                    //            newasnitem.PORefNo = dtl.PortalNum;
                    //            newasnitem.BaseDoc = dtl.BaseEntry.ToString();
                    //            newasnitem.BaseId = dtl.BaseLine.ToString();

                    //            newasn.ASNDetails.Add(newasnitem);
                    //        }
                    //    }

                    //    ShowViewParameters svp = new ShowViewParameters();
                    //    DetailView dv = Application.CreateDetailView(os, newasn);
                    //    dv.ViewEditMode = ViewEditMode.Edit;
                    //    dv.IsRoot = true;
                    //    svp.CreatedView = dv;

                    //    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //    showMsg("Success", "Copy Success.", InformationType.Success);
                    //}
                    //else
                    //{
                        foreach (vwPO po in e.PopupWindowViewSelectedObjects)
                        {
                            IList<vwPODetails> polist = ObjectSpace.GetObjects<vwPODetails>
                                (CriteriaOperator.Parse("SAPDocNum = ?", po.SAPDocNum));

                            foreach (vwPODetails dtl in polist)
                            {
                                ASNDetails newasnitem = ObjectSpace.CreateObject<ASNDetails>();

                                newasnitem.PONo = dtl.SAPDocNum;
                                newasnitem.ItemCode = newasnitem.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newasnitem.ItemDesc = dtl.ItemDescrip;
                                newasnitem.CatalogNo = dtl.CatalogNum;
                                newasnitem.UOM = dtl.UOM;
                                if (dtl.WhsCode != null)
                                {
                                    newasnitem.Location = newasnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                                }
                                if (dtl.DefBin != null)
                                {
                                    newasnitem.DefBin = newasnitem.Session.FindObject
                                    <vwBin>(CriteriaOperator.Parse("AbsEntry = ?", dtl.DefBin));
                                }
                                newasnitem.Quantity = dtl.OpenQty;
                                newasnitem.UnloadQty = dtl.Quantity;
                                newasnitem.PORefNo = dtl.PortalNum;
                                newasnitem.BaseDoc = dtl.BaseEntry.ToString();
                                newasnitem.BaseId = dtl.BaseLine.ToString();

                                asn.ASNDetails.Add(newasnitem);
                            }
                        }

                        if (asn.DocNum == null)
                        {
                            string docprefix = genCon.GetDocPrefix();
                            asn.DocNum = genCon.GenerateDocNum(DocTypeList.ASN, ObjectSpace, TransferType.NA, 0, docprefix);
                        }
                        
                        // Start ver 1.0.8.1
                        string duppo = null;
                        // End ver 1.0.8.1
                        foreach (ASNDetails dtl2 in asn.ASNDetails)
                        {
                            dtl2.OIDKey = dtl2.Oid;

                            // Start ver 1.0.8.1
                            //if (duppo != dtl2.PORefNo)
                            //{
                            //    if (asn.PONo == null)
                            //    {
                            //        asn.PONo = dtl2.PORefNo;
                            //    }
                            //    else
                            //    {
                            //        asn.PONo = asn.PONo + ", " + dtl2.PORefNo;
                            //    }

                            //    duppo = dtl2.PORefNo;
                            //}
                            // End ver 1.0.8.1
                        }

                        ObjectSpace.CommitChanges();
                        ObjectSpace.Refresh();
                        
                        // Start ver 1.0.11
                        IObjectSpace os = Application.CreateObjectSpace();
                        ASN trx = os.FindObject<ASN>(new BinaryOperator("Oid", asn.Oid));

                        if (trx.Oid > 0)
                        {
                            trx.PONo = null;
                            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                            string getporef = "SELECT PORefNo FROM ASNDetails where ASN = " + trx.Oid + " GROUP BY PORefNo";
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(getporef, conn);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                if (trx.PONo != null)
                                {
                                    trx.PONo = trx.PONo + ", " + reader.GetString(0);
                                }
                                else
                                {
                                    trx.PONo = reader.GetString(0);
                                }
                            }
                        }

                        os.CommitChanges();
                        openNewView(os, trx, ViewEditMode.Edit);
                        // End ver 1.0.11

                        showMsg("Success", "Copy Success.", InformationType.Success);
                    //}
                }
                catch (Exception)
                {
                    showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
        }

        private void ASNCopyFromPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            ASN asn = (ASN)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vwPO));
            var cs = Application.CreateCollectionSource(os, typeof(vwPO), viewId);
            if (asn.Supplier != null)
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", asn.Supplier.BPCode);
            }
            else
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", "");
            }

            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void ASNCopyFromPODetails_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    ASN asn = (ASN)View.CurrentObject;

                    //if (asn.IsNew == true)
                    //{
                    //    IObjectSpace os = Application.CreateObjectSpace();
                    //    ASN newasn = os.CreateObject<ASN>();

                    //    foreach (vwPODetails dtl in e.PopupWindowViewSelectedObjects)
                    //    {
                    //        if (dtl.CardCode != null)
                    //        {
                    //            newasn.Supplier = newasn.Session.GetObjectByKey<vwBusniessPartner>(dtl.CardCode);
                    //        }

                    //        ASNDetails newasnitem = os.CreateObject<ASNDetails>();

                    //        newasnitem.PONo = dtl.SAPDocNum;
                    //        newasnitem.ItemCode = dtl.ItemCode;
                    //        newasnitem.ItemDesc = dtl.ItemDescrip;
                    //        newasnitem.CatalogNo = dtl.CatalogNum;
                    //        newasnitem.UOM = dtl.UOM;
                    //        if (dtl.WhsCode != null)
                    //        {
                    //            newasnitem.Location = newasnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                    //        }
                    //        newasnitem.DefBin = dtl.DefBin;
                    //        newasnitem.Quantity = dtl.OpenQty;
                    //        newasnitem.UnloadQty = dtl.OpenQty;
                    //        newasnitem.PORefNo = dtl.PortalNum;
                    //        newasnitem.BaseDoc = dtl.BaseEntry.ToString();
                    //        newasnitem.BaseId = dtl.BaseLine.ToString();

                    //        newasn.ASNDetails.Add(newasnitem);
                    //    }

                    //    ShowViewParameters svp = new ShowViewParameters();
                    //    DetailView dv = Application.CreateDetailView(os, newasn);
                    //    dv.ViewEditMode = ViewEditMode.Edit;
                    //    dv.IsRoot = true;
                    //    svp.CreatedView = dv;

                    //    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //    showMsg("Success", "Copy Success.", InformationType.Success);
                    //}
                    //else
                    //{
                        foreach (vwPODetails dtl in e.PopupWindowViewSelectedObjects)
                        {
                            ASNDetails newasnitem = ObjectSpace.CreateObject<ASNDetails>();

                            newasnitem.PONo = dtl.SAPDocNum;
                            newasnitem.ItemCode = newasnitem.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newasnitem.ItemDesc = dtl.ItemDescrip;
                            newasnitem.CatalogNo = dtl.CatalogNum;
                            newasnitem.UOM = dtl.UOM;
                            if (dtl.WhsCode != null)
                            {
                                newasnitem.Location = newasnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                            }
                            if (dtl.DefBin != null)
                            {
                                newasnitem.DefBin = newasnitem.Session.FindObject
                                    <vwBin>(CriteriaOperator.Parse("AbsEntry = ?", dtl.DefBin));
                            }
                            newasnitem.Quantity = dtl.OpenQty;
                            newasnitem.UnloadQty = dtl.Quantity;
                            newasnitem.PORefNo = dtl.PortalNum;
                            newasnitem.BaseDoc = dtl.BaseEntry.ToString();
                            newasnitem.BaseId = dtl.BaseLine.ToString();

                            asn.ASNDetails.Add(newasnitem);
                        }

                        if (asn.DocNum == null)
                        {
                            string docprefix = genCon.GetDocPrefix();
                            asn.DocNum = genCon.GenerateDocNum(DocTypeList.ASN, ObjectSpace, TransferType.NA, 0, docprefix);
                        }

                        // Start ver 1.0.8.1
                        string duppo = null;
                        // End ver 1.0.8.1
                        foreach (ASNDetails dtl2 in asn.ASNDetails)
                        {
                            dtl2.OIDKey = dtl2.Oid;

                            // Start ver 1.0.8.1
                            //if (duppo != dtl2.PORefNo)
                            //{
                            //    if (asn.PONo == null)
                            //    {
                            //        asn.PONo = dtl2.PORefNo;
                            //    }
                            //    else
                            //    {
                            //        asn.PONo = asn.PONo + ", " + dtl2.PORefNo;
                            //    }

                            //    duppo = dtl2.PORefNo;
                            //}
                            // End ver 1.0.8.1
                        }

                        ObjectSpace.CommitChanges();
                        ObjectSpace.Refresh();

                        // Start ver 1.0.11
                        IObjectSpace os = Application.CreateObjectSpace();
                        ASN trx = os.FindObject<ASN>(new BinaryOperator("Oid", asn.Oid));

                        if (trx.Oid > 0)
                        {
                            trx.PONo = null;
                            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                            string getporef = "SELECT PORefNo FROM ASNDetails where ASN = " + trx.Oid + " GROUP BY PORefNo";
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(getporef, conn);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                if (trx.PONo != null)
                                {
                                    trx.PONo = trx.PONo + ", " + reader.GetString(0);
                                }
                                else
                                {
                                    trx.PONo = reader.GetString(0);
                                }
                            }
                        }

                        os.CommitChanges();
                        openNewView(os, trx, ViewEditMode.Edit);
                        // End ver 1.0.11

                        showMsg("Success", "Copy Success.", InformationType.Success);

                    //}
                }
                catch (Exception)
                {
                    showMsg("Fail", "Copy Fail.", InformationType.Error);
                }
            }
        }

        private void ASNCopyFromPODetails_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            ASN asn = (ASN)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vwPODetails));
            var cs = Application.CreateCollectionSource(os, typeof(vwPODetails), viewId);
            if (asn.Supplier != null)
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", asn.Supplier.BPCode);
            }
            else
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", "");
            }

            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void SubmitASN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ASN selectedObject = (ASN)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            // Start ver 1.0.9
            foreach(ASNDetails dtl in selectedObject.ASNDetails)
            {
                if (dtl.UnloadQty > dtl.Quantity)
                {
                    showMsg("Error", "Unload Qty cannot more than Open Qty. Item : " + dtl.ItemCode, InformationType.Error);
                    return;
                }
            }
            // End ver 1.0.9

            if (selectedObject.IsValid == true)
            {
                selectedObject.Status = DocStatus.Submitted;
                ASNDocTrail ds = ObjectSpace.CreateObject<ASNDocTrail>();
                ds.DocStatus = DocStatus.Submitted;
                ds.DocRemarks = p.ParamString;
                selectedObject.ASNDocTrail.Add(ds);

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

                IObjectSpace os = Application.CreateObjectSpace();
                ASN trx = os.FindObject<ASN>(new BinaryOperator("Oid", selectedObject.Oid));
                openNewView(os, trx, ViewEditMode.View);
                showMsg("Successful", "Submit Done.", InformationType.Success);
            }
            else
            {
                showMsg("Error", "No Content.", InformationType.Error);
            }
        }

        private void SubmitASN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelASN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ASN selectedObject = (ASN)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            ASNDocTrail ds = ObjectSpace.CreateObject<ASNDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.ASNDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            ASN trx = os.FindObject<ASN>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelASN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void PreviewASN_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            ASN asn = (ASN)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\ASN.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("dockey@", asn.Oid);
                doc.SetParameterValue("dbName@", conn.Database);
                // Start ver 1.0.12
                doc.SetParameterValue("userName@", user.Staff.StaffName);
                // End ver 1.0.12

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + asn.Oid + "_" + user.UserName + "_ASN_"
                    + DateTime.Parse(asn.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + asn.Oid + "_" + user.UserName + "_ASN_"
                    + DateTime.Parse(asn.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                asn.PrintStatus = PrintStatus.Printed;

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }
        }

        private void PrintLabelASN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void PrintLabelASN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {

        }

        private void ASNCopyToGRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                ASN asn = (ASN)View.CurrentObject;

                // Start 1.0.13
                IObjectSpace asnos = Application.CreateObjectSpace();
                ASN checkasn = asnos.FindObject<ASN>(CriteriaOperator.Parse("DocNum = ?", asn.DocNum));

                if (checkasn.IsValid1 == false)
                {
                    showMsg("Error", "ASN already created GRN, please refresh data.", InformationType.Error);
                    return;
                }
                // End ver 1.0.13

                IObjectSpace os = Application.CreateObjectSpace();
                GRN newgrn = os.CreateObject<GRN>();

                string docprefix = genCon.GetDocPrefix();

                newgrn.Supplier = newgrn.Session.GetObjectByKey<vwBusniessPartner>(asn.Supplier.BPCode);
                newgrn.SupplierName = asn.SupplierName;
                if (asn.Vehicle != null)
                {
                    newgrn.Vehicle = newgrn.Session.GetObjectByKey<vwVehicle>(asn.Vehicle.VehicleCode);
                }
                newgrn.Container = asn.Container;

                foreach (ASNDetails dtl in asn.ASNDetails)
                {
                    // Start ver 1.0.10
                    if (dtl.LineClosed == false)
                    {
                    // End ver 1.0.10
                        GRNDetails newgrnitem = os.CreateObject<GRNDetails>();

                        newgrnitem.PONo = dtl.PONo;
                        newgrnitem.ItemCode = newgrnitem.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode.ItemCode);
                        newgrnitem.ItemDesc = dtl.ItemDesc;
                        newgrnitem.CatalogNo = dtl.CatalogNo;
                        if (dtl.Location != null)
                        {
                            newgrnitem.Location = newgrnitem.Session.GetObjectByKey<vwWarehouse>(dtl.Location.WarehouseCode);
                        }
                        if (dtl.DefBin != null)
                        {
                            newgrnitem.DefBin = newgrnitem.Session.FindObject
                                <vwBin>(CriteriaOperator.Parse("BinCode = ?", dtl.DefBin.BinCode));
                        }
                        // Start ver 1.0.10
                        //newgrnitem.OpenQty = dtl.UnloadQty;
                        newgrnitem.OpenQty = dtl.UnloadQty - dtl.CopyTotalQty;
                        // End ver 1.0.10
                        newgrnitem.Received = 0;
                        newgrnitem.PORefNo = dtl.PORefNo;
                        newgrnitem.ASNBaseDoc = asn.DocNum;
                        newgrnitem.ASNBaseId = dtl.Oid.ToString();
                        newgrnitem.ASNPOBaseDoc = dtl.BaseDoc.ToString();
                        newgrnitem.ASNPOBaseId = dtl.BaseId.ToString();
                        newgrnitem.BaseType = "ASN";

                        newgrn.GRNDetails.Add(newgrnitem);
                    // Start ver 1.0.10
                    }
                    // End ver 1.0.10
                }

                ShowViewParameters svp = new ShowViewParameters();
                DetailView dv = Application.CreateDetailView(os, newgrn);
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

        private void ASNCopyToGRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CloseASN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ASN selectedObject = (ASN)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Closed;
            ASNDocTrail ds = ObjectSpace.CreateObject<ASNDocTrail>();
            ds.DocStatus = DocStatus.Closed;
            ds.DocRemarks = p.ParamString;
            selectedObject.ASNDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            IObjectSpace os = Application.CreateObjectSpace();
            ASN trx = os.FindObject<ASN>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Close Done.", InformationType.Success);
        }

        private void CloseASN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void ExportASN_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            ASN asn = (ASN)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\ASNImportFormat.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("DocNum", asn.DocNum);
                doc.SetParameterValue("Type", "StarLaiPortal.Module.BusinessObjects.Advanced_Shipment_Notice.ASNDetails");

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + asn.DocNum + "_" + user.UserName + "_ASNImport_" + ".xls";

                doc.ExportToDisk(ExportFormatType.Excel, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + asn.DocNum + "_" + user.UserName + "_ASNImport_" + ".xls";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }
        }

        private void ImportASN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void ImportASN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            ASN trx = (ASN)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var solution = os.CreateObject<ImportData>();
            solution.Option = new ImportOption();

            solution.Option.UpdateProgress = (x) => solution.Progress = x;
            solution.Option.DocNum = trx.DocNum;
            solution.Option.ConnectionString = genCon.getConnectionString();
            solution.Option.Type = "ASN";

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
    }
}
