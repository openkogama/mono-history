using System;

namespace MV.WorldObject;

public class RandomGenerator
{
	private static readonly ushort[] randomNumbers = new ushort[256]
	{
		63101, 24072, 4277, 64989, 41233, 27206, 8226, 58274, 64073, 30497,
		343, 55219, 36771, 12206, 6068, 5577, 61010, 56982, 14848, 5351,
		35880, 42365, 44522, 51264, 49301, 34600, 56776, 37237, 10329, 22669,
		35298, 42843, 54371, 26315, 69, 47337, 43491, 40573, 766, 63643,
		46728, 14311, 28448, 7481, 22278, 39758, 15436, 44807, 38772, 26600,
		50563, 10276, 48339, 49565, 38251, 1819, 41303, 44838, 43032, 32767,
		43963, 9880, 4559, 44569, 57923, 1439, 49147, 23691, 29958, 40602,
		1442, 34592, 41997, 20781, 65216, 1273, 4768, 20963, 3411, 15315,
		55730, 15772, 14178, 15261, 53763, 2991, 54902, 24824, 4231, 5444,
		45844, 50708, 45622, 62029, 15092, 6599, 60977, 24087, 22966, 11514,
		45739, 4245, 40307, 45960, 34742, 5538, 61084, 16790, 15638, 57540,
		39857, 42186, 60109, 7869, 16190, 9745, 61157, 53772, 4736, 51523,
		6345, 59792, 36141, 23915, 12503, 20302, 27994, 10643, 10056, 26930,
		2320, 10445, 6750, 15147, 3620, 60866, 24912, 57607, 3122, 30420,
		29073, 27489, 28388, 61971, 52781, 19570, 37397, 53596, 59116, 47467,
		36351, 26121, 56819, 2031, 56017, 9383, 57751, 5180, 57812, 12329,
		18713, 49999, 6262, 51116, 14655, 36422, 26210, 43623, 35575, 806,
		48897, 36257, 22604, 343, 58908, 64157, 28603, 60877, 17733, 22535,
		32545, 32831, 51411, 31119, 49644, 14403, 46640, 42324, 31817, 29555,
		42399, 30451, 14017, 38829, 24192, 30765, 8779, 28590, 48839, 28907,
		46225, 190, 36769, 59475, 53718, 2855, 60795, 2175, 48397, 24695,
		60880, 6811, 55668, 61384, 53131, 26026, 27307, 49121, 59029, 25897,
		25699, 20940, 20230, 22501, 62539, 43098, 44270, 62468, 18438, 42098,
		19966, 28370, 5391, 40152, 46679, 2341, 57944, 57683, 57769, 38572,
		47755, 37049, 40874, 37340, 46801, 18609, 56352, 29646, 8927, 26372,
		6088, 16185, 39723, 16217, 3662, 3563
	};

	private uint seed;

	private uint step = 1u;

	private uint round = 1u;

	public RandomGenerator(uint seed)
	{
		this.seed = seed;
	}

	public RandomGenerator(uint seed, uint step, uint round)
	{
		this.seed = seed;
		this.step = step;
		this.round = round;
	}

	public int[] ToIntArray()
	{
		return new int[3]
		{
			(int)seed,
			(int)step,
			(int)round
		};
	}

	public int Range(int min, int max)
	{
		max--;
		if (max < min)
		{
			throw new Exception("max must be greater than min");
		}
		int num = Math.Abs(GetNewRandom() / 2);
		int num2 = max - min;
		return min + num % (num2 + 1);
	}

	public int GetNewRandom()
	{
		uint num = IncrementRandomIndex();
		ushort num2 = randomNumbers[num % randomNumbers.Length];
		ushort num3 = randomNumbers[~num % randomNumbers.Length];
		int num4 = num2;
		num4 <<= 16;
		return num4 + num3;
	}

	private uint IncrementRandomIndex()
	{
		uint result = seed * step * round;
		step++;
		if (step >= randomNumbers.Length)
		{
			step = 0u;
			round++;
		}
		return result;
	}

	public override string ToString()
	{
		return $"Seed {seed}. Round {round}. Step {step}.";
	}
}
