using NUnit.Framework;
using Tablazor.Components;

namespace Tablazor.Tests;

public class TablazorBaseComponentTests
{
    [Test]
    public void ShouldNotAddDuplicateClasses()
    {
        // Arrange
        var tablerComponent = new TablerComponent();

        // Act
        tablerComponent.AddClass("btn");
        tablerComponent.AddClass("btn");
        tablerComponent.AddClasses("btn btn-primary");
        tablerComponent.AddClasses("btn-primary btn-primary");

        // Assert
        Assert.AreEqual("btn btn-primary", tablerComponent.Class);
    }
    
    [Test]
    public void ShouldRemoveClass()
    {
        // Arrange
        var tablerComponent = new TablerComponent();

        // Act
        tablerComponent.AddClasses("btn btn-primary active disabled");
        tablerComponent.RemoveClasses("disabled active");

        // Assert
        Assert.AreEqual("btn btn-primary", tablerComponent.Class);
    }
    
    [Test]
    public void ShouldNotErrorOnRemoveMissingClass()
    {
        // Arrange
        var tablerComponent = new TablerComponent();

        // Act
        tablerComponent.AddClasses("btn btn-primary active");
        tablerComponent.RemoveClasses("disabled");

        // Assert
        Assert.AreEqual("btn btn-primary active", tablerComponent.Class);
    }
    
    
}