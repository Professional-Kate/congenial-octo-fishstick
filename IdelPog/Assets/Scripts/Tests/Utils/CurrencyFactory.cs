﻿using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace Tests.Utils
{
    internal static class CurrencyFactory
    {
        internal static Currency CreateWood()
        {
            return new Currency(CurrencyType.WOOD);
        }

        internal static Currency CreateFood()
        {
            return new Currency(CurrencyType.FOOD);
        }
    }
}