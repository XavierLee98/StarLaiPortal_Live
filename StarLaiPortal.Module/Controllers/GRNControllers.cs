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
using DevExpress.Persistent.Validation;
using DevExpress.Web.Internal.XmlProcessor;
using StarLaiPortal.Module.BusinessObjects;
using StarLaiPortal.Module.BusinessObjects.Advanced_Shipment_Notice;
using StarLaiPortal.Module.BusinessObjects.GRN;
using StarLaiPortal.Module.BusinessObjects.Pack_List;
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

// 2023-07-20 - do not close asn if partial - ver 1.0.6 (UAT)
// 2023-04-09 fix speed issue ver 1.0.8.1
// 2023-09-25 add copyto qty ver 1.0.10
// 2023-12-04 add outstanding qty ver 1.0.13

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GRNControllers : ViewController
    {
        GeneralControllers genCon;
        public GRNControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.GRNCopyFromPO.Active.SetItemValue("Enabled", false);
            this.GRNCopyFromASN.Active.SetItemValue("Enabled", false);
            this.SubmitGRN.Active.SetItemValue("Enabled", false);
            this.CancelGRN.Active.SetItemValue("Enabled", false);
            this.PreviewGRN.Active.SetItemValue("Enabled", false);
            this.ExportGRN.Active.SetItemValue("Enabled", false);
            this.ImportGRN.Active.SetItemValue("Enabled", false);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GeneralControllers>();

            if (View.Id == "GRN_DetailView")
            {
                //this.BackToInquiry.Active.SetItemValue("Enabled", true);
                if (((DetailView)View).ViewEditMode == ViewEditMode.View)
                {
                    this.SubmitGRN.Active.SetItemValue("Enabled", true);
                    this.CancelGRN.Active.SetItemValue("Enabled", true);
                    this.PreviewGRN.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.SubmitGRN.Active.SetItemValue("Enabled", false);
                    this.CancelGRN.Active.SetItemValue("Enabled", false);
                    this.PreviewGRN.Active.SetItemValue("Enabled", false);
                }

                if (((DetailView)View).ViewEditMode == ViewEditMode.Edit)
                {
                    this.GRNCopyFromPO.Active.SetItemValue("Enabled", true);
                    this.GRNCopyFromASN.Active.SetItemValue("Enabled", true);
                    this.ExportGRN.Active.SetItemValue("Enabled", true);
                    this.ImportGRN.Active.SetItemValue("Enabled", true);
                }
                else
                {
                    this.GRNCopyFromPO.Active.SetItemValue("Enabled", false);
                    this.GRNCopyFromASN.Active.SetItemValue("Enabled", false);
                    this.ExportGRN.Active.SetItemValue("Enabled", false);
                    this.ImportGRN.Active.SetItemValue("Enabled", false);
                }
            }
            else
            {
                this.GRNCopyFromPO.Active.SetItemValue("Enabled", false);
                this.GRNCopyFromASN.Active.SetItemValue("Enabled", false);
                this.SubmitGRN.Active.SetItemValue("Enabled", false);
                this.CancelGRN.Active.SetItemValue("Enabled", false);
                this.PreviewGRN.Active.SetItemValue("Enabled", false);
                this.ExportGRN.Active.SetItemValue("Enabled", false);
                this.ImportGRN.Active.SetItemValue("Enabled", false);
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

        private void GRNCopyFromPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    GRN grn = (GRN)View.CurrentObject;
                    string docprefix = genCon.GetDocPrefix();

                    //if (grn.IsNew == true)
                    //{
                    //    IObjectSpace os = Application.CreateObjectSpace();
                    //    GRN newgrn = os.CreateObject<GRN>();

                    //    foreach (vwPODetails dtl in e.PopupWindowViewSelectedObjects)
                    //    {
                    //        if (dtl.CardCode != null)
                    //        {
                    //            newgrn.Supplier = newgrn.Session.GetObjectByKey<vwBusniessPartner>(dtl.CardCode);
                    //        }

                    //        GRNDetails newgrnitem = os.CreateObject<GRNDetails>();

                    //        newgrnitem.PONo = dtl.SAPDocNum;
                    //        newgrnitem.ItemCode = dtl.ItemCode;
                    //        newgrnitem.ItemDesc = dtl.ItemDescrip;
                    //        newgrnitem.CatalogNo = dtl.CatalogNum;
                    //        if (dtl.WhsCode != null)
                    //        {
                    //            newgrnitem.Location = newgrnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                    //        }
                    //        newgrnitem.DefBin = dtl.DefBin;
                    //        newgrnitem.OpenQty = dtl.OpenQty;
                    //        newgrnitem.Received = dtl.OpenQty;
                    //        newgrnitem.PORefNo = dtl.PortalNum;
                    //        newgrnitem.BaseDoc = dtl.BaseEntry.ToString();
                    //        newgrnitem.BaseId = dtl.BaseLine.ToString();

                    //        newgrn.GRNDetails.Add(newgrnitem);
                    //    }

                    //    if (newgrn.DocNum == null)
                    //    {
                    //        newgrn.DocNum = genCon.GenerateDocNum(DocTypeList.GRN, os);
                    //    }

                    //    ShowViewParameters svp = new ShowViewParameters();
                    //    DetailView dv = Application.CreateDetailView(os, newgrn);
                    //    dv.ViewEditMode = ViewEditMode.Edit;
                    //    dv.IsRoot = true;
                    //    svp.CreatedView = dv;

                    //    os.CommitChanges();
                    //    os.Refresh();

                    //    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //    showMsg("Success", "Copy Success.", InformationType.Success);
                    //}
                    //else
                    //{
                        foreach (vwPODetails dtl in e.PopupWindowViewSelectedObjects)
                        {
                            //grn.InvoiceNo = dtl.SAPDocNum;
                            GRNDetails newgrnitem = ObjectSpace.CreateObject<GRNDetails>();

                            newgrnitem.PONo = dtl.SAPDocNum;
                            newgrnitem.ItemCode = newgrnitem.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                            newgrnitem.ItemDesc = dtl.ItemDescrip;
                            newgrnitem.CatalogNo = dtl.CatalogNum;
                            if (dtl.WhsCode != null)
                            {
                                newgrnitem.Location = newgrnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                            }
                            if (dtl.DefBin != null)
                            {
                                newgrnitem.DefBin = newgrnitem.Session.FindObject
                                    <vwBin>(CriteriaOperator.Parse("AbsEntry = ?", dtl.DefBin));
                            }
                            newgrnitem.OpenQty = dtl.OpenQty;
                            newgrnitem.Received = 0;
                            newgrnitem.PORefNo = dtl.PortalNum;
                            newgrnitem.BaseDoc = dtl.BaseEntry.ToString();
                            newgrnitem.BaseId = dtl.BaseLine.ToString();
                            newgrnitem.BaseType = "PO";

                            grn.GRNDetails.Add(newgrnitem);
                        }

                        if (grn.DocNum == null)
                        {
                            grn.DocNum = genCon.GenerateDocNum(DocTypeList.GRN, ObjectSpace, TransferType.NA, 0, docprefix);
                        }

                        ObjectSpace.CommitChanges();
                        ObjectSpace.Refresh();

                        // Start ver 1.0.8.1
                        //string duppo = null;
                        //string dupporef = null;
                        //// End ver 1.0.8.1
                        //foreach (GRNDetails dtl2 in grn.GRNDetails)
                        //{
                        //    dtl2.OIDKey = dtl2.Oid;

                            // Start ver 1.0.8.1
                            //if (dtl2.PONo != null)
                            //{
                            //    if (duppo != dtl2.PONo)
                            //    {
                            //        if (grn.SAPPONo == null)
                            //        {
                            //            grn.SAPPONo = dtl2.PONo;
                            //        }
                            //        else
                            //        {
                            //            grn.SAPPONo = grn.SAPPONo + ", " + dtl2.PONo;
                            //        }

                            //        duppo = dtl2.PONo;
                            //    }
                            //}

                            //if (dtl2.PORefNo != null)
                            //{
                            //    if (dupporef != dtl2.PORefNo)
                            //    {
                            //        if (grn.PortalPONo == null)
                            //        {
                            //            grn.PortalPONo = dtl2.PORefNo;
                            //        }
                            //        else
                            //        {
                            //            grn.PortalPONo = grn.PortalPONo + ", " + dtl2.PORefNo;
                            //        }

                            //        dupporef = dtl2.PORefNo;
                            //    }
                            //}
                            // End ver 1.0.8.1
                        //}

                        //ObjectSpace.CommitChanges();
                        //ObjectSpace.Refresh();

                        // Start ver 1.0.11
                        IObjectSpace os = Application.CreateObjectSpace();
                        GRN trx = os.FindObject<GRN>(new BinaryOperator("Oid", grn.Oid));

                        foreach (GRNDetails dtl2 in trx.GRNDetails)
                        {
                            dtl2.OIDKey = dtl2.Oid;
                        }

                        if (trx.Oid > 0)
                        {
                            trx.PortalPONo = null;
                            trx.SAPPONo = null;
                            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                            string getporef = "SELECT PONo, ISNULL(PORefNo, '') From GRNDetails WHERE GRN = " + trx.Oid + " GROUP BY PONo, PORefNo";
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(getporef, conn);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                if (reader.GetString(1) != "")
                                {
                                    if (trx.PortalPONo != null)
                                    {
                                        trx.PortalPONo = trx.PortalPONo + ", " + reader.GetString(1);
                                    }
                                    else
                                    {
                                        trx.PortalPONo = reader.GetString(1);
                                    }
                                }

                                if (trx.SAPPONo != null)
                                {
                                    trx.SAPPONo = trx.SAPPONo + ", " + reader.GetString(0);
                                }
                                else
                                {
                                    trx.SAPPONo = reader.GetString(0);
                                }
                            }
                            cmd.Dispose();
                            conn.Close();
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

        private void GRNCopyFromPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            GRN grn = (GRN)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vwPODetails));
            var cs = Application.CreateCollectionSource(os, typeof(vwPODetails), viewId);
            if (grn.Supplier != null)
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", grn.Supplier.BPCode);
            }
            else
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", "");
            }

            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void GRNCopyFromASN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count > 0)
            {
                try
                {
                    GRN grn = (GRN)View.CurrentObject;
                    string docprefix = genCon.GetDocPrefix();

                    //if (grn.IsNew == true)
                    //{
                    //    IObjectSpace os = Application.CreateObjectSpace();
                    //    GRN newgrn = os.CreateObject<GRN>();

                    //    foreach (vwASN asn in e.PopupWindowViewSelectedObjects)
                    //    {
                    //        IList<vwASNDetails> asnlist = ObjectSpace.GetObjects<vwASNDetails>
                    //            (CriteriaOperator.Parse("ASNDocNum = ?", asn.ASNDocNum));

                    //        foreach (vwASNDetails dtl in asnlist)
                    //        {
                    //            if (dtl.CardCode != null)
                    //            {
                    //                newgrn.Supplier = newgrn.Session.GetObjectByKey<vwBusniessPartner>(dtl.CardCode);
                    //            }

                    //            GRNDetails newgrnitem = os.CreateObject<GRNDetails>();

                    //            newgrnitem.PONo = dtl.SAPDocNum;
                    //            newgrnitem.ItemCode = dtl.ItemCode;
                    //            newgrnitem.ItemDesc = dtl.ItemDescrip;
                    //            newgrnitem.CatalogNo = dtl.CatalogNum;
                    //            if (dtl.WhsCode != null)
                    //            {
                    //                newgrnitem.Location = newgrnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                    //            }
                    //            newgrnitem.DefBin = dtl.DefBin;
                    //            newgrnitem.OpenQty = dtl.OpenQty;
                    //            newgrnitem.Received = dtl.OpenQty;
                    //            newgrnitem.PORefNo = dtl.PortalNum;
                    //            newgrnitem.ASNBaseDoc = dtl.BaseEntry.ToString();
                    //            newgrnitem.ASNBaseId = dtl.BaseLine.ToString();

                    //            newgrn.GRNDetails.Add(newgrnitem);
                    //        }
                    //    }

                    //    if (newgrn.DocNum == null)
                    //    {
                    //        newgrn.DocNum = genCon.GenerateDocNum(DocTypeList.GRN, os);
                    //    }

                    //    ShowViewParameters svp = new ShowViewParameters();
                    //    DetailView dv = Application.CreateDetailView(os, newgrn);
                    //    dv.ViewEditMode = ViewEditMode.Edit;
                    //    dv.IsRoot = true;
                    //    svp.CreatedView = dv;

                    //    os.CommitChanges();
                    //    os.Refresh();

                    //    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                    //    showMsg("Success", "Copy Success.", InformationType.Success);
                    //}
                    //else
                    //{
                        foreach (vwASN asn in e.PopupWindowViewSelectedObjects)
                        {
                            // Start 1.0.13
                            IObjectSpace asnos = Application.CreateObjectSpace();
                            vwASN checkasn = asnos.FindObject<vwASN>(CriteriaOperator.Parse("ASNDocNum = ?", asn.ASNDocNum));

                            if (checkasn == null)
                            {
                                showMsg("Error", "ASN already created GRN, please refresh data.", InformationType.Error);
                                return;
                            }
                            // End ver 1.0.13

                            IList<vwASNDetails> asnlist = ObjectSpace.GetObjects<vwASNDetails>
                                (CriteriaOperator.Parse("ASNDocNum = ?", asn.ASNDocNum));

                            foreach (vwASNDetails dtl in asnlist)
                            {
                                //grn.InvoiceNo = dtl.SAPDocNum;
                                GRNDetails newgrnitem = ObjectSpace.CreateObject<GRNDetails>();

                                newgrnitem.PONo = dtl.SAPDocNum;
                                newgrnitem.ItemCode = newgrnitem.Session.GetObjectByKey<vwItemMasters>(dtl.ItemCode);
                                newgrnitem.ItemDesc = dtl.ItemDescrip;
                                newgrnitem.CatalogNo = dtl.CatalogNum;
                                if (dtl.WhsCode != null)
                                {
                                    newgrnitem.Location = newgrnitem.Session.GetObjectByKey<vwWarehouse>(dtl.WhsCode.WarehouseCode);
                                }
                                if (dtl.DefBin != null)
                                {
                                    newgrnitem.DefBin = newgrnitem.Session.FindObject
                                        <vwBin>(CriteriaOperator.Parse("AbsEntry = ?", dtl.DefBin));
                                }
                                newgrnitem.OpenQty = dtl.OpenQty;
                                newgrnitem.Received = 0;
                                newgrnitem.PORefNo = dtl.PortalNum;
                                newgrnitem.ASNBaseDoc = dtl.ASNDocNum;
                                newgrnitem.ASNBaseId = dtl.ASNOid.ToString();
                                newgrnitem.ASNPOBaseDoc = dtl.BaseEntry.ToString();
                                newgrnitem.ASNPOBaseId = dtl.BaseLine.ToString();
                                newgrnitem.BaseType = "ASN";

                                grn.GRNDetails.Add(newgrnitem);
                            }

                            // Start ver 1.0.11
                            if (grn.ASNNo != null)
                            {
                                grn.ASNNo = grn.ASNNo + ", " + asn.ASNDocNum;
                            }
                            else
                            {
                                grn.ASNNo = asn.ASNDocNum;
                            }
                            // End ver 1.0.11
                        }

                        if (grn.DocNum == null)
                        {
                             grn.DocNum = genCon.GenerateDocNum(DocTypeList.GRN, ObjectSpace, TransferType.NA, 0, docprefix);
                        }

                        // Start ver 1.0.8.1
                        string duppo = null;
                        string dupporef = null;
                        string dupasn = null;
                        // End ver 1.0.8.1
                        //foreach (GRNDetails dtl2 in grn.GRNDetails)
                        //{
                        //    dtl2.OIDKey = dtl2.Oid;

                            //if (dtl2.PONo != null)
                            //{
                            //    if (duppo != dtl2.PONo)
                            //    {
                            //        if (grn.SAPPONo == null)
                            //        {
                            //            grn.SAPPONo = dtl2.PONo;
                            //        }
                            //        else
                            //        {
                            //            grn.SAPPONo = grn.SAPPONo + ", " + dtl2.PONo;
                            //        }

                            //        duppo = dtl2.PONo;
                            //    }
                            //}

                            //if (dtl2.PORefNo != null)
                            //{
                            //    if (dupporef != dtl2.PORefNo)
                            //    {
                            //        if (grn.PortalPONo == null)
                            //        {
                            //            grn.PortalPONo = dtl2.PORefNo;
                            //        }
                            //        else
                            //        {
                            //            grn.PortalPONo = grn.PortalPONo + ", " + dtl2.PORefNo;
                            //        }

                            //        dupporef = dtl2.PORefNo;
                            //    }
                            //}

                            //if (dtl2.ASNBaseDoc != null)
                            //{
                            //    if (dupasn != dtl2.ASNBaseDoc)
                            //    {
                            //        if (grn.ASNNo == null)
                            //        {
                            //            grn.ASNNo = dtl2.ASNBaseDoc;
                            //        }
                            //        else
                            //        {
                            //            grn.ASNNo = grn.ASNNo + ", " + dtl2.ASNBaseDoc;
                            //        }

                            //        dupasn = dtl2.ASNBaseDoc;
                            //    }
                            //}
                        //}

                        ObjectSpace.CommitChanges();
                        ObjectSpace.Refresh();

                    // Start ver 1.0.11
                    IObjectSpace os = Application.CreateObjectSpace();
                    GRN trx = os.FindObject<GRN>(new BinaryOperator("Oid", grn.Oid));

                    foreach (GRNDetails dtl2 in trx.GRNDetails)
                    {
                        dtl2.OIDKey = dtl2.Oid;
                    }

                    // Start ver 1.0.13
                    foreach (GRNDetails dtl3 in trx.GRNDetails)
                    {
                        if (dtl3.ASNBaseDoc != null)
                        {
                            genCon.CloseASN(dtl3.ASNBaseDoc, "Copy", ObjectSpace);
                            break;
                        }
                    }
                    // End ver 1.0.13

                    if (trx.Oid > 0)
                    {
                        trx.PortalPONo = null;
                        trx.SAPPONo = null;
                        SqlConnection conn = new SqlConnection(genCon.getConnectionString());
                        string getporef = "SELECT PONo, ISNULL(PORefNo, '') FROM GRNDetails WHERE GRN = " + trx.Oid + " GROUP BY PONo, PORefNo";
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(getporef, conn);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetString(1) != "")
                            {
                                if (trx.PortalPONo != null)
                                {
                                    trx.PortalPONo = trx.PortalPONo + ", " + reader.GetString(1);
                                }
                                else
                                {
                                    trx.PortalPONo = reader.GetString(1);
                                }
                            }

                            if (trx.SAPPONo != null)
                            {
                                trx.SAPPONo = trx.SAPPONo + ", " + reader.GetString(0);
                            }
                            else
                            {
                                trx.SAPPONo = reader.GetString(0);
                            }
                        }
                        cmd.Dispose();
                        conn.Close();
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

        private void GRNCopyFromASN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            GRN grn = (GRN)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var viewId = Application.FindListViewId(typeof(vwASN));
            var cs = Application.CreateCollectionSource(os, typeof(vwASN), viewId);
            if (grn.Supplier != null)
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", grn.Supplier.BPCode);
            }
            else
            {
                cs.Criteria["CardCode"] = new BinaryOperator("CardCode", "");
            }

            var lv1 = Application.CreateListView(viewId, cs, true);
            e.View = lv1;
        }

        private void SubmitGRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            GRN selectedObject = (GRN)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            // Start ver 1.0.9
            foreach (GRNDetails dtl in selectedObject.GRNDetails)
            {
                if (dtl.Received > dtl.OpenQty)
                {
                    showMsg("Error", "Received Qty cannot more than Open Qty. Item : " + dtl.ItemCode, InformationType.Error);
                    return;
                }

                if (selectedObject.GRNDetails.Where(w => w.BaseType == "PO" && w.BaseDoc == dtl.BaseDoc && w.BaseId == dtl.BaseId).Count() > 1)
                {
                    showMsg("Error", "Duplicate Base Entry found.", InformationType.Error);
                    return;
                }

                if (selectedObject.GRNDetails.Where(w => w.BaseType == "ASN" && w.ASNPOBaseDoc == dtl.ASNPOBaseDoc 
                && w.ASNPOBaseId == dtl.ASNPOBaseId).Count() > 1)
                {
                    showMsg("Error", "Duplicate Base Entry found.", InformationType.Error);
                    return;
                }
            }

            if (selectedObject.GRNDetails.Where(x => x.Received == 0).Count() == selectedObject.GRNDetails.Count())
            {
                showMsg("Error", "No receiving qty in this document.", InformationType.Error);
                return;
            }
            // End ver 1.0.9

            if (selectedObject.InvoiceNo != null)
            {
                if (selectedObject.IsValid == true)
                {
                    //if (selectedObject.IsValid1 == false)
                    //{
                    selectedObject.Status = DocStatus.Submitted;
                    GRNDocTrail ds = ObjectSpace.CreateObject<GRNDocTrail>();
                    ds.DocStatus = DocStatus.Submitted;
                    ds.DocRemarks = p.ParamString;
                    selectedObject.GRNDocTrail.Add(ds);

                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();

                    foreach (GRNDetails dtl in selectedObject.GRNDetails)
                    {
                        if (dtl.ASNBaseDoc != null)
                        {
                            bool closed = true;
                            IObjectSpace asnos = Application.CreateObjectSpace();
                            ASN asn = asnos.FindObject<ASN>(new BinaryOperator("DocNum", dtl.ASNBaseDoc));

                            if (asn != null)
                            {
                                foreach (ASNDetails asndetail in asn.ASNDetails)
                                {
                                    // Start ver 1.0.10
                                    // Start ver 1.0.6 (UAT)
                                    //if (asndetail.Oid.ToString() == dtl.ASNBaseId)
                                    //if (asndetail.Oid.ToString() == dtl.ASNBaseId && dtl.Received >= asndetail.UnloadQty)
                                    //// End ver 1.0.6 (UAT)
                                    //{
                                    //    asndetail.LineClosed = true;
                                    //}

                                    if (asndetail.Oid.ToString() == dtl.ASNBaseId)
                                    {
                                        asndetail.CopyToQty = asndetail.CopyToQty - (asndetail.CopyToQty - dtl.Received);
                                        asndetail.CopyTotalQty = asndetail.CopyTotalQty + dtl.Received;
                                        // Start ver 1.0.13
                                        asndetail.OutstandingQty = asndetail.UnloadQty - dtl.Received;
                                        // End ver 1.0.13
                                    }

                                    if (asndetail.Oid.ToString() == dtl.ASNBaseId && asndetail.CopyTotalQty >= asndetail.UnloadQty)
                                    {
                                        asndetail.LineClosed = true;
                                        asndetail.CopyToQty = asndetail.CopyTotalQty;
                                    }
                                    // End ver 1.0.10
                                }
                            }

                            if (asn.ASNDetails.Where(x => x.LineClosed == false).Count() > 0)
                            {
                                closed = false;
                            }

                            if (closed == true)
                            {
                                asn.Status = DocStatus.Closed;
                            }

                            asnos.CommitChanges();
                            asnos.Refresh();
                        }
                    }

                    IObjectSpace os = Application.CreateObjectSpace();
                    GRN trx = os.FindObject<GRN>(new BinaryOperator("Oid", selectedObject.Oid));
                    openNewView(os, trx, ViewEditMode.View);
                    showMsg("Successful", "Submit Done.", InformationType.Success);
                    //}
                    //else
                    //{
                    //    showMsg("Error", "Received qty cannot be zero.", InformationType.Error);
                    //}
                }
                else
                {
                    showMsg("Error", "No Content.", InformationType.Error);
                }
            }
            else
            {
                showMsg("Error", "Invoice number cannot blank.", InformationType.Error);
            }
        }

        private void SubmitGRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void CancelGRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            GRN selectedObject = (GRN)e.CurrentObject;
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            selectedObject.Status = DocStatus.Cancelled;
            GRNDocTrail ds = ObjectSpace.CreateObject<GRNDocTrail>();
            ds.DocStatus = DocStatus.Cancelled;
            ds.DocRemarks = p.ParamString;
            selectedObject.GRNDocTrail.Add(ds);

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            // Start ver 1.0.13
            foreach (GRNDetails dtl in selectedObject.GRNDetails)
            {
                if (dtl.ASNBaseDoc != null)
                {
                    IObjectSpace asnos = Application.CreateObjectSpace();
                    ASN asn = asnos.FindObject<ASN>(new BinaryOperator("DocNum", dtl.ASNBaseDoc));

                    if (asn != null)
                    {
                        foreach (ASNDetails asndetail in asn.ASNDetails)
                        {
                            if (asndetail.Oid.ToString() == dtl.ASNBaseId)
                            {
                                asndetail.CopyToQty = asndetail.CopyToQty - dtl.OpenQty;
                                asndetail.OutstandingQty = dtl.OpenQty - asndetail.CopyToQty;
                                break;
                            }
                        }
                    }

                    asnos.CommitChanges();
                    asnos.Refresh();
                }
            }
            // End ver 1.0.13

            IObjectSpace os = Application.CreateObjectSpace();
            GRN trx = os.FindObject<GRN>(new BinaryOperator("Oid", selectedObject.Oid));
            openNewView(os, trx, ViewEditMode.View);
            showMsg("Successful", "Cancel Done.", InformationType.Success);
        }

        private void CancelGRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace(typeof(StringParameters));
            StringParameters message = os.CreateObject<StringParameters>();

            DetailView dv = Application.CreateDetailView(os, message);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = false;
            ((StringParameters)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            e.View = dv;
        }

        private void PreviewGRN_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            GRN grn = (GRN)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\GRN.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("dockey@", grn.Oid);
                doc.SetParameterValue("dbName@", conn.Database);

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + grn.Oid + "_" + user.UserName + "_GRN_"
                    + DateTime.Parse(grn.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";

                doc.ExportToDisk(ExportFormatType.PortableDocFormat, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + grn.Oid + "_" + user.UserName + "_GRN_"
                    + DateTime.Parse(grn.DocDate.ToString()).ToString("yyyyMMdd") + ".pdf";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);

                grn.PrintStatus = PrintStatus.Printed;

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();

            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }
        }

        private void ExportGRN_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string strServer;
            string strDatabase;
            string strUserID;
            string strPwd;
            string filename;

            SqlConnection conn = new SqlConnection(genCon.getConnectionString());
            GRN grn = (GRN)View.CurrentObject;
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;

            try
            {
                ReportDocument doc = new ReportDocument();
                strServer = ConfigurationManager.AppSettings.Get("SQLserver").ToString();
                doc.Load(HttpContext.Current.Server.MapPath("~\\Reports\\GRNImportFormat.rpt"));
                strDatabase = conn.Database;
                strUserID = ConfigurationManager.AppSettings.Get("SQLID").ToString();
                strPwd = ConfigurationManager.AppSettings.Get("SQLPass").ToString();
                doc.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                doc.Refresh();

                doc.SetParameterValue("DocNum", grn.DocNum);
                doc.SetParameterValue("Type", "StarLaiPortal.Module.BusinessObjects.GRN.GRNDetails");

                filename = ConfigurationManager.AppSettings.Get("ReportPath").ToString() + conn.Database
                    + "_" + grn.DocNum + "_" + user.UserName + "_GRNImport_" + ".xls";

                doc.ExportToDisk(ExportFormatType.Excel, filename);
                doc.Close();
                doc.Dispose();

                string url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                    ConfigurationManager.AppSettings.Get("PrintPath").ToString() + conn.Database
                    + "_" + grn.DocNum + "_" + user.UserName + "_GRNImport_" + ".xls";
                var script = "window.open('" + url + "');";

                WebWindow.CurrentRequestWindow.RegisterStartupScript("DownloadFile", script);
            }
            catch (Exception ex)
            {
                showMsg("Fail", ex.Message, InformationType.Error);
            }
        }

        private void ImportGRN_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        private void ImportGRN_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            GRN trx = (GRN)View.CurrentObject;

            var os = Application.CreateObjectSpace();
            var solution = os.CreateObject<ImportData>();
            solution.Option = new ImportOption();

            solution.Option.UpdateProgress = (x) => solution.Progress = x;
            solution.Option.DocNum = trx.DocNum;
            solution.Option.ConnectionString = genCon.getConnectionString();
            solution.Option.Type = "GoodsReceiptPO";

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
