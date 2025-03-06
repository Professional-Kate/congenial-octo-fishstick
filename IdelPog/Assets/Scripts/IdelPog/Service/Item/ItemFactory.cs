using System;
using IdelPog.Structures;
using IdelPog.Structures.Models.Item;

namespace IdelPog.Service
{
    /// <inheritdoc cref="IItemFactory"/>
    public class ItemFactory : IItemFactory
    {
        public Item CreateItem(InventoryID id, Information information, int sellPrice, int startingAmount)
        {
            AssertNumberIsGreaterThanZero(sellPrice);
            AssertNumberIsGreaterThanZero(startingAmount);
            AssertInformationIsValid(information);
            
            return new Item(id, information, sellPrice, startingAmount);
        }
        
        /// <summary>
        /// Asserts that the passed number is greater than 0
        /// </summary>
        /// <param name="number">The number you want to check</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed number is equal to or less than zero</exception>
        private static void AssertNumberIsGreaterThanZero(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException("Error! Passed amount {number} is expected to be greater than zero.");
            }
        }

        /// <summary>
        /// Asserts that the passed Name and Description string on the passed <see cref="Information"/> are valid
        /// </summary>
        /// <param name="information"><see cref="Information"/></param>
        private static void AssertInformationIsValid(Information information)
        {
            AssertStringIsValid(information.Name);
            AssertStringIsValid(information.Description);

        }

        /// <summary>
        /// Asserts that the passed string is not null, or empty
        /// </summary>
        /// <param name="text">The string you want to check</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed string is not valid</exception>
        private static void AssertStringIsValid(string text)
        {
            if (text == null || text.Trim().Length == 0)
            {
                throw new ArgumentException("Error! Passed text is empty.");
            }
        }
    }
}