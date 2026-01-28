using System;
using System.Collections.Generic;
using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    /// <summary>
    /// Importer za profil Projekat96.
    /// </summary>
    public class Projekat96Importer
    {
        private static Projekat96Importer instance = null;
        private static readonly object singletonLock = new object();

        private ConcreteModel concreteModel;
        private Delta delta;
        private ImportHelper importHelper;
        private TransformAndLoadReport report;

        public static Projekat96Importer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (singletonLock)
                    {
                        if (instance == null)
                        {
                            instance = new Projekat96Importer();
                            instance.Reset();
                        }
                    }
                }
                return instance;
            }
        }

        public Delta NMSDelta
        {
            get { return delta; }
        }

        public void Reset()
        {
            concreteModel = null;
            delta = new Delta();
            importHelper = new ImportHelper();
            report = null;
        }

        public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
        {
            LogManager.Log("Importing Projekat96 elements...", LogLevel.Info);
            report = new TransformAndLoadReport();
            concreteModel = cimConcreteModel;
            delta.ClearDeltaOperations();

            if ((concreteModel != null) && (concreteModel.ModelMap != null))
            {
                try
                {
                    ConvertModelAndPopulateDelta();
                }
                catch (Exception ex)
                {
                    string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
                    LogManager.Log(message);
                    report.Report.AppendLine(ex.Message);
                    report.Success = false;
                }
            }
            LogManager.Log("Importing Projekat96 elements - END.", LogLevel.Info);
            return report;
        }

        private void ConvertModelAndPopulateDelta()
        {
            LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            ImportAssetModels();
            ImportProductAssetModels();
            ImportAssetInfos();
            ImportAssetContainers();
            ImportAssets();
            ImportSeals();
            ImportComMedias();
            ImportOrganisationRoles();
            ImportAssetOrgRoles();
            ImportAssetOwners();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
        }

        #region Import helpers

        private void ImportAssetModels()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.AssetModel");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.AssetModel cimObj = pair.Value as FTN.AssetModel;
                    ResourceDescription rd = CreateAssetModelResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "AssetModel");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAssetModelResourceDescription(FTN.AssetModel cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASSETMODEL, importHelper.CheckOutIndexForDMSType(DMSType.ASSETMODEL));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateAssetModelProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportProductAssetModels()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.ProductAssetModel");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.ProductAssetModel cimObj = pair.Value as FTN.ProductAssetModel;
                    ResourceDescription rd = CreateProductAssetModelResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "ProductAssetModel");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateProductAssetModelResourceDescription(FTN.ProductAssetModel cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.PRODUCTASSETMODEL, importHelper.CheckOutIndexForDMSType(DMSType.PRODUCTASSETMODEL));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateProductAssetModelProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportAssetInfos()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.AssetInfo");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.AssetInfo cimObj = pair.Value as FTN.AssetInfo;
                    ResourceDescription rd = CreateAssetInfoResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "AssetInfo");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAssetInfoResourceDescription(FTN.AssetInfo cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASSETINFO, importHelper.CheckOutIndexForDMSType(DMSType.ASSETINFO));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateAssetInfoProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportAssets()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.Asset");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.Asset cimObj = pair.Value as FTN.Asset;
                    ResourceDescription rd = CreateAssetResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "Asset");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAssetResourceDescription(FTN.Asset cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASSET, importHelper.CheckOutIndexForDMSType(DMSType.ASSET));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateAssetProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportAssetContainers()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.AssetContainer");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.AssetContainer cimObj = pair.Value as FTN.AssetContainer;
                    ResourceDescription rd = CreateAssetContainerResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "AssetContainer");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAssetContainerResourceDescription(FTN.AssetContainer cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASSETCONTAINER, importHelper.CheckOutIndexForDMSType(DMSType.ASSETCONTAINER));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateAssetContainerProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportSeals()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.Seal");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.Seal cimObj = pair.Value as FTN.Seal;
                    ResourceDescription rd = CreateSealResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "Seal");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateSealResourceDescription(FTN.Seal cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SEAL, importHelper.CheckOutIndexForDMSType(DMSType.SEAL));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateSealProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportComMedias()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.ComMedia");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.ComMedia cimObj = pair.Value as FTN.ComMedia;
                    ResourceDescription rd = CreateComMediaResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "ComMedia");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateComMediaResourceDescription(FTN.ComMedia cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.COMMEDIA, importHelper.CheckOutIndexForDMSType(DMSType.COMMEDIA));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateComMediaProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportOrganisationRoles()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.OrganisationRole");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.OrganisationRole cimObj = pair.Value as FTN.OrganisationRole;
                    ResourceDescription rd = CreateOrganisationRoleResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "OrganisationRole");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateOrganisationRoleResourceDescription(FTN.OrganisationRole cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ORGANISATIONROLE, importHelper.CheckOutIndexForDMSType(DMSType.ORGANISATIONROLE));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateOrganisationRoleProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportAssetOrgRoles()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.AssetOrganisationRole");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.AssetOrganisationRole cimObj = pair.Value as FTN.AssetOrganisationRole;
                    ResourceDescription rd = CreateAssetOrgRoleResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "AssetOrganisationRole");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAssetOrgRoleResourceDescription(FTN.AssetOrganisationRole cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASSETORGROLE, importHelper.CheckOutIndexForDMSType(DMSType.ASSETORGROLE));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateAssetOrganisationRoleProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void ImportAssetOwners()
        {
            SortedDictionary<string, object> cimObjects = concreteModel.GetAllObjectsOfType("FTN.AssetOwner");
            if (cimObjects != null)
            {
                foreach (var pair in cimObjects)
                {
                    FTN.AssetOwner cimObj = pair.Value as FTN.AssetOwner;
                    ResourceDescription rd = CreateAssetOwnerResourceDescription(cimObj);
                    AddInsertOperation(rd, cimObj, "AssetOwner");
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAssetOwnerResourceDescription(FTN.AssetOwner cimObj)
        {
            if (cimObj == null)
            {
                return null;
            }

            long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASSETOWNER, importHelper.CheckOutIndexForDMSType(DMSType.ASSETOWNER));
            ResourceDescription rd = new ResourceDescription(gid);
            importHelper.DefineIDMapping(cimObj.ID, gid);
            Projekat96Converter.PopulateAssetOwnerProperties(cimObj, rd, importHelper, report);
            return rd;
        }

        private void AddInsertOperation(ResourceDescription rd, FTN.IDClass cimObj, string label)
        {
            if (rd != null)
            {
                delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                report.Report.Append(label).Append(" ID = ").Append(cimObj != null ? cimObj.ID : string.Empty).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
            }
            else
            {
                report.Report.Append(label).Append(" ID = ").Append(cimObj != null ? cimObj.ID : string.Empty).AppendLine(" FAILED to be converted");
            }
        }

        #endregion
    }
}
