using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class ComMedia : Asset
    {
        private string lifecycle;

        public ComMedia(long globalId) : base(globalId)
        {
        }

        public string Lifecycle
        {
            get { return lifecycle; }
            set { lifecycle = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ComMedia x = (ComMedia)obj;
                return x.lifecycle == lifecycle;
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
                case ModelCode.COMMEDIA_ASSETCONTAINER:
                case ModelCode.COMMEDIA_ASSETINFO:
                case ModelCode.COMMEDIA_ORGROLES:
                case ModelCode.COMMEDIA_CRITICAL:
                case ModelCode.COMMEDIA_INITIALCONDITION:
                case ModelCode.COMMEDIA_INITIALLOSSOFLIFE:
                case ModelCode.COMMEDIA_PURCHASEPRICE:
                case ModelCode.COMMEDIA_LIFECYCLE:
                case ModelCode.COMMEDIA_STATUS:
                case ModelCode.COMMEDIA_TYPE:
                case ModelCode.COMMEDIA_UTCNUMBER:
                case ModelCode.COMMEDIA_LOTNUMBER:
                case ModelCode.COMMEDIA_SERIALNUMBER:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.COMMEDIA_ASSETCONTAINER:
                    property.SetValue(AssetContainer);
                    break;
                case ModelCode.COMMEDIA_ASSETINFO:
                    property.SetValue(AssetInfo);
                    break;
                case ModelCode.COMMEDIA_ORGROLES:
                    property.SetValue(OrganisationRoles);
                    break;
                case ModelCode.COMMEDIA_CRITICAL:
                    property.SetValue(Critical);
                    break;
                case ModelCode.COMMEDIA_INITIALCONDITION:
                    property.SetValue(InitialCondition);
                    break;
                case ModelCode.COMMEDIA_INITIALLOSSOFLIFE:
                    property.SetValue(InitialLossOfLife);
                    break;
                case ModelCode.COMMEDIA_PURCHASEPRICE:
                    property.SetValue(PurchasePrice);
                    break;
                case ModelCode.COMMEDIA_LIFECYCLE:
                    property.SetValue(lifecycle);
                    break;
                case ModelCode.COMMEDIA_STATUS:
                    property.SetValue(Status);
                    break;
                case ModelCode.COMMEDIA_TYPE:
                    property.SetValue(Type);
                    break;
                case ModelCode.COMMEDIA_UTCNUMBER:
                    property.SetValue(UtcNumber);
                    break;
                case ModelCode.COMMEDIA_LOTNUMBER:
                    property.SetValue(LotNumber);
                    break;
                case ModelCode.COMMEDIA_SERIALNUMBER:
                    property.SetValue(SerialNumber);
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
                case ModelCode.COMMEDIA_ASSETCONTAINER:
                    AssetContainer = property.AsReference();
                    break;
                case ModelCode.COMMEDIA_ASSETINFO:
                    AssetInfo = property.AsReference();
                    break;
                case ModelCode.COMMEDIA_ORGROLES:
                    OrganisationRoles.Clear();
                    List<long> orgRefs = property.AsReferences();
                    if (orgRefs != null)
                    {
                        OrganisationRoles.AddRange(orgRefs);
                    }
                    break;
                case ModelCode.COMMEDIA_CRITICAL:
                    Critical = property.AsBool();
                    break;
                case ModelCode.COMMEDIA_INITIALCONDITION:
                    InitialCondition = property.AsString();
                    break;
                case ModelCode.COMMEDIA_INITIALLOSSOFLIFE:
                    InitialLossOfLife = property.AsFloat();
                    break;
                case ModelCode.COMMEDIA_PURCHASEPRICE:
                    PurchasePrice = property.AsFloat();
                    break;
                case ModelCode.COMMEDIA_LIFECYCLE:
                    lifecycle = property.AsString();
                    break;
                case ModelCode.COMMEDIA_STATUS:
                    Status = property.AsString();
                    break;
                case ModelCode.COMMEDIA_TYPE:
                    Type = property.AsString();
                    break;
                case ModelCode.COMMEDIA_UTCNUMBER:
                    UtcNumber = property.AsString();
                    break;
                case ModelCode.COMMEDIA_LOTNUMBER:
                    LotNumber = property.AsString();
                    break;
                case ModelCode.COMMEDIA_SERIALNUMBER:
                    SerialNumber = property.AsString();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (AssetContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.COMMEDIA_ASSETCONTAINER] = new List<long>() { AssetContainer };
            }
            if (AssetInfo != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.COMMEDIA_ASSETINFO] = new List<long>() { AssetInfo };
            }
            if (OrganisationRoles.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.COMMEDIA_ORGROLES] = new List<long>(OrganisationRoles);
            }

            base.GetReferences(references, refType);
        }
    }
}
