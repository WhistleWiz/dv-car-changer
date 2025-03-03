﻿using UnityEngine;

namespace CarChanger.Common
{
    public enum WagonType
    {
        Flatbed = 200,
        FlatbedStakes = 201,
        FlatbedMilitary = 202,
        [InspectorName("Utility Flatbed")]
        FlatbedShort = 220,
        Autorack = 250,
        TankOil = 301,
        TankGas = 303,
        TankChem = 305,
        TankShortFood = 325,
        Stock = 350,
        Boxcar = 400,
        BoxcarMilitary = 404,
        Refrigerator = 450,
        Hopper = 500,
        HopperCovered = 510,
        Gondola = 550,
        NuclearFlask = 800,
        UseLivery = 1000
    }

    public enum WagonLivery
    {
        FlatbedEmpty = 200,
        FlatbedStakes = 201,
        FlatbedMilitary = 202,
        [InspectorName("Utility Flatbed")]
        FlatbedShort = 220,
        AutorackRed = 250,
        AutorackBlue = 251,
        AutorackGreen = 252,
        AutorackYellow = 253,
        TankOrange = 300,
        TankWhite = 301,
        TankYellow = 302,
        TankBlue = 303,
        TankChrome = 304,
        TankBlack = 305,
        TankShortMilk = 325,
        StockRed = 350,
        StockGreen = 351,
        StockBrown = 352,
        BoxcarBrown = 400,
        BoxcarGreen = 401,
        BoxcarPink = 402,
        BoxcarRed = 403,
        BoxcarMilitary = 404,
        RefrigeratorWhite = 450,
        HopperBrown = 500,
        HopperTeal = 501,
        HopperYellow = 502,
        HopperCoveredBrown = 510,
        GondolaRed = 550,
        GondolaGreen = 551,
        GondolaGray = 552,
        NuclearFlask = 800
    }

    public enum PassengerType
    {
        All = 0,
        Red = 600,
        Green = 601,
        Blue = 602,
        FirstClass = 1000,
        SecondClass = 1001
    }
}
