namespace CIMarketPriceFinder2007
{
    partial class CIExcelRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public CIExcelRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.tab2 = this.Factory.CreateRibbonTab();
            this.groupLoginInfo = this.Factory.CreateRibbonGroup();
            this.box1 = this.Factory.CreateRibbonBox();
            this.btnLogin = this.Factory.CreateRibbonButton();
            this.lblStatus = this.Factory.CreateRibbonLabel();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.box3 = this.Factory.CreateRibbonBox();
            this.editBoxMarketId = this.Factory.CreateRibbonEditBox();
            this.box4 = this.Factory.CreateRibbonBox();
            this.editBoxInterval = this.Factory.CreateRibbonEditBox();
            this.box5 = this.Factory.CreateRibbonBox();
            this.editBoxNo = this.Factory.CreateRibbonEditBox();
            this.buttonGroup1 = this.Factory.CreateRibbonButtonGroup();
            this.btnGetBars = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.tab2.SuspendLayout();
            this.groupLoginInfo.SuspendLayout();
            this.box1.SuspendLayout();
            this.group1.SuspendLayout();
            this.box3.SuspendLayout();
            this.box4.SuspendLayout();
            this.box5.SuspendLayout();
            this.buttonGroup1.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // tab2
            // 
            this.tab2.Groups.Add(this.groupLoginInfo);
            this.tab2.Groups.Add(this.group1);
            this.tab2.Label = "CityIndex API Excel Tools";
            this.tab2.Name = "tab2";
            // 
            // groupLoginInfo
            // 
            this.groupLoginInfo.Items.Add(this.box1);
            this.groupLoginInfo.Items.Add(this.lblStatus);
            this.groupLoginInfo.Label = "Login Info";
            this.groupLoginInfo.Name = "groupLoginInfo";
            // 
            // box1
            // 
            this.box1.Items.Add(this.btnLogin);
            this.box1.Name = "box1";
            // 
            // btnLogin
            // 
            this.btnLogin.Label = "Login";
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLogin_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Label = "label1";
            this.lblStatus.Name = "lblStatus";
            // 
            // group1
            // 
            this.group1.Items.Add(this.box3);
            this.group1.Items.Add(this.box4);
            this.group1.Items.Add(this.box5);
            this.group1.Items.Add(this.buttonGroup1);
            this.group1.Label = "Price Bar Example";
            this.group1.Name = "group1";
            // 
            // box3
            // 
            this.box3.Items.Add(this.editBoxMarketId);
            this.box3.Name = "box3";
            // 
            // editBoxMarketId
            // 
            this.editBoxMarketId.Label = "market Id";
            this.editBoxMarketId.Name = "editBoxMarketId";
            this.editBoxMarketId.Text = null;
            // 
            // box4
            // 
            this.box4.Items.Add(this.editBoxInterval);
            this.box4.Name = "box4";
            // 
            // editBoxInterval
            // 
            this.editBoxInterval.Label = "Interval";
            this.editBoxInterval.Name = "editBoxInterval";
            this.editBoxInterval.Text = null;
            // 
            // box5
            // 
            this.box5.Items.Add(this.editBoxNo);
            this.box5.Name = "box5";
            // 
            // editBoxNo
            // 
            this.editBoxNo.Label = "No";
            this.editBoxNo.Name = "editBoxNo";
            this.editBoxNo.Text = null;
            // 
            // buttonGroup1
            // 
            this.buttonGroup1.Items.Add(this.btnGetBars);
            this.buttonGroup1.Name = "buttonGroup1";
            // 
            // btnGetBars
            // 
            this.btnGetBars.Label = "Get PriceBars";
            this.btnGetBars.Name = "btnGetBars";
            this.btnGetBars.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnGetBars_Click);
            // 
            // CIExcelRibbon
            // 
            this.Name = "CIExcelRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Tabs.Add(this.tab2);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon1_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.tab2.ResumeLayout(false);
            this.tab2.PerformLayout();
            this.groupLoginInfo.ResumeLayout(false);
            this.groupLoginInfo.PerformLayout();
            this.box1.ResumeLayout(false);
            this.box1.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.box3.ResumeLayout(false);
            this.box3.PerformLayout();
            this.box4.ResumeLayout(false);
            this.box4.PerformLayout();
            this.box5.ResumeLayout(false);
            this.box5.PerformLayout();
            this.buttonGroup1.ResumeLayout(false);
            this.buttonGroup1.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        private Microsoft.Office.Tools.Ribbon.RibbonTab tab2;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupLoginInfo;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox editBoxMarketId;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox editBoxInterval;
        internal Microsoft.Office.Tools.Ribbon.RibbonEditBox editBoxNo;
        internal Microsoft.Office.Tools.Ribbon.RibbonButtonGroup buttonGroup1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnGetBars;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box3;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box4;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box5;
        internal Microsoft.Office.Tools.Ribbon.RibbonBox box1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLogin;
        internal Microsoft.Office.Tools.Ribbon.RibbonLabel lblStatus;
    }

    partial class ThisRibbonCollection
    {
        internal CIExcelRibbon Ribbon1
        {
            get { return this.GetRibbon<CIExcelRibbon>(); }
        }
    }
}
