using Carbon.Engine.Logic.Scripting;

namespace Carbon.Engine.Contracts.Logic
{
    public interface IScriptingEngine
    {
        void Register(IScriptingProvider provider);
        void Unregister(IScriptingProvider provider);

        void Execute(CarbonScript script);
    }
}
