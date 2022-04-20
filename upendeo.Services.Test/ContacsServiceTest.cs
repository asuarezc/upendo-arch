using System;
using System.Collections.Generic;
using LiteDB;
using Moq;
using upendo.CrossCutting.Entities;
using upendo.CrossCutting.Interfaces.Data.LocalDB;
using upendo.CrossCutting.Interfaces.Logic;
using upendo.Services.Logic;
using Xunit;

namespace upendeo.Services.Test
{
    public class ContacsServiceTest
    {
        private static readonly IContactsService service = GetService();

        private static ILocalDBProvider GetDBMock()
        {
            Mock<ILocalDBProvider> result = new();
            Mock<ILiteDatabase> databaseMock = new();
            Mock<ILiteCollection<Contact>> collectionMock = new();

            collectionMock.Setup(mock => mock.FindAll()).Returns(new List<Contact> { new Contact() });
            collectionMock.Setup(mock => mock.Insert(It.IsAny<Contact>())).Returns(new BsonValue());
            databaseMock.Setup(mock => mock.GetCollection<Contact>()).Returns(collectionMock.Object);
            result.Setup(mock => mock.Database).Returns(databaseMock.Object);

            return result.Object;
        }

        private static IContactsService GetService()
        {
            return new ContactsService(GetDBMock());
        }

        [Fact]
        public void InitServiceWithoutMock()
        {
            static IContactsService func() { return new ContactsService(null); }

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(func);
            Assert.Equal("localDBProvider", exception.ParamName);
        }

        [Fact]
        public void GetStoredContacts()
        {
            IEnumerable<Contact> contacts = service.GetStoredContacts();

            Assert.NotNull(contacts);
            Assert.NotEmpty(contacts);
        }

        [Fact]
        public void AddContact()
        {
            service.AddContact(new Contact());
        }

        [Fact]
        public void AddContact_WhenContactIsNull()
        {
            static void action() => service.AddContact(null);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("contact", exception.ParamName);
        }
    }
}
