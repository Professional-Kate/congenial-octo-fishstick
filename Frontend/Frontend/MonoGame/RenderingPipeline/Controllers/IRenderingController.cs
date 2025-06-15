using System.Collections.Generic;
using IdelPog.Common.DTO;

namespace IdelPog.Frontend.MonoGame.Controllers
{
    public interface IRenderingController
    {
        public void RenderTextures(IReadOnlyList<RenderableDTO> dtos);
    }
}