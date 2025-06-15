using System.Collections.Generic;
using IdelPog.Frontend.UI.Structures;

namespace IdelPog.Frontend.MonoGame.Controllers
{
    public interface IRenderingController
    {
        public void RenderTextures(IReadOnlyList<RenderableDTO> dtos);
    }
}