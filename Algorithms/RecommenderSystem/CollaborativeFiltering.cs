namespace Algorithms.RecommenderSystem;

public class CollaborativeFiltering(ISimilarityCalculator similarityCalculator)
{
    private readonly ISimilarityCalculator similarityCalculator =
        similarityCalculator ?? throw new ArgumentNullException(nameof(similarityCalculator));

    /// <summary>
    /// Compute Pearson correlation between two users' rating vectors.
    /// This method is pure (no reliance on similarityCalculator) and is kept public
    /// so it can be unit-tested independently from PredictRating.
    /// </summary>
    /// <remarks>
    /// - When there are no common items, return 0.
    /// - When either variance is 0 (denominator ~ 0), return 0 to avoid division by zero.
    /// </remarks>
    public double CalculateSimilarity(
        Dictionary<string, double> user1Ratings,
        Dictionary<string, double> user2Ratings)
    {
        var commonItems = user1Ratings.Keys.Intersect(user2Ratings.Keys).ToList();
        if (commonItems.Count == 0)
        {
            return 0d;
        }

        var u1 = commonItems.Select(i => user1Ratings[i]).ToArray();
        var u2 = commonItems.Select(i => user2Ratings[i]).ToArray();

        var avg1 = u1.Average();
        var avg2 = u2.Average();

        double num = 0d, sumSq1 = 0d, sumSq2 = 0d;

        for (int i = 0; i < commonItems.Count; i++)
        {
            var d1 = u1[i] - avg1;
            var d2 = u2[i] - avg2;
            num += d1 * d2;
            sumSq1 += d1 * d1;
            sumSq2 += d2 * d2;
        }

        var denom = Math.Sqrt(sumSq1 * sumSq2);

        // Using a tiny threshold to avoid numerical issues when denom is ~0.
        return denom <= 1e-10 ? 0d : num / denom;
    }

    /// <summary>
    /// Predict a rating for a specific item by a target user using
    /// a standard user-based CF formula:
    ///     predicted = sum(sim(u, v) * rating_v_item) / sum(|sim(u, v)|)
    ///
    /// Notes:
    /// - Only neighbors who have rated the target item are considered.
    /// - Denominator uses absolute similarity (common in CF literature) to avoid
    ///   cancellation from positive/negative neighbors, while the numerator keeps the sign.
    /// - If no neighbors or the denominator is ~0, return 0 (insufficient signal).
    /// </summary>
    public double PredictRating(
        string targetItem,
        string targetUser,
        Dictionary<string, Dictionary<string, double>> ratings)
    {
        // Guard: target user must exist
        if (!ratings.TryGetValue(targetUser, out var targetUserRatings))
        {
            return 0d;
        }

        double totalAbsSim = 0d;
        double weightedSum = 0d;

        foreach (var (otherUser, otherUserRatings) in ratings)
        {
            if (otherUser == targetUser)
            {
                continue;
            }

            // Skip neighbors who haven't rated the target item
            if (!otherUserRatings.TryGetValue(targetItem, out var neighborRating))
            {
                continue;
            }

            // Critical: use the injected similarity strategy (mockable in tests)
            var sim = similarityCalculator.CalculateSimilarity(targetUserRatings, otherUserRatings);

            // Skip zero-similarity neighbors entirely
            if (sim == 0d)
            {
                continue;
            }

            totalAbsSim += Math.Abs(sim);
            weightedSum += sim * neighborRating;
        }

        return totalAbsSim <= 1e-10 ? 0d : weightedSum / totalAbsSim;
    }
}
