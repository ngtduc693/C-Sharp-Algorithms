namespace Algorithms.RecommenderSystem;

/// <summary>
/// Strategy interface for computing similarity between two users' rating vectors.
/// Keeping this interface allows us to:
///  - plug in different similarity functions (Pearson, cosine, etc.)
///  - and mock it easily in unit tests for PredictRating.
/// </summary>
public interface ISimilarityCalculator
{
    double CalculateSimilarity(
        Dictionary<string, double> user1Ratings,
        Dictionary<string, double> user2Ratings);
}
