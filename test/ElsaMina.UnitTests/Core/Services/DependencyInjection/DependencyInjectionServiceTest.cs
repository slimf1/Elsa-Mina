using System.Reflection;
using Autofac;
using ElsaMina.Core.Services.DependencyInjection;
using NSubstitute;

namespace ElsaMina.UnitTests.Core.Services.DependencyInjection;

public class DependencyContainerServiceTest
{
    private DependencyContainerService _service;
    private IContainer _container;

    [SetUp]
    public void SetUp()
    {
        _service = new DependencyContainerService();
        _container = Substitute.For<IContainer>();
        _service.SetContainer(_container);
    }

    [Test]
    public void Test_SetContainer_ShouldStoreContainerInstance_WhenContainerIsProvided()
    {
        // Arrange
        var newContainer = Substitute.For<IContainer>();

        // Act
        _service.SetContainer(newContainer);

        // Assert
        Assert.That(newContainer, Is.EqualTo(typeof(DependencyContainerService)
            .GetField("_container", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(_service)));
    }

    [Test]
    public void Test_Resolve_ShouldReturnResolvedInstance_WhenContainerIsSet()
    {
        // Arrange
        var expectedInstance = new object();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(expectedInstance).As<object>();
        _service.SetContainer(builder.Build());

        // Act
        var result = _service.Resolve<object>();

        // Assert
        Assert.That(result, Is.SameAs(expectedInstance));
    }

    [Test]
    public void Test_Resolve_ShouldReturnDefault_WhenContainerIsNull()
    {
        // Arrange
        _service.SetContainer(null);

        // Act
        var result = _service.Resolve<object>();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_ResolveNamed_ShouldReturnNamedInstance_WhenContainerIsSet()
    {
        // Arrange
        var expectedInstance = new object();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(expectedInstance).SingleInstance().Named<object>("test").As<object>();
        _service.SetContainer(builder.Build());

        // Act
        var result = _service.ResolveNamed<object>("test");

        // Assert
        Assert.That(result, Is.SameAs(expectedInstance));
    }

    [Test]
    public void Test_ResolveNamed_ShouldReturnDefault_WhenContainerIsNull()
    {
        // Arrange
        _service.SetContainer(null);

        // Act
        var result = _service.ResolveNamed<object>("test");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Test_IsRegisteredWithName_ShouldReturnTrue_WhenTypeIsRegistered()
    {
        // Arrange
        _container.IsRegisteredWithName<object>("test").Returns(true);

        // Act
        var result = _service.IsRegisteredWithName<object>("test");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Test_IsRegisteredWithName_ShouldReturnFalse_WhenContainerIsNull()
    {
        // Arrange
        _service.SetContainer(null);

        // Act
        var result = _service.IsRegisteredWithName<object>("test");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Test_GetAllRegistrations_ShouldReturnEmptyList_WhenContainerIsNull()
    {
        // Arrange
        _service.SetContainer(null);

        // Act
        var result = _service.GetAllNamedRegistrations<object>();

        // Assert
        Assert.That(result, Is.Empty);
    }
}