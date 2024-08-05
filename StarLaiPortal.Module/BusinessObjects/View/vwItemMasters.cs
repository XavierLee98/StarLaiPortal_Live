﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

// 2023-09-25 add printing uom ver 1.0.10
// 2024-06-12 e-invoice - ver 1.0.18

namespace StarLaiPortal.Module.BusinessObjects.View
{
    [DefaultClassOptions]
    [NavigationItem("SAP")]
    [XafDisplayName("Item Masters")]
    [DefaultProperty("BoFullName")]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("hideSave", AppearanceItemType = "Action", TargetItems = "Save", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    public class vwItemMasters : XPLiteObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public vwItemMasters(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        [Key]
        [Browsable(true)]
        [XafDisplayName("Item Code")]
        [Appearance("ItemCode", Enabled = false)]
        [Index(0), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string ItemCode
        {
            get; set;
        }

        [XafDisplayName("Item Name")]
        [Appearance("ItemName", Enabled = false)]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string ItemName
        {
            get; set;
        }

        [XafDisplayName("UOM Group")]
        [Appearance("UOM", Enabled = false)]
        [Index(4), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string UOM
        {
            get; set;
        }

        [XafDisplayName("ManBtchNum")]
        [Appearance("ManBtchNum", Enabled = false)]
        [Index(5), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string ManBtchNum
        {
            get; set;
        }

        [XafDisplayName("ManSerNum")]
        [Appearance("ManSerNum", Enabled = false)]
        [Index(8), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string ManSerNum
        {
            get; set;
        }

        [XafDisplayName("PrchseItem")]
        [Appearance("PrchseItem", Enabled = false)]
        [Index(10), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string PrchseItem
        {
            get; set;
        }
        [XafDisplayName("InvntItem")]
        [Appearance("InvntItem", Enabled = false)]
        [Index(13), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string InvntItem
        {
            get; set;
        }

        [XafDisplayName("SellItem")]
        [Appearance("SellItem", Enabled = false)]
        [Index(15), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string SellItem
        {
            get; set;
        }

        [XafDisplayName("Def. Barcode")]
        [Appearance("DefBarcode", Enabled = false)]
        [Index(18), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string DefBarcode
        {
            get; set;
        }

        [XafDisplayName("Catalog No")]
        [Appearance("CatalogNo", Enabled = false)]
        [Index(20), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string CatalogNo
        {
            get; set;
        }

        [XafDisplayName("Model")]
        [Appearance("Model", Enabled = false)]
        [Index(23), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Model
        {
            get; set;
        }

        [XafDisplayName("Legacy Item Code")]
        [Appearance("LegacyItemCode", Enabled = false)]
        [Index(25), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        public string LegacyItemCode
        {
            get; set;
        }

        [XafDisplayName("frozenFor")]
        [Appearance("frozenFor", Enabled = false)]
        [Index(28), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string frozenFor
        {
            get; set;
        }

        [XafDisplayName("LabelType")]
        [Appearance("LabelType", Enabled = false)]
        [Index(30), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string LabelType
        {
            get; set;
        }

        [XafDisplayName("Frgn Name")]
        [Appearance("FrgnName", Enabled = false)]
        [Index(33), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string FrgnName
        {
            get; set;
        }

        [XafDisplayName("Picture Name")]
        [Appearance("PicturName", Enabled = false)]
        [Index(35), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string PicturName
        {
            get; set;
        }

        [XafDisplayName("Last Purchase Price")]
        [Appearance("LastPurchasePrice", Enabled = false)]
        [Index(38), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal LastPurchasePrice
        {
            get; set;
        }

        // Start ver 1.0.10
        [XafDisplayName("Print UOM")]
        [Appearance("PrintUoM", Enabled = false)]
        [Index(40), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string PrintUoM
        {
            get; set;
        }
        // End ver 1.0.10

        // Start ver 1.0.18
        [XafDisplayName("U_EIV_ClassificationS")]
        [Appearance("U_EIV_ClassificationS", Enabled = false)]
        [Index(43), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string U_EIV_ClassificationS
        {
            get; set;
        }
        // End ver 1.0.18

        [Index(30), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return ItemCode + "-" + ItemName; }
        }
    }
}