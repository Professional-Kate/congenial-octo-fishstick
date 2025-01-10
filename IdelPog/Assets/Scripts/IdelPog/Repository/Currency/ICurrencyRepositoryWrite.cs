﻿using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    /// <summary>
    /// The purpose of this interface is to give access to Adding, and Removing a <see cref="Currency"/> from the Repository.
    /// </summary>
    /// <seealso cref="Add"/>
    /// <seealso cref="Remove"/>
    public interface ICurrencyRepositoryWrite
    {
        /// <summary>
        /// Adds a <see cref="Currency"/> model with a connected <see cref="CurrencyType"/> key into the Repository
        /// </summary>
        /// <param name="key">The <see cref="CurrencyType"/> of the <see cref="Currency"/> you want to add</param>
        /// <param name="value">The <see cref="Currency"/> model you want to add</param>
        /// <exception cref="ArgumentNullException">If the passed <see cref="Currency"/> model is null</exception>
        /// <exception cref="NoTypeException">If the <see cref="Currency"/> model <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <exception cref="ArgumentException">If the passed <see cref="Currency"/> model already exists in the Repository</exception>
        /// <remarks>
        /// The passed <see cref="Currency"/> will be linked to the <see cref="CurrencyType"/> key in the Repository
        /// </remarks>
        public void Add(CurrencyType key, Currency value);

        /// <summary>
        /// Removes a <see cref="Currency"/> model from the Repository by using its <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="currencyType">The <see cref="Currency"/> model you want to remove should have this <see cref="CurrencyType"/></param>
        /// <exception cref="NoTypeException">If the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <exception cref="NotFoundException">If the passed <see cref="CurrencyType"/> is not in the Repository</exception>
        public void Remove(CurrencyType currencyType);
    }
}