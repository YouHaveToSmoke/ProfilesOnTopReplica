using System;
using System.Linq;
using System.Windows.Forms;

namespace OnTopReplica.Forms {
    
    /// <summary>
    /// Dialog to select an existing profile and add the current region to it.
    /// </summary>
    public class AddRegionToExistingProfileDialog : Form {
        
        private Label lblSelectProfile;
        private ComboBox cmbProfiles;
        private Label lblRegionName;
        private TextBox txtRegionName;
        private GroupBox grpAnchor;
        private RadioButton rbTopLeft;
        private RadioButton rbTopRight;
        private RadioButton rbBottomLeft;
        private RadioButton rbBottomRight;
        private CheckBox chkScaleWithSource;
        private Button btnOK;
        private Button btnCancel;

        public string SelectedProfileName => cmbProfiles.SelectedItem?.ToString();
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
        
        public AddRegionToExistingProfileDialog() {
            InitializeComponent();
            LoadProfiles();
        }
        
        private void InitializeComponent() {
            this.lblSelectProfile = new System.Windows.Forms.Label();
            this.cmbProfiles = new System.Windows.Forms.ComboBox();
            this.lblRegionName = new System.Windows.Forms.Label();
            this.txtRegionName = new System.Windows.Forms.TextBox();
            this.grpAnchor = new System.Windows.Forms.GroupBox();
            this.rbTopLeft = new System.Windows.Forms.RadioButton();
            this.rbTopRight = new System.Windows.Forms.RadioButton();
            this.rbBottomLeft = new System.Windows.Forms.RadioButton();
            this.rbBottomRight = new System.Windows.Forms.RadioButton();
            this.chkScaleWithSource = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpAnchor.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSelectProfile
            // 
            this.lblSelectProfile.AutoSize = true;
            this.lblSelectProfile.Location = new System.Drawing.Point(12, 15);
            this.lblSelectProfile.Name = "lblSelectProfile";
            this.lblSelectProfile.Size = new System.Drawing.Size(71, 13);
            this.lblSelectProfile.TabIndex = 6;
            this.lblSelectProfile.Text = "Select profile:";
            // 
            // cmbProfiles
            // 
            this.cmbProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProfiles.FormattingEnabled = true;
            this.cmbProfiles.Location = new System.Drawing.Point(12, 35);
            this.cmbProfiles.Name = "cmbProfiles";
            this.cmbProfiles.Size = new System.Drawing.Size(360, 21);
            this.cmbProfiles.TabIndex = 0;
            // 
            // lblRegionName
            // 
            this.lblRegionName.AutoSize = true;
            this.lblRegionName.Location = new System.Drawing.Point(12, 65);
            this.lblRegionName.Name = "lblRegionName";
            this.lblRegionName.Size = new System.Drawing.Size(89, 13);
            this.lblRegionName.TabIndex = 5;
            this.lblRegionName.Text = "Name this region:";
            // 
            // txtRegionName
            // 
            this.txtRegionName.Location = new System.Drawing.Point(12, 85);
            this.txtRegionName.Name = "txtRegionName";
            this.txtRegionName.Size = new System.Drawing.Size(360, 20);
            this.txtRegionName.TabIndex = 1;
            // 
            // grpAnchor
            // 
            this.grpAnchor.Controls.Add(this.rbTopLeft);
            this.grpAnchor.Controls.Add(this.rbTopRight);
            this.grpAnchor.Controls.Add(this.rbBottomLeft);
            this.grpAnchor.Controls.Add(this.rbBottomRight);
            this.grpAnchor.Location = new System.Drawing.Point(12, 115);
            this.grpAnchor.Name = "grpAnchor";
            this.grpAnchor.Size = new System.Drawing.Size(360, 90);
            this.grpAnchor.TabIndex = 2;
            this.grpAnchor.TabStop = false;
            this.grpAnchor.Text = "Region-anchor";
            // 
            // rbTopLeft
            // 
            this.rbTopLeft.AutoSize = true;
            this.rbTopLeft.Checked = true;
            this.rbTopLeft.Location = new System.Drawing.Point(15, 25);
            this.rbTopLeft.Name = "rbTopLeft";
            this.rbTopLeft.Size = new System.Drawing.Size(79, 17);
            this.rbTopLeft.TabIndex = 0;
            this.rbTopLeft.TabStop = true;
            this.rbTopLeft.Text = "Top left";
            this.rbTopLeft.UseVisualStyleBackColor = true;
            // 
            // rbTopRight
            // 
            this.rbTopRight.AutoSize = true;
            this.rbTopRight.Location = new System.Drawing.Point(185, 25);
            this.rbTopRight.Name = "rbTopRight";
            this.rbTopRight.Size = new System.Drawing.Size(88, 17);
            this.rbTopRight.TabIndex = 1;
            this.rbTopRight.Text = "Top right";
            this.rbTopRight.UseVisualStyleBackColor = true;
            // 
            // rbBottomLeft
            // 
            this.rbBottomLeft.AutoSize = true;
            this.rbBottomLeft.Location = new System.Drawing.Point(15, 55);
            this.rbBottomLeft.Name = "rbBottomLeft";
            this.rbBottomLeft.Size = new System.Drawing.Size(82, 17);
            this.rbBottomLeft.TabIndex = 2;
            this.rbBottomLeft.Text = "Bottom left";
            this.rbBottomLeft.UseVisualStyleBackColor = true;
            // 
            // rbBottomRight
            // 
            this.rbBottomRight.AutoSize = true;
            this.rbBottomRight.Location = new System.Drawing.Point(185, 55);
            this.rbBottomRight.Name = "rbBottomRight";
            this.rbBottomRight.Size = new System.Drawing.Size(91, 17);
            this.rbBottomRight.TabIndex = 3;
            this.rbBottomRight.Text = "Bottom right";
            this.rbBottomRight.UseVisualStyleBackColor = true;
            // 
            // chkScaleWithSource
            // 
            this.chkScaleWithSource.AutoSize = true;
            this.chkScaleWithSource.Location = new System.Drawing.Point(12, 215);
            this.chkScaleWithSource.Name = "chkScaleWithSource";
            this.chkScaleWithSource.Size = new System.Drawing.Size(264, 17);
            this.chkScaleWithSource.TabIndex = 3;
            this.chkScaleWithSource.Text = "Scale replicas with relative size of target";
            this.chkScaleWithSource.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(216, 245);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 245);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddRegionToExistingProfileDialog
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 280);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkScaleWithSource);
            this.Controls.Add(this.grpAnchor);
            this.Controls.Add(this.txtRegionName);
            this.Controls.Add(this.lblRegionName);
            this.Controls.Add(this.cmbProfiles);
            this.Controls.Add(this.lblSelectProfile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddRegionToExistingProfileDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add region to profile:";
            this.grpAnchor.ResumeLayout(false);
            this.grpAnchor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        private void LoadProfiles() {
            cmbProfiles.Items.Clear();
            
            var profiles = ProfileManager.GetAllProfiles();
            
            if (profiles.Count == 0) {
                cmbProfiles.Items.Add("(No profiles found)");
                cmbProfiles.Enabled = false;
            }
            else {
                foreach (var profile in profiles) {
                    cmbProfiles.Items.Add(profile.Name);
                }
                
                if (cmbProfiles.Items.Count > 0) {
                    cmbProfiles.SelectedIndex = 0;
                }
            }
        }
        
        private void BtnOK_Click(object sender, EventArgs e) {
            if (cmbProfiles.SelectedItem == null || cmbProfiles.SelectedItem.ToString().StartsWith("(")) {
                MessageBox.Show(
                    "Please select a profile",
                    "No profile selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                cmbProfiles.Focus();
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtRegionName.Text)) {
                MessageBox.Show(
                    "Please enter a name for this region.",
                    "Region not named",
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
