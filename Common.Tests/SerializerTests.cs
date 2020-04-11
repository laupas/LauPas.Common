using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using LauPas.Common;

namespace Common.Tests
{
    [TestClass]
    public class SerializerTests : BaseTest
    {
        public SerializerTests()
        {
            this.StartAllServices<SerializerTests>();
        }
        
        [TestMethod]
        public void Serialize_XML()
        {
            // Arrange
            var data = "1234";

            // Act
            var value = data.Serialize(SerializationType.Xml);

            //Assert
            value.Should().Be("<?xml version=\"1.0\" encoding=\"utf-16\"?><string>1234</string>");
        }
        
        [TestMethod]
        public void DeSerialize_XML_ValidXmlFormat_DetectAndSerialize()
        {
            // Arrange
            var data = "<?xml version=\"1.0\" encoding=\"utf-16\"?><string>1234</string>";

            // Act
            var value = data.Deserialize<string>();

            //Assert
            value.Should().Be("1234");
        }
        
        [TestMethod]
        public void DeSerialize_StringOnly_DetectAndSerialize()
        {
            // Arrange
            var data = "hello test";

            // Act
            var value = data.Deserialize<string>();

            //Assert
            value.Should().Be("hello test");
        }

    }
}