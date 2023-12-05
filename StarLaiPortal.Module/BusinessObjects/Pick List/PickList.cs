﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using StarLaiPortal.Module.BusinessObjects.Setup;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

// 2023-08-25 add picklistactual validation ver 1.0.9
// 2023-04-09 fix speed issue ver 1.0.8.1
// 2023-11-27 add validation to block submit if pickqty is zero ver 1.0.13
// 2023-12-01 show createtime in list view ver 1.0.13

namespace StarLaiPortal.Module.BusinessObjects.Pick_List
{
    [DefaultClassOptions]
    [XafDisplayName("Pick List")]
    [NavigationItem("Pick List")]
    [DefaultProperty("DocNum")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSubmit", AppearanceItemType.Action, "True", TargetItems = "SubmitPL", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "CancelPL", Criteria = "not (Status in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class PickList : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public PickList(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;
            if (user != null)
            {
                CreateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
            }
            else
            {
                CreateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
            }
            CreateDate = DateTime.Now;
            DocDate = DateTime.Now;
            DeliveryDate = DateTime.Now;
            Status = DocStatus.Draft;
            //Picker = Session.GetObjectByKey<ApplicationUser>(user.Oid);
        }

        private ApplicationUser _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("CreateUser", Enabled = false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public ApplicationUser CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        // Start ver 1.0.13
        //[Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Index(301), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:hh:mm tt}")]
        // End ver 1.0.13
        [Appearance("CreateDate", Enabled = false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private ApplicationUser _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("UpdateUser", Enabled = false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public ApplicationUser UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateDate", Enabled = false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private string _DocNum;
        [XafDisplayName("Pick List Num")]
        [Appearance("DocNum", Enabled = false)]
        [Index(0), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private ApplicationUser _Picker;
        [XafDisplayName("Picker")]
        [NoForeignKey]
        [ImmediatePostData]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceCriteria("Staff.IsActive = 'True'")]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public ApplicationUser Picker
        {
            get { return _Picker; }
            set
            {
                SetPropertyValue("Picker", ref _Picker, value);
            }
        }

        private DateTime _DeliveryDate;
        [XafDisplayName("Delivery Date")]
        [Index(4), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DeliveryDate
        {
            get { return _DeliveryDate; }
            set
            {
                SetPropertyValue("DeliveryDate", ref _DeliveryDate, value);
            }
        }

        private DateTime _DocDate;
        [XafDisplayName("Doc Date")]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DocDate

        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);
            }
        }

        private vwWarehouse _Warehouse;
        [XafDisplayName("Warehouse")]
        [NoForeignKey]
        [ImmediatePostData]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceCriteria("Inactive = 'N'")]
        [Appearance("Warehouse", Enabled = false, Criteria = "IsValid6")]
        [Index(6), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwWarehouse Warehouse
        {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
            }
        }

        private vwTransporter _Transporter;
        [XafDisplayName("Transporter")]
        [NoForeignKey]
        [ImmediatePostData]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(7), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwTransporter Transporter
        {
            get { return _Transporter; }
            set
            {
                SetPropertyValue("Transporter", ref _Transporter, value);
            }
        }

        private string _Remarks;
        [XafDisplayName("Remarks")]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private vwDriver _Driver;
        [NoForeignKey]
        [XafDisplayName("Driver")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(9), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwDriver Driver
        {
            get { return _Driver; }
            set
            {
                SetPropertyValue("Driver", ref _Driver, value);
            }
        }

        private vwVehicle _Vehicle;
        [NoForeignKey]
        [XafDisplayName("Vehicle")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public vwVehicle Vehicle
        {
            get { return _Vehicle; }
            set
            {
                SetPropertyValue("Vehicle", ref _Vehicle, value);
            }
        }

        private string _CustomerGroup;
        [XafDisplayName("Customer Group")]
        [Appearance("CustomerGroup", Enabled = false)]
        [Index(13), VisibleInDetailView(false), VisibleInListView(true), VisibleInLookupListView(false)]
        public string CustomerGroup
        {
            get { return _CustomerGroup; }
            set
            {
                SetPropertyValue("CustomerGroup", ref _CustomerGroup, value);
            }
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _Customer;
        // End ver 1.0.8.1
        [XafDisplayName("Customer")]
        [Index(14), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Customer", Enabled = false)]
        public string Customer
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;
            //    foreach (PickListDetails dtl in this.PickListDetails)
            //    {
            //        rtn = dtl.Customer.BPCode;
            //        break;
            //    }

            //    return rtn;
            //}
            get { return _Customer; }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
            // End ver 1.0.8.1
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _CustomerName;
        // End ver 1.0.8.1
        [XafDisplayName("Customer Name")]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("CustomerName", Enabled = false)]
        public string CustomerName
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;
            //    foreach (PickListDetails dtl in this.PickListDetails)
            //    {
            //        rtn = dtl.Customer.BPName;
            //        break;
            //    }

            //    return rtn;
            //}
            get { return _CustomerName; }
            set
            {
                SetPropertyValue("CustomerName", ref _CustomerName, value);
            }
            // End ver 1.0.8.1
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _SONumber;
        // End ver 1.0.8.1
        [XafDisplayName("SO No.")]
        [Index(16), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("SONumber", Enabled = false)]
        public string SONumber
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;
            //    string dupso = null;
            //    foreach (PickListDetails dtl in this.PickListDetails)
            //    {
            //        if (dupso != dtl.SOBaseDoc)
            //        {
            //            if (rtn == null)
            //            {
            //                rtn = dtl.SOBaseDoc;
            //            }
            //            else
            //            {
            //                rtn = rtn + ", " + dtl.SOBaseDoc;
            //            }

            //            dupso = dtl.SOBaseDoc;
            //        }
            //    }

            //    return rtn;
            //}
            get { return _SONumber; }
            set
            {
                SetPropertyValue("SONumber", ref _SONumber, value);
            }
            // End ver 1.0.8.1
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private string _SODeliveryDate;
        // End ver 1.0.8.1
        [XafDisplayName("SO Delivery Date")]
        [Index(17), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("SODeliveryDate", Enabled = false)]
        public string SODeliveryDate
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    string rtn = null;

            //    if (this.PickListDetails.Count() > 0)
            //    {
            //        rtn = this.PickListDetails.Where(x => x.SOBaseDoc != null).OrderBy(c => c.SODeliveryDate).Min().SODeliveryDate.Date.ToString();
            //    }

            //    if (rtn != null)
            //    {
            //        return rtn.Substring(0, 10);
            //    }
            //    else
            //    {
            //        return rtn;
            //    }
            //}
            get { return _SODeliveryDate; }
            set
            {
                SetPropertyValue("SODeliveryDate", ref _SODeliveryDate, value);
            }
            // End ver 1.0.8.1
        }

        // Start ver 1.0.8.1
        //[NonPersistent]
        private PriorityType _Priority;
        // End ver 1.0.8.1
        [XafDisplayName("Priority")]
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Priority", Enabled = false)]
        public PriorityType Priority
        {
            // Start ver 1.0.8.1
            //get
            //{
            //    PriorityType rtn = null;

            //    if (this.PickListDetails.Count() > 0)
            //    {
            //        rtn = this.PickListDetails.Where(x => x.SOBaseDoc != null).OrderBy(c => c.Priority).Max().Priority;
            //    }

            //    return rtn;
            //}
            get { return _Priority; }
            set
            {
                SetPropertyValue("Priority", ref _Priority, value);
            }
            // End ver 1.0.8.1
        }

        private DocStatus _Status;
        [XafDisplayName("Status")]
        [Appearance("Status", Enabled = false)]
        [Index(29), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DocStatus Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        private PrintStatus _PrintStatus;
        [XafDisplayName("Print Status")]
        [Appearance("PrintStatus", Enabled = false)]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public PrintStatus PrintStatus
        {
            get { return _PrintStatus; }
            set
            {
                SetPropertyValue("PrintStatus", ref _PrintStatus, value);
            }
        }

        private int _PrintCount;
        [XafDisplayName("Print Count")]
        [Appearance("PrintCount", Enabled = false)]
        [Index(33), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public int PrintCount
        {
            get { return _PrintCount; }
            set
            {
                SetPropertyValue("PrintCount", ref _PrintCount, value);
            }
        }

        private bool _Sap;
        [XafDisplayName("Sap")]
        [Index(80), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool Sap
        {
            get { return _Sap; }
            set
            {
                SetPropertyValue("Sap", ref _Sap, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                bool detail = false;
                bool actual = false;
                foreach (PickListDetails dtl in this.PickListDetails)
                {
                    detail = true;
                }

                foreach (PickListDetailsActual dtl2 in this.PickListDetailsActual)
                {
                    actual = true;
                }

                if (detail == true && actual == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Browsable(false)]
        public bool IsValid1
        {
            get
            {
                foreach (PickListDetailsActual dtl in this.PickListDetailsActual)
                {
                    if (dtl.FromBin == null || dtl.ToBin == null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid3
        {
            get
            {
                foreach (PickListDetails dtl in this.PickListDetails)
                {
                    if (dtl.SOBaseDoc != null)
                    {
                        SalesOrder so;
                        so = Session.FindObject<SalesOrder>(CriteriaOperator.Parse("DocNum = ?", dtl.SOBaseDoc));

                        if (so != null)
                        {
                            if (so.Series.SeriesName == "BackOrdP" || so.Series.SeriesName == "BackOrdS")
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        [Browsable(false)]
        public bool IsValid4
        {
            get
            {
                if (PickListDetails.GroupBy(x => x.Customer).Count() > 1)
                {
                    return true;
                }

                return false;
            }
        }

        // Start ver 1.0.9
        [Browsable(false)]
        public bool IsValid5
        {
            get
            {
                foreach(PickListDetails dtl in this.PickListDetails)
                {
                    if (dtl.PickQty > dtl.PlanQty)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        // End ver 1.0.9

        [Browsable(false)]
        public bool IsValid6
        {
            get
            {
                foreach (PickListDetails dtl in this.PickListDetails)
                {
                    return true;
                }

                return false;
            }
        }

        // Start ver 1.0.13
        [Browsable(false)]
        public bool IsValid7
        {
            get
            {
                foreach (PickListDetails dtl in this.PickListDetails)
                {
                    if (dtl.PickQty <= 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        // End ver 1.0.13

        [Association("PickList-PickListDetails")]
        [XafDisplayName("Plan")]
        public XPCollection<PickListDetails> PickListDetails
        {
            get { return GetCollection<PickListDetails>("PickListDetails"); }
        }

        [Association("PickList-PickListDetailsActual")]
        [XafDisplayName("Actual")]
        public XPCollection<PickListDetailsActual> PickListDetailsActual
        {
            get { return GetCollection<PickListDetailsActual>("PickListDetailsActual"); }
        }

        [Association("PickList-PickListDocTrail")]
        [XafDisplayName("Document Trail")]
        public XPCollection<PickListDocTrail> PickListDocTrail
        {
            get { return GetCollection<PickListDocTrail>("PickListDocTrail"); }
        }

        [Association("PickList-PickListAttachment")]
        [XafDisplayName("Attachment")]
        public XPCollection<PickListAttachment> PickListAttachment
        {
            get { return GetCollection<PickListAttachment>("PickListAttachment"); }
        }

        private XPCollection<AuditDataItemPersistent> auditTrail;
        public XPCollection<AuditDataItemPersistent> AuditTrail
        {
            get
            {
                if (auditTrail == null)
                {
                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return auditTrail;
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                ApplicationUser user = (ApplicationUser)SecuritySystem.CurrentUser;
                if (user != null)
                {
                    UpdateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
                }
                else
                {
                    UpdateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                }
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {
                    PickListDocTrail ds = new PickListDocTrail(Session);
                    ds.DocStatus = DocStatus.Draft;
                    ds.DocRemarks = "";
                    if (user != null)
                    {
                        ds.CreateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
                        ds.UpdateUser = Session.GetObjectByKey<ApplicationUser>(user.Oid);
                    }
                    else
                    {
                        ds.CreateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                        ds.UpdateUser = Session.GetObjectByKey<ApplicationUser>(Guid.Parse("100348B5-290E-47DF-9355-557C7E2C56D3"));
                    }
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateDate = DateTime.Now;
                    this.PickListDocTrail.Add(ds);
                }
            }
        }
    }
}