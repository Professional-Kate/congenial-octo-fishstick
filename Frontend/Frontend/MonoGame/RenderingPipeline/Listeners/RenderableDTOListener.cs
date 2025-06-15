using System;
using System.Collections.Generic;
using IdelPog.Common.DTO;
using IdelPog.Frontend.MonoGame.Controllers;
using IdelPog.Messaging.Messaging;

namespace IdelPog.Frontend.MonoGame.Listeners
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