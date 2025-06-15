using System.Collections.Generic;
using Frontend.Content.Service;
using Frontend.Rendering.Structures;
using Frontend.UI.Structures;

namespace Frontend.MonoGame.Converter
{
    public class RenderableDTOConverter : IRenderableDTOConverter
    {
        private readonly IUITextureResolver _textureResolver;

        public RenderableDTOConverter(IUITextureResolver textureResolver)
        {
            _textureResolver = textureResolver;
        }

        public IReadOnlyList<RenderEntity> ConvertCollection(IReadOnlyList<RenderableDTO> dtos)
        {
            List<RenderEntity> renderEntities = new(dtos.Count);
            
            foreach (RenderableDTO dto in dtos)
            {
                renderEntities.Add(new RenderEntity
                {
                    Texture = _textureResolver.GetTexture(dto.TextureID),
                    Position = dto.Transform.Position,
                    Size = dto.Transform.Size,
                    Z = dto.Transform.Z
                });
            }
            
            return renderEntities;
        }
    }
}