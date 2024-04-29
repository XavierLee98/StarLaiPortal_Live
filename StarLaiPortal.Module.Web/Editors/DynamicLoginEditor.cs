﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using DevExpress.Xpo;
using StarLaiPortal.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

// 2024-04-04 add login new loginpage ver 1.0.15

namespace StarLaiPortal.Module.Web.Editors
{
    [PropertyEditor(typeof(String), "CustomLogin", false)]
    public class CustomStringEditor : ASPxPropertyEditor
    {
        ASPxComboBox dropDownControl = null;
        public CustomStringEditor(
        Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
        protected override void SetupControl(WebControl control)
        {
            if (ViewEditMode == ViewEditMode.Edit)
            {
                DataTable dt = null;
                string databases = "";
                // Start ver 1.0.15
                //string sqlQuery = "SELECT * FROM ODBC ";
                string sqlQuery = "SELECT * FROM ODBC WHERE DBName = '" + ConfigurationManager.AppSettings["PortalDB"].ToString() + "'";
                // End ver 1.0.15

                dt = SQLHandler.GetDataTable(sqlQuery);
                if (dt.Rows.Count > 0)
                {
                    ((ASPxComboBox)control).Items.Add("", "");
                    foreach (DataRow row in dt.Rows)
                    {
                        ((ASPxComboBox)control).Items.Add(row["Description"].ToString(), row["DBName"].ToString());

                        if (databases == "")
                            databases = row["Description"].ToString();
                        else
                            databases += ";" + row["Description"].ToString();
                    }

                    // Start ver 1.0.15
                    //((ASPxComboBox)control).SelectedIndex = 0;
                    ((ASPxComboBox)control).SelectedItem.Text = databases;
                    // Start ver 1.0.15
                }
            }
        }
        protected override WebControl CreateEditModeControlCore()
        {
            dropDownControl = RenderHelper.CreateASPxComboBox();
            dropDownControl.ValueChanged += EditValueChangedHandler;
            return dropDownControl;
        }
        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (dropDownControl != null)
            {
                dropDownControl.ValueChanged -= new EventHandler(EditValueChangedHandler);
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }
    }
}