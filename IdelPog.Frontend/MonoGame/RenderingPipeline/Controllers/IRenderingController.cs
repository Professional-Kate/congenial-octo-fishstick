using System.Collections.Generic;
using Frontend.Rendering.Structures.Enums;
using Frontend.UI.Structures;

namespace Frontend.MonoGame.Controllers
{
    public interface IRenderingController
    {
        public void RenderTextures(IReadOnlyList<RenderableDTO> dtos);
    }
}