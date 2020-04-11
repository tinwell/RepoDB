﻿using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A class used to cache the mapped-name of the property.
    /// </summary>
    public static class PropertyMappedNameCache
    {
        private static readonly ConcurrentDictionary<int, string> m_cache = new ConcurrentDictionary<int, string>();

        #region Methods

        /// <summary>
        /// Gets the cached mapped-name of the property (via expression).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get<T>(Expression<Func<T, object>> expression)
            where T : class =>
            Get(ExpressionExtension.GetProperty<T>(expression));

        /// <summary>
        /// Gets the cached mapped-name of the property (via property name).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get<T>(string propertyName)
            where T : class =>
            Get(TypeExtension.GetProperty<T>(propertyName));

        /// <summary>
        /// Gets the cached mapped-name of the property (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="field">The instance of <see cref="Field"/> object.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get<T>(Field field)
            where T : class =>
            Get(TypeExtension.GetProperty<T>(field.Name));

        /// <summary>
        /// Gets the cached mapped-name of the <see cref="ClassProperty"/> object.
        /// </summary>
        /// <param name="classProperty">The instance of <see cref="ClassProperty"/>.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(ClassProperty classProperty) =>
            Get(classProperty.PropertyInfo);

        /// <summary>
        /// Gets the cached mapped-name of the property.
        /// </summary>
        /// <param name="propertyInfo">The target property.</param>
        /// <returns>The cached mapped-name of the property.</returns>
        public static string Get(PropertyInfo propertyInfo)
        {
            // Validate
            ThrowNullReferenceException(propertyInfo, "PropertyInfo");

            // Variables
            var key = propertyInfo.GenerateCustomizedHashCode();
            var result = (string)null;

            // Try get the value
            if (m_cache.TryGetValue(key, out result) == false)
            {
                result = PropertyInfoExtension.GetMappedName(propertyInfo);
                m_cache.TryAdd(key, result);
            }

            // Return the value
            return result;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Flushes all the existing cached property mapped names.
        /// </summary>
        public static void Flush()
        {
            m_cache.Clear();
        }

        /// <summary>
        /// Validates the target object presence.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be checked.</param>
        /// <param name="argument">The name of the argument.</param>
        private static void ThrowNullReferenceException<T>(T obj,
            string argument)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"The argument '{argument}' cannot be null.");
            }
        }

        #endregion
    }
}
