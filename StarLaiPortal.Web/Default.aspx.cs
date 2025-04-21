using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;

public partial class Default : BaseXafPage {
    protected override ContextActionsMenu CreateContextActionsMenu() {
        return new ContextActionsMenu(this, "Edit", "RecordEdit", "ObjectsCreation", "ListView", "Reports");
    }
    public override Control InnerContentPlaceHolder {
        get {
            return Content;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        TimeoutControl1.TimeOutUrl = ConfigurationManager.AppSettings["CommonUrl"].ToString();
    }
}