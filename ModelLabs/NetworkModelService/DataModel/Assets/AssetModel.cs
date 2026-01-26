using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class AssetModel : IdentifiedObject
    {
        private long assetInfo = 0;
        private List<long> assetInfos = new List<long>();

        public AssetModel(long globalId) : base(globalId)
        {
        }

        public long AssetInfo
        {
            get { return assetInfo; }
            set { assetInfo = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                AssetModel x = (AssetModel)obj;
                return x.assetInfo == assetInfo && CompareHelper.CompareLists(x.assetInfos, assetInfos);
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
                case ModelCode.ASSETMODEL_ASSETINFO:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASSETMODEL_ASSETINFO:
                    property.SetValue(assetInfo);
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
                case ModelCode.ASSETMODEL_ASSETINFO:
                    assetInfo = property.AsReference();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (assetInfo != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETMODEL_ASSETINFO] = new List<long>() { assetInfo };
            }
            if (assetInfos.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETINFO_ASSETMODEL] = new List<long>(assetInfos);
            }
            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ASSETINFO_ASSETMODEL:
                    if (!assetInfos.Contains(globalId))
                    {
                        assetInfos.Add(globalId);
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
                case ModelCode.ASSETINFO_ASSETMODEL:
                    if (assetInfos.Contains(globalId))
                    {
                        assetInfos.Remove(globalId);
                    }
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get
            {
                return assetInfos.Count > 0 || base.IsReferenced;
            }
        }
    }
}
