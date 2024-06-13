﻿namespace StarLaiPortal.Module.Controllers
{
    partial class MemoryManagementController
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
            this.ForceReleaseMemory = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ForceReleaseMemory
            // 
            this.ForceReleaseMemory.Caption = "Force Release Memory";
            this.ForceReleaseMemory.Category = "ObjectsCreation";
            this.ForceReleaseMemory.ConfirmationMessage = null;
            this.ForceReleaseMemory.Id = "ForceReleaseMemory";
            this.ForceReleaseMemory.ToolTip = null;
            this.ForceReleaseMemory.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ForceReleaseMemory_Execute);
            // 
            // MemoryManagementController
            // 
            this.Actions.Add(this.ForceReleaseMemory);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ForceReleaseMemory;
    }
}
