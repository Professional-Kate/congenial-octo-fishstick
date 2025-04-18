using ContentHydrator.Assertions;
using ContentHydrator.DTO;
using IdelPog.Validation.Assertions;

namespace ContentHydrator.Converters
{
    /// <inheritdoc cref="IJsonConverter{T}"/>
    public class JobConverter(IAssertNotNull assertNotNull, IAssertFound assertFound, IAssertValidCast assertCastable) : IJsonConverter<JobDTO>
    {
        public JobDTO Convert(Dictionary<string, object> content)
        {
            assertNotNull.AssertObjectNotNull(content);

            bool contains = content.TryGetValue("JobID", out object? id);
            
            assertFound.AssertItemIsFound("JobID", () => contains);
            assertNotNull.AssertObjectNotNull(id);
            assertCastable.AssertCastable<string>(id!);
            
            JobDTO jobDTO = new(id as string, new InformationDTO("", ""));
            return jobDTO;
        }
    }
}