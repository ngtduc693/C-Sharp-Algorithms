namespace Algorithms.Other;

public static class Geohash
{
    private const string Base32Characters = "0123456789bcdefghjkmnpqrstuvwxyz";
    private const int GeohashLength = 12;

    // Encodes the provided latitude and longitude coordinates into a Geohash string.
    public static string Encode(double latitude, double longitude)
    {
        Span<double> latitudeRange = stackalloc double[] { -90.0, 90.0 };
        Span<double> longitudeRange = stackalloc double[] { -180.0, 180.0 };
        bool isEncodingLongitude = true;
        int currentBit = 0, base32Index = 0;
        var geohashResult = new StringBuilder(GeohashLength);

        while (geohashResult.Length < GeohashLength)
        {
            var range = isEncodingLongitude ? longitudeRange : latitudeRange;
            var value = isEncodingLongitude ? longitude : latitude;
            var midpoint = (range[0] + range[1]) / 2;
            if (value > midpoint)
            {
                base32Index |= 1 << (4 - currentBit);
                range[0] = midpoint;
            }
            else
            {
                range[1] = midpoint;
            }
            isEncodingLongitude = !isEncodingLongitude;
            if (currentBit < 4)
                currentBit++;
            else
            {
                geohashResult.Append(Base32Characters[base32Index]);
                currentBit = 0;
                base32Index = 0;
            }
        }
        return geohashResult.ToString();
    }
}
