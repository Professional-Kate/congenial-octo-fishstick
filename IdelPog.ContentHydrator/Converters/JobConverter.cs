using ContentHydrator.DTO;
using IdelPog.Validation.Assertions;

namespace ContentHydrator.Converters
{
    /// <inheritdoc cref="IConverter{T}"/>
    public class JobConverter(IAssertNotNull assertNotNull) : IConverter<JobDTO>
    {
        public JobDTO Convert(string content)
        {
            assertNotNull.AssertObjectNotNull(content);

            JobDTO jobDTO = new(content);
            
            return jobDTO;
        }
    }
}