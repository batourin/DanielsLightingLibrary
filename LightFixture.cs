using System;
using System.Text;
using Newtonsoft.Json;
using Daniels.Common;

namespace Daniels.Lighting
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class LightFixture
    {
        private readonly uint _id;
        [JsonMergeKey]
        [JsonProperty]
        public uint Id { get { return _id; } }
        private readonly string _name;
        [JsonProperty]
        public string Name { get { return _name; } }
        protected readonly LightGroup _master;

        //[JsonConstructor]
        public LightFixture(uint id, string name):this(id, name, null) {}

        public LightFixture(uint id, string name, LightGroup master)
        {
            _id = id;
            _name = name;
            if (master != null)
            {
                _master = master;
                _master.AddLight(this);
                _master.IntensityChanged += new EventHandler<ReadOnlyEventArgs<ushort>>(masterIntensityChanged);
                _master.MuteChanged += new EventHandler<ReadOnlyEventArgs<bool>>(masterMuteChanged);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\tId: " + Id);
            sb.AppendLine("\tName: " + Name);
            sb.AppendLine("\tMuted: " + Muted);
            sb.AppendLine("\tIntensity: " + Intensity + " (" + (1f * Intensity / ushort.MaxValue).ToString("P2") + ")");
            if (_master != null)
            {
                sb.AppendLine("\tEffective Mute: " + EffectiveMute);
                sb.AppendLine("\tEffectiveIntensity: " + EffectiveIntensity + " (" + (1f * EffectiveIntensity / ushort.MaxValue).ToString("P2") + ")");
                sb.AppendLine("\tMaster: " + _master.Name);
            }
            return sb.ToString();
        }

        public event EventHandler<ReadOnlyEventArgs<bool>> MuteChanged;
        protected virtual void OnMuteChanged(ReadOnlyEventArgs<bool> e)
        {
            EventHandler<ReadOnlyEventArgs<bool>> handler = MuteChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private bool muted = true;
        [JsonProperty]
        public virtual bool Muted
        {
            get { return muted; }
            set
            {
                if (value != muted)
                {
                    bool oldEffectiveMute = EffectiveMute;
                    muted = value;
                    OnMuteChanged(new ReadOnlyEventArgs<bool>(value));
                    if(oldEffectiveMute != EffectiveMute)
                        OnEffectiveMuteChanged(new ReadOnlyEventArgs<bool>(EffectiveMute));
                }
            }
        }

        public event EventHandler<ReadOnlyEventArgs<ushort>> IntensityChanged;
        protected virtual void OnIntensityChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = IntensityChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private ushort intensity;
        [JsonProperty]
        public virtual ushort Intensity
        {
            get { return intensity; }
            set
            {
                ushort oldEffectiveIntensity = EffectiveIntensity;
                intensity = value;
                OnIntensityChanged(new ReadOnlyEventArgs<ushort>(value));
                if(oldEffectiveIntensity != EffectiveIntensity)
                    OnEffectiveIntensityChanged(new ReadOnlyEventArgs<ushort>(EffectiveIntensity));
            }
        }

        protected virtual void masterIntensityChanged(object sender, ReadOnlyEventArgs<ushort> e)
        {
            OnEffectiveIntensityChanged(new ReadOnlyEventArgs<ushort>(EffectiveIntensity));
        }

        protected virtual void masterMuteChanged(object sender, ReadOnlyEventArgs<bool> e)
        {
            OnEffectiveMuteChanged(new ReadOnlyEventArgs<bool>(EffectiveMute));
        }

        public event EventHandler<ReadOnlyEventArgs<bool>> EffectiveMuteChanged;
        protected virtual void OnEffectiveMuteChanged(ReadOnlyEventArgs<bool> e)
        {
            EventHandler<ReadOnlyEventArgs<bool>> handler = EffectiveMuteChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual bool EffectiveMute
        {
            get
            {
                return ((_master != null) ? _master.Muted : this.Muted) | this.Muted;
            }
        }

        public event EventHandler<ReadOnlyEventArgs<ushort>> EffectiveIntensityChanged;
        protected virtual void OnEffectiveIntensityChanged(ReadOnlyEventArgs<ushort> e)
        {
            EventHandler<ReadOnlyEventArgs<ushort>> handler = EffectiveIntensityChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual ushort EffectiveIntensity
        {
            get
            {
                if (_master != null)
                {
                    return (ushort)(this.Intensity*_master.Intensity/ushort.MaxValue);
                }
                else 
                    return this.Intensity; 
            }
        }

        public virtual string SavePreset()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public virtual void ApplyPreset(string preset)
        {
            JsonConvert.PopulateObject(preset, this, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Reuse});
        }
    }
}