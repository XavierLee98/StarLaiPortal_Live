namespace StarLaiPortal.Module.Controllers
{
    partial class DocumentControllers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DocumentDateFrom = new DevExpress.ExpressApp.Actions.ParametrizedAction(this.components);
            // 
            // DocumentDateFrom
            // 
            this.DocumentDateFrom.Caption = "From";
            this.DocumentDateFrom.Category = "ObjectsCreation";
            this.DocumentDateFrom.ConfirmationMessage = null;
            this.DocumentDateFrom.Id = "DocumentDateFrom";
            this.DocumentDateFrom.NullValuePrompt = null;
            this.DocumentDateFrom.ShortCaption = null;
            this.DocumentDateFrom.ToolTip = null;
            this.DocumentDateFrom.ValueType = typeof(System.DateTime);
            // 
            // DocumentControllers
            // 
            this.Actions.Add(this.DocumentDateFrom);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.ParametrizedAction DocumentDateFrom;
    }
}
