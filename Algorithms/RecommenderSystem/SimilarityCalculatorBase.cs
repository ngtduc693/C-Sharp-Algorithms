using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.RecommenderSystem
{
    public class SimilarityCalculatorBase : ISimilarityCalculator
    {
        /// <summary>
        /// Method to calculate similarity between two users using Pearson correlation.
        /// </summary>
        /// <param name="user1Ratings">Rating of User 1.</param>
        /// <param name="user2Ratings">Rating of User 2.</param>
        /// <returns>double value to reflect the index of similarity between two users.</returns>
        public double CalculateSimilarity(Dictionary<string, double> user1Ratings, Dictionary<string, double> user2Ratings)
        {
            var commonItems = user1Ratings.Keys.Intersect(user2Ratings.Keys).ToList();
            if (commonItems.Count == 0)
            {
                return 0;
            }

            var user1Scores = commonItems.Select(item => user1Ratings[item]).ToArray();
            var user2Scores = commonItems.Select(item => user2Ratings[item]).ToArray();

            var avgUser1 = user1Scores.Average();
            var avgUser2 = user2Scores.Average();

            double numerator = 0;
            double sumSquare1 = 0;
            double sumSquare2 = 0;
            double epsilon = 1e-10;

            for (var i = 0; i < commonItems.Count; i++)
            {
                var diff1 = user1Scores[i] - avgUser1;
                var diff2 = user2Scores[i] - avgUser2;

                numerator += diff1 * diff2;
                sumSquare1 += diff1 * diff1;
                sumSquare2 += diff2 * diff2;
            }

            var denominator = Math.Sqrt(sumSquare1 * sumSquare2);
            return Math.Abs(denominator) < epsilon ? 0 : numerator / denominator;
        }
    }
}
