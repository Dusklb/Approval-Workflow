
namespace Trinhky.Win.Editors
{
    partial class ServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            textEdit1 = new DevExpress.XtraEditors.TextEdit();
            textEdit3 = new DevExpress.XtraEditors.TextEdit();
            textEdit4 = new DevExpress.XtraEditors.TextEdit();
            textEdit5 = new DevExpress.XtraEditors.TextEdit();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            cboMode = new DevExpress.XtraEditors.ComboBoxEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit3.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit4.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit5.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboMode.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(textEdit1);
            layoutControl1.Controls.Add(textEdit3);
            layoutControl1.Controls.Add(textEdit4);
            layoutControl1.Controls.Add(textEdit5);
            layoutControl1.Controls.Add(simpleButton1);
            layoutControl1.Controls.Add(simpleButton2);
            layoutControl1.Controls.Add(cboMode);
            layoutControl1.Dock = DockStyle.Fill;
            layoutControl1.Location = new Point(0, 0);
            layoutControl1.Margin = new Padding(5);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new Rectangle(2923, 0, 812, 500);
            layoutControl1.Root = Root;
            layoutControl1.Size = new Size(427, 283);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // textEdit1
            // 
            textEdit1.Location = new Point(120, 38);
            textEdit1.Margin = new Padding(5);
            textEdit1.Name = "textEdit1";
            textEdit1.Size = new Size(295, 22);
            textEdit1.StyleController = layoutControl1;
            textEdit1.TabIndex = 2;
            // 
            // textEdit3
            // 
            textEdit3.Location = new Point(120, 64);
            textEdit3.Margin = new Padding(5);
            textEdit3.Name = "textEdit3";
            textEdit3.Size = new Size(295, 22);
            textEdit3.StyleController = layoutControl1;
            textEdit3.TabIndex = 3;
            // 
            // textEdit4
            // 
            textEdit4.Location = new Point(120, 90);
            textEdit4.Margin = new Padding(5);
            textEdit4.Name = "textEdit4";
            textEdit4.Properties.PasswordChar = '*';
            textEdit4.Size = new Size(295, 22);
            textEdit4.StyleController = layoutControl1;
            textEdit4.TabIndex = 4;
            // 
            // textEdit5
            // 
            textEdit5.Location = new Point(120, 116);
            textEdit5.Margin = new Padding(5);
            textEdit5.Name = "textEdit5";
            textEdit5.Properties.PasswordChar = '*';
            textEdit5.Size = new Size(295, 22);
            textEdit5.StyleController = layoutControl1;
            textEdit5.TabIndex = 5;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new Point(69, 142);
            simpleButton1.Margin = new Padding(5);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new Size(141, 27);
            simpleButton1.StyleController = layoutControl1;
            simpleButton1.TabIndex = 6;
            simpleButton1.Text = "Lưu cấu hình";
            simpleButton1.ToolTip = "Lưu cấu hình máy chủ dữ liệu";
            simpleButton1.Click += SimpleButton1_Click;
            // 
            // simpleButton2
            // 
            simpleButton2.Location = new Point(214, 142);
            simpleButton2.Margin = new Padding(5);
            simpleButton2.Name = "simpleButton2";
            simpleButton2.Size = new Size(141, 27);
            simpleButton2.StyleController = layoutControl1;
            simpleButton2.TabIndex = 7;
            simpleButton2.Text = "Hủy";
            simpleButton2.Click += SimpleButton2_Click;
            // 
            // cboMode
            // 
            cboMode.Location = new Point(120, 12);
            cboMode.Margin = new Padding(3, 4, 3, 4);
            cboMode.Name = "cboMode";
            cboMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboMode.Properties.Items.AddRange(new object[] { "Mạng Internet", "Mạng nội bộ", "Máy cá nhân", "Lập trình" });
            cboMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboMode.Size = new Size(295, 22);
            cboMode.StyleController = layoutControl1;
            cboMode.TabIndex = 8;
            cboMode.SelectedIndexChanged += CboMode_SelectedIndexChanged;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem3, layoutControlItem4, layoutControlItem5, layoutControlItem6, layoutControlItem7, emptySpaceItem1, emptySpaceItem2, layoutControlItem2 });
            Root.Name = "Root";
            Root.Size = new Size(427, 283);
            Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = textEdit1;
            layoutControlItem1.Location = new Point(0, 26);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new Size(407, 26);
            layoutControlItem1.Text = "Địa chỉ IP:";
            layoutControlItem1.TextSize = new Size(96, 16);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = textEdit3;
            layoutControlItem3.Location = new Point(0, 52);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new Size(407, 26);
            layoutControlItem3.Text = "Cơ sở dữ liệu:";
            layoutControlItem3.TextSize = new Size(96, 16);
            // 
            // layoutControlItem4
            // 
            layoutControlItem4.Control = textEdit4;
            layoutControlItem4.Location = new Point(0, 78);
            layoutControlItem4.Name = "layoutControlItem4";
            layoutControlItem4.Size = new Size(407, 26);
            layoutControlItem4.Text = "Tài khoản:";
            layoutControlItem4.TextSize = new Size(96, 16);
            // 
            // layoutControlItem5
            // 
            layoutControlItem5.Control = textEdit5;
            layoutControlItem5.Location = new Point(0, 104);
            layoutControlItem5.Name = "layoutControlItem5";
            layoutControlItem5.Size = new Size(407, 26);
            layoutControlItem5.Text = "Mật khẩu:";
            layoutControlItem5.TextSize = new Size(96, 16);
            // 
            // layoutControlItem6
            // 
            layoutControlItem6.Control = simpleButton1;
            layoutControlItem6.Location = new Point(57, 130);
            layoutControlItem6.Name = "layoutControlItem6";
            layoutControlItem6.Size = new Size(145, 133);
            layoutControlItem6.TextSize = new Size(0, 0);
            layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            layoutControlItem7.Control = simpleButton2;
            layoutControlItem7.Location = new Point(202, 130);
            layoutControlItem7.Name = "layoutControlItem7";
            layoutControlItem7.Size = new Size(145, 133);
            layoutControlItem7.TextSize = new Size(0, 0);
            layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new Point(347, 130);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new Size(60, 133);
            emptySpaceItem1.TextSize = new Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.AllowHotTrack = false;
            emptySpaceItem2.Location = new Point(0, 130);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new Size(57, 133);
            emptySpaceItem2.TextSize = new Size(0, 0);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = cboMode;
            layoutControlItem2.Location = new Point(0, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new Size(407, 26);
            layoutControlItem2.Text = "Chế độ làm việc:";
            layoutControlItem2.TextSize = new Size(96, 16);
            // 
            // ServerForm
            // 
            Appearance.Options.UseFont = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(427, 283);
            Controls.Add(layoutControl1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            IconOptions.Image = (Image)resources.GetObject("ServerForm.IconOptions.Image");
            Margin = new Padding(5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ServerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cấu hình máy chủ dữ liệu";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit3.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit4.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit5.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboMode.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem4).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem5).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem6).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem7).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.TextEdit textEdit4;
        private DevExpress.XtraEditors.TextEdit textEdit5;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.ComboBoxEdit cboMode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}