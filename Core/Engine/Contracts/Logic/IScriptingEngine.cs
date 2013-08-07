namespace Core.Engine.Contracts.Logic
{
    using Core.Engine.Logic.Scripting;

    using LuaInterface;

    public interface IScriptingEngine
    {
        void Register(IScriptingProvider provider);
        void Unregister(IScriptingProvider provider);

        void Execute(CarbonScript script);

        Lua GetContext();
    }
}
