using Microsoft.VisualStudio.TestTools.UnitTesting;

using ObjectComparisonTest.Service;
using ObjectComparisonTest.Service.ObjectParameters;
using ObjectComparisonTest.Service.ObjectResponse;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectComparisonTest.Tests.Service
{
    [TestClass]
    public class ComparisonServiceTest
    {
        private readonly ComparisonService comparisonService = new ComparisonService();

        #region CanDirectlyCompare
        [TestMethod]
        public void CanDirectlyCompare_WithValidTypeString_ReturnTrue()
        {
            //Arrange
            object inputToCheck = "SomeString";

            //Act
            var result = comparisonService.CanDirectlyCompare(inputToCheck.GetType());

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDirectlyCompare_WithValidTypeInt_ReturnTrue()
        {
            //Arrange
            object inputToCheck = 1;

            //Act
            var result = comparisonService.CanDirectlyCompare(inputToCheck.GetType());

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanDirectlyCompare_WithInvalidType_ReturnFalse()
        {
            //Arrange
            object inputToCheck = new List<string>() { "1", "2", "3" };

            //Act
            var result = comparisonService.CanDirectlyCompare(inputToCheck.GetType());

            //Assert
            Assert.IsFalse(result);
        }
        #endregion
        #region AreValuesEqual
        [TestMethod]
        public void AreValuesEqual_WithValidInputParameters_ReturnTrue()
        {
            //Arrange
            object itemA = "TestParam";
            object itemB = "TestParam";

            //Act
            var result = comparisonService.AreValuesEqual(itemA, itemB);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreValuesEqual_WithInvalidInputParameters_ReturnTrue()
        {
            //Arrange
            object itemA = "TestParam";
            object itemB = "AnotherTest";

            //Act
            var result = comparisonService.AreValuesEqual(itemA, itemB);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AreValuesEqual_WithInvalidNullInputParameters_ReturnTrue()
        {
            //Arrange
            object itemA = "TestParam";
            object itemB = null;

            //Act
            var result = comparisonService.AreValuesEqual(itemA, itemB);

            //Assert
            Assert.IsFalse(result);
        }
        #endregion

        #region GetChanges
        [TestMethod]
        public void GetChanges_WithValidInputTypes_ReturnDifferenceCountZero()
        {
            //Arrange
            var inputParametersA = new SomeTypeParameters()
            {
                Forename = "David",
                DateOfBirth = new DateTime(1990, 5, 2),
                SpecialNumber = 23
            };

            var inputParametersB = new SomeTypeParameters()
            {
                Forename = "David",
                DateOfBirth = new DateTime(1990, 5, 2),
                SpecialNumber = 23
            };


            //Act
            var result = comparisonService.GetChanges(inputParametersA, inputParametersB);

            //Assert
            Assert.AreEqual(0, result.Differences.Count);
        }

        [TestMethod]
        public void GetChanges_WithInvalidInputTypes_ReturnFalse()
        {
            //Arrange
            var inputParametersA = new SomeTypeParameters()
            {
                Forename = "David",
                DateOfBirth = new DateTime(1990, 5, 2),
                SpecialNumber = 23
            };

            var inputParametersB = new SomeTypeParameters()
            {
                Forename = "Janet",
                DateOfBirth = new DateTime(2000, 11, 22),
                SpecialNumber = 23
            };


            //Act
            var result = comparisonService.GetChanges(inputParametersA, inputParametersB);

            //Assert
            Assert.IsNotNull(result.Differences);
        }


        [TestMethod]
        public void GetChanges_WithExpectedInputParameters_ReturnExpectedResults()
        {
            //Arrange
            var expectedResult0 = "Forename changed from 'David' to 'Janet'";
            var expectedResult1 = "Date Of Birth changed from '02/05/1990' to '22/11/2000'";
            var inputParametersA = new SomeTypeParameters()
            {
                Forename = "David",
                DateOfBirth = new DateTime(1990, 5, 2),
                SpecialNumber = 23
            };

            var inputParametersB = new SomeTypeParameters()
            {
                Forename = "Janet",
                DateOfBirth = new DateTime(2000, 11, 22),
                SpecialNumber = 23
            };


            //Act
            var result = comparisonService.GetChanges(inputParametersA, inputParametersB);

            //Assert
            Assert.AreEqual(2, result.Differences.Count);
            Assert.AreEqual(expectedResult0, result.Differences[0]);
            Assert.AreEqual(expectedResult1, result.Differences[1]);
        }
        #endregion


    }
}