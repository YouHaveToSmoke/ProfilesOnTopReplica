using System;
using System.Linq;
using System.Windows.Forms;

namespace OnTopReplica.Forms {
    
    public class ProfileRegionSelectionDialog : Form {
        
        private Label lblPrompt;
        private ListBox lstRegions;
        private Button btnLoad;
        private Button btnLoadAll;
        private Button btnCancel;
        
        private Profile _profile;
        
        public ProfileRegionConfiguration SelectedConfiguration => 
            lstRegions.SelectedItem as ProfileRegionConfiguration;
        
        public bool LoadAll { get; private set; }
        
        public ProfileRegionSelectionDialog(Profile profile) {
            _profile = profile;
            InitializeComponent();
            LoadRegions();
        }
        
        private void InitializeComponent() {
            this.lblPrompt = new Label();
            this.lstRegions = new ListBox();
            this.btnLoad = new Button();
            this.btnLoadAll = new Button();
            this.btnCancel = new Button();
            
            this.SuspendLayout();
            
            // lblPrompt
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(12, 15);
            this.lblPrompt.Size = new System.Drawing.Size(360, 13);
            this.lblPrompt.Text = "Wählen Sie eine Region zum Laden:";
            
            // lstRegions
            this.lstRegions.FormattingEnabled = true;
            this.lstRegions.Location = new System.Drawing.Point(12, 35);
            this.lstRegions.Size = new System.Drawing.Size(360, 150);
            this.lstRegions.TabIndex = 0;
            this.lstRegions.DoubleClick += LstRegions_DoubleClick;
            
            // btnLoad
            this.btnLoad.Location = new System.Drawing.Point(135, 195);
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Laden";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += BtnLoad_Click;
            
            // btnLoadAll
            this.btnLoadAll.Location = new System.Drawing.Point(216, 195);
            this.btnLoadAll.Size = new System.Drawing.Size(75, 23);
            this.btnLoadAll.TabIndex = 2;
            this.btnLoadAll.Text = "Alle laden";
            this.btnLoadAll.UseVisualStyleBackColor = true;
            this.btnLoadAll.Click += BtnLoadAll_Click;
            
            // btnCancel
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 195);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Abbrechen";
            this.btnCancel.UseVisualStyleBackColor = true;
            
            // ProfileRegionSelectionDialog
            this.AcceptButton = this.btnLoad;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 230);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLoadAll);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lstRegions);
            this.Controls.Add(this.lblPrompt);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Region auswählen";
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        private void LoadRegions() {
            lstRegions.Items.Clear();
            
            if (_profile.RegionConfigurations.Count == 0) {
                lstRegions.Items.Add("(Keine Regionen vorhanden)");
                lstRegions.Enabled = false;
                btnLoad.Enabled = false;
                btnLoadAll.Enabled = false;
            }
            else {
                foreach (var config in _profile.RegionConfigurations) {
                    lstRegions.Items.Add(config);
                }
                
                if (lstRegions.Items.Count > 0) {
                    lstRegions.SelectedIndex = 0;
                }
                
                btnLoadAll.Enabled = lstRegions.Items.Count > 1;
            }
        }
        
        private void BtnLoad_Click(object sender, EventArgs e) {
            if (lstRegions.SelectedItem == null) {
                MessageBox.Show(
                    "Bitte wählen Sie eine Region aus.",
                    "Auswahl erforderlich",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            
            LoadAll = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void BtnLoadAll_Click(object sender, EventArgs e) {
            LoadAll = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        private void LstRegions_DoubleClick(object sender, EventArgs e) {
            if (lstRegions.SelectedItem != null) {
                BtnLoad_Click(sender, e);
            }
        }
    }
}
