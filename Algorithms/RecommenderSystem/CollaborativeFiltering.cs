using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.RecommenderSystem
{
    public class CollaborativeFiltering
    {
        private readonly ISimilarityCalculator similarityCalculator;

        public CollaborativeFiltering(SimilarityCalculatorBase similarityCalculator)
        {
            this.similarityCalculator = similarityCalculator;
        }

        /// <summary>
        /// Predict a rating for a specific item by a target user.
        /// </summary>
        /// <param name="targetItem">The item for which the rating needs to be predicted.</param>
        /// <param name="targetUser">The user for whom the rating is being predicted.</param>
        /// <param name="ratings">
        /// A dictionary containing user ratings where:
        /// - The key is the user's identifier (string).
        /// - The value is another dictionary where the key is the item identifier (string), and the value is the rating given by the user (double).
        /// </param>
        /// <returns>The predicted rating for the target item by the target user.
        /// If there is insufficient data to predict a rating, the method returns 0.
        /// </returns>
        public double PredictRating(string targetItem, string targetUser, Dictionary<string, Dictionary<string, double>> ratings)
        {
            var targetUserRatings = ratings[targetUser];
            double totalSimilarity = 0;
            double weightedSum = 0;
            double epsilon = 1e-10;

            foreach (var otherUser in ratings.Keys.Where(u => u != targetUser))
            {
                var otherUserRatings = ratings[otherUser];
                if (otherUserRatings.ContainsKey(targetItem))
                {
                    var similarity = similarityCalculator.CalculateSimilarity(targetUserRatings, otherUserRatings);
                    totalSimilarity += Math.Abs(similarity);
                    weightedSum += similarity * otherUserRatings[targetItem];
                }
            }

            return Math.Abs(totalSimilarity) < epsilon ? 0 : weightedSum / totalSimilarity;
        }
    }
}
