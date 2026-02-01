using FTN.Common;
using System.Collections.Generic;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class ProductAssetModel : AssetModel
    {
        private CorporateStandardKind corporateStandardKind;
        private string modelNumber;
        private string modelVersion;
        private AssetModelUsageKind usageKind;
        private float weightTotal;

        public ProductAssetModel(long globalId) : base(globalId)
        {
        }

        public CorporateStandardKind CorporateStandardKind
        {
            get { return corporateStandardKind; }
            set { corporateStandardKind = value; }
        }

        public string ModelNumber
        {
            get { return modelNumber; }
            set { modelNumber = value; }
        }

        public string ModelVersion
        {
            get { return modelVersion; }
            set { modelVersion = value; }
        }

        public AssetModelUsageKind UsageKind
        {
            get { return usageKind; }
            set { usageKind = value; }
        }

        public float WeightTotal
        {
            get { return weightTotal; }
            set { weightTotal = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ProductAssetModel x = (ProductAssetModel)obj;
                return x.corporateStandardKind == corporateStandardKind &&
                       x.modelNumber == modelNumber &&
                       x.modelVersion == modelVersion &&
                       x.usageKind == usageKind &&
                       x.weightTotal == weightTotal;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.PRODASSETMODEL_CORPSTANDKIND:
                case ModelCode.PRODASSETMODEL_MODELNUMBER:
                case ModelCode.PRODASSETMODEL_MODELVERSION:
                case ModelCode.PRODASSETMODEL_USAGEKIND:
                case ModelCode.PRODASSETMODEL_WEIGHTTOTAL:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.PRODASSETMODEL_CORPSTANDKIND:
                    property.SetValue((short)corporateStandardKind);
                    break;
                case ModelCode.PRODASSETMODEL_MODELNUMBER:
                    property.SetValue(modelNumber);
                    break;
                case ModelCode.PRODASSETMODEL_MODELVERSION:
                    property.SetValue(modelVersion);
                    break;
                case ModelCode.PRODASSETMODEL_USAGEKIND:
                    property.SetValue((short)usageKind);
                    break;
                case ModelCode.PRODASSETMODEL_WEIGHTTOTAL:
                    property.SetValue(weightTotal);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override void SetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.PRODASSETMODEL_CORPSTANDKIND:
                    corporateStandardKind = (CorporateStandardKind)property.AsEnum();
                    break;
                case ModelCode.PRODASSETMODEL_MODELNUMBER:
                    modelNumber = property.AsString();
                    break;
                case ModelCode.PRODASSETMODEL_MODELVERSION:
                    modelVersion = property.AsString();
                    break;
                case ModelCode.PRODASSETMODEL_USAGEKIND:
                    usageKind = (AssetModelUsageKind)property.AsEnum();
                    break;
                case ModelCode.PRODASSETMODEL_WEIGHTTOTAL:
                    weightTotal = property.AsFloat();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            base.GetReferences(references, refType);
        }
    }
}
