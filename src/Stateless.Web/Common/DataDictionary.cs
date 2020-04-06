﻿namespace Stateless.Web
{
    using System;
    using System.Collections.Generic;

    public class DataDictionary : Dictionary<string, object>
    {
        public static readonly DataDictionary Empty = new DataDictionary();

        public DataDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public DataDictionary(IEnumerable<KeyValuePair<string, object>> items)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            foreach (var item in items.Safe())
            {
                if (!item.Key.IsNullOrEmpty())
                {
                    this.Add(item.Key, item.Value);
                }
            }
        }

        public DataDictionary(string key, object value)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (!key.IsNullOrEmpty())
            {
                this.Add(key, value);
            }
        }

        public DataDictionary AddOrUpdate(string key, object value)
        {
            if (key.IsNullOrEmpty())
            {
                return this;
            }

            if (this.ContainsKey(key))
            {
                this.Remove(key);
            }

            this.Add(key, value);

            return this;
        }

        public object GetValueOrDefault(string key)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            return this.TryGetValue(key, out var value) ? value : null;
        }

        public object GetValueOrDefault(string key, object defaultValue)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            return this.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public object GetValueOrDefault(string key, Func<object> defaultValueProvider)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            return this.TryGetValue(key, out var value) ? value : defaultValueProvider();
        }

        public T GetValue<T>(string key, bool throwIfKeyNotFound = false)
        {
            if (key.IsNullOrEmpty())
            {
                return default;
            }

            if (!this.ContainsKey(key) && throwIfKeyNotFound)
            {
                throw new KeyNotFoundException($"Key \"{key}\" not found in the dictionary.");
            }

            return this.GetValueOrDefault<T>(key);
        }

        public T GetValueOrDefault<T>(string key, T defaultValue = default)
        {
            if (!this.ContainsKey(key))
            {
                return defaultValue;
            }

            var value = this[key];
            if (value is T)
            {
                return (T)value;
            }

            if (value == null)
            {
                return defaultValue;
            }

            try
            {
                return value.ToType<T>();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // return default
            }

            return defaultValue;
        }

        public string GetStringValue(string name)
        {
            return this.GetStringValue(name, string.Empty);
        }

        public string GetStringValue(string name, string defaultValue)
        {
            if (!this.TryGetValue(name, out var value))
            {
                return defaultValue;
            }

            if (value is string)
            {
                return (string)value;
            }

            return string.Empty;
        }
    }
}