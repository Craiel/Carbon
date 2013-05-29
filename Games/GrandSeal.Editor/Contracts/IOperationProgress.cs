namespace GrandSeal.Editor.Contracts
{
    public interface IOperationProgress
    {
        int Minimum { get; set; }
        int Maximum { get; set; }
        int Value { get; set; }

        bool InProgress { get; set; }
    }
}
