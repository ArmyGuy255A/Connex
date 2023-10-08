using NUnit.Framework;
using Tablazor.Components; // Your namespace might differ.
using Microsoft.AspNetCore.Components.Web;

namespace Tablazor.Tests
{
    public class TablazorButtonTests
    {
        private TablerButton _tablerButton;

        [SetUp]
        public void Setup()
        {
            _tablerButton = new TablerButton();
        }

        [TestCase("primary", "pill", 100, "btn btn-pill btn-primary w-100")]
        [TestCase("secondary", "square", 100, "btn btn-square btn-secondary w-100")]
        [TestCase("info", null, 100, "btn btn-info w-100")]
        [TestCase("", "pill", 100, "btn btn-pill w-100")]
        [TestCase("", "", 100, "btn w-100")]
        [TestCase("", "", 50, "btn w-50")]
        [TestCase(null, "outline", 100, "btn btn-outline w-100")]
        [TestCase(null, null, 100, "btn w-100")]
        public void OnInitialized_AddsCorrectClasses(string bootStyle, string bootType, int width, string expectedClasses)
        {
            // Arrange
            _tablerButton.BootStyle = bootStyle;
            _tablerButton.BootType = bootType;
            _tablerButton.Width = width;

            // Act
            _tablerButton.Initialize();

            // Assert
            Assert.AreEqual(expectedClasses, _tablerButton.Class);
        }

        [Test]
        public void ActiveParameter_AddsActiveClass()
        {
            // Arrange
            _tablerButton.Active = true;

            // Act
            _tablerButton.Initialize();

            // Assert
            StringAssert.Contains("active", _tablerButton.Class);
        }

        [Test]
        public void DisabledParameter_AddsDisabledClass()
        {
            // Arrange
            _tablerButton.Disabled = true;

            // Act
            _tablerButton.Initialize();

            // Assert
            StringAssert.Contains("disabled", _tablerButton.Class);
        }
    }
}