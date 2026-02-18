using System;
using System.Linq;
using System.Windows.Forms;

namespace OnTopReplica.Forms {
    
    public class ProfileSelectionDialog : Form {
        
        private Label lblPrompt;
        private ListBox lstProfiles;
        private Button btnLoad;
        private Button btnCancel;
        
        public string SelectedProfileName => lstProfiles.SelectedItem?.ToString();
        
        public ProfileSelectionDialog() {
            InitializeComponent();
            LoadProfiles();
        }
        
        private void InitializeComponent() {
            this.lblPrompt = new Label();
            this.lstProfiles = new ListBox();
            this.btnLoad = new Button();
            this.btnCancel = new Button();
            
            this.SuspendLayout();
            
            // lblPrompt
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(12, 15);
            this.lblPrompt.Size = new System.Drawing.Size(250, 13);
            this.lblPrompt.Text = "Select a profile:";
            
            // lstProfiles
            this.lstProfiles.FormattingEnabled = true;
            this.lstProfiles.Location = new System.Drawing.Point(12, 35);
            this.lstProfiles.Size = new System.Drawing.Size(360, 200);
            this.lstProfiles.TabIndex = 0;
            this.lstProfiles.DoubleClick += LstProfiles_DoubleClick;
            
            // btnLoad
            this.btnLoad.Location = new System.Drawing.Point(216, 245);
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += BtnLoad_Click;
            
            // btnCancel
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 245);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            
            // ProfileSelectionDialog
            this.AcceptButton = this.btnLoad;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 280);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lstProfiles);
            this.Controls.Add(this.lblPrompt);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Load profile";
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private void LoadProfiles() {
            lstProfiles.Items.Clear();
            
            var profiles = ProfileManager.GetAllProfiles();
            
            if (profiles.Count == 0) {
                lstProfiles.Items.Add("(No profiles found)");
                lstProfiles.Enabled = false;
                btnLoad.Enabled = false;
            }
            else {
                foreach (var profile in profiles) {
                    lstProfiles.Items.Add(profile.Name);
                }
                
                if (lstProfiles.Items.Count > 0) {
                    lstProfiles.SelectedIndex = 0;
                }
            }
        }
        
        private void BtnLoad_Click(object sender, EventArgs e) {
            if (lstProfiles.SelectedItem == null) {
                MessageBox.Show(
                    "Select a profile",
                    "Selection required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void LstProfiles_DoubleClick(object sender, EventArgs e) {
            if (lstProfiles.SelectedItem != null) {
                BtnLoad_Click(sender, e);
            }
        }
    }
}
