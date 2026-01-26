using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using System.Collections.Generic;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class Seal : IdentifiedObject
    {
        private System.DateTime appliedDateTime;
        private SealConditionKind condition;
        private SealKind kind;
        private string sealNumber;
        private long assetContainer = 0;

        public Seal(long globalId) : base(globalId)
        {
        }

        public System.DateTime AppliedDateTime
        {
            get { return appliedDateTime; }
            set { appliedDateTime = value; }
        }

        public SealConditionKind Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public SealKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        public string SealNumber
        {
            get { return sealNumber; }
            set { sealNumber = value; }
        }

        public long AssetContainer
        {
            get { return assetContainer; }
            set { assetContainer = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                Seal x = (Seal)obj;
                return x.appliedDateTime == appliedDateTime &&
                       x.condition == condition &&
                       x.kind == kind &&
                       x.sealNumber == sealNumber &&
                       x.assetContainer == assetContainer;
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
                case ModelCode.SEAL_APPLIEDDATETIME:
                case ModelCode.SEAL_CONDITION:
                case ModelCode.SEAL_KIND:
                case ModelCode.SEAL_SEALNUMBER:
                case ModelCode.SEAL_ASSETCONTAINER:
                    return true;

                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.SEAL_APPLIEDDATETIME:
                    property.SetValue(appliedDateTime);
                    break;
                case ModelCode.SEAL_CONDITION:
                    property.SetValue((short)condition);
                    break;
                case ModelCode.SEAL_KIND:
                    property.SetValue((short)kind);
                    break;
                case ModelCode.SEAL_SEALNUMBER:
                    property.SetValue(sealNumber);
                    break;
                case ModelCode.SEAL_ASSETCONTAINER:
                    property.SetValue(assetContainer);
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
                case ModelCode.SEAL_APPLIEDDATETIME:
                    appliedDateTime = property.AsDateTime();
                    break;
                case ModelCode.SEAL_CONDITION:
                    condition = (SealConditionKind)property.AsEnum();
                    break;
                case ModelCode.SEAL_KIND:
                    kind = (SealKind)property.AsEnum();
                    break;
                case ModelCode.SEAL_SEALNUMBER:
                    sealNumber = property.AsString();
                    break;
                case ModelCode.SEAL_ASSETCONTAINER:
                    assetContainer = property.AsReference();
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
                return assetContainer != 0 || base.IsReferenced;
            }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (assetContainer != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.SEAL_ASSETCONTAINER] = new List<long>() { assetContainer };
            }
            base.GetReferences(references, refType);
        }
    }
}
