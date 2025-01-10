using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    /// <summary>
    /// This interface will give access to updating a <see cref="Currency"/> model
    /// </summary>
    /// <seealso cref="Update"/>
    public interface ICurrencyRepositoryUpdate
    {
        /// <summary>
        /// Updates a passed <see cref="Currency"/> model by using its <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="key">The <see cref="CurrencyType"/> of the <see cref="Currency"/> you want to add</param>
        /// <param name="value">The <see cref="Currency"/> that you want to update</param>
        /// <exception cref="NotFoundException">If the passed <see cref="CurrencyType"/> is not in the Repository</exception>
        /// <exception cref="NoTypeException">If the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <exception cref="ArgumentNullException">If the passed <see cref="Currency"/> is null</exception>
        /// <remarks>
        /// This operation will completely replace the <see cref="Currency"/> in the Repository with the passed <see cref="Currency"/>
        /// </remarks>
        public void Update(CurrencyType key, Currency value);
    }
}