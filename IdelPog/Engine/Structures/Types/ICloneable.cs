namespace IdelPog.Engine.Structures.Types
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Clone"/>
    public interface ICloneable<out T>
    {
        public T Clone();
    }
}