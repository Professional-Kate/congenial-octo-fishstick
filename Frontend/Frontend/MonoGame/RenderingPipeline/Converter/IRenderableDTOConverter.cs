using System.Collections.Generic;
using IdelPog.Common.DTO;
using IdelPog.Frontend.Rendering.Structures;

namespace IdelPog.Frontend.MonoGame.Converter
{
    public interface IRenderableDTOConverter
    {
        public IReadOnlyList<RenderEntity> ConvertCollection(IReadOnlyList<RenderableDTO> dtos);
    }
}