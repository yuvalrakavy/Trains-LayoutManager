namespace LayoutManager.Dialogs {
	partial class ImportPolicies {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportPolicies));
			this.wizardImportPolicies = new Gui.Wizard.Wizard();
			this.wizardPageSetFilename = new Gui.Wizard.WizardPage();
			this.label1 = new System.Windows.Forms.Label();
			this.headerImportFromFile = new Gui.Wizard.Header();
			this.textBoxFilename = new System.Windows.Forms.TextBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.wizardPageSelectScripts = new Gui.Wizard.WizardPage();
			this.headerSelectScript = new Gui.Wizard.Header();
			this.listViewScripts = new System.Windows.Forms.ListView();
			this.columnHeaderScriptName = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderScript = new System.Windows.Forms.ColumnHeader();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonViewScript = new System.Windows.Forms.Button();
			this.wizardImportPolicies.SuspendLayout();
			this.wizardPageSetFilename.SuspendLayout();
			this.wizardPageSelectScripts.SuspendLayout();
			this.SuspendLayout();
			// 
			// wizardImportPolicies
			// 
			this.wizardImportPolicies.Controls.Add(this.wizardPageSelectScripts);
			this.wizardImportPolicies.Controls.Add(this.wizardPageSetFilename);
			this.wizardImportPolicies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardImportPolicies.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.wizardImportPolicies.Location = new System.Drawing.Point(0, 0);
			this.wizardImportPolicies.Name = "wizardImportPolicies";
			this.wizardImportPolicies.Pages.AddRange(new Gui.Wizard.WizardPage[] {
            this.wizardPageSetFilename,
            this.wizardPageSelectScripts});
			this.wizardImportPolicies.Size = new System.Drawing.Size(543, 356);
			this.wizardImportPolicies.TabIndex = 0;
			// 
			// wizardPageSetFilename
			// 
			this.wizardPageSetFilename.Controls.Add(this.buttonBrowse);
			this.wizardPageSetFilename.Controls.Add(this.textBoxFilename);
			this.wizardPageSetFilename.Controls.Add(this.headerImportFromFile);
			this.wizardPageSetFilename.Controls.Add(this.label1);
			this.wizardPageSetFilename.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPageSetFilename.IsFinishPage = false;
			this.wizardPageSetFilename.Location = new System.Drawing.Point(0, 0);
			this.wizardPageSetFilename.Name = "wizardPageSetFilename";
			this.wizardPageSetFilename.Size = new System.Drawing.Size(496, 308);
			this.wizardPageSetFilename.TabIndex = 1;
			this.wizardPageSetFilename.CloseFromNext += new Gui.Wizard.PageEventHandler(this.wizardPageSetFilename_CloseFromNext);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 81);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Filenmae:";
			// 
			// headerImportFromFile
			// 
			this.headerImportFromFile.BackColor = System.Drawing.SystemColors.Control;
			this.headerImportFromFile.CausesValidation = false;
			this.headerImportFromFile.Description = "Enter the name of the file from which you would like to import scripts";
			this.headerImportFromFile.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerImportFromFile.Image = ((System.Drawing.Image)(resources.GetObject("headerImportFromFile.Image")));
			this.headerImportFromFile.Location = new System.Drawing.Point(0, 0);
			this.headerImportFromFile.Name = "headerImportFromFile";
			this.headerImportFromFile.Size = new System.Drawing.Size(496, 64);
			this.headerImportFromFile.TabIndex = 1;
			this.headerImportFromFile.Title = "Import Scripts Wizard";
			// 
			// textBoxFilename
			// 
			this.textBoxFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFilename.Location = new System.Drawing.Point(71, 78);
			this.textBoxFilename.Name = "textBoxFilename";
			this.textBoxFilename.Size = new System.Drawing.Size(332, 21);
			this.textBoxFilename.TabIndex = 2;
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonBrowse.Location = new System.Drawing.Point(409, 78);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowse.TabIndex = 3;
			this.buttonBrowse.Text = "&Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// wizardPageSelectScripts
			// 
			this.wizardPageSelectScripts.Controls.Add(this.buttonViewScript);
			this.wizardPageSelectScripts.Controls.Add(this.label2);
			this.wizardPageSelectScripts.Controls.Add(this.listViewScripts);
			this.wizardPageSelectScripts.Controls.Add(this.headerSelectScript);
			this.wizardPageSelectScripts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.wizardPageSelectScripts.IsFinishPage = true;
			this.wizardPageSelectScripts.Location = new System.Drawing.Point(0, 0);
			this.wizardPageSelectScripts.Name = "wizardPageSelectScripts";
			this.wizardPageSelectScripts.Size = new System.Drawing.Size(543, 308);
			this.wizardPageSelectScripts.TabIndex = 2;
			this.wizardPageSelectScripts.CloseFromNext += new Gui.Wizard.PageEventHandler(this.wizardPageSelectScripts_CloseFromNext);
			// 
			// headerSelectScript
			// 
			this.headerSelectScript.BackColor = System.Drawing.SystemColors.Control;
			this.headerSelectScript.CausesValidation = false;
			this.headerSelectScript.Description = "Select the scripts you would that like to import";
			this.headerSelectScript.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerSelectScript.Image = ((System.Drawing.Image)(resources.GetObject("headerSelectScript.Image")));
			this.headerSelectScript.Location = new System.Drawing.Point(0, 0);
			this.headerSelectScript.Name = "headerSelectScript";
			this.headerSelectScript.Size = new System.Drawing.Size(543, 64);
			this.headerSelectScript.TabIndex = 0;
			this.headerSelectScript.Title = "Select Scripts";
			// 
			// listViewScripts
			// 
			this.listViewScripts.CheckBoxes = true;
			this.listViewScripts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderScriptName,
            this.columnHeaderScript});
			this.listViewScripts.FullRowSelect = true;
			this.listViewScripts.GridLines = true;
			this.listViewScripts.Location = new System.Drawing.Point(12, 70);
			this.listViewScripts.Name = "listViewScripts";
			this.listViewScripts.Size = new System.Drawing.Size(519, 208);
			this.listViewScripts.TabIndex = 1;
			this.listViewScripts.UseCompatibleStateImageBehavior = false;
			this.listViewScripts.View = System.Windows.Forms.View.Details;
			this.listViewScripts.SelectedIndexChanged += new System.EventHandler(this.listViewScripts_SelectedIndexChanged);
			// 
			// columnHeaderScriptName
			// 
			this.columnHeaderScriptName.Text = "Name";
			this.columnHeaderScriptName.Width = 226;
			// 
			// columnHeaderScript
			// 
			this.columnHeaderScript.Text = "Script";
			this.columnHeaderScript.Width = 282;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "LayoutScripts";
			this.openFileDialog.Filter = "Layout Scripts|*.LayoutScripts|All files|*.*";
			this.openFileDialog.Title = "Select file to import scripts from";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(107, 287);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(410, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Note: If checked, a script shown in bold  will be overwritten with the imported  " +
				"script";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonViewScript
			// 
			this.buttonViewScript.Location = new System.Drawing.Point(12, 282);
			this.buttonViewScript.Name = "buttonViewScript";
			this.buttonViewScript.Size = new System.Drawing.Size(88, 23);
			this.buttonViewScript.TabIndex = 3;
			this.buttonViewScript.Text = "View script...";
			this.buttonViewScript.UseVisualStyleBackColor = true;
			this.buttonViewScript.Click += new System.EventHandler(this.buttonViewScript_Click);
			// 
			// ImportPolicies
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 356);
			this.ControlBox = false;
			this.Controls.Add(this.wizardImportPolicies);
			this.Name = "ImportPolicies";
			this.Text = "Import Scripts Wizard";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportPolicies_FormClosed);
			this.wizardImportPolicies.ResumeLayout(false);
			this.wizardPageSetFilename.ResumeLayout(false);
			this.wizardPageSetFilename.PerformLayout();
			this.wizardPageSelectScripts.ResumeLayout(false);
			this.wizardPageSelectScripts.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Gui.Wizard.Wizard wizardImportPolicies;
		private Gui.Wizard.WizardPage wizardPageSetFilename;
		private Gui.Wizard.Header headerImportFromFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.TextBox textBoxFilename;
		private Gui.Wizard.WizardPage wizardPageSelectScripts;
		private Gui.Wizard.Header headerSelectScript;
		private System.Windows.Forms.ListView listViewScripts;
		private System.Windows.Forms.ColumnHeader columnHeaderScriptName;
		private System.Windows.Forms.ColumnHeader columnHeaderScript;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonViewScript;

	}
}