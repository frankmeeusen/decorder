/* 
 * 
 * Decorder Test - Extracts files from a certain HDD
 * Copyright (C) 2007  Jan Boon
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 */


namespace Decorder
{
	partial class MainWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.ExtractButton = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.openImageButton = new System.Windows.Forms.Button();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.openDiskButton = new System.Windows.Forms.Button();
			this.closeDiskButton = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// ExtractButton
			// 
			this.ExtractButton.Location = new System.Drawing.Point(12, 12);
			this.ExtractButton.Name = "ExtractButton";
			this.ExtractButton.Size = new System.Drawing.Size(75, 75);
			this.ExtractButton.TabIndex = 0;
			this.ExtractButton.Text = "Extract to ...";
			this.ExtractButton.UseVisualStyleBackColor = true;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this.listView1.Location = new System.Drawing.Point(12, 93);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(560, 259);
			this.listView1.SmallImageList = this.imageList;
			this.listView1.TabIndex = 1;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Id";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Name";
			this.columnHeader2.Width = 240;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Size";
			this.columnHeader3.Width = 120;
			// 
			// openImageButton
			// 
			this.openImageButton.Location = new System.Drawing.Point(416, 12);
			this.openImageButton.Name = "openImageButton";
			this.openImageButton.Size = new System.Drawing.Size(75, 75);
			this.openImageButton.TabIndex = 2;
			this.openImageButton.Text = "Open image";
			this.openImageButton.UseVisualStyleBackColor = true;
			this.openImageButton.Click += new System.EventHandler(this.openImageButton_Click);
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Modified";
			this.columnHeader4.Width = 120;
			// 
			// openDiskButton
			// 
			this.openDiskButton.Location = new System.Drawing.Point(335, 12);
			this.openDiskButton.Name = "openDiskButton";
			this.openDiskButton.Size = new System.Drawing.Size(75, 75);
			this.openDiskButton.TabIndex = 3;
			this.openDiskButton.Text = "Open disk";
			this.openDiskButton.UseVisualStyleBackColor = true;
			// 
			// closeDiskButton
			// 
			this.closeDiskButton.Location = new System.Drawing.Point(497, 12);
			this.closeDiskButton.Name = "closeDiskButton";
			this.closeDiskButton.Size = new System.Drawing.Size(75, 75);
			this.closeDiskButton.TabIndex = 4;
			this.closeDiskButton.Text = "Close disk";
			this.closeDiskButton.UseVisualStyleBackColor = true;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "error.ico");
			this.imageList.Images.SetKeyName(1, "blankcd.ico");
			this.imageList.Images.SetKeyName(2, "folderopen.ico");
			this.imageList.Images.SetKeyName(3, "document.ico");
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 364);
			this.Controls.Add(this.closeDiskButton);
			this.Controls.Add(this.openDiskButton);
			this.Controls.Add(this.openImageButton);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.ExtractButton);
			this.Name = "MainWindow";
			this.Text = "Decorder";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button ExtractButton;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button openImageButton;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button openDiskButton;
		private System.Windows.Forms.Button closeDiskButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.ImageList imageList;
	}
}