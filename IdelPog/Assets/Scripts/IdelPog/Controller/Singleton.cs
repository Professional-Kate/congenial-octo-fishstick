using System;

namespace IdelPog.Controller
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class
    {
        private static T _instance;

        /// <summary>
        /// Returns an instance of the passed type T
        /// </summary>
        /// <returns>An instance of type T</returns>
        /// <remarks>
        ///  Is expected to always return an instance of type T. If the instance does not exist then this will create one and return it
        /// </remarks>
        public static T GetInstance()
        {
            if (_instance == null)
            {
                _instance = (T)Activator.CreateInstance(typeof(T));
            }
            
            return _instance;
        }
    }
}