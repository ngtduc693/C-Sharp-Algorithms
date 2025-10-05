using System.Collections.Generic;
using Algorithms.RecommenderSystem;
using Moq;
using NUnit.Framework;

namespace Algorithms.Tests.RecommenderSystem;

[TestFixture]
public class CollaborativeFilteringTests
{
    private Mock<ISimilarityCalculator> mockSimilarityCalculator = null!;
    private CollaborativeFiltering recommender = null!;
    private Dictionary<string, Dictionary<string, double>> testRatings = null!;

    [SetUp]
    public void Setup()
    {
        mockSimilarityCalculator = new Mock<ISimilarityCalculator>(MockBehavior.Strict);
        recommender = new CollaborativeFiltering(mockSimilarityCalculator.Object);

        testRatings = new Dictionary<string, Dictionary<string, double>>
        {
            ["user1"] = new()
            {
                ["item1"] = 5.0,
                ["item2"] = 3.0,
                ["item3"] = 4.0
            },
            ["user2"] = new()
            {
                ["item1"] = 4.0,
                ["item2"] = 2.0,
                ["item3"] = 5.0
            },
            ["user3"] = new()
            {
                ["item1"] = 3.0,
                ["item2"] = 4.0,
                ["item4"] = 3.0
            }
        };
    }

    [Test]
    public void PredictRating_WithOtherUserHavingRatedTargetItem_ShouldCalculateSimilarityAndWeightedSum()
    {
        const string targetItem = "item1";
        const string targetUser = "user1";

        // Arrange mock: return 0.8 for any pair.
        // We'll verify it's called exactly twice (user1 vs user2, user1 vs user3).
        mockSimilarityCalculator
            .Setup(s => s.CalculateSimilarity(
                It.IsAny<Dictionary<string, double>>(),
                It.IsAny<Dictionary<string, double>>()))
            .Returns(0.8);

        // Act
        var predicted = recommender.PredictRating(targetItem, targetUser, testRatings);

        // Assert
        Assert.That(predicted, Is.Not.EqualTo(0.0d), "Should not be zero with two neighbors contributing.");
        Assert.That(predicted, Is.EqualTo(3.5d).Within(0.01),
            "Expected (0.8*4 + 0.8*3) / (|0.8| + |0.8|) = 3.5.");

        // Make the test more robust by verifying interaction with the mock:
        mockSimilarityCalculator.Verify(
            s => s.CalculateSimilarity(
                It.IsAny<Dictionary<string, double>>(),
                It.IsAny<Dictionary<string, double>>()),
            Times.Exactly(2));
    }

    [Test]
    [TestCase("item1", 4.0, 5.0)]
    [TestCase("item2", 2.0, 4.0)]
    public void CalculateSimilarity_WithValidInputs_ReturnsValueInRange(
        string commonItem,
        double rating1,
        double rating2)
    {
        var user1Ratings = new Dictionary<string, double> { [commonItem] = rating1 };
        var user2Ratings = new Dictionary<string, double> { [commonItem] = rating2 };

        // This calls the SUT's internal Pearson implementation (not the mock)
        var similarity = recommender.CalculateSimilarity(user1Ratings, user2Ratings);

        Assert.That(similarity, Is.InRange(-1.0, 1.0),
            "Pearson correlation should always be within [-1, 1].");
    }

    [Test]
    public void CalculateSimilarity_WithNoCommonItems_ReturnsZero()
    {
        var user1Ratings = new Dictionary<string, double> { ["item1"] = 5.0 };
        var user2Ratings = new Dictionary<string, double> { ["item2"] = 4.0 };

        var similarity = recommender.CalculateSimilarity(user1Ratings, user2Ratings);

        Assert.That(similarity, Is.EqualTo(0d),
            "No overlap → similarity should be 0.");
    }

    [Test]
    public void PredictRating_WithNonexistentItem_ReturnsZero()
    {
        // No neighbor has rated "nonexistentItem"
        var predicted = recommender.PredictRating("nonexistentItem", "user1", testRatings);

        Assert.That(predicted, Is.EqualTo(0d),
            "Insufficient neighbor signal → predicted rating should be 0.");
    }

    
}
