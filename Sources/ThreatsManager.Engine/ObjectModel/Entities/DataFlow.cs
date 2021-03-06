﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PostSharp.Patterns.Contracts;
using ThreatsManager.Engine.Aspects;
using ThreatsManager.Interfaces;
using ThreatsManager.Interfaces.ObjectModel;
using ThreatsManager.Interfaces.ObjectModel.Entities;
using ThreatsManager.Interfaces.ObjectModel.Properties;
using ThreatsManager.Interfaces.ObjectModel.ThreatsMitigations;
using ThreatsManager.Utilities;
using ThreatsManager.Utilities.Aspects;
using ThreatsManager.Utilities.Aspects.Engine;

namespace ThreatsManager.Engine.ObjectModel.Entities
{
#pragma warning disable CS0067
    [JsonObject(MemberSerialization.OptIn)]
    [SimpleNotifyPropertyChanged]
    [AutoDirty]
    [Serializable]
    [IdentityAspect]
    [PropertiesContainerAspect]
    [ThreatModelChildAspect]
    [ThreatEventsContainerAspect]
    [TypeLabel("Flow")]
    [TypeInitial("F")]
    public class DataFlow : IDataFlow, IInitializableObject
    {
        public DataFlow()
        {
            
        }

        public DataFlow([NotNull] IThreatModel model, [Required] string name, Guid sourceId, Guid targetId) : this()
        {
            _modelId = model.Id;
            _model = model;
            _id = Guid.NewGuid();
            Name = name;
            _sourceId = sourceId;
            _targetId = targetId;
            FlowType = FlowType.ReadWriteCommand;
  
            model.AutoApplySchemas(this);
        }

        public DataFlow([NotNull] IThreatModel model, [Required] string name, [NotNull] IEntity source, 
            [NotNull] IEntity target) : this(model, name, source.Id, target.Id)
        {
        }

        public bool IsInitialized => Model != null && _id != Guid.Empty;

        #region Default implementation.
        public Guid Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public event Action<IPropertiesContainer, IProperty> PropertyAdded;
        public event Action<IPropertiesContainer, IProperty> PropertyRemoved;
        public event Action<IPropertiesContainer, IProperty> PropertyValueChanged;
        public IEnumerable<IProperty> Properties { get; }
        public bool HasProperty(IPropertyType propertyType)
        {
            return false;
        }
        public IProperty GetProperty(IPropertyType propertyType)
        {
            return null;
        }

        public IProperty AddProperty(IPropertyType propertyType, string value)
        {
            return null;
        }

        public bool RemoveProperty(IPropertyType propertyType)
        {
            return false;
        }

        public bool RemoveProperty(Guid propertyTypeId)
        {
            return false;
        }

        public event Action<IThreatEventsContainer, IThreatEvent> ThreatEventAdded;
        public event Action<IThreatEventsContainer, IThreatEvent> ThreatEventRemoved;

        public IEnumerable<IThreatEvent> ThreatEvents { get; }
        public IThreatEvent GetThreatEvent(Guid id)
        {
            return null;
        }

        public IThreatEvent GetThreatEventByThreatType(Guid threatTypeId)
        {
            return null;
        }

        public IThreatEvent AddThreatEvent(IThreatType threatType)
        {
            return null;
        }

        public bool RemoveThreatEvent(Guid id)
        {
            return false;
        }

        public IThreatModel Model { get; }
        #endregion

        #region Additional placeholders required.
        protected Guid _id { get; set; }
        private IPropertiesContainer PropertiesContainer => this;
        private List<IProperty> _properties { get; set; }
        private List<IThreatEvent> _threatEvents { get; set; }
        private Guid _modelId { get; set; }
        private IThreatModel _model { get; set; }
        private IThreatEventsContainer ThreatEventsContainer => this;
        #endregion    

        #region Specific implementation.
        [JsonProperty("source")]
        private Guid _sourceId;

        public Guid SourceId => _sourceId;

        [JsonProperty("target")]
        private Guid _targetId;

        public Guid TargetId => _targetId;

        [InitializationRequired]
        public IEntity Source => Model.GetEntity(_sourceId);

        [InitializationRequired]
        public IEntity Target => Model.GetEntity(_targetId);

        [JsonProperty("flowType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FlowType FlowType { get; set; }

        public IDataFlow Clone([NotNull] IDataFlowsContainer container)
        {
            DataFlow result = null;

            if (container is IThreatModel model)
            {
                result = new DataFlow()
                {
                    _id = Id,
                    Name = Name,
                    Description = Description,
                    _model = model,
                    _modelId = model.Id,
                    _sourceId = _sourceId,
                    _targetId = _targetId,
                    FlowType = FlowType
                };
                this.CloneProperties(result);
                this.CloneThreatEvents(result);

                container.Add(result);
            }

            return result;
        }

        [InitializationRequired]
        public void Add([NotNull] IThreatEvent threatEvent)
        {
            if (_threatEvents == null)
                _threatEvents = new List<IThreatEvent>();

            _threatEvents.Add(threatEvent);
        }

        public override string ToString()
        {
            return Name ?? "<undefined>";
        }
        #endregion      
    }
}
