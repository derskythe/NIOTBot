// ***********************************************************************
// Assembly         : SettingsHelper
// Author           : Skif
// Created          : 06-28-2021
//
// Last Modified By : Skif
// Last Modified On : 06-28-2021
// ***********************************************************************
// <copyright file="JsonHelpers.cs" company="SettingsHelper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using SettingsHelper.Models;

namespace SettingsHelper
{
    /// <summary>
    /// Class JsonHelpers.
    /// </summary>
    internal static class JsonHelpers
    {
        /// <summary>
        /// Replaces the nested.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="path">The path.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">self</exception>
        /// <exception cref="ArgumentException">Path cannot be null or empty - path</exception>
        public static void ReplaceNested(this JsonElement self, string path, JsonElement value)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            OverwriteProperty(self, value.GetProperty(name));
        }

        /// <summary>
        /// Compares the json.
        /// </summary>
        /// <param name="sourceToken">The source token.</param>
        /// <param name="targetToken">The target token.</param>
        /// <param name="forcedList"></param>
        /// <returns>System.ValueTuple&lt;System.Boolean, List&lt;DetectedChanges&gt;&gt;.</returns>
        public static (bool IsEqual, List<DetectedChanges> Differences) CompareJson(this JsonElement sourceToken,
            JsonElement targetToken,
            List<string> forcedList)
        {
            if (sourceToken == null && targetToken == null)
            {
                return (false, new List<DetectedChanges>(0));
            }

            if (sourceToken == null)
            {
                return (false, new List<DetectedChanges>
                {
                    new DetectedChanges(
                        MissedSide.Source,
                        DifferenceType.Null
                    )
                });
            }

            if (targetToken == null)
            {
                return (false, new List<DetectedChanges>
                {
                    new DetectedChanges(
                        MissedSide.Target,
                        DifferenceType.Null
                    )
                });
            }

            var comparer = new JsonElementComparer();
            var differences = new List<DetectedChanges>();

            switch (sourceToken.ValueKind)
            {
                case JsonValueKind.Object:
                    {
                        var sourceObject = sourceToken.EnumerateObject().ToList();
                        var targetObject = targetToken.EnumerateObject().ToList();

                        if (sourceObject.Count == 0 && targetObject.Count == 0)
                        {
                            return (false, new List<DetectedChanges>(0));
                        }

                        if (sourceObject.Count == 0)
                        {
                            return (false, new List<DetectedChanges>
                            {
                                new DetectedChanges(
                                    MissedSide.Source,
                                    DifferenceType.Null
                                )
                            });
                        }

                        if (targetObject.Count == 0)
                        {
                            return (false, new List<DetectedChanges>
                            {
                                new DetectedChanges(
                                    MissedSide.Target,
                                    DifferenceType.Null
                                )
                            });
                        }

                        var addedKeys = sourceObject.Select(c => c.Name)
                                                    .Except(
                                                        targetObject.Select(c => c.Name)
                                                    );
                        var removedKeys = targetObject.Select(c => c.Name).Except(sourceObject.Select(c => c.Name));
                        var unchangedKeys = sourceObject
                                                   .Where(c => comparer.Equals(c.Value, targetToken.GetProperty(c.Name)))
                                                   .Select(c => c.Name);
                        var calculated = addedKeys as string[] ?? addedKeys.ToArray();
                        foreach (var token in calculated)
                        {
                            if (token.IsEqual("_forced"))
                            {
                                continue;
                            }
                            differences.Add(new DetectedChanges(
                                    MissedSide.Target,
                                    sourceToken[token]
                                )
                            );
                        }

                        foreach (var token in removedKeys)
                        {
                            differences.Add(
                                new DetectedChanges(
                                    MissedSide.Source,
                                    targetToken[token]
                                )
                            );
                        }

                        var potentiallyModifiedKeys = sourceObject.Select(c => c.Name)
                                                                  .Except(calculated)
                                                                  .Except(unchangedKeys);
                        foreach (var k in potentiallyModifiedKeys)
                        {
                            var foundDiff = CompareJson(sourceObject[k], targetObject[k], forcedList);
                            if (!foundDiff.IsEqual)
                            {
                                differences.AddRange(foundDiff.Differences);
                            }
                        }
                    }
                    break;
                case JsonValueKind.Array:
                    {
                        var sourceArray = sourceToken.EnumerateArray().ToList();
                        var targetArray = targetToken.EnumerateArray().ToList();
                        if (sourceArray.Count == 0 && targetArray.Count == 0)
                        {
                            return (false, new List<DetectedChanges>(0));
                        }

                        if (sourceArray.Count == 0)
                        {
                            return (false, new List<DetectedChanges>
                        {
                            new DetectedChanges(MissedSide.Source, DifferenceType.Null)
                        });
                        }

                        if (targetArray.Count == 0)
                        {
                            return (false, new List<DetectedChanges>
                            {
                                new DetectedChanges(MissedSide.Target, DifferenceType.Null),
                            });
                        }

                        var plus = new JsonArray(sourceArray.Except(targetArray, new JsonElementComparer()));
                        var minus = new JsonArray(targetArray.Except(sourceArray, new JsonElementComparer()));
                        if (plus.Count > 0)
                        {
                            foreach (var token in plus)
                            {
                                differences.Add(new DetectedChanges(MissedSide.Target, token));
                            }
                        }

                        if (minus.Count > 0)
                        {
                            foreach (var token in minus)
                            {
                                differences.Add(new DetectedChanges(MissedSide.Source, token));
                            }
                        }
                    }
                    break;
                default:
                    var (sourceHasValue, sourceValue) = (sourceToken).HasValueTuple();
                    var (targetHasValue, targetValue) = (targetToken).HasValueTuple();
                    if (sourceHasValue && targetHasValue && sourceValue.IsEqual(targetValue))
                    {
                        return (true, new List<DetectedChanges>(0));
                    }
                    if (forcedList.Contains(sourceToken.Name))
                    {
                        return (false, new List<DetectedChanges>
                        {
                            new DetectedChanges(DifferenceType.ForcedChange,sourceToken.Name, sourceValue, targetValue)
                        });
                    }
                    else
                    {
                        return (
                            false,
                            new List<DetectedChanges>
                            {
                                new DetectedChanges(sourceValue, sourceToken.Name, targetValue, sourceHasValue)
                            }
                        );
                    }
            }

            return (false, differences);
        }

        public static (bool hasValue, string value) HasValueTuple(this JsonProperty token)
        {
            if (token.Value != null)
            {
                return (true, token.Value.ToString());
            }

            return (false, string.Empty);
        }

        public static (bool hasValue, string value) HasValueTuple(this JsonElement token)
        {
            if (token.NodeHasValue())
            {
                return (true, token.Value<string>());
            }

            return (false, string.Empty);
        }

        public static bool NodeHasValue(this JsonElement token)
        {
            return (token.ValueKind == JsonValueKind.String || token.ValueKind == JsonValueKind.Number || token.ValueKind == JsonValueKind.False || token.ValueKind == JsonValueKind.True)
        }

        public static void PopulateObject<T>(T target, JsonElement jsonSource) where T : class =>
        PopulateObject(target, jsonSource, typeof(T));

        public static void OverwriteProperty<T>(T target, JsonProperty updatedProperty) where T : class =>
            OverwriteProperty(target, updatedProperty, typeof(T));

        private static void PopulateObject(object target, JsonElement jsonSource, Type type)
        {
            using var json = JsonDocument.Parse(jsonSource).RootElement;

            foreach (var property in json.EnumerateObject())
            {
                OverwriteProperty(target, property, type);
            }
        }

        private static void PopulateCollection(object target, JsonElement json, Type elementType)
        {
            var addMethod = target.GetType().GetMethod("Add", new[] { elementType });
            var containsMethod = target.GetType().GetMethod("Contains", new[] { elementType });

            Debug.Assert(addMethod is not null);
            Debug.Assert(containsMethod is not null);

            foreach (var property in json.EnumerateArray())
            {
                object? element;

                if (elementType.IsValueType || elementType == typeof(string))
                {
                    element = JsonSerializer.Deserialize(jsonSource, elementType);
                }
                else if (IsCollection(elementType))
                {
                    var nestedElementType = elementType.GenericTypeArguments[0];
                    element = Instantiate(elementType);

                    PopulateCollection(element, property.GetRawText(), nestedElementType);
                }
                else
                {
                    element = Instantiate(elementType);

                    PopulateObject(element, property.GetRawText(), elementType);
                }

                var contains = containsMethod.Invoke(target, new[] { element });
                if (contains is false)
                {
                    addMethod.Invoke(target, new[] { element });
                }
            }
        }

        private static void OverwriteProperty(object target, JsonProperty updatedProperty, Type type)
        {
            var propertyInfo = type.GetProperty(updatedProperty.Name);

            if (propertyInfo == null)
            {
                return;
            }

            if (updatedProperty.Value.ValueKind == JsonValueKind.Null)
            {
                propertyInfo.SetValue(target, null);
                return;
            }

            var propertyType = propertyInfo.PropertyType;
            object? parsedValue;

            if (propertyType.IsValueType || propertyType == typeof(string))
            {
                parsedValue = JsonSerializer.Deserialize(
                    updatedProperty.Value.GetRawText(),
                    propertyType);
            }
            else if (IsCollection(propertyType))
            {
                var elementType = propertyType.GenericTypeArguments[0];
                parsedValue = propertyInfo.GetValue(target);
                parsedValue ??= Instantiate(propertyType);

                PopulateCollection(parsedValue, updatedProperty.Value.GetRawText(), elementType);
            }
            else
            {
                parsedValue = propertyInfo.GetValue(target);
                parsedValue ??= Instantiate(propertyType);

                PopulateObject(
                    parsedValue,
                    updatedProperty.Value.GetRawText(),
                    propertyType);
            }

            propertyInfo.SetValue(target, parsedValue);
        }

        private static object Instantiate(Type type)
        {
            var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Array.Empty<Type>());

            if (ctor is null)
            {
                throw new InvalidOperationException($"Type {type.Name} has no parameterless constructor.");
            }

            return ctor.Invoke(Array.Empty<object?>());
        }

        private static bool IsCollection(Type type) =>
            type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

    }
}
