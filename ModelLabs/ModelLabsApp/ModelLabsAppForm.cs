using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using FTN.ServiceContracts;
using System.Collections.Generic;
using System.ServiceModel;

namespace ModelLabsApp
{
    public partial class ModelLabsAppForm : Form
    {
        private CIMAdapter adapter = new CIMAdapter();
        private Delta nmsDelta = null;
        private NetworkModelGDAProxy gdaProxy = null;

        // GDA dynamic controls
        private TextBox txtGid;
        private Button btnGetValues;
        private TextBox txtModelCode;
        private Button btnGetExtent;
        private TextBox txtSrcGid;
        private TextBox txtPropId;
        private TextBox txtType;
        private Button btnGetRelated;

        public ModelLabsAppForm()
        {
            InitializeComponent();

            InitGUIElements();
            BuildFullLayout(); // <-- cela forma preslozena preko layout panela

            this.FormClosing += ModelLabsAppForm_FormClosing;
        }

        private void InitGUIElements()
        {
            buttonConvertCIM.Enabled = false;
            buttonApplyDelta.Enabled = false;

            comboBoxProfile.DataSource = Enum.GetValues(typeof(SupportedProfiles));
            comboBoxProfile.SelectedItem = SupportedProfiles.Projekat96;
            //comboBoxProfile.Enabled = false; //// other profiles are not supported
        }

        // -------------------- FULL LAYOUT --------------------
        private void BuildFullLayout()
        {
            // IMPORTANT: promeni ovo na pravo ime Browse dugmeta iz Designer-a
            // Ako ti se browse dugme drugačije zove, ovde samo zameni:
            // Button browseBtn = buttonBrowseLocation;  (ili kako već)
            Button browseBtn = null;

            // Ako imaš buttonBrowse u Designer-u:
            // browseBtn = buttonBrowse;
            //
            // Ako nisi siguran, probaj da ga pronađeš po tekstu:
            foreach (Control c in this.Controls)
            {
                if (c is Button b && (b.Text?.ToLower().Contains("browse") ?? false))
                {
                    browseBtn = b;
                    break;
                }
            }

            if (browseBtn == null)
            {
                browseBtn = new Button()
                {
                    Text = "Browse...",
                    AutoSize = true,
                    Anchor = AnchorStyles.Right
                };
                browseBtn.Click += buttonBrowseLocationOnClick;
            }

            this.SuspendLayout();

            // Sačuvaj sve kontrole pa očisti formu
            var oldControls = new List<Control>();
            foreach (Control c in this.Controls) oldControls.Add(c);
            this.Controls.Clear();

            // Root layout
            var root = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // top
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // gda
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // report (grow)
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // bottom

            // ---------------- TOP PANEL ----------------
            var top = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,
                ColumnCount = 3,
                AutoSize = true
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // Row 1: CIM/XML file + Browse
            var lblFile = new Label()
            {
                Text = "CIM/XML file :",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 8, 10, 0)
            };

            textBoxCIMFile.Dock = DockStyle.Fill;
            textBoxCIMFile.Margin = new Padding(0, 4, 10, 4);

            browseBtn.AutoSize = true;
            browseBtn.Anchor = AnchorStyles.Right;
            browseBtn.Margin = new Padding(0, 3, 0, 3);

            top.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            top.Controls.Add(lblFile, 0, 0);
            top.Controls.Add(textBoxCIMFile, 1, 0);
            top.Controls.Add(browseBtn, 2, 0);

            // Row 2: Profile + Convert/Apply
            var lblProfile = new Label()
            {
                Text = "CIM Profile :",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 10, 10, 0)
            };

            comboBoxProfile.Dock = DockStyle.Fill;
            comboBoxProfile.Margin = new Padding(0, 10, 10, 4);

            var rightButtons = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Margin = new Padding(0, 2, 0, 0)
            };

            int sideBtnW = 120;
            buttonConvertCIM.Width = sideBtnW;
            buttonApplyDelta.Width = sideBtnW;

            rightButtons.Controls.Add(buttonConvertCIM);
            rightButtons.Controls.Add(buttonApplyDelta);

            top.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            top.Controls.Add(lblProfile, 0, 1);
            top.Controls.Add(comboBoxProfile, 1, 1);
            top.Controls.Add(rightButtons, 2, 1);

            // ---------------- GDA BOX ----------------
            var gdaBox = CreateGdaGroupBox();
            gdaBox.Margin = new Padding(0, 10, 0, 6);

            // ---------------- REPORT PANEL ----------------
            var reportPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            reportPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            reportPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            labelReport.Text = "Report:";
            labelReport.AutoSize = true;
            labelReport.Margin = new Padding(0, 6, 0, 6);

            richTextBoxReport.Dock = DockStyle.Fill;
            richTextBoxReport.Margin = new Padding(0);

            reportPanel.Controls.Add(labelReport, 0, 0);
            reportPanel.Controls.Add(richTextBoxReport, 0, 1);

            // ---------------- BOTTOM PANEL ----------------
            var bottom = new FlowLayoutPanel()
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(0, 8, 0, 0)
            };

            buttonExit.AutoSize = true;
            bottom.Controls.Add(buttonExit);

            // Assemble
            root.Controls.Add(top, 0, 0);
            root.Controls.Add(gdaBox, 0, 1);
            root.Controls.Add(reportPanel, 0, 2);
            root.Controls.Add(bottom, 0, 3);

            this.Controls.Add(root);

            this.ResumeLayout(true);
        }

        private GroupBox CreateGdaGroupBox()
        {
            int actionBtnWidth = 130;

            GroupBox gBox = new GroupBox()
            {
                Text = "GDA citanje",
                Dock = DockStyle.Top,
                Height = 160
            };

            var main = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 3
            };

            main.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

            // Row 1
            var row1 = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2 };
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            row1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, actionBtnWidth));

            txtGid = new TextBox() { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 10, 5) };
            btnGetValues = new Button() { Text = "Get values", Width = actionBtnWidth, Dock = DockStyle.Fill };
            btnGetValues.Click += BtnGetValues_Click;

            row1.Controls.Add(txtGid, 0, 0);
            row1.Controls.Add(btnGetValues, 1, 0);

            var lblGid = new Label() { Text = "GID:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 10, 0) };
            main.Controls.Add(lblGid, 0, 0);
            main.Controls.Add(row1, 1, 0);

            // Row 2
            var row2 = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2 };
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            row2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, actionBtnWidth));

            txtModelCode = new TextBox() { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 10, 5) };
            btnGetExtent = new Button() { Text = "Get extent", Width = actionBtnWidth, Dock = DockStyle.Fill };
            btnGetExtent.Click += BtnGetExtent_Click;

            row2.Controls.Add(txtModelCode, 0, 0);
            row2.Controls.Add(btnGetExtent, 1, 0);

            var lblMc = new Label() { Text = "ModelCode:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 10, 0) };
            main.Controls.Add(lblMc, 0, 1);
            main.Controls.Add(row2, 1, 1);

            // Row 3
            var row3 = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 7
            };

            row3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24f));
            row3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, actionBtnWidth));

            var lblSrc = new Label() { Text = "Src GID:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 6, 0) };
            txtSrcGid = new TextBox() { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 10, 5) };

            var lblProp = new Label() { Text = "PropertyId:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 6, 0) };
            txtPropId = new TextBox() { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 10, 5) };

            var lblType = new Label() { Text = "Type:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 10, 6, 0) };
            txtType = new TextBox() { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 10, 5) };

            btnGetRelated = new Button() { Text = "Get related", Width = actionBtnWidth, Dock = DockStyle.Fill };
            btnGetRelated.Click += BtnGetRelated_Click;

            row3.Controls.Add(lblSrc, 0, 0);
            row3.Controls.Add(txtSrcGid, 1, 0);
            row3.Controls.Add(lblProp, 2, 0);
            row3.Controls.Add(txtPropId, 3, 0);
            row3.Controls.Add(lblType, 4, 0);
            row3.Controls.Add(txtType, 5, 0);
            row3.Controls.Add(btnGetRelated, 6, 0);

            main.Controls.Add(new Panel() { Dock = DockStyle.Fill }, 0, 2);
            main.Controls.Add(row3, 1, 2);

            gBox.Controls.Add(main);
            return gBox;
        }

        // -------------------- CIM FILE --------------------
        private void ShowOpenCIMXMLFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open CIM Document File..";
            openFileDialog.Filter = "CIM-XML Files|*.xml;*.txt;*.rdf|All Files|*.*";
            openFileDialog.RestoreDirectory = true;

            DialogResult dialogResponse = openFileDialog.ShowDialog(this);
            if (dialogResponse == DialogResult.OK)
            {
                textBoxCIMFile.Text = openFileDialog.FileName;
                toolTipControl.SetToolTip(textBoxCIMFile, openFileDialog.FileName);
                buttonConvertCIM.Enabled = true;
                richTextBoxReport.Clear();
            }
            else
            {
                buttonConvertCIM.Enabled = false;
            }
        }

        private void ConvertCIMXMLToDMSNetworkModelDelta()
        {
            try
            {
                if (textBoxCIMFile.Text == string.Empty)
                {
                    MessageBox.Show("Must enter CIM/XML file.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string log;
                nmsDelta = null;
                using (FileStream fs = File.Open(textBoxCIMFile.Text, FileMode.Open))
                {
                    nmsDelta = adapter.CreateDelta(fs, (SupportedProfiles)(comboBoxProfile.SelectedItem), out log);
                    richTextBoxReport.Text = log;
                }
                if (nmsDelta != null)
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(".\\deltaExport.xml", Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        nmsDelta.ExportToXml(xmlWriter);
                        xmlWriter.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error occurred.\n\n{0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            buttonApplyDelta.Enabled = (nmsDelta != null);
            textBoxCIMFile.Text = string.Empty;
        }

        // -------------------- GDA --------------------
        private void EnsureProxy()
        {
            if (gdaProxy != null && gdaProxy.State == CommunicationState.Faulted)
            {
                try { gdaProxy.Abort(); } catch { }
                gdaProxy = null;
            }

            if (gdaProxy == null)
            {
                gdaProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaProxy.Open();
            }
        }

        private void AppendOutput(string text)
        {
            richTextBoxReport.AppendText(text + Environment.NewLine);
        }

        private long ParseGid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("GID cannot be empty.");
            }
            input = input.Trim();
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt64(input, 16);
            }
            return long.Parse(input);
        }

        private ModelCode ParseModelCodeSafe(string input, string fieldName, bool allowZero = false)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (allowZero) return 0;
                throw new Exception($"{fieldName} cannot be empty.");
            }

            input = input.Trim();

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                long hexVal = long.Parse(input.Substring(2), System.Globalization.NumberStyles.HexNumber);
                if (allowZero && hexVal == 0) return 0;
                return (ModelCode)hexVal;
            }

            if (long.TryParse(input, out long decVal))
            {
                if (allowZero && decVal == 0) return 0;
                return (ModelCode)decVal;
            }

            if (Enum.TryParse(input, true, out ModelCode mc))
            {
                if (allowZero && Convert.ToInt64(mc) == 0) return 0;
                return mc;
            }

            throw new Exception($"{fieldName} '{input}' nije validan ModelCode.");
        }

        private string ExportResourceDescription(ResourceDescription rd)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("GID=0x{0:X16}", rd.Id));
            foreach (Property p in rd.Properties)
            {
                sb.AppendLine(string.Format("{0}: {1}", p.Id, p.ToString()));
            }
            return sb.ToString();
        }

        private void BtnGetValues_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureProxy();
                long gid = ParseGid(txtGid.Text);
                ModelResourcesDesc mr = new ModelResourcesDesc();
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(gid);
                List<ModelCode> props = mr.GetAllPropertyIds((DMSType)type);
                ResourceDescription rd = gdaProxy.GetValues(gid, props);
                AppendOutput(ExportResourceDescription(rd));
            }
            catch (Exception ex)
            {
                AppendOutput("GetValues error: " + ex.Message);
            }
        }

        private void BtnGetExtent_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureProxy();
                long mc = long.Parse(txtModelCode.Text);
                ModelResourcesDesc mr = new ModelResourcesDesc();
                List<ModelCode> props = mr.GetAllPropertyIds((ModelCode)mc);
                int itId = gdaProxy.GetExtentValues((ModelCode)mc, props);
                int left = gdaProxy.IteratorResourcesLeft(itId);
                StringBuilder sb = new StringBuilder();
                while (left > 0)
                {
                    List<ResourceDescription> rds = gdaProxy.IteratorNext(10, itId);
                    foreach (var rd in rds)
                    {
                        sb.AppendLine(ExportResourceDescription(rd));
                        sb.AppendLine();
                    }
                    left = gdaProxy.IteratorResourcesLeft(itId);
                }
                gdaProxy.IteratorClose(itId);
                AppendOutput(sb.ToString());
            }
            catch (Exception ex)
            {
                AppendOutput("GetExtent error: " + ex.Message);
            }
        }

     

        private void BtnGetRelated_Click(object sender, EventArgs e)
        {
            try
            {
                EnsureProxy();

                long srcGid = ParseGid(txtSrcGid.Text);

                ModelCode propMc = ParseModelCodeSafe(txtPropId.Text, "PropertyId");
                ModelCode typeMc = ParseModelCodeSafe(txtType.Text, "Type", allowZero: true);

                Association assoc = new Association()
                {
                    PropertyId = propMc,
                    Type = typeMc
                };

                List<ModelCode> props = new List<ModelCode>()
                {
                    ModelCode.IDOBJ_MRID,
                    ModelCode.IDOBJ_NAME,
                    ModelCode.IDOBJ_ALIASNAME
                };

                int itId = gdaProxy.GetRelatedValues(srcGid, props, assoc);
                int left = gdaProxy.IteratorResourcesLeft(itId);

                StringBuilder sb = new StringBuilder();
                while (left > 0)
                {
                    List<ResourceDescription> rds = gdaProxy.IteratorNext(10, itId);
                    foreach (var rd in rds)
                    {
                        sb.AppendLine(ExportResourceDescription(rd));
                        sb.AppendLine();
                    }
                    left = gdaProxy.IteratorResourcesLeft(itId);
                }

                gdaProxy.IteratorClose(itId);
                AppendOutput(sb.ToString());
            }
            catch (Exception ex)
            {
                AppendOutput("GetRelated error: " + ex.Message);
            }

        }

        private void ModelLabsAppForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (gdaProxy != null)
            {
                try { gdaProxy.Close(); } catch { }
                gdaProxy = null;
            }
        }

        // -------------------- APPLY DELTA --------------------
        private void ApplyDMSNetworkModelDelta()
        {
            if (nmsDelta != null)
            {
                try
                {
                    string log = adapter.ApplyUpdates(nmsDelta);
                    richTextBoxReport.AppendText(log);
                    nmsDelta = null;
                    buttonApplyDelta.Enabled = (nmsDelta != null);
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("An error occurred.\n\n{0}", e.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No data is imported into delta object.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // -------------------- DESIGNER EVENTS --------------------
        private void buttonBrowseLocationOnClick(object sender, EventArgs e)
        {
            ShowOpenCIMXMLFileDialog();
        }

        private void textBoxCIMFileOnDoubleClick(object sender, EventArgs e)
        {
            ShowOpenCIMXMLFileDialog();
        }

        private void buttonConvertCIMOnClick(object sender, EventArgs e)
        {
            ConvertCIMXMLToDMSNetworkModelDelta();
        }

        private void buttonApplyDeltaOnClick(object sender, EventArgs e)
        {
            ApplyDMSNetworkModelDelta();
        }

        private void buttonExitOnClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
