using System;
using System.Collections.Generic;
using Frontend.MonoGame.Controllers;
using Frontend.UI.Structures;
using IdelPog.Staging.Messaging;

namespace Frontend.MonoGame.Listeners
{
    public class RenderableDTOListener(IRenderingController renderingController) : IBufferListener<RenderableDTO>
    {
        public Type ListenerType { get; } = typeof(RenderableDTO);
        
        public void Handle(IReadOnlyList<RenderableDTO> buffer)
        {
            renderingController.RenderTextures(buffer);
        }
    }
}