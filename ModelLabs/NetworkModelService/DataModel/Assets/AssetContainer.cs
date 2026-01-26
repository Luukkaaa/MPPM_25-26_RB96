using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class AssetContainer : Asset
    {
        private List<long> assets = new List<long>();
        private List<long> seals = new List<long>();

        public AssetContainer(long globalId) : base(globalId)
        {
        }

        public List<long> Assets
        {
            get { return assets; }
        }

        public List<long> Seals
        {
            get { return seals; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                AssetContainer x = (AssetContainer)obj;
                return CompareHelper.CompareLists(x.assets, assets) &&
                       CompareHelper.CompareLists(x.seals, seals);
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
                case ModelCode.ASSETCONTAINER_ASSETS:
                case ModelCode.ASSETCONTAINER_SEALS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASSETCONTAINER_ASSETS:
                    property.SetValue(assets);
                    break;
                case ModelCode.ASSETCONTAINER_SEALS:
                    property.SetValue(seals);
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
                case ModelCode.ASSETCONTAINER_ASSETS:
                    assets = property.AsReferences() ?? new List<long>();
                    break;
                case ModelCode.ASSETCONTAINER_SEALS:
                    seals = property.AsReferences() ?? new List<long>();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ASSET_ASSETCONTAINER:
                case ModelCode.COMMEDIA_ASSETCONTAINER:
                    assets.Add(globalId);
                    break;
                case ModelCode.SEAL_ASSETCONTAINER:
                    seals.Add(globalId);
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
                case ModelCode.ASSET_ASSETCONTAINER:
                case ModelCode.COMMEDIA_ASSETCONTAINER:
                    assets.Remove(globalId);
                    break;
                case ModelCode.SEAL_ASSETCONTAINER:
                    seals.Remove(globalId);
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
                return assets.Count > 0 || seals.Count > 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (assets.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETCONTAINER_ASSETS] = new List<long>(assets);
            }
            if (seals.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETCONTAINER_SEALS] = new List<long>(seals);
            }
            base.GetReferences(references, refType);
        }
    }
}
