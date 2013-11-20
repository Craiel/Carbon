namespace GrandSeal.DataDemon.Contracts
{
    using GrandSeal.DataDemon.Logic;

    public interface IDataDemon
    {
        void Run(DemonArguments arguments);
    }
}
