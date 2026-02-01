using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class Asset : IdentifiedObject
    {
        private bool critical;
        private string initialCondition;
        private float initialLossOfLife;
        private string lotNumber;
        private float purchasePrice;
        private string serialNumber;
        private string type;
        private string utcNumber;
        private long assetContainer = 0;
        private long assetInfo = 0;
        private List<long> organisationRoles = new List<long>();

        public Asset(long globalId)
            : base(globalId)
        {
        }

        public bool Critical
        {
            get { return critical; }
            set { critical = value; }
        }

        public string InitialCondition
        {
            get { return initialCondition; }
            set { initialCondition = value; }
        }

        public float InitialLossOfLife
        {
            get { return initialLossOfLife; }
            set { initialLossOfLife = value; }
        }

        public string LotNumber
        {
            get { return lotNumber; }
            set { lotNumber = value; }
        }

        public float PurchasePrice
        {
            get { return purchasePrice; }
            set { purchasePrice = value; }
        }

        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }

        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string UtcNumber
        {
            get { return utcNumber; }
            set { utcNumber = value; }
        }

        public long AssetContainer
        {
            get { return assetContainer; }
            set { assetContainer = value; }
        }

        public long AssetInfo
        {
            get { return assetInfo; }
            set { assetInfo = value; }
        }

        public List<long> OrganisationRoles
        {
            get { return organisationRoles; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Asset x = (Asset)obj;
                return x.critical == critical &&
                       x.initialCondition == initialCondition &&
                       x.initialLossOfLife == initialLossOfLife &&
                       x.lotNumber == lotNumber &&
                       x.purchasePrice == purchasePrice &&
                       x.serialNumber == serialNumber &&
                       x.type == type &&
                       x.utcNumber == utcNumber &&
                       x.assetContainer == assetContainer &&
                       x.assetInfo == assetInfo &&
                       CompareHelper.CompareLists(x.organisationRoles, organisationRoles);
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
                case ModelCode.ASSET_CRITICAL:
                case ModelCode.ASSET_INITIALCONDITION:
                case ModelCode.ASSET_INITIALLOSSOFLIFE:
                case ModelCode.ASSET_LOTNUMBER:
                case ModelCode.ASSET_PURCHASEPRICE:
                case ModelCode.ASSET_SERIALNUMBER:
                case ModelCode.ASSET_TYPE:
                case ModelCode.ASSET_UTCNUMBER:
                case ModelCode.ASSET_ASSETCONTAINER:
                case ModelCode.ASSET_ASSETINFO:
                case ModelCode.ASSET_ORGROLES:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.ASSET_CRITICAL:
                    property.SetValue(critical);
                    break;
                case ModelCode.ASSET_INITIALCONDITION:
                    property.SetValue(initialCondition);
                    break;
                case ModelCode.ASSET_INITIALLOSSOFLIFE:
                    property.SetValue(initialLossOfLife);
                    break;
                case ModelCode.ASSET_LOTNUMBER:
                    property.SetValue(lotNumber);
                    break;
                case ModelCode.ASSET_PURCHASEPRICE:
                    property.SetValue(purchasePrice);
                    break;
                case ModelCode.ASSET_SERIALNUMBER:
                    property.SetValue(serialNumber);
                    break;
                case ModelCode.ASSET_TYPE:
                    property.SetValue(type);
                    break;
                case ModelCode.ASSET_UTCNUMBER:
                    property.SetValue(utcNumber);
                    break;
                case ModelCode.ASSET_ASSETCONTAINER:
                    property.SetValue(assetContainer);
                    break;
                case ModelCode.ASSET_ASSETINFO:
                    property.SetValue(assetInfo);
                    break;
                case ModelCode.ASSET_ORGROLES:
                    property.SetValue(organisationRoles);
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
                case ModelCode.ASSET_CRITICAL:
                    critical = property.AsBool();
                    break;
                case ModelCode.ASSET_INITIALCONDITION:
                    initialCondition = property.AsString();
                    break;
                case ModelCode.ASSET_INITIALLOSSOFLIFE:
                    initialLossOfLife = property.AsFloat();
                    break;
                case ModelCode.ASSET_LOTNUMBER:
                    lotNumber = property.AsString();
                    break;
                case ModelCode.ASSET_PURCHASEPRICE:
                    purchasePrice = property.AsFloat();
                    break;
                case ModelCode.ASSET_SERIALNUMBER:
                    serialNumber = property.AsString();
                    break;
                case ModelCode.ASSET_TYPE:
                    type = property.AsString();
                    break;
                case ModelCode.ASSET_UTCNUMBER:
                    utcNumber = property.AsString();
                    break;
                case ModelCode.ASSET_ASSETCONTAINER:
                    assetContainer = property.AsReference();
                    break;
                case ModelCode.ASSET_ASSETINFO:
                    assetInfo = property.AsReference();
                    break;
                case ModelCode.ASSET_ORGROLES:
                    organisationRoles = property.AsReferences() ?? new List<long>();
                    break;
                default:
                    base.SetProperty(property);
                    break;
            }
        }

        // Asset nema sopstvene TARGET (incoming) reference
        // organisationRoles, assetContainer, assetInfo su sve SOURCE reference (Asset referencira druge entitete)
        // Zato nema potrebe za override AddReference/RemoveReference/IsReferenced
        // GDA mehanizam koristi ASSET_ORGROLES da pozove AddReference na AssetOrganisationRole objektima

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (assetContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSET_ASSETCONTAINER] = new List<long>() { assetContainer };
            }
            if (assetInfo != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSET_ASSETINFO] = new List<long>() { assetInfo };
            }

            // ASSET_ORGROLES je SOURCE referenca (Asset -> AssetOrganisationRole)
            // Setuje se eksplicitno, nije TARGET (inverse)
            if (organisationRoles.Count > 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.ASSET_ORGROLES] = new List<long>(organisationRoles);
            }

            base.GetReferences(references, refType);
        }
    }
}
