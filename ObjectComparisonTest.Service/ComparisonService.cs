using ObjectComparisonTest.Service.Interface;
using ObjectComparisonTest.Service.ObjectResponse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectComparisonTest.Service
{
    public class ComparisonService : IComparisonService
    {
        /// <summary>
        /// Main get changes function to compare 2 objects and return any differences.
        /// </summary>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <returns></returns>
        public ComparisonResponse GetChanges(object objectA, object objectB)
        {
            var comparisonResponse = new ComparisonResponse();

            try
            {
                comparisonResponse.Differences = new List<string>();
                var errorMessage = "";
                if (objectA != null && objectB != null)
                {
                    Type objectType = objectA.GetType();

                    foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead))
                    {
                        var valueA = propertyInfo.GetValue(objectA, null);
                        var valueB = propertyInfo.GetValue(objectB, null);

                        if (CanDirectlyCompare(propertyInfo.PropertyType))
                        {
                            AddErrorMessageForInputObjectsAreNotEqual(
                                comparisonResponse,
                                ref errorMessage,
                                propertyInfo,
                                ref valueA,
                                ref valueB);
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            errorMessage = StartProcessingOfInputObjects(
                                comparisonResponse,
                                errorMessage,
                                objectType,
                                propertyInfo,
                                valueA,
                                valueB);
                        }
                    }
                }
            }
            catch{ throw; }
            return comparisonResponse;
        }

        /// <summary>
        /// Add error messages where the objects are not equal
        /// </summary>
        /// <param name="comparisonResponse"></param>
        /// <param name="errorMessage"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        private void AddErrorMessageForInputObjectsAreNotEqual(
            ComparisonResponse comparisonResponse,
            ref string errorMessage,
            PropertyInfo propertyInfo,
            ref object valueA,
            ref object valueB)
        {
            try
            {
                if (!AreValuesEqual(valueA, valueB))
                {
                    var propertyName = "";
                    ProcessDateTimeType(ref valueA, ref valueB);
                    propertyName = ProcessDateOfBirthType(propertyInfo, propertyName);
                    errorMessage = string.Format("{0} changed from '{1}' to '{2}'", propertyName, valueA, valueB);
                    comparisonResponse.Differences.Add(errorMessage);
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// ProcessInput objects but checking for null entries.
        /// </summary>
        /// <param name="comparisonResponse"></param>
        /// <param name="errorMessage"></param>
        /// <param name="objectType"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <returns></returns>
        private string StartProcessingOfInputObjects(
            ComparisonResponse comparisonResponse,
            string errorMessage,
            Type objectType,
            PropertyInfo propertyInfo,
            object valueA,
            object valueB)
        {
            try
            {
                if (valueA == null && valueB != null || valueA != null && valueB == null)
                {
                    comparisonResponse.Differences.Add("The items are not the same");
                }
                else if (valueA != null && valueB != null)
                {
                    var collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                    var collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                    var collectionItemsCount1 = collectionItems1.Count();
                    var collectionItemsCount2 = collectionItems2.Count();

                    if (collectionItemsCount1 != collectionItemsCount2)
                    {
                        errorMessage = string.Format("Collection counts for property '{0}.{1}' do not match.", objectType.FullName, propertyInfo.Name);
                        comparisonResponse.Differences.Add(errorMessage);
                    }
                    else
                    {
                        errorMessage = CheckCollectionItemsAreEqual(
                            comparisonResponse,
                            errorMessage,
                            propertyInfo,
                            collectionItems1,
                            collectionItems2,
                            collectionItemsCount1);
                    }
                }
            }
            catch { throw; }
            return errorMessage;
        }

        /// <summary>
        /// Loops through Collections to check all items contained.
        /// </summary>
        /// <param name="comparisonResponse"></param>
        /// <param name="errorMessage"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="collectionItems1"></param>
        /// <param name="collectionItems2"></param>
        /// <param name="collectionItemsCount1"></param>
        /// <returns></returns>
        private string CheckCollectionItemsAreEqual(
            ComparisonResponse comparisonResponse,
            string errorMessage,
            PropertyInfo propertyInfo,
            IEnumerable<object> collectionItems1,
            IEnumerable<object> collectionItems2,
            int collectionItemsCount1)
        {
            try
            {
                for (int i = 0; i < collectionItemsCount1; i++)
                {
                    var collectionItem1 = collectionItems1.ElementAt(i);
                    var collectionItem2 = collectionItems2.ElementAt(i);
                    var collectionItemType = collectionItem1.GetType();

                    if (CanDirectlyCompare(collectionItemType))
                    {
                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                        {
                            errorMessage = string.Format("{0} changed from '{1}' to '{2}'", propertyInfo.Name, collectionItem1, collectionItem2);
                            comparisonResponse.Differences.Add(errorMessage);
                        }
                    }
                }
            }
            catch { throw; }
            return errorMessage;
        }

        /// <summary>
        /// Checks for type of Date of Birth
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string ProcessDateOfBirthType(PropertyInfo propertyInfo, string propertyName)
        {
            try
            {
                if (propertyInfo.Name.ToLower() == "dateofbirth")
                {
                    propertyName = "Date Of Birth";
                }
                else
                {
                    propertyName = propertyInfo.Name;
                }
            }
            catch { throw; }
            return propertyName;
        }

        /// <summary>
        /// Checks for type of DateTime
        /// </summary>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        private static void ProcessDateTimeType(ref object valueA, ref object valueB)
        {
            try
            {
                if (valueA.GetType() == typeof(DateTime))
                {
                    valueA = valueA.ToString().Substring(0, 10);
                    valueB = valueB.ToString().Substring(0, 10);
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Check if the type can be compared
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanDirectlyCompare(Type type)
        {
            try
            {
                return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
            }
            catch { throw; }
            return false;
        }

        /// <summary>
        /// check if objects are equal
        /// </summary>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <returns></returns>
        public bool AreValuesEqual(object valueA, object valueB)
        {
            try
            {
                var selfValueComparer = valueA as IComparable;

                if (valueA == null && valueB != null || valueA != null && valueB == null)
                    return false;
                else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                    return false;
                else if (!object.Equals(valueA, valueB))
                    return false;
                else
                    return true;
            }
            catch { throw; }
            return false;
        }
    }
}