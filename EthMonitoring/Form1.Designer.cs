namespace EthMonitoring
{
    partial class Form1
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
            this.startMonitoring = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tokenField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hostField = new System.Windows.Forms.TextBox();
            this.addhost = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tokenLink = new System.Windows.Forms.LinkLabel();
            this.hostsList = new System.Windows.Forms.ListView();
            this.host = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eth_hashrate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dcr_hr = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.temp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hostName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.removeItem = new System.Windows.Forms.Button();
            this.clearList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // startMonitoring
            // 
            this.startMonitoring.Location = new System.Drawing.Point(567, 12);
            this.startMonitoring.Name = "startMonitoring";
            this.startMonitoring.Size = new System.Drawing.Size(138, 36);
            this.startMonitoring.TabIndex = 0;
            this.startMonitoring.Text = "Start monitoring";
            this.startMonitoring.UseVisualStyleBackColor = true;
            this.startMonitoring.Click += new System.EventHandler(this.startMonitoring_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Servers for monitoring";
            // 
            // tokenField
            // 
            this.tokenField.Location = new System.Drawing.Point(12, 28);
            this.tokenField.Name = "tokenField";
            this.tokenField.Size = new System.Drawing.Size(549, 20);
            this.tokenField.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Access token";
            // 
            // hostField
            // 
            this.hostField.Location = new System.Drawing.Point(12, 67);
            this.hostField.Name = "hostField";
            this.hostField.Size = new System.Drawing.Size(112, 20);
            this.hostField.TabIndex = 5;
            // 
            // addhost
            // 
            this.addhost.Location = new System.Drawing.Point(265, 65);
            this.addhost.Name = "addhost";
            this.addhost.Size = new System.Drawing.Size(75, 23);
            this.addhost.TabIndex = 6;
            this.addhost.Text = "Add host";
            this.addhost.UseVisualStyleBackColor = true;
            this.addhost.Click += new System.EventHandler(this.addhost_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Host";
            // 
            // tokenLink
            // 
            this.tokenLink.AutoSize = true;
            this.tokenLink.Location = new System.Drawing.Point(554, 70);
            this.tokenLink.Name = "tokenLink";
            this.tokenLink.Size = new System.Drawing.Size(151, 13);
            this.tokenLink.TabIndex = 8;
            this.tokenLink.TabStop = true;
            this.tokenLink.Text = "Get your auth token from here!";
            // 
            // hostsList
            // 
            this.hostsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.host,
            this.name,
            this.eth_hashrate,
            this.dcr_hr,
            this.temp,
            this.version});
            this.hostsList.Location = new System.Drawing.Point(12, 106);
            this.hostsList.Name = "hostsList";
            this.hostsList.Size = new System.Drawing.Size(693, 253);
            this.hostsList.TabIndex = 9;
            this.hostsList.UseCompatibleStateImageBehavior = false;
            this.hostsList.View = System.Windows.Forms.View.Details;
            // 
            // host
            // 
            this.host.Text = "Host";
            this.host.Width = 110;
            // 
            // name
            // 
            this.name.Text = "Miner name";
            this.name.Width = 113;
            // 
            // eth_hashrate
            // 
            this.eth_hashrate.Text = "ETH Hashrate";
            this.eth_hashrate.Width = 80;
            // 
            // dcr_hr
            // 
            this.dcr_hr.Text = "DCR Hashrate";
            this.dcr_hr.Width = 88;
            // 
            // temp
            // 
            this.temp.Text = "Temperatures";
            this.temp.Width = 142;
            // 
            // version
            // 
            this.version.Text = "Version";
            // 
            // hostName
            // 
            this.hostName.Location = new System.Drawing.Point(130, 67);
            this.hostName.Name = "hostName";
            this.hostName.Size = new System.Drawing.Size(129, 20);
            this.hostName.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(127, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Name";
            // 
            // removeItem
            // 
            this.removeItem.Location = new System.Drawing.Point(12, 365);
            this.removeItem.Name = "removeItem";
            this.removeItem.Size = new System.Drawing.Size(150, 24);
            this.removeItem.TabIndex = 12;
            this.removeItem.Text = "Remove selected miner";
            this.removeItem.UseVisualStyleBackColor = true;
            this.removeItem.Click += new System.EventHandler(this.removeItem_Click);
            // 
            // clearList
            // 
            this.clearList.Location = new System.Drawing.Point(169, 366);
            this.clearList.Name = "clearList";
            this.clearList.Size = new System.Drawing.Size(103, 23);
            this.clearList.TabIndex = 13;
            this.clearList.Text = "Clear list";
            this.clearList.UseVisualStyleBackColor = true;
            this.clearList.Click += new System.EventHandler(this.clearList_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(717, 398);
            this.Controls.Add(this.clearList);
            this.Controls.Add(this.removeItem);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.hostName);
            this.Controls.Add(this.hostsList);
            this.Controls.Add(this.tokenLink);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.addhost);
            this.Controls.Add(this.hostField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tokenField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startMonitoring);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Dual miner monitoring";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startMonitoring;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tokenField;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox hostField;
        private System.Windows.Forms.Button addhost;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel tokenLink;
        private System.Windows.Forms.ListView hostsList;
        private System.Windows.Forms.ColumnHeader host;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader eth_hashrate;
        private System.Windows.Forms.TextBox hostName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColumnHeader dcr_hr;
        private System.Windows.Forms.ColumnHeader temp;
        private System.Windows.Forms.ColumnHeader version;
        private System.Windows.Forms.Button removeItem;
        private System.Windows.Forms.Button clearList;
    }
}

