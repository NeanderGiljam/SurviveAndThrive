using UnityEngine;
using System.Collections;

public static class MathHelper {

    public static float CalculateDamage(float attack, float defence) {
        float damage = attack - (defence * 4);
        return Mathf.Clamp(damage, 0, damage);
    }

    public static float RoundToNearest(float toRound, float roundTo, bool roundUp) {
        if (roundUp) {
            return (roundTo - toRound % roundTo) + toRound;
        } else {
            return toRound - toRound % roundTo;
        }
    }

	public static float Average(float[] numbers) {
		float total = 0;
		for (int i = 0; i < numbers.Length; i++) {
			total += Mathf.Abs(numbers[i]);
		}	
		return total / numbers.Length;
	}

	public static float MapValueToRange(float inputValue, float fromRangeStart, float fromRangeEnd, float toRangeStart, float toRangeEnd) {
		float slope = 1.0f * (toRangeEnd - toRangeStart) / (fromRangeEnd - fromRangeStart);
		float mappedValue = toRangeStart + slope * (inputValue - fromRangeStart);
		return mappedValue;
	}
}
