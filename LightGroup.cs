using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SSMono.Collections.Generic;

namespace Daniels.Lighting
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LightGroup: LightFixture, IEnumerable<LightFixture>
    {
        [JsonProperty(PropertyName = "Lighting")]
        private List<LightFixture> _lightFixtures = new List<LightFixture>();

        //[JsonConstructor]
        public LightGroup(uint id, string name) : base(id, name) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine("\tLighting:");
            foreach (LightFixture lightFixture in _lightFixtures)
            {
                sb.AppendFormat("\t\t{0}: {1}\t {2}(set:{3:P2} eff:{4:P2})\r\n", lightFixture.Id, lightFixture.Name, lightFixture.Muted ? "OFF" : "ON", 1f * lightFixture.Intensity / ushort.MaxValue, 1f * lightFixture.EffectiveIntensity / ushort.MaxValue);
            }

            return sb.ToString();
        }

        public void AddLight(LightFixture lightFixture)
        {
            _lightFixtures.Add(lightFixture);
        }

        public override string SavePreset()
        {
            //return base.SavePreset();
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                //PreserveReferencesHandling = PreserveReferencesHandling.Arrays,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects
            });
        }

        public override void ApplyPreset(string preset)
        {
            var serializerSettings = new JsonSerializerSettings() 
            {
                ObjectCreationHandling = ObjectCreationHandling.Reuse,
                //ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.Objects,
            };
            var serializer = JsonSerializer.Create(serializerSettings);
            var converter = new KeyedListMergeConverter(serializer.ContractResolver);
            serializer.Converters.Add(converter);

            using (var reader = new JsonTextReader(new Crestron.SimplSharp.CrestronIO.StringReader(preset)))
            {
                try
                {
                    serializer.Populate(reader, this);
                }
                catch (Exception ex)
                {
                    CrestronConsole.PrintLine("Error:{0}\r\n{1}", ex.Message, ex.StackTrace);
                }
            }
        }

        #region Indexers
        
        public LightFixture this[int index]
        {
            get
            {
                return _lightFixtures[index];
            }
        }

        public LightFixture this[uint id]
        {
            get
            {
                foreach (var light in _lightFixtures)
                {
                    if (light.Id == id)
                        return light;
                }
                throw new IndexOutOfRangeException("id");
            }
        }

        public LightFixture this[string name]
        {
            get
            {
                foreach (var light in _lightFixtures)
                {
                    if (light.Name == name)
                        return light;
                }
                throw new IndexOutOfRangeException("name");
            }
        }

        /*
        public LightFixture[] Lights
        {
            get { return _lightFixtures.ToArray(); }
        }
        */

        #endregion Indexers

        #region IEnumerable<LightFixture> Implementation

        public IEnumerator<LightFixture> GetEnumerator()
        {
            return _lightFixtures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<LightFixture> Implementation
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonMergeKeyAttribute : System.Attribute
    {
    }

    public class KeyedListMergeConverter : JsonConverter
    {
        readonly IContractResolver contractResolver;

        public KeyedListMergeConverter(IContractResolver contractResolver)
        {
            if (contractResolver == null)
                throw new ArgumentNullException("contractResolver");
            this.contractResolver = contractResolver;
        }

        static bool CanConvert(IContractResolver contractResolver, Type objectType, out Type elementType, out JsonProperty keyProperty)
        {
            elementType = objectType.GetListType();
            if (elementType == null)
            {
                keyProperty = null;
                return false;
            }

            var contract = contractResolver.ResolveContract(elementType) as JsonObjectContract;
            if (contract == null)
            {
                keyProperty = null;
                return false;
            }
            //keyProperty = contract.Properties.Where(p => p.AttributeProvider.GetAttributes(typeof(JsonMergeKeyAttribute), true).Count > 0).SingleOrDefault();
            keyProperty = null;
            foreach (var p in contract.Properties)
            {
                if (p.PropertyName == "Id")
                    keyProperty = p;
            }
            return keyProperty != null;
        }

        public override bool CanConvert(Type objectType)
        {
            Type elementType;
            JsonProperty keyProperty;
            return CanConvert(contractResolver, objectType, out elementType, out keyProperty);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (contractResolver != serializer.ContractResolver)
                throw new InvalidOperationException("Inconsistent contract resolvers");
            Type elementType;
            JsonProperty keyProperty;
            if (!CanConvert(contractResolver, objectType, out elementType, out keyProperty))
                throw new JsonSerializationException(string.Format("Invalid input type {0}", objectType));

            if (reader.TokenType == JsonToken.Null)
                return existingValue;

            var list = existingValue as IList;
            if (list == null || list.Count == 0)
            {
                list = list ?? (IList)contractResolver.ResolveContract(objectType).DefaultCreator();
                serializer.Populate(reader, list);
            }
            else
            {
                var jArray = JArray.Load(reader);
                var comparer = new KeyedListMergeComparer();
                var lookup = jArray.ToLookup(i => i[keyProperty.PropertyName].ToObject(keyProperty.PropertyType, serializer), comparer);
                var done = new HashSet<JToken>();
                foreach (var item in list)
                {
                    var key = keyProperty.ValueProvider.GetValue(item);
                    var replacement = lookup[key].Where(v => !done.Contains(v)).FirstOrDefault();
                    if (replacement != null)
                    {
                        using (var subReader = replacement.CreateReader())
                            serializer.Populate(subReader, item);
                        done.Add(replacement);
                    }
                }
                // Populate the NEW items into the list.
                if (done.Count < jArray.Count)
                    foreach (var item in jArray.Where(i => !done.Contains(i)))
                    {
                        list.Add(item.ToObject(elementType, serializer));
                    }
            }
            return list;
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        class KeyedListMergeComparer : IEqualityComparer<object>
        {
            #region IEqualityComparer<object> Members

            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;
                else if (x == null || y == null)
                    return false;
                return x.Equals(y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                if (obj == null)
                    return 0;
                return obj.GetHashCode();
            }

            #endregion
        }
    }
    public static class TypeExtensions
    {
        public static Type GetListType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    var genType = type.GetGenericTypeDefinition();
                    if (genType == typeof(List<>))
                        return type.GetGenericArguments()[0];
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}