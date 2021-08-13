
namespace S10Cuantificacion
{
    partial class FrmBuscarPres
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBuscarPres));
            DevComponents.DotNetBar.SuperGrid.Style.Background background1 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            DevComponents.DotNetBar.SuperGrid.Style.BackColorBlend backColorBlend1 = new DevComponents.DotNetBar.SuperGrid.Style.BackColorBlend();
            DevComponents.DotNetBar.SuperGrid.Style.Background background3 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            DevComponents.DotNetBar.SuperGrid.Style.Background background4 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            DevComponents.DotNetBar.SuperGrid.Style.Background background5 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            DevComponents.DotNetBar.SuperGrid.Style.Background background6 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            DevComponents.DotNetBar.SuperGrid.Style.Background background7 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            DevComponents.DotNetBar.SuperGrid.Style.Background background2 = new DevComponents.DotNetBar.SuperGrid.Style.Background();
            this.metroShell1 = new DevComponents.DotNetBar.Metro.MetroShell();
            this.MetroTabInicioPrincipal = new DevComponents.DotNetBar.Metro.MetroTabPanel();
            this.GrupoContenido = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.LabelX1 = new DevComponents.DotNetBar.Metro.MetroTabItem();
            this.ButtonItem2 = new DevComponents.DotNetBar.ButtonItem();
            this.ButtonItem3 = new DevComponents.DotNetBar.ButtonItem();
            this.SGPresupuestos = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.gridColumn140 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn177 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn179 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn180 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.gridColumn178 = new DevComponents.DotNetBar.SuperGrid.GridColumn();
            this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.metroShell1.SuspendLayout();
            this.MetroTabInicioPrincipal.SuspendLayout();
            this.GrupoContenido.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroShell1
            // 
            this.metroShell1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            // 
            // 
            // 
            this.metroShell1.BackgroundStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metroShell1.BackgroundStyle.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(0)))), ((int)(((byte)(65)))), ((int)(((byte)(66)))));
            this.metroShell1.BackgroundStyle.BackColorGradientType = DevComponents.DotNetBar.eGradientType.Radial;
            this.metroShell1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.metroShell1.BackgroundStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.metroShell1.BackgroundStyle.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metroShell1.BackgroundStyle.TextShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(198)))));
            this.metroShell1.CaptionVisible = true;
            this.metroShell1.CategorizeMode = DevComponents.DotNetBar.Metro.eMetroCategorizeMode.Categories;
            this.metroShell1.Controls.Add(this.MetroTabInicioPrincipal);
            this.metroShell1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.metroShell1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroShell1.Font = new System.Drawing.Font("Palatino Linotype", 8.75F);
            this.metroShell1.ForeColor = System.Drawing.Color.Black;
            this.metroShell1.HelpButtonText = null;
            this.metroShell1.HelpButtonVisible = false;
            this.metroShell1.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.LabelX1,
            this.ButtonItem2,
            this.ButtonItem3});
            this.metroShell1.KeyTipsFont = new System.Drawing.Font("Tahoma", 7F);
            this.metroShell1.Location = new System.Drawing.Point(0, 0);
            this.metroShell1.Name = "metroShell1";
            this.metroShell1.SettingsButtonVisible = false;
            this.metroShell1.ShowIcon = false;
            this.metroShell1.Size = new System.Drawing.Size(489, 852);
            this.metroShell1.SystemText.MaximizeRibbonText = "&Maximize the Ribbon";
            this.metroShell1.SystemText.MinimizeRibbonText = "Mi&nimize the Ribbon";
            this.metroShell1.SystemText.QatAddItemText = "&Add to Quick Access Toolbar";
            this.metroShell1.SystemText.QatCustomizeMenuLabel = "<b>Customize Quick Access Toolbar</b>";
            this.metroShell1.SystemText.QatCustomizeText = "&Customize Quick Access Toolbar...";
            this.metroShell1.SystemText.QatDialogAddButton = "&Add >>";
            this.metroShell1.SystemText.QatDialogCancelButton = "Cancel";
            this.metroShell1.SystemText.QatDialogCaption = "Customize Quick Access Toolbar";
            this.metroShell1.SystemText.QatDialogCategoriesLabel = "&Choose commands from:";
            this.metroShell1.SystemText.QatDialogOkButton = "OK";
            this.metroShell1.SystemText.QatDialogPlacementCheckbox = "&Place Quick Access Toolbar below the Ribbon";
            this.metroShell1.SystemText.QatDialogRemoveButton = "&Remove";
            this.metroShell1.SystemText.QatPlaceAboveRibbonText = "&Place Quick Access Toolbar above the Ribbon";
            this.metroShell1.SystemText.QatPlaceBelowRibbonText = "&Place Quick Access Toolbar below the Ribbon";
            this.metroShell1.SystemText.QatRemoveItemText = "&Remove from Quick Access Toolbar";
            this.metroShell1.TabIndex = 74;
            this.metroShell1.TabStripFont = new System.Drawing.Font("Segoe UI", 10.25F, System.Drawing.FontStyle.Bold);
            this.metroShell1.UseCustomizeDialog = false;
            // 
            // MetroTabInicioPrincipal
            // 
            this.MetroTabInicioPrincipal.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.MetroTabInicioPrincipal.Controls.Add(this.GrupoContenido);
            this.MetroTabInicioPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetroTabInicioPrincipal.Location = new System.Drawing.Point(0, 65);
            this.MetroTabInicioPrincipal.Name = "MetroTabInicioPrincipal";
            this.MetroTabInicioPrincipal.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.MetroTabInicioPrincipal.Size = new System.Drawing.Size(489, 787);
            // 
            // 
            // 
            this.MetroTabInicioPrincipal.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MetroTabInicioPrincipal.Style.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MetroTabInicioPrincipal.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.MetroTabInicioPrincipal.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.MetroTabInicioPrincipal.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.MetroTabInicioPrincipal.TabIndex = 1;
            // 
            // GrupoContenido
            // 
            this.GrupoContenido.AutoScroll = true;
            this.GrupoContenido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            this.GrupoContenido.CanvasColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
            this.GrupoContenido.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014;
            this.GrupoContenido.Controls.Add(this.SGPresupuestos);
            this.GrupoContenido.DisabledBackColor = System.Drawing.Color.Empty;
            this.GrupoContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GrupoContenido.Location = new System.Drawing.Point(3, 4);
            this.GrupoContenido.Name = "GrupoContenido";
            this.GrupoContenido.Size = new System.Drawing.Size(483, 780);
            // 
            // 
            // 
            this.GrupoContenido.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(0)))), ((int)(((byte)(65)))), ((int)(((byte)(66)))));
            this.GrupoContenido.Style.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.GrupoContenido.Style.BackColorGradientAngle = 90;
            this.GrupoContenido.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.GrupoContenido.Style.BorderBottomWidth = 1;
            this.GrupoContenido.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.GrupoContenido.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.GrupoContenido.Style.BorderLeftWidth = 1;
            this.GrupoContenido.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.GrupoContenido.Style.BorderRightWidth = 1;
            this.GrupoContenido.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.GrupoContenido.Style.BorderTopWidth = 1;
            this.GrupoContenido.Style.CornerDiameter = 4;
            this.GrupoContenido.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.GrupoContenido.Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrupoContenido.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.GrupoContenido.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.GrupoContenido.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.GrupoContenido.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.GrupoContenido.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.GrupoContenido.TabIndex = 5;
            // 
            // LabelX1
            // 
            this.LabelX1.Checked = true;
            this.LabelX1.EnableMarkup = false;
            this.LabelX1.FixedSize = new System.Drawing.Size(250, 40);
            this.LabelX1.FontBold = true;
            this.LabelX1.Image = ((System.Drawing.Image)(resources.GetObject("LabelX1.Image")));
            this.LabelX1.ImageFixedSize = new System.Drawing.Size(60, 45);
            this.LabelX1.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.Default;
            this.LabelX1.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Center;
            this.LabelX1.Name = "LabelX1";
            this.LabelX1.NotificationMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.LabelX1.NotificationMarkOffset = new System.Drawing.Point(0, 0);
            this.LabelX1.NotificationMarkSize = 2;
            this.LabelX1.Panel = this.MetroTabInicioPrincipal;
            this.LabelX1.PopupSide = DevComponents.DotNetBar.ePopupSide.Left;
            this.LabelX1.SplitButton = true;
            this.LabelX1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.LabelX1.Text = "BUSCAR UN PRESUPUESTO";
            // 
            // ButtonItem2
            // 
            this.ButtonItem2.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ButtonItem2.FixedSize = new System.Drawing.Size(90, 35);
            this.ButtonItem2.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.ButtonItem2.Name = "ButtonItem2";
            this.ButtonItem2.Symbol = "";
            this.ButtonItem2.Text = "Aceptar";
            this.ButtonItem2.Click += new System.EventHandler(this.ButtonItem2_Click);
            // 
            // ButtonItem3
            // 
            this.ButtonItem3.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText;
            this.ButtonItem3.FixedSize = new System.Drawing.Size(90, 32);
            this.ButtonItem3.ItemAlignment = DevComponents.DotNetBar.eItemAlignment.Far;
            this.ButtonItem3.Name = "ButtonItem3";
            this.ButtonItem3.Symbol = "";
            this.ButtonItem3.Text = "Cancelar";
            this.ButtonItem3.Click += new System.EventHandler(this.ButtonItem3_Click);
            // 
            // SGPresupuestos
            // 
            this.SGPresupuestos.AllowDrop = true;
            this.SGPresupuestos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            backColorBlend1.Colors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))))};
            background1.BackColorBlend = backColorBlend1;
            this.SGPresupuestos.DefaultVisualStyles.ColumnHeaderStyles.Default.Background = background1;
            this.SGPresupuestos.DefaultVisualStyles.ColumnHeaderStyles.Default.Font = new System.Drawing.Font("Palatino Linotype", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SGPresupuestos.DefaultVisualStyles.ColumnHeaderStyles.Default.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.SGPresupuestos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SGPresupuestos.ExpandButtonType = DevComponents.DotNetBar.SuperGrid.ExpandButtonType.Circle;
            this.SGPresupuestos.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.SGPresupuestos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SGPresupuestos.ForeColor = System.Drawing.Color.Black;
            this.SGPresupuestos.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
            this.SGPresupuestos.Location = new System.Drawing.Point(0, 0);
            this.SGPresupuestos.Name = "SGPresupuestos";
            // 
            // 
            // 
            // 
            // 
            // 
            this.SGPresupuestos.PrimaryGrid.ColumnHeader.RowHeaderText = "0";
            this.SGPresupuestos.PrimaryGrid.ColumnHeader.RowHeight = 25;
            this.SGPresupuestos.PrimaryGrid.Columns.Add(this.gridColumn140);
            this.SGPresupuestos.PrimaryGrid.Columns.Add(this.gridColumn177);
            this.SGPresupuestos.PrimaryGrid.Columns.Add(this.gridColumn179);
            this.SGPresupuestos.PrimaryGrid.Columns.Add(this.gridColumn180);
            this.SGPresupuestos.PrimaryGrid.Columns.Add(this.gridColumn178);
            this.SGPresupuestos.PrimaryGrid.DefaultRowHeight = 40;
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.AlternateRowCellStyles.Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.CellStyles.Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            background3.Color1 = System.Drawing.Color.Navy;
            background3.Color2 = System.Drawing.Color.White;
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.ColumnHeaderStyles.MouseOver.Background = background3;
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.ColumnHeaderStyles.MouseOver.TextColor = System.Drawing.Color.White;
            background4.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(204)))), ((int)(((byte)(203)))));
            background4.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.FilterColumnHeaderStyles.Default.Background = background4;
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.FilterColumnHeaderStyles.Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            background5.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(204)))), ((int)(((byte)(203)))));
            background5.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.FilterRowStyles.Default.FilterBackground = background5;
            background6.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            background6.Color2 = System.Drawing.Color.White;
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.GridPanelStyle.Background = background6;
            background7.Color1 = System.Drawing.Color.AliceBlue;
            background7.Color2 = System.Drawing.Color.LightSteelBlue;
            background7.GradientAngle = 45;
            this.SGPresupuestos.PrimaryGrid.DefaultVisualStyles.GroupHeaderStyles.Default.Background = background7;
            this.SGPresupuestos.PrimaryGrid.EnableFiltering = true;
            this.SGPresupuestos.PrimaryGrid.ExpandButtonType = DevComponents.DotNetBar.SuperGrid.ExpandButtonType.Circle;
            // 
            // 
            // 
            this.SGPresupuestos.PrimaryGrid.Filter.RowHeight = 25;
            this.SGPresupuestos.PrimaryGrid.Filter.ShowPanelFilterExpr = true;
            this.SGPresupuestos.PrimaryGrid.Filter.Visible = true;
            this.SGPresupuestos.PrimaryGrid.FilterExpr = "Descripcion is like Descripcion";
            this.SGPresupuestos.PrimaryGrid.FilterMatchType = DevComponents.DotNetBar.SuperGrid.FilterMatchType.RegularExpressions;
            this.SGPresupuestos.PrimaryGrid.GridLines = DevComponents.DotNetBar.SuperGrid.GridLines.Horizontal;
            this.SGPresupuestos.PrimaryGrid.GroupHeaderClickBehavior = DevComponents.DotNetBar.SuperGrid.GroupHeaderClickBehavior.ExpandCollapse;
            this.SGPresupuestos.PrimaryGrid.GroupHeaderHeight = 40;
            this.SGPresupuestos.PrimaryGrid.InitialSelection = DevComponents.DotNetBar.SuperGrid.RelativeSelection.Row;
            this.SGPresupuestos.PrimaryGrid.LevelIndentSize = new System.Drawing.Size(6, 10);
            this.SGPresupuestos.PrimaryGrid.MinRowHeight = 40;
            this.SGPresupuestos.PrimaryGrid.MultiSelect = false;
            this.SGPresupuestos.PrimaryGrid.PrimaryColumnIndex = 1;
            this.SGPresupuestos.PrimaryGrid.RowWhitespaceClickBehavior = DevComponents.DotNetBar.SuperGrid.RowWhitespaceClickBehavior.ExtendSelection;
            this.SGPresupuestos.PrimaryGrid.ShowGroupUnderline = false;
            this.SGPresupuestos.PrimaryGrid.ShowRowHeaders = false;
            this.SGPresupuestos.PrimaryGrid.ShowTreeButtons = true;
            this.SGPresupuestos.PrimaryGrid.VirtualRowHeight = 35;
            this.SGPresupuestos.Size = new System.Drawing.Size(477, 774);
            this.SGPresupuestos.TabIndex = 211;
            this.SGPresupuestos.CellDoubleClick += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridCellDoubleClickEventArgs>(this.SGPresupuestos_CellDoubleClick);
            // 
            // gridColumn140
            // 
            this.gridColumn140.Name = "Id";
            this.gridColumn140.SortIndicator = DevComponents.DotNetBar.SuperGrid.SortIndicator.Ascending;
            this.gridColumn140.Visible = false;
            this.gridColumn140.Width = 20;
            // 
            // gridColumn177
            // 
            this.gridColumn177.CellMergeMode = DevComponents.DotNetBar.SuperGrid.CellMergeMode.HorizontalRight;
            this.gridColumn177.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleLeft;
            this.gridColumn177.CellStyles.Default.AllowMultiLine = DevComponents.DotNetBar.SuperGrid.Style.Tbool.True;
            this.gridColumn177.CellStyles.Default.AllowWrap = DevComponents.DotNetBar.SuperGrid.Style.Tbool.True;
            background2.Color1 = System.Drawing.Color.White;
            background2.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(245)))), ((int)(((byte)(237)))));
            this.gridColumn177.CellStyles.Default.Background = background2;
            this.gridColumn177.CellStyles.Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn177.CellStyles.Default.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.gridColumn177.DefaultNewRowCellValue = "";
            this.gridColumn177.EnableFiltering = DevComponents.DotNetBar.SuperGrid.Style.Tbool.True;
            this.gridColumn177.Name = "PRESUPUESTOS";
            this.gridColumn177.ReadOnly = true;
            this.gridColumn177.Width = 430;
            // 
            // gridColumn179
            // 
            this.gridColumn179.CellMergeMode = ((DevComponents.DotNetBar.SuperGrid.CellMergeMode)(((DevComponents.DotNetBar.SuperGrid.CellMergeMode.HorizontalLeft | DevComponents.DotNetBar.SuperGrid.CellMergeMode.HorizontalRight) 
            | DevComponents.DotNetBar.SuperGrid.CellMergeMode.Vertical)));
            this.gridColumn179.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleCenter;
            this.gridColumn179.CellStyles.Default.AllowMultiLine = DevComponents.DotNetBar.SuperGrid.Style.Tbool.True;
            this.gridColumn179.CellStyles.Default.AllowWrap = DevComponents.DotNetBar.SuperGrid.Style.Tbool.True;
            this.gridColumn179.CellStyles.Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn179.DefaultNewRowCellValue = "";
            this.gridColumn179.EditorType = typeof(DevComponents.DotNetBar.SuperGrid.GridComboBoxExEditControl);
            this.gridColumn179.Name = "NIVEL";
            this.gridColumn179.Visible = false;
            this.gridColumn179.Width = 40;
            // 
            // gridColumn180
            // 
            this.gridColumn180.CellMergeMode = ((DevComponents.DotNetBar.SuperGrid.CellMergeMode)(((DevComponents.DotNetBar.SuperGrid.CellMergeMode.HorizontalLeft | DevComponents.DotNetBar.SuperGrid.CellMergeMode.HorizontalRight) 
            | DevComponents.DotNetBar.SuperGrid.CellMergeMode.Vertical)));
            this.gridColumn180.CellStyles.Default.Alignment = DevComponents.DotNetBar.SuperGrid.Style.Alignment.MiddleRight;
            this.gridColumn180.CellStyles.Default.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn180.Name = "PHANTOMID";
            this.gridColumn180.Visible = false;
            this.gridColumn180.Width = 80;
            // 
            // gridColumn178
            // 
            this.gridColumn178.Name = "PADRE";
            this.gridColumn178.Visible = false;
            // 
            // ImageList1
            // 
            this.ImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList1.ImageStream")));
            this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList1.Images.SetKeyName(0, "Folder.ico");
            this.ImageList1.Images.SetKeyName(1, "Photo.ico");
            this.ImageList1.Images.SetKeyName(2, "planillas1.png");
            this.ImageList1.Images.SetKeyName(3, "IconsLov (124).png");
            this.ImageList1.Images.SetKeyName(4, "Users 2.png");
            this.ImageList1.Images.SetKeyName(5, "software.png");
            this.ImageList1.Images.SetKeyName(6, "Desktop Share.png");
            this.ImageList1.Images.SetKeyName(7, "Microsoft FixIt.png");
            this.ImageList1.Images.SetKeyName(8, "Doc.OrangeBlack.png");
            this.ImageList1.Images.SetKeyName(9, "iconos azules (4).ico");
            this.ImageList1.Images.SetKeyName(10, "My Documents 3.png");
            this.ImageList1.Images.SetKeyName(11, "icono naranja (33).ico");
            this.ImageList1.Images.SetKeyName(12, "pl11.jpg");
            this.ImageList1.Images.SetKeyName(13, "EMail Chrome.ico");
            this.ImageList1.Images.SetKeyName(14, "MYEGY&WALL3 (34).ico");
            this.ImageList1.Images.SetKeyName(15, "Frontpage.ico");
            this.ImageList1.Images.SetKeyName(16, "Documents.ico");
            this.ImageList1.Images.SetKeyName(17, "HardDrive.ico");
            this.ImageList1.Images.SetKeyName(18, "CF II.ico");
            this.ImageList1.Images.SetKeyName(19, "laptop.ico");
            this.ImageList1.Images.SetKeyName(20, "Avant.ico");
            this.ImageList1.Images.SetKeyName(21, "MS Office 3D Word.png");
            this.ImageList1.Images.SetKeyName(22, "MS Office 2D Excel.png");
            this.ImageList1.Images.SetKeyName(23, "Excel.ico");
            this.ImageList1.Images.SetKeyName(24, "adobe Reader.ico");
            // 
            // FrmBuscarPres
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 852);
            this.ControlBox = false;
            this.Controls.Add(this.metroShell1);
            this.Name = "FrmBuscarPres";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.FrmBuscarPres_Load);
            this.metroShell1.ResumeLayout(false);
            this.metroShell1.PerformLayout();
            this.MetroTabInicioPrincipal.ResumeLayout(false);
            this.GrupoContenido.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Metro.MetroShell metroShell1;
        private DevComponents.DotNetBar.Metro.MetroTabPanel MetroTabInicioPrincipal;
        internal DevComponents.DotNetBar.Controls.GroupPanel GrupoContenido;
        public DevComponents.DotNetBar.Metro.MetroTabItem LabelX1;
        internal DevComponents.DotNetBar.ButtonItem ButtonItem2;
        internal DevComponents.DotNetBar.ButtonItem ButtonItem3;
        internal DevComponents.DotNetBar.SuperGrid.SuperGridControl SGPresupuestos;
        internal DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn140;
        internal DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn177;
        internal DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn179;
        internal DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn180;
        private DevComponents.DotNetBar.SuperGrid.GridColumn gridColumn178;
        internal System.Windows.Forms.ImageList ImageList1;
    }
}