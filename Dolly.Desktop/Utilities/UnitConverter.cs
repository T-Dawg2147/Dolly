namespace Dolly.Desktop.Utilities;

public static class UnitConverter
{
    public static double TwipsToDiu(long twips)
    {
        return twips / 20f;
    }

    /// <summary>
    /// Converts WPF DIU to twips
    /// </summary>
    public static long DiuToTwips(double diu)
    {
        return (long)(diu * 20);
    }
}