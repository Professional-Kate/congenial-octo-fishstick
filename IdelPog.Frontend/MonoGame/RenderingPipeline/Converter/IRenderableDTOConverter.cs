using System.Collections.Generic;
using Frontend.Rendering.Structures;
using Frontend.UI.Structures;

namespace Frontend.MonoGame.Converter
{
    public interface IRenderableDTOConverter
    {
        public IReadOnlyList<RenderEntity> ConvertCollection(IReadOnlyList<RenderableDTO> dtos);
    }
}