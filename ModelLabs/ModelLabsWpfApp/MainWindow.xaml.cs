using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Xml;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using FTN.ServiceContracts;

namespace ModelLabsWpfApp
{
    public partial class MainWindow : Window
    {
        private readonly CIMAdapter adapter = new CIMAdapter();
        private Delta currentDelta;
        private NetworkModelGDAProxy gdaProxy;

        public MainWindow()
        {
            InitializeComponent();
            InitUi();
        }

        private void InitUi()
        {
            cmbProfile.ItemsSource = Enum.GetValues(typeof(SupportedProfiles)).Cast<SupportedProfiles>();
            cmbProfile.SelectedItem = SupportedProfiles.Projekat96;
        }

        private void BtnBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open CIM Document File..",
                Filter = "CIM-XML Files|*.xml;*.txt;*.rdf|All Files|*.*",
                RestoreDirectory = true
            };
            if (dlg.ShowDialog() == true)
            {
                txtCimPath.Text = dlg.FileName;
                txtReport.Clear();
            }
        }

        private void BtnConvert_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCimPath.Text) || !File.Exists(txtCimPath.Text))
            {
                MessageBox.Show("Izaberi CIM/XML fajl.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                string log;
                currentDelta = null;
                using (FileStream fs = File.Open(txtCimPath.Text, FileMode.Open, FileAccess.Read))
                {
                    currentDelta = adapter.CreateDelta(fs, (SupportedProfiles)cmbProfile.SelectedItem, out log);
                    txtReport.Text = log;
                }

                if (currentDelta != null)
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(".\\deltaExport.xml", Encoding.UTF8))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        currentDelta.ExportToXml(xmlWriter);
                        xmlWriter.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri konverziji: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (currentDelta == null)
            {
                MessageBox.Show("Nema delta objekta za slanje.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                string log = adapter.ApplyUpdates(currentDelta);
                AppendOutput(log);
                currentDelta = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri Apply Delta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGetValues_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureProxy();
                if (!TryParseGid(txtGid.Text, out long gid, out string gidError))
                {
                    AppendOutput(gidError);
                    return;
                }

                if (!EntityExists(gid))
                {
                    AppendOutput("GID ne postoji u modelu.");
                    return;
                }
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

        private void BtnGetExtent_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureProxy();
                ModelCode mc = ParseModelCodeSafe(txtModelCode.Text, "ModelCode");
                ModelResourcesDesc mr = new ModelResourcesDesc();
                List<ModelCode> props = mr.GetAllPropertyIds(mc);
                int itId = gdaProxy.GetExtentValues(mc, props);
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

        private void BtnGetRelated_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EnsureProxy();

                if (!TryParseGid(txtSrcGid.Text, out long srcGid, out string gidError))
                {
                    AppendOutput(gidError);
                    return;
                }

                if (!EntityExists(srcGid))
                {
                    AppendOutput("Src GID ne postoji u modelu.");
                    return;
                }
                ModelCode propMc = ParseModelCodeSafe(txtPropertyId.Text, "PropertyId");
                ModelCode typeMc = ParseModelCodeSafe(txtAssociationType.Text, "Type", allowZero: true);

                try
                {
                    ModelResourcesDesc mr = new ModelResourcesDesc();
                    List<ModelCode> allowedProps = mr.GetAllPropertyIdsForEntityId(srcGid);
                    if (!allowedProps.Contains(propMc))
                    {
                        AppendOutput("PropertyId ne pripada tipu izvornog GID-a.");
                        return;
                    }

                    PropertyType pType = Property.GetPropertyType(propMc);
                    if (pType != PropertyType.Reference && pType != PropertyType.ReferenceVector)
                    {
                        AppendOutput("PropertyId nije tip reference ili reference vector.");
                        return;
                    }
                }
                catch
                {
                    AppendOutput("Ne mogu da proverim PropertyId za dati GID.");
                    return;
                }

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

        private long ParseGid(string input, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception($"{fieldName} je prazan.");
            }

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt64(input, 16);
            }

            return long.Parse(input);
        }

        private bool TryParseGid(string input, out long gid, out string error)
        {
            gid = 0;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Src GID je prazan.";
                return false;
            }

            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                if (long.TryParse(input.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out gid))
                {
                    return true;
                }

                error = "Src GID nije validan hex format.";
                return false;
            }

            if (long.TryParse(input, out gid))
            {
                return true;
            }

            error = "Src GID nije validan broj.";
            return false;
        }

        private bool EntityExists(long gid)
        {
            try
            {
                DMSType type = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid);
                ModelResourcesDesc mr = new ModelResourcesDesc();
                ModelCode typeCode = mr.GetModelCodeFromType(type);
                List<ModelCode> props = new List<ModelCode> { ModelCode.IDOBJ_GID };

                int itId = gdaProxy.GetExtentValues(typeCode, props);
                int left = gdaProxy.IteratorResourcesLeft(itId);

                while (left > 0)
                {
                    List<ResourceDescription> rds = gdaProxy.IteratorNext(50, itId);
                    foreach (var rd in rds)
                    {
                        if (rd.Id == gid)
                        {
                            gdaProxy.IteratorClose(itId);
                            return true;
                        }
                    }
                    left = gdaProxy.IteratorResourcesLeft(itId);
                }

                gdaProxy.IteratorClose(itId);
                return false;
            }
            catch
            {
                return false;
            }
        }

        private ModelCode ParseModelCodeSafe(string input, string fieldName, bool allowZero = false)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                if (allowZero) { return 0; }
                throw new Exception($"{fieldName} je prazan.");
            }

            // hex/dec
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return (ModelCode)Convert.ToInt64(input, 16);
            }

            if (long.TryParse(input, out long decVal))
            {
                return (ModelCode)decVal;
            }

            // enum name
            if (Enum.TryParse(input, true, out ModelCode byName))
            {
                return byName;
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

        private void AppendOutput(string text)
        {
            txtReport.AppendText(text + Environment.NewLine);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (gdaProxy != null)
            {
                try { gdaProxy.Close(); } catch { }
            }
        }
    }
}
