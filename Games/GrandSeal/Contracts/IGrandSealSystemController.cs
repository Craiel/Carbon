using System;

namespace GrandSeal.Contracts
{
    using Core.Engine.Contracts.Logic;

    using Logic;

    public interface IGrandSealSystemController : IBoundController
    {
        event Action<GrandSealSystemAction> ActionTriggered;
    }
}
