using DevExpress.Data.Filtering;
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
using StarLaiPortal.Module.BusinessObjects.Inquiry_View;
using StarLaiPortal.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarLaiPortal.Module.BusinessObjects.Sales_Order;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using StarLaiPortal.Module.BusinessObjects.Sales_Quotation;
using DevExpress.Xpo.DB;

namespace StarLaiPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DocumentControllers : ViewController
    {
        private DateTime Fromdate;
        private DateTime Todate;
        public DocumentControllers()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.DocumentDateFrom.Active.SetItemValue("Enabled", false);
            this.DocumentDateTo.Active.SetItemValue("Enabled", false);
            this.DocumentStatus.Active.SetItemValue("Enabled", false);
            this.DocumentFilter.Active.SetItemValue("Enabled", false);

            if (typeof(SalesOrder).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(SalesOrder))
                {
                    if (View.Id == "SalesOrder_ListView")
                    {
                        DocumentStatus.Items.Clear();

                        DocumentStatus.Items.Add(new ChoiceActionItem("All", "All"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Open", "Open"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Draft", "Draft"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Submitted", "Submitted"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Cancelled", "Cancelled"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Closed", "Closed"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Posted", "Posted"));
                        DocumentStatus.Items.Add(new ChoiceActionItem("Pending Post", "Pending Post"));

                        DocumentStatus.SelectedIndex = 1;

                        this.DocumentStatus.Active.SetItemValue("Enabled", true);
                        DocumentStatus.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        DocumentStatus.CustomizeControl += action_CustomizeControl;

                        this.DocumentDateFrom.Active.SetItemValue("Enabled", true);
                        this.DocumentDateFrom.Value = DateTime.Today.AddMonths(-3);
                        DocumentDateFrom.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.DocumentDateFrom.CustomizeControl += DateActionFrom_CustomizeControl;

                        this.DocumentDateTo.Active.SetItemValue("Enabled", true);
                        this.DocumentDateTo.Value = DateTime.Today;
                        DocumentDateTo.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.DocumentDateTo.CustomizeControl += DateActionTo_CustomizeControl;

                        this.DocumentFilter.Active.SetItemValue("Enabled", true);
                    }
                }
            }

            if (typeof(SalesQuotation).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (View.ObjectTypeInfo.Type == typeof(SalesQuotation))
                {
                    if (View.Id == "SalesQuotation_ListView")
                    {
                        this.DocumentDateFrom.Active.SetItemValue("Enabled", true);
                        this.DocumentDateFrom.Value = DateTime.Today.AddMonths(-3);
                        DocumentDateFrom.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.DocumentDateFrom.CustomizeControl += DateActionFrom_CustomizeControl;
                        this.DocumentDateFrom.Execute += DocumentDateFrom_Execute;

                        this.DocumentDateTo.Active.SetItemValue("Enabled", true);
                        this.DocumentDateTo.Value = DateTime.Today;
                        DocumentDateTo.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
                        this.DocumentDateTo.CustomizeControl += DateActionTo_CustomizeControl;
                        this.DocumentDateTo.Execute += DocumentDateTo_Execute;

                        this.DocumentFilter.Active.SetItemValue("Enabled", true);
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void DateActionFrom_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            ParametrizedActionMenuActionItem actionItem = e.Control as ParametrizedActionMenuActionItem;

            if (actionItem != null)
            {
                ASPxDateEdit dateEdit = actionItem.Control.Editor as ASPxDateEdit;
                if (dateEdit != null)
                {
                    dateEdit.Width = 110;
                    dateEdit.Buttons.Clear();
                    if (dateEdit.Text != "")
                    {
                        Fromdate = Convert.ToDateTime(dateEdit.Text);
                    }
                }
            }
        }

        private void DateActionTo_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            ParametrizedActionMenuActionItem actionItem = e.Control as ParametrizedActionMenuActionItem;

            if (actionItem != null)
            {
                ASPxDateEdit dateEdit = actionItem.Control.Editor as ASPxDateEdit;
                if (dateEdit != null)
                {
                    dateEdit.Width = 110;
                    dateEdit.Buttons.Clear();
                    if (dateEdit.Text != "")
                    {
                        Todate = Convert.ToDateTime(dateEdit.Text);
                    }
                }
            }
        }

        void action_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            SingleChoiceActionAsModeMenuActionItem actionItem = e.Control as SingleChoiceActionAsModeMenuActionItem;
            if (actionItem != null && actionItem.Action.PaintStyle == DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption)
            {
                DropDownSingleChoiceActionControlBase control = (DropDownSingleChoiceActionControlBase)actionItem.Control;
                control.Label.Text = actionItem.Action.Caption;
                control.Label.Style["padding-right"] = "5px";
                control.ComboBox.Width = 100;
            }
        }

        private void DocumentDateFrom_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (View.ObjectTypeInfo.Type == typeof(SalesOrder))
            {
                if (View.Id == "SalesOrder_ListView")
                {
                    if (DocumentStatus.SelectedItem.Id != "All")
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                        "and DocDate >= ? and DocDate <= ?", DocumentStatus.SelectedItem.Id, Fromdate, Todate);
                    }
                    else
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?",
                            Fromdate, Todate);
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(SalesQuotation))
            {
                if (View.Id == "SalesQuotation_ListView")
                {
                    ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?", Fromdate, Todate);
                }
            }
        }

        private void DocumentDateTo_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            if (View.ObjectTypeInfo.Type == typeof(SalesOrder))
            {
                if (View.Id == "SalesOrder_ListView")
                {
                    if (DocumentStatus.SelectedItem.Id != "All")
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                        "and DocDate >= ? and DocDate <= ?", DocumentStatus.SelectedItem.Id, Fromdate, Todate);
                    }
                    else
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?",
                            Fromdate, Todate);
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(SalesQuotation))
            {
                if (View.Id == "SalesQuotation_ListView")
                {
                    ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?", Fromdate, Todate);
                }
            }
        }

        private void DocumentStatus_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (View.ObjectTypeInfo.Type == typeof(SalesOrder))
            {
                if (View.Id == "SalesOrder_ListView")
                {
                    if (DocumentStatus.SelectedItem.Id != "All")
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                            "and DocDate >= ? and DocDate <= ?", DocumentStatus.SelectedItem.Id, Fromdate, Todate);
                    }
                    else
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?",
                            Fromdate, Todate);
                    }
                }
            }
        }

        private void DocumentFilter_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (View.ObjectTypeInfo.Type == typeof(SalesOrder))
            {
                if (View.Id == "SalesOrder_ListView")
                {
                    if (DocumentStatus.SelectedItem.Id != "All")
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("[Status] = ? " +
                            "and DocDate >= ? and DocDate <= ?", DocumentStatus.SelectedItem.Id, Fromdate, Todate);
                    }
                    else
                    {
                        ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?", 
                           Fromdate, Todate);
                    }
                }
            }

            if (View.ObjectTypeInfo.Type == typeof(SalesQuotation))
            {
                if (View.Id == "SalesQuotation_ListView")
                {
                    ((ListView)View).CollectionSource.Criteria["Filter1"] = CriteriaOperator.Parse("DocDate >= ? and DocDate <= ?", Fromdate, Todate);
                }
            }
        }
    }
}
