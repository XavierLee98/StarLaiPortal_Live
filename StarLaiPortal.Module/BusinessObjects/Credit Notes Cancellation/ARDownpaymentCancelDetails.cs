﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using StarLaiPortal.Module.BusinessObjects.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

// 2023-08-16 Add reason code ver 1.0.8
// 2024-05-16 enhance speed - ver 1.0.15

namespace StarLaiPortal.Module.BusinessObjects.Credit_Notes_Cancellation
{
    [DefaultClassOptions]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("AR Downpayment Cancellation Details")]
    public class ARDownpaymentCancelDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public ARDownpaymentCancelDetails(Session session)
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
            Quantity = 1;
        }
        private ApplicationUser _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
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
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
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
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private vwItemMasters _ItemCode;
        [ImmediatePostData]
        [NoForeignKey]
        [XafDisplayName("Item Code")]
        [DataSourceCriteria("frozenFor = 'N'")]
        // Start ver 1.0.15
        //[LookupEditorMode(LookupEditorMode.AllItems)]
        // End ver 1.0.15
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("ItemCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwItemMasters ItemCode

        {
            get { return _ItemCode; }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
                if (!IsLoading && value != null)
                {
                    ItemDesc = ItemCode.ItemName;
                    UOM = ItemCode.UOM;
                    LegacyItemCode = ItemCode.LegacyItemCode;
                    if (Customer != null)
                    {
                        vwPrice tempprice = Session.FindObject<vwPrice>(CriteriaOperator.Parse("ItemCode = ? and PriceList = ?",
                            ItemCode.ItemCode, Customer.ListNum));

                        if (tempprice != null)
                        {
                            Price = tempprice.Price;
                        }
                    }
                }
                else if (!IsLoading && value == null)
                {
                    ItemDesc = null;
                    UOM = null;
                    LegacyItemCode = null;
                    Price = 0;
                }
            }
        }

        private string _LegacyItemCode;
        [XafDisplayName("Legacy Item Code")]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("LegacyItemCode", Enabled = false)]
        public string LegacyItemCode
        {
            get { return _LegacyItemCode; }
            set
            {
                SetPropertyValue("LegacyItemCode", ref _LegacyItemCode, value);
            }
        }

        private string _ItemDesc;
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Item Description")]
        [Appearance("ItemDesc", Enabled = false)]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemDesc
        {
            get { return _ItemDesc; }
            set
            {
                SetPropertyValue("ItemDesc", ref _ItemDesc, value);
            }
        }

        private string _ItemDetails;
        [XafDisplayName("Item Details")]
        [Appearance("ItemDetails", Enabled = false)]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemDetails
        {
            get { return _ItemDetails; }
            set
            {
                SetPropertyValue("ItemDetails", ref _ItemDetails, value);
            }
        }

        private string _UOM;
        [XafDisplayName("UOM Group")]
        [Appearance("UOM", Enabled = false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string UOM
        {
            get { return _UOM; }
            set
            {
                SetPropertyValue("UOM", ref _UOM, value);
            }
        }

        // Start ver 1.0.8
        private vwReasonCode _ReasonCode;
        [NoForeignKey]
        [ImmediatePostData]
        [DataSourceCriteria("Type = 'SalesReturn'")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Reason Code")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwReasonCode ReasonCode
        {
            get { return _ReasonCode; }
            set
            {
                SetPropertyValue("ReasonCode", ref _ReasonCode, value);
            }
        }
        // End ver 1.0.8

        private vwWarehouse _Warehouse;
        [NoForeignKey]
        [ImmediatePostData]
        // Start ver 1.0.15
        //[LookupEditorMode(LookupEditorMode.AllItems)]
        // End ver 1.0.15
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Warehouse")]
        [DataSourceCriteria("Inactive = 'N'")]
        [Appearance("Warehouse", Enabled = false)]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwWarehouse Warehouse
        {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
                if (!IsLoading && value != null)
                {
                    Bin = Session.FindObject<vwBin>(CriteriaOperator.Parse("AbsEntry = ?", Warehouse.DftBinAbs));
                }
                else if (!IsLoading && value == null)
                {
                    Bin = null;
                }

            }
        }

        private vwBin _Bin;
        [NoForeignKey]
        // Start ver 1.0.15
        //[LookupEditorMode(LookupEditorMode.AllItems)]
        // End ver 1.0.15
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Bin")]
        [Appearance("Bin", Enabled = false)]
        [DataSourceCriteria("Warehouse = '@this.FromWarehouse.WarehouseCode'")]
        [Index(14), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public vwBin Bin
        {
            get { return _Bin; }
            set
            {
                SetPropertyValue("Bin", ref _Bin, value);
            }
        }

        private decimal _Quantity;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:N0}")]
        [ModelDefault("EditMask", "d")]
        [XafDisplayName("Quantity")]
        [Appearance("Quantity", Enabled = false)]
        [Index(15), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading && value != 0)
                {
                    Total = Quantity * Price;
                }
            }
        }

        private decimal _Price;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Price")]
        [Appearance("Price", Enabled = false)]
        [Index(18), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Price
        {
            get { return _Price; }
            set
            {
                SetPropertyValue("Price", ref _Price, value);
                if (!IsLoading && value != 0)
                {
                    Total = Quantity * Price;
                }
            }
        }

        private decimal _Total;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Total")]
        [Appearance("Total", Enabled = false)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Total
        {
            get { return _Total; }
            set
            {
                SetPropertyValue("Total", ref _Total, value);
            }
        }

        private string _BaseDoc;
        [XafDisplayName("BaseDoc")]
        [Index(23), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string BaseDoc
        {
            get { return _BaseDoc; }
            set
            {
                SetPropertyValue("BaseDoc", ref _BaseDoc, value);
            }
        }

        private string _BaseId;
        [XafDisplayName("BaseId")]
        [Index(25), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string BaseId
        {
            get { return _BaseId; }
            set
            {
                SetPropertyValue("BaseId", ref _BaseId, value);
            }
        }

        private string _SODocNum;
        [XafDisplayName("SO Doc Num")]
        [Index(26), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string SODocNum
        {
            get { return _SODocNum; }
            set
            {
                SetPropertyValue("SODocNum", ref _SODocNum, value);
            }
        }

        private string _ARDownpaymentDoc;
        [XafDisplayName("ARDownpaymentDoc")]
        [Index(27), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public string ARDownpaymentDoc
        {
            get { return _ARDownpaymentDoc; }
            set
            {
                SetPropertyValue("ARDownpaymentDoc", ref _ARDownpaymentDoc, value);
            }
        }

        private vwBusniessPartner _Customer;
        [NoForeignKey]
        [XafDisplayName("Customer")]
        [Appearance("Customer", Enabled = false)]
        [Index(28), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public vwBusniessPartner Customer
        {
            get { return _Customer; }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        private bool _SapPayment;
        [XafDisplayName("SapPayment")]
        [Index(85), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool SapPayment
        {
            get { return _SapPayment; }
            set
            {
                SetPropertyValue("SapPayment", ref _SapPayment, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private ARDownpaymentCancel _ARDownpaymentCancel;
        [Association("ARDownpaymentCancel-ARDownpaymentCancelDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("ARDownpaymentCancel", Enabled = false)]
        public ARDownpaymentCancel ARDownpaymentCancel
        {
            get { return _ARDownpaymentCancel; }
            set { SetPropertyValue("ARDownpaymentCancel", ref _ARDownpaymentCancel, value); }
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

                }
            }
        }
    }
}