﻿using Carbon.Engine.Contracts.Logic;

using SlimDX;

namespace Carbon.Engine.Contracts.UserInterface
{
    public interface IUserInterfaceControl : IEngineComponent
    {
        bool IsActive { get; set; }
        bool IsVisible { get; set; }

        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Vector2 AbsolutePosition { get; }
        Vector2 AbsoluteSize { get; }

        void Invalidate();
    }

}
