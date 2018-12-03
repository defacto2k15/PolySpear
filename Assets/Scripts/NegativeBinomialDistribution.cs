using UnityEngine;
using System.Collections;

public class NegativeBinomialDistribution
{

	/// <summary>
	/// Returns a random integer with a negative binomial distribution and the given values of r and p.
	/// Attempts are made with probability p of success until there are r failures. The number of successes is returned.
	/// </summary>
	/// <returns>A random integer with a negative binomial distribution and the given values of r and p.</returns>
	/// <param name="r">The number of failures.</param>
	/// <param name="p">The probability of success.</param>
	public static int fromRAndP (int r, float p)
	{
		int successes = 0;
		int failures = 0;
		while (failures < r) {
			if (Random.value < p) {
				++successes;
			} else {
				++failures;
			}
		}
		return successes;
	}

	/// <summary>
	/// Returns a random integer with a negative binomial distribution and the given values of r and p.
	/// For integer values of r, it gives results as if attempts are made with probability p of success until there are r failures.
	/// The number of successes is returned.
	/// </summary>
	/// <returns>A random integer with a negative binomial distribution and the given values of r and p.</returns>
	/// <param name="r">The number of failures.</param>
	/// <param name="p">The probability of success.</param>
	public static int fromRAndP (float r, float p)
	{
		float pmf = Mathf.Pow (1 - p, r);
		float rand = Random.value;
		int k = 0;
		while (k < 256) {
			if (rand < pmf) {
				return k;
			} else {
				rand -= pmf;
				++k;
				pmf *= (k + r - 1) * p / k;
			}
		}
		return k;
	}

	/// <summary>
	/// Returns a random integer with a Poisson distribution and the given value of lambda.
	/// This distribution is what you'd expect for the number of heads if there are lots of coins and they're each very unlikely to land on heads.
	/// It's also the limit of negative binomial distribution for the smallest standard deviation and a given mean.
	/// </summary>
	/// <returns>A random integer with a Poisson distribution and the given values of lambda.</returns>
	/// <param name="lambda">The mean.</param>
	public static int poisson (float lambda)
	{
		float pmf = Mathf.Exp (-lambda);
		float rand = Random.value;
		int k = 0;
		while (k < 256) {
			if (rand < pmf) {
				return k;
			} else {
				rand -= pmf;
				++k;
				pmf *= lambda / k;
			}
		}
		return k;
	}

	/// <summary>
	/// Returns a random integer with a negative binomial distribution with a mean of mu and a standard deviation of sigma,
	/// unless the given value of sigma is too high to produce a nice curve.
	/// </summary>
	/// <returns>A random integer with a negative binomial distribution with a mean of mu and a standard deviation of sigma,
	/// unless the given value of sigma is too high to produce a nice curve, or too low to be possible.</returns>
	/// <param name="mu">The mean.</param>
	/// <param name="sigma">The standard deviation.</param>
	public static int fromMeanAndStandardDeviation (float mu, float sigma)
	{
		const float R_MIN = 1.5f;
		if (sigma * sigma <= mu) {
			return poisson (mu);
		}
		float r = mu * mu / (sigma * sigma - mu);
		r = Mathf.Max (r, R_MIN);
		float p = mu / (mu + r);
		return fromRAndP (r, p);
	}
}
