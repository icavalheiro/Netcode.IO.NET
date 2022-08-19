using NetcodeIO.NET.Encryption.Tools;
using NetcodeIO.NET.Encryption.Interfaces;
using NetcodeIO.NET.Encryption.Digests;

namespace NetcodeIO.NET.Encryption.Security;

public class SecureRandom : Random
{
    private static long counter = DateTime.UtcNow.Ticks * 100L;
    protected readonly IRandomGenerator generator;
    private static readonly SecureRandom master = new SecureRandom(new CryptoRandomGenerator());
    private static readonly double DoubleScale = 1.0 / Convert.ToDouble(1L << 53);

    public SecureRandom() : this(CreatePrng()) { }

    public SecureRandom(IRandomGenerator generator)
    {
        this.generator = generator;
    }

    private static long NextCounterValue()
    {
        return Interlocked.Increment(ref counter);
    }

    private static SecureRandom Master
    {
        get { return master; }
    }

    private static DigestRandomGenerator CreatePrng()
    {
        IDigest digest = new Sha256Digest();

        DigestRandomGenerator prng = new DigestRandomGenerator(digest);
        prng.AddSeedMaterial(NextCounterValue());
        prng.AddSeedMaterial(GetNextBytes(Master, digest.GetDigestSize()));

        return prng;
    }

    public static byte[] GetNextBytes(SecureRandom secureRandom, int length)
    {
        byte[] result = new byte[length];
        secureRandom.NextBytes(result);
        return result;
    }

    [Obsolete("Call GenerateSeed() on a SecureRandom instance instead")]
    public static byte[] GetSeed(int length)
    {
        return GetNextBytes(Master, length);
    }

    public virtual byte[] GenerateSeed(int length)
    {
        return GetNextBytes(Master, length);
    }

    public override int Next()
    {
        return NextInt() & int.MaxValue;
    }

    public override int Next(int maxValue)
    {

        if (maxValue < 2)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException("maxValue", "cannot be negative");

            return 0;
        }

        int bits;

        // Test whether maxValue is a power of 2
        if ((maxValue & (maxValue - 1)) == 0)
        {
            bits = NextInt() & int.MaxValue;
            return (int)(((long)bits * maxValue) >> 31);
        }

        int result;
        do
        {
            bits = NextInt() & int.MaxValue;
            result = bits % maxValue;
        }
        while (bits - result + (maxValue - 1) < 0); // Ignore results near overflow

        return result;
    }

    public override int Next(int minValue, int maxValue)
    {
        if (maxValue <= minValue)
        {
            if (maxValue == minValue)
                return minValue;

            throw new ArgumentException("maxValue cannot be less than minValue");
        }

        int diff = maxValue - minValue;
        if (diff > 0)
            return minValue + Next(diff);

        for (; ; )
        {
            int i = NextInt();

            if (i >= minValue && i < maxValue)
                return i;
        }
    }

    public override void NextBytes(byte[] buf)
    {
        generator.NextBytes(buf);
    }

    public virtual void NextBytes(byte[] buf, int off, int len)
    {
        generator.NextBytes(buf, off, len);
    }

    public override double NextDouble()
    {
        ulong x = (ulong)NextLong() >> 11;

        return Convert.ToDouble(x) * DoubleScale;
    }

    public virtual int NextInt()
    {
        byte[] bytes = new byte[4];
        NextBytes(bytes);
        return (int)Pack.BE_To_UInt32(bytes, 0);
    }

    public virtual long NextLong()
    {
        byte[] bytes = new byte[8];
        NextBytes(bytes);
        return (long)Pack.BE_To_UInt64(bytes, 0);
    }
}

