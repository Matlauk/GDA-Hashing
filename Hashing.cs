using System;
using System.Numerics;
#if GODOT_REAL_T_IS_DOUBLE
using Real = System.Double;
#else
using Real = System.Single;
#endif

public static class Hashing
{
	private const uint PRIME_X = 0x27d4eb2d;
	private const uint PRIME_Y = 0x85ebca6b;
	private const uint PRIME_Z = 0xc2b2ae35;
	
	
	private static uint StringHash(string value)
	{
		unchecked
		{
			uint hash = 2166136261;
			for (int i = 0; i < value.Length; i++)
			{
				hash ^= value[i];
				hash *= 16777619;
			}

			return hash;
		}
	}

	// --------------------------------------------------
	// Final avalanche mixer
	// --------------------------------------------------

	private static uint Avalanche(uint h)
	{
		unchecked
		{
			h ^= h >> 16;
			h *= 0x7feb352d;
			h ^= h >> 15;
			h *= 0x846ca68b;
			h ^= h >> 16;

			return h;
		}
	}

	private static uint FoldDoubleBits(double value)
	{
		ulong bits = (ulong)BitConverter.DoubleToInt64Bits(value);
		return (uint)bits ^ (uint)(bits >> 32);
	}

	public static uint Rehash(uint value)
	{
		return Avalanche(value);
	}
	
	// --------------------------------------------------
	// 1D Hash
	// --------------------------------------------------

	public static uint Hash1(int x, int seed)
	{
		unchecked
		{
			uint h = (uint)seed;

			h ^= (uint)x * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			return Avalanche(h);
		}
	}
	
	public static uint Hash1(float x, int seed)
	{
		unchecked
		{
			uint hx = BitConverter.SingleToUInt32Bits(x);

			uint h = (uint)seed;

			h ^= hx * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			return Avalanche(h);
		}
	}

#if GODOT_REAL_T_IS_DOUBLE
	public static uint Hash1(double x, int seed)
	{
		unchecked
		{
			uint hx = FoldDoubleBits(x);

			uint h = (uint)seed;

			h ^= hx * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			return Avalanche(h);
		}
	}
#endif

	// --------------------------------------------------
	// 2D Hash
	// --------------------------------------------------

	public static uint Hash2(int x, int z, int seed)
	{
		unchecked
		{
			uint h = (uint)seed;

			h ^= (uint)x * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			h ^= (uint)z * PRIME_Y;
			h = BitOperations.RotateLeft(h, 13);

			return Avalanche(h);
		}
	}
	
	public static uint Hash2(float x, float z, int seed)
	{
		unchecked
		{
			uint hx = BitConverter.SingleToUInt32Bits(x);
			uint hz = BitConverter.SingleToUInt32Bits(z);

			uint h = (uint)seed;

			h ^= hx * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			h ^= hz * PRIME_Y;
			h = BitOperations.RotateLeft(h, 13);

			return Avalanche(h);
		}
	}

#if GODOT_REAL_T_IS_DOUBLE
	public static uint Hash2(double x, double z, int seed)
	{
		unchecked
		{
			uint hx = FoldDoubleBits(x);
			uint hz = FoldDoubleBits(z);

			uint h = (uint)seed;

			h ^= hx * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			h ^= hz * PRIME_Y;
			h = BitOperations.RotateLeft(h, 13);

			return Avalanche(h);
		}
	}
#endif

	// --------------------------------------------------
	// 3D Hash
	// --------------------------------------------------

	public static uint Hash3(int x, int y, int z, int seed)
	{
		unchecked
		{
			uint h = (uint)seed;

			h ^= (uint)x * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			h ^= (uint)y * PRIME_Y;
			h = BitOperations.RotateLeft(h, 13);

			h ^= (uint)z * PRIME_Z;
			h = BitOperations.RotateLeft(h, 17);

			return Avalanche(h);
		}
	}
	
	public static uint Hash3(float x, float y, float z, int seed)
	{
		unchecked
		{
			uint hx = BitConverter.SingleToUInt32Bits(x);
			uint hy = BitConverter.SingleToUInt32Bits(y);
			uint hz = BitConverter.SingleToUInt32Bits(z);

			uint h = (uint)seed;

			h ^= hx * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			h ^= hy * PRIME_Y;
			h = BitOperations.RotateLeft(h, 13);

			h ^= hz * PRIME_Z;
			h = BitOperations.RotateLeft(h, 17);

			return Avalanche(h);
		}
	}

#if GODOT_REAL_T_IS_DOUBLE
	public static uint Hash3(double x, double y, double z, int seed)
	{
		unchecked
		{
			uint hx = FoldDoubleBits(x);
			uint hy = FoldDoubleBits(y);
			uint hz = FoldDoubleBits(z);

			uint h = (uint)seed;

			h ^= hx * PRIME_X;
			h = BitOperations.RotateLeft(h, 15);

			h ^= hy * PRIME_Y;
			h = BitOperations.RotateLeft(h, 13);

			h ^= hz * PRIME_Z;
			h = BitOperations.RotateLeft(h, 17);

			return Avalanche(h);
		}
	}
#endif

	// --------------------------------------------------
	// Hash -> [0, 1]
	// --------------------------------------------------

	public static float HashToFloat01(uint h)
	{
		return h / (float)uint.MaxValue;
	}

	public static Real HashToReal01(uint h)
	{
		return h / (Real)uint.MaxValue;
	}

	// --------------------------------------------------
	// Hash -> [-1, 1]
	// --------------------------------------------------

	public static float HashToFloatSigned(uint h)
	{
		return (h / (float)uint.MaxValue) * 2f - 1f;
	}

	public static Real HashToRealSigned(uint h)
	{
		return (h / (Real)uint.MaxValue) * (Real)2.0 - (Real)1.0;
	}

	// --------------------------------------------------
	// Hash -> Range
	// --------------------------------------------------

	public static float HashToFloat(uint h, float min, float max)
	{
		return min + (max - min) * HashToFloat01(h);
	}

	public static Real HashToReal(uint h, Real min, Real max)
	{
		return min + (max - min) * HashToReal01(h);
	}
}
