﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Tools.Dialogs {
    partial class LocomotiveController : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocomotiveController));
            this.panelInfo = new System.Windows.Forms.Panel();
            this.imageListMotionButtons = new System.Windows.Forms.ImageList(this.components);
            this.buttonBackward = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonForward = new System.Windows.Forms.Button();
            this.buttonFunction = new System.Windows.Forms.Button();
            this.buttonLight = new System.Windows.Forms.Button();
            this.contextMenuLights = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemLightsOn = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLightsOff = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonLocate = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.buttonProperties = new System.Windows.Forms.Button();
            this.labelDriverInstructions = new System.Windows.Forms.Label();
            this.panelSpeedLimit = new System.Windows.Forms.Panel();
            this.labelSpeedLimit = new System.Windows.Forms.Label();
            this.buttonBackwardMenu = new System.Windows.Forms.Button();
            this.buttonStopMenu = new System.Windows.Forms.Button();
            this.buttonForwardMenu = new System.Windows.Forms.Button();
            this.contextMenuLights.SuspendLayout();
            this.panelSpeedLimit.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelInfo
            // 
            this.panelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInfo.Location = new System.Drawing.Point(3, 1);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(188, 56);
            this.panelInfo.TabIndex = 0;
            // 
            // imageListMotionButtons
            // 
            this.imageListMotionButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListMotionButtons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMotionButtons.ImageStream")));
            this.imageListMotionButtons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMotionButtons.Images.SetKeyName(0, "");
            this.imageListMotionButtons.Images.SetKeyName(1, "");
            this.imageListMotionButtons.Images.SetKeyName(2, "");
            this.imageListMotionButtons.Images.SetKeyName(3, "");
            this.imageListMotionButtons.Images.SetKeyName(4, "");
            this.imageListMotionButtons.Images.SetKeyName(5, "");
            // 
            // buttonBackward
            // 
            this.buttonBackward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBackward.ImageIndex = 0;
            this.buttonBackward.ImageList = this.imageListMotionButtons;
            this.buttonBackward.Location = new System.Drawing.Point(5, 64);
            this.buttonBackward.Name = "buttonBackward";
            this.buttonBackward.Size = new System.Drawing.Size(32, 23);
            this.buttonBackward.TabIndex = 1;
            this.toolTips.SetToolTip(this.buttonBackward, "Backward");
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.ImageIndex = 1;
            this.buttonStop.ImageList = this.imageListMotionButtons;
            this.buttonStop.Location = new System.Drawing.Point(40, 64);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(32, 23);
            this.buttonStop.TabIndex = 2;
            this.toolTips.SetToolTip(this.buttonStop, "Stop");
            // 
            // buttonForward
            // 
            this.buttonForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonForward.ImageIndex = 2;
            this.buttonForward.ImageList = this.imageListMotionButtons;
            this.buttonForward.Location = new System.Drawing.Point(76, 64);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(32, 23);
            this.buttonForward.TabIndex = 3;
            this.toolTips.SetToolTip(this.buttonForward, "Forward");
            // 
            // buttonFunction
            // 
            this.buttonFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFunction.Location = new System.Drawing.Point(165, 64);
            this.buttonFunction.Name = "buttonFunction";
            this.buttonFunction.Size = new System.Drawing.Size(56, 23);
            this.buttonFunction.TabIndex = 5;
            this.buttonFunction.Text = "Function";
            // 
            // buttonLight
            // 
            this.buttonLight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLight.ContextMenuStrip = this.contextMenuLights;
            this.buttonLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLight.ImageIndex = 3;
            this.buttonLight.ImageList = this.imageListMotionButtons;
            this.buttonLight.Location = new System.Drawing.Point(133, 64);
            this.buttonLight.Name = "buttonLight";
            this.buttonLight.Size = new System.Drawing.Size(26, 23);
            this.buttonLight.TabIndex = 4;
            this.toolTips.SetToolTip(this.buttonLight, "Turn lights on/off");
            // 
            // contextMenuLights
            // 
            this.contextMenuLights.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuLights.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLightsOn,
            this.menuItemLightsOff});
            this.contextMenuLights.Name = "contextMenuLights";
            this.contextMenuLights.Size = new System.Drawing.Size(241, 80);
            // 
            // menuItemLightsOn
            // 
            this.menuItemLightsOn.Name = "menuItemLightsOn";
            this.menuItemLightsOn.Size = new System.Drawing.Size(240, 38);
            this.menuItemLightsOn.Text = "Lights are ON";
            // 
            // menuItemLightsOff
            // 
            this.menuItemLightsOff.Name = "menuItemLightsOff";
            this.menuItemLightsOff.Size = new System.Drawing.Size(240, 38);
            this.menuItemLightsOff.Text = "Lights are OFF";
            // 
            // buttonLocate
            // 
            this.buttonLocate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLocate.ImageIndex = 4;
            this.buttonLocate.ImageList = this.imageListMotionButtons;
            this.buttonLocate.Location = new System.Drawing.Point(193, 27);
            this.buttonLocate.Name = "buttonLocate";
            this.buttonLocate.Size = new System.Drawing.Size(26, 23);
            this.buttonLocate.TabIndex = 6;
            this.toolTips.SetToolTip(this.buttonLocate, "Locate locomotive");
            // 
            // buttonProperties
            // 
            this.buttonProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProperties.ImageIndex = 5;
            this.buttonProperties.ImageList = this.imageListMotionButtons;
            this.buttonProperties.Location = new System.Drawing.Point(193, 1);
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.Size = new System.Drawing.Size(26, 23);
            this.buttonProperties.TabIndex = 7;
            this.toolTips.SetToolTip(this.buttonProperties, "Edit train configuration");
            // 
            // labelDriverInstructions
            // 
            this.labelDriverInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDriverInstructions.BackColor = System.Drawing.SystemColors.Control;
            this.labelDriverInstructions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDriverInstructions.Location = new System.Drawing.Point(5, 104);
            this.labelDriverInstructions.Name = "labelDriverInstructions";
            this.labelDriverInstructions.Size = new System.Drawing.Size(173, 18);
            this.labelDriverInstructions.TabIndex = 8;
            this.labelDriverInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelSpeedLimit
            // 
            this.panelSpeedLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSpeedLimit.Controls.Add(this.labelSpeedLimit);
            this.panelSpeedLimit.Location = new System.Drawing.Point(185, 94);
            this.panelSpeedLimit.Name = "panelSpeedLimit";
            this.panelSpeedLimit.Size = new System.Drawing.Size(34, 34);
            this.panelSpeedLimit.TabIndex = 9;
            // 
            // labelSpeedLimit
            // 
            this.labelSpeedLimit.BackColor = System.Drawing.Color.Transparent;
            this.labelSpeedLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelSpeedLimit.Location = new System.Drawing.Point(2, 7);
            this.labelSpeedLimit.Name = "labelSpeedLimit";
            this.labelSpeedLimit.Size = new System.Drawing.Size(30, 20);
            this.labelSpeedLimit.TabIndex = 0;
            this.labelSpeedLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonBackwardMenu
            // 
            this.buttonBackwardMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBackwardMenu.Location = new System.Drawing.Point(5, 87);
            this.buttonBackwardMenu.Name = "buttonBackwardMenu";
            this.buttonBackwardMenu.Size = new System.Drawing.Size(32, 10);
            this.buttonBackwardMenu.TabIndex = 10;
            // 
            // buttonStopMenu
            // 
            this.buttonStopMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStopMenu.Location = new System.Drawing.Point(40, 87);
            this.buttonStopMenu.Name = "buttonStopMenu";
            this.buttonStopMenu.Size = new System.Drawing.Size(32, 10);
            this.buttonStopMenu.TabIndex = 10;
            // 
            // buttonForwardMenu
            // 
            this.buttonForwardMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonForwardMenu.Location = new System.Drawing.Point(76, 87);
            this.buttonForwardMenu.Name = "buttonForwardMenu";
            this.buttonForwardMenu.Size = new System.Drawing.Size(32, 10);
            this.buttonForwardMenu.TabIndex = 10;
            // 
            // LocomotiveController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 130);
            this.Controls.Add(this.buttonBackwardMenu);
            this.Controls.Add(this.panelSpeedLimit);
            this.Controls.Add(this.labelDriverInstructions);
            this.Controls.Add(this.buttonFunction);
            this.Controls.Add(this.buttonBackward);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonForward);
            this.Controls.Add(this.buttonLight);
            this.Controls.Add(this.buttonLocate);
            this.Controls.Add(this.buttonProperties);
            this.Controls.Add(this.buttonStopMenu);
            this.Controls.Add(this.buttonForwardMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(220, 104);
            this.Name = "LocomotiveController";
            this.Text = "LocomotiveController";
            this.contextMenuLights.ResumeLayout(false);
            this.panelSpeedLimit.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        private Panel panelInfo;
        private ImageList imageListMotionButtons;
        private Button buttonBackward;
        private Button buttonStop;
        private Button buttonForward;
        private Button buttonFunction;
        private Button buttonLight;
        private Button buttonLocate;
        private System.Windows.Forms.ToolTip toolTips;
        private ContextMenuStrip contextMenuLights;
        private ToolStripMenuItem menuItemLightsOn;
        private ToolStripMenuItem menuItemLightsOff;
        private Button buttonProperties;
        private Label labelDriverInstructions;
        private Panel panelSpeedLimit;
        private Label labelSpeedLimit;
        private Button buttonBackwardMenu;
        private Button buttonStopMenu;
        private Button buttonForwardMenu;
        private IContainer components;
    }
}