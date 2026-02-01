using System.Collections.Generic;
using FTN.Common;
using CommonSealKind = FTN.Common.SealKind;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    public static class Projekat96Converter
    {
        #region IdentifiedObject helpers

        public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimObj, ResourceDescription rd)
        {
            if ((cimObj != null) && (rd != null))
            {
                if (cimObj.MRIDHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimObj.MRID));
                }
                if (cimObj.NameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimObj.Name));
                }
                if (cimObj.AliasNameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_ALIASNAME, cimObj.AliasName));
                }
            }
        }

        #endregion

        #region Asset hierarchy

        public static void PopulateOrganisationRoleProperties(FTN.OrganisationRole cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimObj, rd);
            }
        }

        public static void PopulateAssetProperties(FTN.Asset cimAsset, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimAsset != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimAsset, rd);

                if (cimAsset.CriticalHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_CRITICAL, cimAsset.Critical));
                }
                if (cimAsset.InitialConditionHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_INITIALCONDITION, cimAsset.InitialCondition));
                }
                if (cimAsset.InitialLossOfLifeHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_INITIALLOSSOFLIFE, cimAsset.InitialLossOfLife));
                }
                if (cimAsset.LotNumberHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_LOTNUMBER, cimAsset.LotNumber));
                }
                if (cimAsset.PurchasePriceHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_PURCHASEPRICE, cimAsset.PurchasePrice));
                }
                if (cimAsset.SerialNumberHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_SERIALNUMBER, cimAsset.SerialNumber));
                }
                if (cimAsset.TypeHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_TYPE, cimAsset.Type));
                }
                if (cimAsset.UtcNumberHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASSET_UTCNUMBER, cimAsset.UtcNumber));
                }

                if (cimAsset.AssetContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimAsset.AssetContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Failed to set reference AssetContainer for Asset rdfID = ").Append(cimAsset.ID).AppendLine();
                    }
                    rd.AddProperty(new Property(ModelCode.ASSET_ASSETCONTAINER, gid));
                }

                if (cimAsset.AssetInfoHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimAsset.AssetInfo.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Failed to set reference AssetInfo for Asset rdfID = ").Append(cimAsset.ID).AppendLine();
                    }
                    rd.AddProperty(new Property(ModelCode.ASSET_ASSETINFO, gid));
                }

                if (cimAsset.OrganisationRolesHasValue)
                {
                    List<long> refs = new List<long>();
                    foreach (var role in cimAsset.OrganisationRoles)
                    {
                        long gid = importHelper.GetMappedGID(role.ID);
                        if (gid < 0)
                        {
                            report.Report.Append("WARNING: Failed to set reference OrganisationRoles for Asset rdfID = ").Append(cimAsset.ID).AppendLine();
                        }
                        else
                        {
                            refs.Add(gid);
                        }
                    }
                    rd.AddProperty(new Property(ModelCode.ASSET_ORGROLES, refs));
                }
            }
        }

        public static void PopulateAssetContainerProperties(FTN.AssetContainer cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateAssetProperties(cimObj, rd, importHelper, report);
            }
        }

        public static void PopulateAssetInfoProperties(FTN.AssetInfo cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimObj, rd);

                if (cimObj.AssetModelHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimObj.AssetModel.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Failed to set AssetModel reference for AssetInfo rdfID = ").Append(cimObj.ID).AppendLine();
                    }
                    rd.AddProperty(new Property(ModelCode.ASSETINFO_ASSETMODEL, gid));
                }
            }
        }

        public static void PopulateAssetModelProperties(FTN.AssetModel cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimObj, rd);

                if (cimObj.AssetInfoHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimObj.AssetInfo.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Failed to set AssetInfo reference for AssetModel rdfID = ").Append(cimObj.ID).AppendLine();
                    }
                    else
                    {
                        rd.AddProperty(new Property(ModelCode.ASSETMODEL_ASSETINFO, gid));
                    }
                }
            }
        }

        public static void PopulateProductAssetModelProperties(FTN.ProductAssetModel cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateAssetModelProperties(cimObj, rd, importHelper, report);

                if (cimObj.CorporateStandardKindHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PRODASSETMODEL_CORPSTANDKIND, (short)GetCorporateStandardKind(cimObj.CorporateStandardKind)));
                }
                if (cimObj.ModelNumberHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PRODASSETMODEL_MODELNUMBER, cimObj.ModelNumber));
                }
                if (cimObj.ModelVersionHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PRODASSETMODEL_MODELVERSION, cimObj.ModelVersion));
                }
                if (cimObj.UsageKindHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PRODASSETMODEL_USAGEKIND, (short)GetAssetModelUsageKind(cimObj.UsageKind)));
                }
                if (cimObj.WeightTotalHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PRODASSETMODEL_WEIGHTTOTAL, cimObj.WeightTotal));
                }
            }
        }

        public static void PopulateSealProperties(FTN.Seal cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimObj, rd);

                if (cimObj.AppliedDateTimeHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SEAL_APPLIEDDATETIME, cimObj.AppliedDateTime));
                }
                if (cimObj.ConditionHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SEAL_CONDITION, (short)GetSealConditionKind(cimObj.Condition)));
                }
                if (cimObj.KindHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SEAL_KIND, (short)GetSealKind(cimObj.Kind)));
                }
                if (cimObj.SealNumberHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SEAL_SEALNUMBER, cimObj.SealNumber));
                }
                if (cimObj.AssetContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimObj.AssetContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Failed to set AssetContainer reference for Seal rdfID = ").Append(cimObj.ID).AppendLine();
                    }
                    rd.AddProperty(new Property(ModelCode.SEAL_ASSETCONTAINER, gid));
                }
            }
        }

        public static void PopulateComMediaProperties(FTN.ComMedia cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {

                PopulateAssetProperties(cimObj, rd, importHelper, report);
            }
        }

        public static void PopulateAssetOrganisationRoleProperties(FTN.AssetOrganisationRole cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateOrganisationRoleProperties(cimObj, rd, importHelper, report);
            }
        }

        public static void PopulateAssetOwnerProperties(FTN.AssetOwner cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimObj != null) && (rd != null))
            {
                PopulateAssetOrganisationRoleProperties(cimObj, rd, importHelper, report);
            }
        }

        #endregion

        #region Enum converters

        public static AssetModelUsageKind GetAssetModelUsageKind(FTN.AssetModelUsageKind kind)
        {
            switch (kind)
            {
                case FTN.AssetModelUsageKind.customerSubstation: return AssetModelUsageKind.customerSubstation;
                case FTN.AssetModelUsageKind.distributionOverhead: return AssetModelUsageKind.distributionOverhead;
                case FTN.AssetModelUsageKind.distributionUnderground: return AssetModelUsageKind.distributionUnderground;
                case FTN.AssetModelUsageKind.streetlight: return AssetModelUsageKind.streetlight;
                case FTN.AssetModelUsageKind.substation: return AssetModelUsageKind.substation;
                case FTN.AssetModelUsageKind.transmission: return AssetModelUsageKind.transmission;
                case FTN.AssetModelUsageKind.unknown: return AssetModelUsageKind.unknown;
                default: return AssetModelUsageKind.other;
            }
        }

        public static CorporateStandardKind GetCorporateStandardKind(FTN.CorporateStandardKind kind)
        {
            switch (kind)
            {
                case FTN.CorporateStandardKind.experimental: return CorporateStandardKind.experimental;
                case FTN.CorporateStandardKind.other: return CorporateStandardKind.other;
                case FTN.CorporateStandardKind.standard: return CorporateStandardKind.standard;
                case FTN.CorporateStandardKind.underEvaluation: return CorporateStandardKind.underEvaluation;
                default: return CorporateStandardKind.other;
            }
        }

        public static SealConditionKind GetSealConditionKind(FTN.SealConditionKind kind)
        {
            switch (kind)
            {
                case FTN.SealConditionKind.broken: return SealConditionKind.broken;
                case FTN.SealConditionKind.locked: return SealConditionKind.locked;
                case FTN.SealConditionKind.missing: return SealConditionKind.missing;
                case FTN.SealConditionKind.open: return SealConditionKind.open;
                case FTN.SealConditionKind.other: return SealConditionKind.other;
                default: return SealConditionKind.other;
            }
        }

        public static CommonSealKind GetSealKind(FTN.SealKind kind)
        {
			switch (kind)
			{
				case FTN.SealKind.lead: return CommonSealKind.lead;
				case (FTN.SealKind)1: return CommonSealKind.@lock; // lock keyword in source enum
				case FTN.SealKind.other: return CommonSealKind.other;
				case FTN.SealKind.steel: return CommonSealKind.steel;
				default: return CommonSealKind.other;
			}
        }

        #endregion
    }
}
