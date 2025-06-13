using System;
using System.Collections.Generic;
using Frontend.Rendering.Structures.Enums;
using IdelPog.Staging.Messaging;

namespace Frontend.MonoGame.Listeners
{
    public class TextureIDListener : IBufferListener<TextureID>
    {
        public Type ListenerType { get; } = typeof(TextureID);
        
        public void Handle(IReadOnlyList<TextureID> buffer)
        {
            // TODO: need some kinda service class that can accept these TextureIDs, resolve them into textures, then pass them into the renderer. 
            throw new NotImplementedException();
        }

    }
}