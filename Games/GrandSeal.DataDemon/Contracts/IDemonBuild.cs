namespace GrandSeal.DataDemon.Contracts
{
    public interface IDemonBuild : IDemonOperation
    {
        void SetConfig(Logic.DemonBuildConfig build);
    }
}
