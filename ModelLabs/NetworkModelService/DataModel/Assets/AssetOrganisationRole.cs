using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class AssetOrganisationRole : OrganisationRole
    {
        private List<long> assets = new List<long>();

        public AssetOrganisationRole(long globalId) : base(globalId)
        {
        }

        public List<long> Assets
        {
            get { return assets; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                AssetOrganisationRole x = (AssetOrganisationRole)obj;
                return CompareHelper.CompareLists(x.assets, assets);
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
                case ModelCode.ASSETORGROLE_ASSETS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASSETORGROLE_ASSETS:
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
                case ModelCode.ASSETORGROLE_ASSETS:
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
                return assets.Count > 0 || base.IsReferenced;
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ASSET_ORGROLES:
                    assets.Add(globalId);
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
                case ModelCode.ASSET_ORGROLES:
                    assets.Remove(globalId);
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (assets.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETORGROLE_ASSETS] = new List<long>(assets);
            }
            base.GetReferences(references, refType);
        }
    }
}
