using ElsaMina.DataAccess.Models;
using ElsaMina.DataAccess.Repositories;
using FluentAssertions;
using NSubstitute;
using Serilog;

namespace ElsaMina.Test.DataAccess.Repositories;

public class CachedRepositoryTest
{
    // ReSharper disable once MemberCanBePrivate.Global
    public class MyEntity : IKeyed<int>
    {
        public int Key { get; set; }
    }

    private ILogger _logger;
    private IRepository<MyEntity, int> _repository;

    private CachedRepository<IRepository<MyEntity, int>, MyEntity, int> _cachedRepository;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _repository = Substitute.For<IRepository<MyEntity, int>>();

        _cachedRepository = new CachedRepository<IRepository<MyEntity, int>, MyEntity, int>(
            _logger, _repository
        );
    }

    #region GetByIdAsync

    [Test]
    public async Task Test_GetByIdAsync_ShouldRetrieveFromRepository_WhenIsFirstCall()
    {
        // Arrange
        _repository.GetByIdAsync(1).Returns(new MyEntity { Key = 1 });
        
        // Act
        var result = await _cachedRepository.GetByIdAsync(1);
        
        // Assert
        result.Key.Should().Be(1);
        await _repository.Received(1).GetByIdAsync(1);
    }
    
    [Test]
    public async Task Test_GetByIdAsync_ShouldRetrieveFromRepository_WhenIsSecondCall()
    {
        // Arrange
        _repository.GetByIdAsync(1).Returns(new MyEntity { Key = 1 });
        await _cachedRepository.GetByIdAsync(1);
        
        // Act
        var result = await _cachedRepository.GetByIdAsync(1);
        
        // Assert
        result.Key.Should().Be(1);
        await _repository.Received(1).GetByIdAsync(1);
    }

    #endregion

}