using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Assets
{
    public class OrganisationRole : IdentifiedObject
    {
        public OrganisationRole(long globalId) : base(globalId)
        {
        }

        public override bool HasProperty(ModelCode property)
        {
            return base.HasProperty(property);
        }

        public override void GetProperty(Property property)
        {
            base.GetProperty(property);
        }

        public override void SetProperty(Property property)
        {
            base.SetProperty(property);
        }
    }
}
