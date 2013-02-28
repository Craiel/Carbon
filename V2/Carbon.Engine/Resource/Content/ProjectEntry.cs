namespace Carbon.Engine.Resource.Content
{
    // This is a virtual content entry so it is not mapped up to a table
    public class ProjectEntry : ContentEntry
    {
        public override bool IsNew
        {
            get
            {
                return false;
            }
        }

        public override MetaDataTargetEnum MetaDataTarget
        {
            get
            {
                return MetaDataTargetEnum.Project;
            }
        }
    }
}
