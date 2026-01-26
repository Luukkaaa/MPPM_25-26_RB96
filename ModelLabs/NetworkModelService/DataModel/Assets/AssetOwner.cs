using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class AssetOwner : AssetOrganisationRole
    {
        public AssetOwner(long globalId) : base(globalId)
        {
        }

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.ASSETOWNER_ASSETS:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ASSETOWNER_ASSETS:
                    Assets.Add(globalId);
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
                case ModelCode.ASSETOWNER_ASSETS:
                    Assets.Remove(globalId);
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASSETOWNER_ASSETS:
                    property.SetValue(Assets);
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
                case ModelCode.ASSETOWNER_ASSETS:
                    Assets.Clear();
                    List<long> refs = property.AsReferences();
                    if (refs != null)
                    {
                        Assets.AddRange(refs);
                    }
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (Assets.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSETOWNER_ASSETS] = new List<long>(Assets);
            }

            base.GetReferences(references, refType);
        }
    }
}
