using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTS.Tests.Ninject;
using VTS.Core.CrossCutting;

namespace VTS.Tests
{
    [TestClass]
    public class TestLocalizeServices
    {
        string empty = @"N/A";
        string key = "key";
        string russian = "Russian";
        string english = "English";
        LocalizeService _localizeService = FactorySingleton.FactoryOffline.Get<LocalizeService>();

        [ClassInitialize]
        public static void InitLocalize(TestContext context)
        {
        }

        [TestMethod]
        public void TestLocalizeDefault()
        {

            Assert.IsNotNull(_localizeService, "Error: localizeService Is Null");
            var result = _localizeService.Localize(key);
            Assert.IsNotNull(result, "Error: Localize(key) Is Null");
            Assert.AreNotEqual(result, empty, "Error: Default key is empty");
            Assert.AreEqual(result, english, "Error: Default key is not equal English");
        }

        [TestMethod]
        public void TestLocalizeRu()
        {
            _localizeService.LoadLocalization("ru");
            var result = _localizeService.Localize(key);
            Assert.IsNotNull(result, "Error: Localize(key) Is Null");
            Assert.AreNotEqual(result, empty, "Error: Default key is empty");
            Assert.AreNotEqual(result, english, "Error: Change language is not work");
            Assert.AreEqual(result, russian, "Error: ru key is not equal ru");
        }

        [TestMethod]
        public void TestLocalizeChangeLanguage()
        {
            _localizeService.LoadLocalization("ru");
            var result = _localizeService.Localize(key);
            Assert.IsNotNull(result, "Error: Localize(key) Is Null");
            Assert.AreNotEqual(result, empty, "Error: Default key is empty");
            Assert.AreNotEqual(result, english, "Error: Change language is not work");
            Assert.AreEqual(result, russian, "Error: ru key is not equal ru");
            _localizeService.LoadLocalization("en");
            result = _localizeService.Localize(key);
            Assert.IsNotNull(result, "Error: Localize(key) Is Null");
            Assert.AreNotEqual(result, empty, "Error: Default key is empty");
            Assert.AreNotEqual(result, russian, "Error: Change language is not work");
            Assert.AreEqual(result, english, "Error: en key is not equal en");

        }

        [TestMethod]
        public void TestLocalizeFalseKey()
        {
            var result = _localizeService.Localize("!=s");
            Assert.IsNotNull(result, "Error: Localize(key) Is Null");
            Assert.AreEqual(result, empty, "Error: empty key is not empty");
        }
    }
}
