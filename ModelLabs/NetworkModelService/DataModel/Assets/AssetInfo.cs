using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class AssetInfo : IdentifiedObject
    {
        private long assetModel = 0;
        private List<long> assets = new List<long>();
        private List<long> assetModels = new List<long>();

        public AssetInfo(long globalId) : base(globalId)
        {
        }

        public long AssetModel
        {
            get { return assetModel; }
            set { assetModel = value; }
        }

        public List<long> Assets
        {
            get { return assets; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                AssetInfo x = (AssetInfo)obj;
                return x.assetModel == assetModel && CompareHelper.CompareLists(x.assets, assets);
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
                case ModelCode.ASSETINFO_ASSETMODEL:
                case ModelCode.ASSETINFO_ASSETS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASSETINFO_ASSETMODEL:
                    property.SetValue(assetModel);
                    break;
                case ModelCode.ASSETINFO_ASSETS:
                    property.SetValue(assets);
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
                case ModelCode.ASSETINFO_ASSETMODEL:
                    assetModel = property.AsReference();
                    break;
                case ModelCode.ASSETINFO_ASSETS:
                    assets = property.AsReferences() ?? new List<long>();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get
            {
                return assets.Count > 0 || assetModels.Count > 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ASSET_ASSETINFO:
                case ModelCode.COMMEDIA_ASSETINFO:
                    assets.Add(globalId);
                    break;
                case ModelCode.ASSETMODEL_ASSETINFO:
                case ModelCode.PRODASSETMODEL_ASSETINFO:
                    if (!assetModels.Contains(globalId))
                    {
                        assetModels.Add(globalId);
                    }
                    break;
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ASSET_ASSETINFO:
                case ModelCode.COMMEDIA_ASSETINFO:
                    assets.Remove(globalId);
                    break;
                case ModelCode.ASSETMODEL_ASSETINFO:
                case ModelCode.PRODASSETMODEL_ASSETINFO:
                    assetModels.Remove(globalId);
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (assetModel != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETINFO_ASSETMODEL] = new List<long>() { assetModel };
            }
            if (assets.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETINFO_ASSETS] = new List<long>(assets);
            }
            base.GetReferences(references, refType);
        }
    }
}
