namespace IdelPog.Messaging.Factory
{
    public interface IErrorFactory<out TErrorDTO, in TContext>
    {
        public TErrorDTO Create(TContext context, Exception exception);
    }
}