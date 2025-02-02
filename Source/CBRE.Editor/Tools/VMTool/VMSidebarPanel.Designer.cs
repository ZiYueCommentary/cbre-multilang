﻿using CBRE.Localization;

namespace CBRE.Editor.Tools.VMTool
{
    partial class VMSidebarPanel
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
			this.ButtonLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.ControlPanel = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ResetButton = new System.Windows.Forms.Button();
			this.DeselectAllButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonLayoutPanel
			// 
			this.ButtonLayoutPanel.AutoSize = true;
			this.ButtonLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.ButtonLayoutPanel.Location = new System.Drawing.Point(5, 5);
			this.ButtonLayoutPanel.Name = "ButtonLayoutPanel";
			this.ButtonLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.ButtonLayoutPanel.Size = new System.Drawing.Size(210, 5);
			this.ButtonLayoutPanel.TabIndex = 7;
			// 
			// ControlPanel
			// 
			this.ControlPanel.AutoSize = true;
			this.ControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.ControlPanel.Location = new System.Drawing.Point(5, 10);
			this.ControlPanel.Name = "ControlPanel";
			this.ControlPanel.Size = new System.Drawing.Size(210, 19);
			this.ControlPanel.TabIndex = 10;
			this.ControlPanel.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tableLayoutPanel1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(5, 29);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(210, 29);
			this.panel1.TabIndex = 11;
			// 
			// ResetButton
			// 
			this.ResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ResetButton.Location = new System.Drawing.Point(108, 3);
			this.ResetButton.Name = "ResetButton";
			this.ResetButton.Size = new System.Drawing.Size(99, 23);
			this.ResetButton.TabIndex = 6;
			this.ResetButton.Text = Local.LocalString("tool.reset_brush");
			this.ResetButton.UseVisualStyleBackColor = true;
			this.ResetButton.Click += new System.EventHandler(this.ResetButtonClicked);
			// 
			// DeselectAllButton
			// 
			this.DeselectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DeselectAllButton.Location = new System.Drawing.Point(3, 3);
			this.DeselectAllButton.Name = "DeselectAllButton";
			this.DeselectAllButton.Size = new System.Drawing.Size(99, 23);
			this.DeselectAllButton.TabIndex = 5;
			this.DeselectAllButton.Text = Local.LocalString("deselect_all");
			this.DeselectAllButton.UseVisualStyleBackColor = true;
			this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButtonClicked);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.ResetButton, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.DeselectAllButton, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(210, 29);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// VMSidebarPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.ControlPanel);
			this.Controls.Add(this.ButtonLayoutPanel);
			this.Name = "VMSidebarPanel";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.Size = new System.Drawing.Size(220, 68);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonLayoutPanel;
        private System.Windows.Forms.GroupBox ControlPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button DeselectAllButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
