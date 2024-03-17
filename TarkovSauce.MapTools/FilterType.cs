﻿namespace TarkovSauce.MapTools
{
    [Flags]
    public enum FilterType
    {
        None = 0,
        PmcExtract = 1 << 0,
        ScavExtract = 1 << 1,
        CurrentPos = 1 << 2,
    }
}