using System;
using System.Windows.Forms;

namespace OnTopReplica.Forms {
    
    public class AddRegionToProfileDialog : Form {
        
        private Label lblPrompt;
        private TextBox txtRegionName;
        private GroupBox grpAnchor;
        private RadioButton rbTopLeft;
        private RadioButton rbTopRight;
        private RadioButton rbBottomLeft;
        private RadioButton rbBottomRight;
        private CheckBox chkScaleWithSource;
        private Button btnOK;
        private Button btnCancel;

        public string RegionName => txtRegionName.Text.Trim();

        public RegionAnchor RegionAnchor {
            get {
                if (rbTopRight.Checked) return RegionAnchor.TopRight;
                if (rbBottomLeft.Checked) return RegionAnchor.BottomLeft;
                if (rbBottomRight.Checked) return RegionAnchor.BottomRight;
                return RegionAnchor.TopLeft;
            }
        }

        public bool ScaleWithSourceWindow => chkScaleWithSource.Checked;
        
        public AddRegionToProfileDialog() {
            InitializeComponent();
        }
        
        private void InitializeComponent() {
            this.lblPrompt = new Label();
            this.txtRegionName = new TextBox();
            this.grpAnchor = new GroupBox();
            this.rbTopLeft = new RadioButton();
            this.rbTopRight = new RadioButton();
            this.rbBottomLeft = new RadioButton();
            this.rbBottomRight = new RadioButton();
            this.chkScaleWithSource = new CheckBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.grpAnchor.SuspendLayout();
            this.SuspendLayout();
            
            // lblPrompt
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(12, 15);
            this.lblPrompt.Size = new System.Drawing.Size(360, 13);
            this.lblPrompt.Text = "Name für diese Region/Konfiguration:";
            
            // txtRegionName
            this.txtRegionName.Location = new System.Drawing.Point(12, 35);
            this.txtRegionName.Size = new System.Drawing.Size(360, 20);
            this.txtRegionName.TabIndex = 0;

            // grpAnchor
            this.grpAnchor.Controls.Add(this.rbTopLeft);
            this.grpAnchor.Controls.Add(this.rbTopRight);
            this.grpAnchor.Controls.Add(this.rbBottomLeft);
            this.grpAnchor.Controls.Add(this.rbBottomRight);
            this.grpAnchor.Location = new System.Drawing.Point(12, 65);
            this.grpAnchor.Name = "grpAnchor";
            this.grpAnchor.Size = new System.Drawing.Size(360, 90);
            this.grpAnchor.TabIndex = 1;
            this.grpAnchor.TabStop = false;
            this.grpAnchor.Text = "Region-Anker";

            // rbTopLeft
            this.rbTopLeft.AutoSize = true;
            this.rbTopLeft.Checked = true;
            this.rbTopLeft.Location = new System.Drawing.Point(15, 25);
            this.rbTopLeft.Name = "rbTopLeft";
            this.rbTopLeft.Size = new System.Drawing.Size(120, 17);
            this.rbTopLeft.TabIndex = 0;
            this.rbTopLeft.TabStop = true;
            this.rbTopLeft.Text = "Oben Links";
            this.rbTopLeft.UseVisualStyleBackColor = true;

            // rbTopRight
            this.rbTopRight.AutoSize = true;
            this.rbTopRight.Location = new System.Drawing.Point(185, 25);
            this.rbTopRight.Name = "rbTopRight";
            this.rbTopRight.Size = new System.Drawing.Size(120, 17);
            this.rbTopRight.TabIndex = 1;
            this.rbTopRight.Text = "Oben Rechts";
            this.rbTopRight.UseVisualStyleBackColor = true;

            // rbBottomLeft
            this.rbBottomLeft.AutoSize = true;
            this.rbBottomLeft.Location = new System.Drawing.Point(15, 55);
            this.rbBottomLeft.Name = "rbBottomLeft";
            this.rbBottomLeft.Size = new System.Drawing.Size(120, 17);
            this.rbBottomLeft.TabIndex = 2;
            this.rbBottomLeft.Text = "Unten Links";
            this.rbBottomLeft.UseVisualStyleBackColor = true;

            // rbBottomRight
            this.rbBottomRight.AutoSize = true;
            this.rbBottomRight.Location = new System.Drawing.Point(185, 55);
            this.rbBottomRight.Name = "rbBottomRight";
            this.rbBottomRight.Size = new System.Drawing.Size(120, 17);
            this.rbBottomRight.TabIndex = 3;
            this.rbBottomRight.Text = "Unten Rechts";
            this.rbBottomRight.UseVisualStyleBackColor = true;

            // chkScaleWithSource
            this.chkScaleWithSource.AutoSize = true;
            this.chkScaleWithSource.Location = new System.Drawing.Point(12, 165);
            this.chkScaleWithSource.Name = "chkScaleWithSource";
            this.chkScaleWithSource.Size = new System.Drawing.Size(360, 17);
            this.chkScaleWithSource.TabIndex = 2;
            this.chkScaleWithSource.Text = "Fenstergröße mit Quell-Fenster-Auflösung skalieren";
            this.chkScaleWithSource.UseVisualStyleBackColor = true;

            // btnOK
            this.btnOK.Location = new System.Drawing.Point(216, 195);
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += BtnOK_Click;

            // btnCancel
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 195);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Abbrechen";
            this.btnCancel.UseVisualStyleBackColor = true;

            // AddRegionToProfileDialog
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 230);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkScaleWithSource);
            this.Controls.Add(this.grpAnchor);
            this.Controls.Add(this.txtRegionName);
            this.Controls.Add(this.lblPrompt);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Region hinzufügen";
            this.grpAnchor.ResumeLayout(false);
            this.grpAnchor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private void BtnOK_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(txtRegionName.Text)) {
                MessageBox.Show(
                    "Bitte geben Sie einen Namen für die Region ein.",
                    "Eingabe erforderlich",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtRegionName.Focus();
                return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
