
namespace WinFormsApp1
{
    partial class YOLO
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Processing = new System.Windows.Forms.Button();
            this.btn_import = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btn_cloud_import = new System.Windows.Forms.Button();
            this.label_file_imported = new System.Windows.Forms.Label();
            this.label_proccessing = new System.Windows.Forms.Label();
            this.checkBox_proccessing = new System.Windows.Forms.CheckBox();
            this.listBox_found_objects = new System.Windows.Forms.ListBox();
            this.label_download_from_cloud = new System.Windows.Forms.Label();
            this.button_save_processed_image = new System.Windows.Forms.Button();
            this.textBox_cloud_import = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Processing
            // 
            this.btn_Processing.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Processing.Location = new System.Drawing.Point(856, 70);
            this.btn_Processing.Name = "btn_Processing";
            this.btn_Processing.Size = new System.Drawing.Size(116, 42);
            this.btn_Processing.TabIndex = 0;
            this.btn_Processing.Text = "Обработка";
            this.btn_Processing.UseVisualStyleBackColor = true;
            this.btn_Processing.Click += new System.EventHandler(this.btn_Processing_Click);
            // 
            // btn_import
            // 
            this.btn_import.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_import.Location = new System.Drawing.Point(856, 22);
            this.btn_import.Name = "btn_import";
            this.btn_import.Size = new System.Drawing.Size(116, 42);
            this.btn_import.TabIndex = 2;
            this.btn_import.Text = "Импорт";
            this.btn_import.UseVisualStyleBackColor = true;
            this.btn_import.Click += new System.EventHandler(this.btn_import_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Location = new System.Drawing.Point(12, 14);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(814, 555);
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // btn_cloud_import
            // 
            this.btn_cloud_import.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_cloud_import.Location = new System.Drawing.Point(856, 118);
            this.btn_cloud_import.Name = "btn_cloud_import";
            this.btn_cloud_import.Size = new System.Drawing.Size(116, 54);
            this.btn_cloud_import.TabIndex = 4;
            this.btn_cloud_import.Text = "Загрузка из облака";
            this.btn_cloud_import.UseVisualStyleBackColor = true;
            this.btn_cloud_import.Click += new System.EventHandler(this.btn_cloud_import_Click);
            // 
            // label_file_imported
            // 
            this.label_file_imported.AutoSize = true;
            this.label_file_imported.Location = new System.Drawing.Point(44, 14);
            this.label_file_imported.Name = "label_file_imported";
            this.label_file_imported.Size = new System.Drawing.Size(0, 20);
            this.label_file_imported.TabIndex = 5;
            // 
            // label_proccessing
            // 
            this.label_proccessing.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_proccessing.AutoSize = true;
            this.label_proccessing.Location = new System.Drawing.Point(806, 248);
            this.label_proccessing.Name = "label_proccessing";
            this.label_proccessing.Size = new System.Drawing.Size(156, 20);
            this.label_proccessing.TabIndex = 6;
            this.label_proccessing.Text = "Найденные объекты:";
            // 
            // checkBox_proccessing
            // 
            this.checkBox_proccessing.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.checkBox_proccessing.AutoSize = true;
            this.checkBox_proccessing.Checked = true;
            this.checkBox_proccessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_proccessing.Location = new System.Drawing.Point(832, 84);
            this.checkBox_proccessing.Name = "checkBox_proccessing";
            this.checkBox_proccessing.Size = new System.Drawing.Size(18, 17);
            this.checkBox_proccessing.TabIndex = 8;
            this.checkBox_proccessing.UseVisualStyleBackColor = true;
            this.checkBox_proccessing.Visible = false;
            // 
            // listBox_found_objects
            // 
            this.listBox_found_objects.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.listBox_found_objects.FormattingEnabled = true;
            this.listBox_found_objects.ItemHeight = 20;
            this.listBox_found_objects.Location = new System.Drawing.Point(806, 271);
            this.listBox_found_objects.Name = "listBox_found_objects";
            this.listBox_found_objects.Size = new System.Drawing.Size(182, 184);
            this.listBox_found_objects.TabIndex = 11;
            // 
            // label_download_from_cloud
            // 
            this.label_download_from_cloud.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_download_from_cloud.AutoSize = true;
            this.label_download_from_cloud.Location = new System.Drawing.Point(806, 208);
            this.label_download_from_cloud.Name = "label_download_from_cloud";
            this.label_download_from_cloud.Size = new System.Drawing.Size(0, 20);
            this.label_download_from_cloud.TabIndex = 13;
            // 
            // button_save_processed_image
            // 
            this.button_save_processed_image.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button_save_processed_image.Location = new System.Drawing.Point(856, 516);
            this.button_save_processed_image.Name = "button_save_processed_image";
            this.button_save_processed_image.Size = new System.Drawing.Size(114, 53);
            this.button_save_processed_image.TabIndex = 14;
            this.button_save_processed_image.Text = "Сохранить изображение";
            this.button_save_processed_image.UseVisualStyleBackColor = true;
            this.button_save_processed_image.Click += new System.EventHandler(this.button_save_processed_image_Click);
            // 
            // textBox_cloud_import
            // 
            this.textBox_cloud_import.Location = new System.Drawing.Point(806, 178);
            this.textBox_cloud_import.Name = "textBox_cloud_import";
            this.textBox_cloud_import.Size = new System.Drawing.Size(182, 27);
            this.textBox_cloud_import.TabIndex = 15;
            // 
            // YOLO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 608);
            this.Controls.Add(this.textBox_cloud_import);
            this.Controls.Add(this.button_save_processed_image);
            this.Controls.Add(this.label_download_from_cloud);
            this.Controls.Add(this.listBox_found_objects);
            this.Controls.Add(this.checkBox_proccessing);
            this.Controls.Add(this.label_proccessing);
            this.Controls.Add(this.label_file_imported);
            this.Controls.Add(this.btn_cloud_import);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btn_import);
            this.Controls.Add(this.btn_Processing);
            this.Name = "YOLO";
            this.Text = "YOLO";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.YOLO_FormClosing);
            this.Load += new System.EventHandler(this.YOLO_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Processing; //name
        private System.Windows.Forms.Button btn_import;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btn_cloud_import;
        private System.Windows.Forms.Label label_file_imported;
        private System.Windows.Forms.Label label_proccessing;
        private System.Windows.Forms.CheckBox checkBox_proccessing;
        private System.Windows.Forms.ListBox listBox_found_objects;
        private System.Windows.Forms.Label label_download_from_cloud;
        private System.Windows.Forms.Button button_save_processed_image;
        private System.Windows.Forms.TextBox textBox_cloud_import;
    }
}

