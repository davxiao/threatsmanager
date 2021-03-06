﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using PostSharp.Patterns.Contracts;
using ThreatsManager.Engine.Aspects;
using ThreatsManager.Interfaces;
using ThreatsManager.Interfaces.Exceptions;
using ThreatsManager.Interfaces.ObjectModel;
using ThreatsManager.Interfaces.ObjectModel.Diagrams;
using ThreatsManager.Interfaces.ObjectModel.Entities;
using ThreatsManager.Interfaces.ObjectModel.Properties;
using ThreatsManager.Interfaces.ObjectModel.ThreatsMitigations;
using ThreatsManager.Utilities;
using ThreatsManager.Utilities.Aspects;
using ThreatsManager.Utilities.Aspects.Engine;

namespace ThreatsManager.Engine.ObjectModel
{
#pragma warning disable CS0067
    [JsonObject(MemberSerialization.OptIn)]
    [SimpleNotifyPropertyChanged]
    [AutoDirty]
    [Serializable]
    [IdentityAspect]
    [PropertiesContainerAspect]
    [ThreatEventsContainerAspect]
    [TypeLabel("Threat Model")]
    [TypeInitial("M")]
    public partial class ThreatModel : IThreatModel, IInitializableObject, IDisposable
    {
        #region Events management.
        public event Action<IIdentity> ChildCreated;
        public event Action<IIdentity> ChildRemoved;
        public event Action<IIdentity, string> ChildChanged;
        public event Action<IIdentity, IPropertyType, IProperty> ChildPropertyAdded;
        public event Action<IIdentity, IPropertyType, IProperty> ChildPropertyRemoved;
        public event Action<IIdentity, IPropertyType, IProperty> ChildPropertyChanged;
        public event Action<string> ContributorAdded;
        public event Action<string> ContributorRemoved;
        public event Action<string, string> ContributorChanged;
        public event Action<string> AssumptionAdded;
        public event Action<string> AssumptionRemoved;
        public event Action<string, string> AssumptionChanged;
        public event Action<string> DependencyAdded;
        public event Action<string> DependencyRemoved;
        public event Action<string, string> DependencyChanged;

        private void RegisterEvents()
        {
            PropertyAdded += OnPropertyAdded;
            PropertyRemoved += OnPropertyRemoved;
            PropertyValueChanged += OnPropertyValueChanged;

            if (_actors?.Any() ?? false)
            {
                foreach (var actor in _actors)
                {
                    RegisterEvents(actor);
                }
            }
 
            if (_dataFlows?.Any() ?? false)
            {
                foreach (var dataFlow in _dataFlows)
                {
                    RegisterEvents(dataFlow);
                }
            }           

            if (_diagrams?.Any() ?? false)
            {
                foreach (var diagram in _diagrams)
                {
                    RegisterEvents(diagram);
                }
            }

            if (_entities?.Any() ?? false)
            {
                foreach (var entity in _entities)
                {
                    RegisterEvents(entity);
                }
            }

            if (_groups?.Any() ?? false)
            {
                foreach (var group in _groups)
                {
                    RegisterEvents(group);
                }
            }

            if (_mitigations?.Any() ?? false)
            {
                foreach (var mitigation in _mitigations)
                {
                    RegisterEvents(mitigation);
                }
            }

            if (_schemas?.Any() ?? false)
            {
                foreach (var schema in _schemas)
                {
                    RegisterEvents(schema);
                }
            }

            if (_severities?.Any() ?? false)
            {
                foreach (var severity in _severities)
                {
                    RegisterEvents(severity);
                }
            }

            if (_threatTypes?.Any() ?? false)
            {
                foreach (var threatType in _threatTypes)
                {
                    RegisterEvents(threatType);
                }
            }
        }

        private void UnregisterEvents()
        {
            PropertyAdded -= OnPropertyAdded;
            PropertyRemoved -= OnPropertyRemoved;
            PropertyValueChanged -= OnPropertyValueChanged;

            if (_actors?.Any() ?? false)
            {
                foreach (var actor in _actors)
                {
                    UnregisterEvents(actor);
                }
            }
 
            if (_dataFlows?.Any() ?? false)
            {
                foreach (var dataFlow in _dataFlows)
                {
                    UnregisterEvents(dataFlow);
                }
            }           

            if (_diagrams?.Any() ?? false)
            {
                foreach (var diagram in _diagrams)
                {
                    UnregisterEvents(diagram);
                }
            }

            if (_entities?.Any() ?? false)
            {
                foreach (var entity in _entities)
                {
                    UnregisterEvents(entity);
                }
            }

            if (_groups?.Any() ?? false)
            {
                foreach (var group in _groups)
                {
                    UnregisterEvents(group);
                }
            }

            if (_mitigations?.Any() ?? false)
            {
                foreach (var mitigation in _mitigations)
                {
                    UnregisterEvents(mitigation);
                }
            }

            if (_schemas?.Any() ?? false)
            {
                foreach (var schema in _schemas)
                {
                    UnregisterEvents(schema);
                }
            }

            if (_severities?.Any() ?? false)
            {
                foreach (var severity in _severities)
                {
                    UnregisterEvents(severity);
                }
            }

            if (_threatTypes?.Any() ?? false)
            {
                foreach (var threatType in _threatTypes)
                {
                    UnregisterEvents(threatType);
                }
            }
        }

        private void RegisterEvents([NotNull] IEntity entity)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) entity).PropertyChanged += OnPropertyChanged;
            entity.PropertyAdded += OnPropertyAdded;
            entity.PropertyRemoved += OnPropertyRemoved;
            entity.PropertyValueChanged += OnPropertyValueChanged;
            entity.ThreatEventAdded += OnThreatEventAddedToEntity;
            entity.ThreatEventRemoved += OnThreatEventRemovedFromEntity;
        }

        private void UnregisterEvents([NotNull] IEntity entity)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) entity).PropertyChanged -= OnPropertyChanged;
            entity.PropertyAdded -= OnPropertyAdded;
            entity.PropertyRemoved -= OnPropertyRemoved;
            entity.PropertyValueChanged -= OnPropertyValueChanged;
            entity.ThreatEventAdded -= OnThreatEventAddedToEntity;
            entity.ThreatEventRemoved -= OnThreatEventRemovedFromEntity;
        }

        private void RegisterEvents([NotNull] IGroup group)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) group).PropertyChanged += OnPropertyChanged;
            group.PropertyAdded += OnPropertyAdded;
            group.PropertyRemoved += OnPropertyRemoved;
            group.PropertyValueChanged += OnPropertyValueChanged;
        }

        private void UnregisterEvents([NotNull] IGroup group)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) group).PropertyChanged -= OnPropertyChanged;
            group.PropertyAdded -= OnPropertyAdded;
            group.PropertyRemoved -= OnPropertyRemoved;
            group.PropertyValueChanged -= OnPropertyValueChanged;
        }

        private void RegisterEvents([NotNull] IDataFlow dataFlow)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) dataFlow).PropertyChanged += OnPropertyChanged;
            dataFlow.PropertyAdded += OnPropertyAdded;
            dataFlow.PropertyRemoved += OnPropertyRemoved;
            dataFlow.PropertyValueChanged += OnPropertyValueChanged;
            dataFlow.ThreatEventAdded += OnThreatEventAddedToDataFlow;
            dataFlow.ThreatEventRemoved += OnThreatEventRemovedFromDataFlow;
        }

        private void UnregisterEvents([NotNull] IDataFlow dataFlow)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) dataFlow).PropertyChanged -= OnPropertyChanged;
            dataFlow.PropertyAdded -= OnPropertyAdded;
            dataFlow.PropertyRemoved -= OnPropertyRemoved;
            dataFlow.PropertyValueChanged -= OnPropertyValueChanged;
            dataFlow.ThreatEventAdded -= OnThreatEventAddedToDataFlow;
            dataFlow.ThreatEventRemoved -= OnThreatEventRemovedFromDataFlow;
        }

        private void RegisterEvents([NotNull] IPropertySchema schema)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) schema).PropertyChanged += OnPropertyChanged;
        }

        private void UnregisterEvents([NotNull] IPropertySchema schema)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) schema).PropertyChanged -= OnPropertyChanged;
        }

        private void RegisterEvents([NotNull] IDiagram diagram)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) diagram).PropertyChanged += OnPropertyChanged;
            diagram.PropertyAdded += OnPropertyAdded;
            diagram.PropertyRemoved += OnPropertyRemoved;
            diagram.PropertyValueChanged += OnPropertyValueChanged;
            diagram.EntityShapeAdded += OnEntityShapeAdded;
            diagram.EntityShapeRemoved += OnEntityShapeRemoved;
            diagram.GroupShapeAdded += OnGroupShapeAdded;
            diagram.GroupShapeRemoved += OnGroupShapeRemoved;
            diagram.LinkAdded += OnLinkAdded;
            diagram.LinkRemoved += OnLinkRemoved;
        }

        private void UnregisterEvents([NotNull] IDiagram diagram)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) diagram).PropertyChanged -= OnPropertyChanged;
            diagram.PropertyAdded -= OnPropertyAdded;
            diagram.PropertyRemoved -= OnPropertyRemoved;
            diagram.PropertyValueChanged -= OnPropertyValueChanged;
            diagram.EntityShapeAdded -= OnEntityShapeAdded;
            diagram.EntityShapeRemoved -= OnEntityShapeRemoved;
            diagram.GroupShapeAdded -= OnGroupShapeAdded;
            diagram.GroupShapeRemoved -= OnGroupShapeRemoved;
            diagram.LinkAdded -= OnLinkAdded;
            diagram.LinkRemoved -= OnLinkRemoved;
        }

        private void RegisterEvents([NotNull] ISeverity severity)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) severity).PropertyChanged += OnPropertyChanged;
            severity.PropertyAdded += OnPropertyAdded;
            severity.PropertyRemoved += OnPropertyRemoved;
            severity.PropertyValueChanged += OnPropertyValueChanged;
        }

        private void UnregisterEvents([NotNull] ISeverity severity)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) severity).PropertyChanged -= OnPropertyChanged;
            severity.PropertyAdded -= OnPropertyAdded;
            severity.PropertyRemoved -= OnPropertyRemoved;
            severity.PropertyValueChanged -= OnPropertyValueChanged;
        }

        private void RegisterEvents([NotNull] IThreatType threatType)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) threatType).PropertyChanged += OnPropertyChanged;
            threatType.PropertyAdded += OnPropertyAdded;
            threatType.PropertyRemoved += OnPropertyRemoved;
            threatType.PropertyValueChanged += OnPropertyValueChanged;
        }

        private void UnregisterEvents([NotNull] IThreatType threatType)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) threatType).PropertyChanged -= OnPropertyChanged;
            threatType.PropertyAdded -= OnPropertyAdded;
            threatType.PropertyRemoved -= OnPropertyRemoved;
            threatType.PropertyValueChanged -= OnPropertyValueChanged;
        }

        private void RegisterEvents([NotNull] IMitigation mitigation)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) mitigation).PropertyChanged += OnPropertyChanged;
            mitigation.PropertyAdded += OnPropertyAdded;
            mitigation.PropertyRemoved += OnPropertyRemoved;
            mitigation.PropertyValueChanged += OnPropertyValueChanged;
        }

        private void UnregisterEvents([NotNull] IMitigation mitigation)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) mitigation).PropertyChanged -= OnPropertyChanged;
            mitigation.PropertyAdded -= OnPropertyAdded;
            mitigation.PropertyRemoved -= OnPropertyRemoved;
            mitigation.PropertyValueChanged -= OnPropertyValueChanged;
        }

        private void RegisterEvents([NotNull] IThreatActor actor)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) actor).PropertyChanged += OnPropertyChanged;
            actor.PropertyAdded += OnPropertyAdded;
            actor.PropertyRemoved += OnPropertyRemoved;
            actor.PropertyValueChanged += OnPropertyValueChanged;
        }

        private void UnregisterEvents([NotNull] IThreatActor actor)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            ((INotifyPropertyChanged) actor).PropertyChanged -= OnPropertyChanged;
            actor.PropertyAdded -= OnPropertyAdded;
            actor.PropertyRemoved -= OnPropertyRemoved;
            actor.PropertyValueChanged -= OnPropertyValueChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is IIdentity identity)
            {
                ChildChanged?.Invoke(identity, e.PropertyName);
            }
        }

        private void OnPropertyAdded([NotNull] IPropertiesContainer container, [NotNull] IProperty property)
        {
            if (container is IIdentity identity)
            {
                ChildPropertyAdded?.Invoke(identity, property.PropertyType, property);
            }
        }

        private void OnPropertyRemoved([NotNull] IPropertiesContainer container, [NotNull] IProperty property)
        {
            if (container is IIdentity identity)
            {
                ChildPropertyRemoved?.Invoke(identity, property.PropertyType, property);
            }
        }

        private void OnPropertyValueChanged([NotNull] IPropertiesContainer container, [NotNull] IProperty property)
        {
            if (container is IIdentity identity)
            {
                ChildPropertyChanged?.Invoke(identity, property.PropertyType, property);
            }
        }
        #endregion

        #region Constructors.
        public ThreatModel()
        {
            _lastMitigation = 0;
            _lastDataStore = 0;
            _lastDiagram = 0;
            _lastExternalInteractor = 0;
            _lastGroup = 0;
            _lastProcess = 0;
            _lastTrustBoundary = 0;
        }

        public ThreatModel([Required] string name) : this()
        {
            _id = Guid.NewGuid();
            Name = name;
        }
        #endregion

        public bool IsInitialized => Id != Guid.Empty;

        #region General properties and methods.
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("contributors")]
        private List<string> _contributors;

        public IEnumerable<string> Contributors => _contributors?.AsReadOnly();

        public bool AddContributor([Required] string name)
        {
            bool result = false;

            if (!(_contributors?.Any(name.IsEqual) ?? false))
            {
                if (_contributors == null)
                    _contributors = new List<string>();
                _contributors.Add(name);
                Dirty.IsDirty = true;
                result = true;
                ContributorAdded?.Invoke(name);
            }

            return result;
        }

        public bool RemoveContributor([Required] string name)
        {
            bool result = false;

            if (_contributors?.Any(name.IsEqual) ?? false)
            {
                result = _contributors.Remove(name);
                if (result)
                {
                    Dirty.IsDirty = true;
                    ContributorRemoved?.Invoke(name);
                }
            }

            return result;
        }

        public bool ChangeContributor([Required] string oldName, [Required] string newName)
        {
            bool result = false;

            int index = _contributors?.IndexOf(oldName) ?? -1;
            if (index >= 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                _contributors[index] = newName;
                Dirty.IsDirty = true;
                result = true;
                ContributorChanged?.Invoke(oldName, newName);
            }

            return result;
        }

        [JsonProperty("assumptions")]
        private List<string> _assumptions;

        public IEnumerable<string> Assumptions => _assumptions?.AsReadOnly();

        public bool AddAssumption([Required] string text)
        {
            bool result = false;

            if (!(_assumptions?.Any(text.IsEqual) ?? false))
            {
                if (_assumptions == null)
                    _assumptions = new List<string>();
                _assumptions.Add(text);
                Dirty.IsDirty = true;
                result = true;
                AssumptionAdded?.Invoke(text);
            }

            return result;
        }

        public bool RemoveAssumption([Required] string text)
        {
            bool result = false;

            if (_assumptions?.Any(text.IsEqual) ?? false)
            {
                result = _assumptions.Remove(text);
                if (result)
                {
                    Dirty.IsDirty = true;
                    AssumptionRemoved?.Invoke(text);
                }
            }

            return result;
        }

        public bool ChangeAssumption([Required] string oldText, [Required] string newText)
        {
            bool result = false;

            int index = _assumptions?.IndexOf(oldText) ?? -1;
            if (index >= 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                _assumptions[index] = newText;
                Dirty.IsDirty = true;
                result = true;
                AssumptionChanged?.Invoke(oldText, newText);
            }

            return result;
        }

        [JsonProperty("_dependencies")]
        private List<string> _dependencies;

        public IEnumerable<string> ExternalDependencies => _dependencies?.AsReadOnly();

        public bool AddDependency([Required] string text)
        {
            bool result = false;

            if (!(_dependencies?.Any(text.IsEqual) ?? false))
            {
                if (_dependencies == null)
                    _dependencies = new List<string>();
                _dependencies.Add(text);
                Dirty.IsDirty = true;
                result = true;
                DependencyAdded?.Invoke(text);
            }

            return result;
        }

        public bool RemoveDependency([Required] string text)
        {
            bool result = false;

            if (_dependencies?.Any(text.IsEqual) ?? false)
            {
                result = _dependencies.Remove(text);
                if (result)
                {
                    Dirty.IsDirty = true;
                    DependencyRemoved?.Invoke(text);
                }
            }

            return result;
        }

        public bool ChangeDependency([Required] string oldText, [Required] string newText)
        {
            bool result = false;

            int index = _dependencies?.IndexOf(oldText) ?? -1;
            if (index >= 0)
            {
                // ReSharper disable once PossibleNullReferenceException
                _dependencies[index] = newText;
                Dirty.IsDirty = true;
                result = true;
                DependencyChanged?.Invoke(oldText, newText);
            }

            return result;
        }

        public string GetIdentityTypeName([NotNull] IIdentity identity)
        {
            return GetIdentityTypeName(identity.GetType());
        }

        private string GetIdentityTypeName(Type type)
        {
            TypeLabelAttribute[] attribs = type.GetCustomAttributes(
                typeof(TypeLabelAttribute), false) as TypeLabelAttribute[];

            return attribs?.Length > 0 ? attribs[0].Label : type.Name;
        }

        public string GetIdentityTypeInitial([NotNull] IIdentity identity)
        {
            return GetIdentityTypeInitial(identity.GetType());
        }

        private string GetIdentityTypeInitial(Type type)
        {
            TypeInitialAttribute[] attribs = type.GetCustomAttributes(
                typeof(TypeInitialAttribute), false) as TypeInitialAttribute[];

            return attribs?.Length > 0 ? attribs[0].Initial : null;
        }

        public IIdentity GetIdentity(Guid id)
        {
            IIdentity result = null;

            if (Id == id)
                result = this;
            if (result == null)
                result = GetEntity(id);
            if (result == null)
                result = GetDataFlow(id);
            if (result == null)
                result = GetGroup(id);
            if (result == null)
                result = GetDiagram(id);
            if (result == null)
                result = GetSchema(id);
            if (result == null)
                result = GetThreatType(id);
            if (result == null)
                result = GetMitigation(id);
            if (result == null)
                result = _entities?.FirstOrDefault(x => x.ThreatEvents?.Any(y => y.Id == id) ?? false)?
                    .ThreatEvents?.FirstOrDefault(x => x.Id == id);
            if (result == null)
                result = _dataFlows?.FirstOrDefault(x => x.ThreatEvents?.Any(y => y.Id == id) ?? false)?
                    .ThreatEvents?.FirstOrDefault(x => x.Id == id);

            return result ?? GetThreatActor(id);
        }

        public int AssignedThreatTypes => CountThreatTypes();
        public int FullyMitigatedThreatTypes => _threatTypes?
            .Where(x => (x.Mitigations?.Sum(y => y.StrengthId) ?? 0) >= 100).Count() ?? 0;
        
        public int PartiallyMitigatedThreatTypes
        {
            get
            {
                var result = 0;

                var threatTypes = _threatTypes?.ToArray();
                if (threatTypes?.Any() ?? false)
                {
                    foreach (var threatType in threatTypes)
                    {
                        var totalStrength = threatType.Mitigations?.Sum(x => x.StrengthId) ?? 0;
                        if (totalStrength > 0 && totalStrength < 100)
                            result++;
                    }
                }

                return result;
            }
        }

        public int NotMitigatedThreatTypes => _threatTypes?
                                                  .Where(x => (x.Mitigations?.Sum(y => y.StrengthId) ?? 0) == 0).Count() ?? 0;

        public int TotalThreatEvents => CountThreatEvents();

        public int UniqueMitigations => GetUniqueMitigations()?.Count() ?? 0;

        public int FullyMitigatedThreatEvents => GetThreatEvents()?
            .Where(x => (x.Mitigations?.Sum(y => y.StrengthId) ?? 0) >= 100).Count() ?? 0;

        public int PartiallyMitigatedThreatEvents
        {
            get
            {
                var result = 0;

                var threatEvents = GetThreatEvents()?.ToArray();
                if (threatEvents?.Any() ?? false)
                {
                    foreach (var threatEvent in threatEvents)
                    {
                        var totalStrength = threatEvent.Mitigations?.Sum(x => x.StrengthId) ?? 0;
                        if (totalStrength > 0 && totalStrength < 100)
                            result++;
                    }
                }

                return result;
            }
        }

        public int NotMitigatedThreatEvents => GetThreatEvents()?
            .Where(x => (x.Mitigations?.Sum(y => y.StrengthId) ?? 0) == 0).Count() ?? 0;

        public int CountThreatEvents([NotNull] ISeverity severity)
        {
            return CountThreatEvents(severity.Id);
        }

        public int CountThreatEvents([Positive] int severityId)
        {
            return (_threatEvents?
                        .Where(x => x.Severity != null && x.SeverityId == severityId).Count() ?? 0) +
                   (_entities?.Sum(x => x.ThreatEvents?
                                            .Where(y => y.Severity != null && y.SeverityId == severityId)
                                            .Count() ?? 0) ?? 0) +
                   (_dataFlows?.Sum(x => x.ThreatEvents?
                                             .Where(y => y.Severity != null && y.SeverityId == severityId)
                                             .Count() ?? 0) ?? 0);
        }

        public int CountThreatEventsByType([NotNull] ISeverity severity)
        {
            return CountThreatEventsByType(severity.Id);
        }

        public int CountThreatEventsByType([Positive] int severityId)
        {
            int result = 0;

            var types = _threatTypes?.ToArray();

            if (types?.Any() ?? false)
            {
                foreach (var type in types)
                {
                    var severity = type.GetTopSeverity();
                    if (severity != null && severity.Id == severityId)
                        result++;
                }
            }

            return result;
        }

        public int CountMitigationsByStatus(MitigationStatus status)
        {
            return (_threatEvents?.Sum(x => x.Mitigations?.Where(y => y.Status == status).Count() ?? 0) ?? 0) +
                   (_entities?.Sum(x => x.ThreatEvents?.Sum(y => y.Mitigations?.Where(z => z.Status == status).Count() ?? 0) ?? 0) ?? 0) +
                   (_dataFlows?.Sum(x => x.ThreatEvents?.Sum(y => y.Mitigations?.Where(z => z.Status == status).Count() ?? 0) ?? 0) ?? 0);
        }

        public IEnumerable<IThreatEvent> GetThreatEvents()
        {
            var threatEvents = new List<IThreatEvent>();

            GetThreatEvents(this, null, threatEvents);
            GetThreatEvents(_entities, null, threatEvents);
            GetThreatEvents(_dataFlows, null, threatEvents);

            return threatEvents;
        }

        public IEnumerable<IThreatEvent> GetThreatEvents(IThreatType threatType)
        {
            var threatEvents = new List<IThreatEvent>();

            GetThreatEvents(this, threatType, threatEvents);
            GetThreatEvents(_entities, threatType, threatEvents);
            GetThreatEvents(_dataFlows, threatType, threatEvents);

            return threatEvents;
        }

        public IEnumerable<IMitigation> GetUniqueMitigations()
        {
            var mitigations = new List<IMitigation>();

            GetMitigations(_threatEvents?.Select(x => x.Mitigations), mitigations);
            GetMitigations(_entities?.Select(x => x.ThreatEvents?.Select(y => y.Mitigations)), mitigations);
            GetMitigations(_dataFlows?.Select(x => x.ThreatEvents?.Select(y => y.Mitigations)), mitigations);

            return mitigations;
        }

        public IEnumerable<IThreatEventMitigation> GetThreatEventMitigations()
        {
            var result = new List<IThreatEventMitigation>();

            GetMitigations(this, null, result);
            GetMitigations(_entities, null, result);
            GetMitigations(_dataFlows, null, result);

            return result;
        }

        public IEnumerable<IThreatEventMitigation> GetThreatEventMitigations([NotNull] IMitigation mitigation)
        {
            var result = new List<IThreatEventMitigation>();

            GetMitigations(this, mitigation, result);
            GetMitigations(_entities, mitigation, result);
            GetMitigations(_dataFlows, mitigation, result);

            return result;
        }

        private int CountThreatTypes()
        {
            List<Guid> threatTypes = new List<Guid>();

            GetThreatTypes(_threatEvents, threatTypes);
            GetThreatTypes(_entities?.Select(x => x.ThreatEvents), threatTypes);
            GetThreatTypes(_dataFlows?.Select(x => x.ThreatEvents), threatTypes);

            return threatTypes.Count();
        }

        private static void GetThreatTypes(
            IEnumerable<IEnumerable<IThreatEvent>> arraysOfEnumerables, List<Guid> threatTypes)
        {
            if (arraysOfEnumerables?.Any() ?? false)
            {
                foreach (var enumerables in arraysOfEnumerables)
                {
                    GetThreatTypes(enumerables, threatTypes);
                }
            }
        }

        private static void GetThreatTypes(IEnumerable<IThreatEvent> enumerables, List<Guid> threatTypes)
        {
            if (enumerables?.Any() ?? false)
            {
                foreach (var enumerable in enumerables)
                {
                    if (!threatTypes.Contains(enumerable.ThreatTypeId))
                        threatTypes.Add(enumerable.ThreatTypeId);
                }
            }
        }

        private int CountThreatEvents()
        {
            return (_threatEvents?.Count() ?? 0) +
                   (_entities?.Sum(x => x.ThreatEvents?.Count() ?? 0) ?? 0) +
                   (_dataFlows?.Sum(x => x.ThreatEvents?.Count() ?? 0) ?? 0);
        }
        
        private void GetThreatEvents(IEnumerable<IThreatEventsContainer> containers, 
            IThreatType reference,
            [NotNull] List<IThreatEvent> list)
        {
            var cs = containers?.ToArray();
            if (cs?.Any() ?? false)
            {
                foreach (var container in cs)
                {
                    GetThreatEvents(container, reference, list);
                }
            }
        }

        private void GetThreatEvents(IThreatEventsContainer container, 
            IThreatType reference,
            [NotNull] List<IThreatEvent> list)
        {
            var threats = container?.ThreatEvents?
                .Where(x => (reference == null) || (x.ThreatTypeId == reference.Id)).ToArray();
            if (threats?.Any() ?? false)
            {
                list.AddRange(threats);
            }
        }

        private static void GetMitigations(
            IEnumerable<IEnumerable<IEnumerable<IThreatEventMitigation>>> arrayOfArraysOfEnumerables, 
            [NotNull] List<IMitigation> mitigations)
        {
            if (arrayOfArraysOfEnumerables?.Any() ?? false)
            {
                foreach (var arrayOfEnumerables in arrayOfArraysOfEnumerables)
                {
                    GetMitigations(arrayOfEnumerables, mitigations);
                }
            }
        }

        private static void GetMitigations(IEnumerable<IEnumerable<IThreatEventMitigation>> arrayOfEnumerables, 
            [NotNull] List<IMitigation> mitigations)
        {
            if (arrayOfEnumerables?.Any() ?? false)
            {
                foreach (var enumerable in arrayOfEnumerables)
                {
                    if (enumerable?.Any() ?? false)
                    {
                        foreach (var mitigation in enumerable)
                        {
                            if (!mitigations.Contains(mitigation.Mitigation))
                                mitigations.Add(mitigation.Mitigation);
                        }
                    }
                }
            }
        }
        
        private void GetMitigations(IEnumerable<IThreatEventsContainer> containers, IMitigation reference,
            [NotNull] List<IThreatEventMitigation> list)
        {
            var cs = containers?.ToArray();
            if (cs?.Any() ?? false)
            {
                foreach (var container in cs)
                {
                    GetMitigations(container, reference, list);
                }
            }
        }

        private void GetMitigations(IThreatEventsContainer container, IMitigation reference,
            [NotNull] List<IThreatEventMitigation> list)
        {
            var threats = container?.ThreatEvents?.ToArray();
            if (threats?.Any() ?? false)
            {
                foreach (var threat in threats)
                {
                    var mitigations = threat.Mitigations?
                        .Where(x => (reference == null) || x.MitigationId == reference.Id).ToArray();
                    if (mitigations?.Any() ?? false)
                        list.AddRange(mitigations);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion

        #region Implementation of IPropertyFinder.
        public IProperty FindProperty(Guid id)
        {
            IProperty result = ThreatTypes?.Select(x => x.Properties)
                                   .FirstOrDefault(x => x?.Any(y => y.Id == id) ?? false)?
                                   .FirstOrDefault(x => x.Id == id) ?? Properties.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                result = DataFlows?.Select(x => x.Properties)
                    .FirstOrDefault(x => x?.Any(y => y.Id == id) ?? false)?
                    .FirstOrDefault(x => x.Id == id);
            }
            
            if (result == null)
            {
                result = Entities?.Select(x => x.Properties)
                    .FirstOrDefault(x => x?.Any(y => y.Id == id) ?? false)?
                    .FirstOrDefault(x => x.Id == id);
            }

            if (result == null)
            {
                result = Groups?.Select(x => x.Properties)
                    .FirstOrDefault(x => x?.Any(y => y.Id == id) ?? false)?
                    .FirstOrDefault(x => x.Id == id);
            }
            
            if (result == null)
            {
                result = Mitigations?.Select(x => x.Properties)
                    .FirstOrDefault(x => x?.Any(y => y.Id == id) ?? false)?
                    .FirstOrDefault(x => x.Id == id);
            }

            if (result == null)
            {
                result = Severities?.Select(x => x.Properties)
                    .FirstOrDefault(x => x?.Any(y => y.Id == id) ?? false)?
                    .FirstOrDefault(x => x.Id == id);
            }

            return result;
        }
        #endregion

        #region Implementation of Threat Model Duplication.
        public IThreatModel Duplicate([Required] string name, [NotNull] DuplicationDefinition def)
        {
            if (!Validate(def, out var reasons))
                throw new InvalidDuplicationDefinitionException(reasons);

            ThreatModel result;

            try
            {
                Dirty.SuspendDirty();
                result = new ThreatModel(name);
                if (def.Contributors)
                    DuplicateContributors(result);
                if (def.Assumptions)
                    DuplicateAssumptions(result);
                if (def.Dependencies)
                    DuplicateDependencies(result);
                DuplicateSeverities(result, def.AllSeverities ? Severities?.Select(x => x.Id) : def.Severities);
                DuplicateStrengths(result, def.AllStrengths ? Strengths?.Select(x => x.Id) : def.Strengths);
                DuplicatePropertySchemas(result,
                    def.AllPropertySchemas ? Schemas?.Select(x => x.Id) : def.PropertySchemas);
                DuplicateProperties(result, def.AllProperties ? Properties?.Select(x => x.Id) : def.Properties);
                DuplicateThreatActors(result, def.AllThreatActors ? ThreatActors?.Select(x => x.Id) : def.ThreatActors);
                DuplicateMitigations(result, def.AllMitigations ? Mitigations?.Select(x => x.Id) : def.Mitigations);
                DuplicateThreatTypes(result, def.AllThreatTypes ? ThreatTypes?.Select(x => x.Id) : def.ThreatTypes);
                DuplicateGroups(result, def.AllGroups ? Groups?.Select(x => x.Id) : def.Groups);
                DuplicateEntityTemplates(result, def.AllEntityTemplates ? EntityTemplates?.Select(x => x.Id) : def.EntityTemplates);
                DuplicateEntities(result, def.AllEntities ? Entities?.Select(x => x.Id) : def.Entities);
                DuplicateDataFlows(result, def.AllDataFlows ? DataFlows?.Select(x => x.Id) : def.DataFlows);
                DuplicateDiagrams(result, def.AllDiagrams ? Diagrams?.Select(x => x.Id) : def.Diagrams);
            }
            finally
            {
                Dirty.ResumeDirty();
            }

            return result;
        }

        private void DuplicateContributors([NotNull] ThreatModel dest)
        {
            var list = Contributors?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    dest.AddContributor(item);
                }
            }
        }

        private void DuplicateAssumptions([NotNull] ThreatModel dest)
        {
            var list = Assumptions?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    dest.AddAssumption(item);
                }
            }
        }

        private void DuplicateDependencies([NotNull] ThreatModel dest)
        {
            var list = ExternalDependencies?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    dest.AddDependency(item);
                }
            }
        }

        private void DuplicateSeverities([NotNull] ThreatModel dest, IEnumerable<int> list)
        {
            var severities = Severities?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (severities?.Any() ?? false)
            {
                foreach (var severity in severities)
                {
                   severity.Clone(dest);
                }
            }
        }

        private void DuplicateStrengths([NotNull] ThreatModel dest, IEnumerable<int> list)
        {
            var strengths = Strengths?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (strengths?.Any() ?? false)
            {
                foreach (var strength in strengths)
                {
                    strength.Clone(dest);
                }
            }
        }

        private void DuplicatePropertySchemas([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var schemas = Schemas?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (schemas?.Any() ?? false)
            {
                foreach (var schema in schemas)
                {
                    schema.Clone(dest);
                }
            }
        }

        private void DuplicateProperties([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var properties = Properties?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (properties?.Any() ?? false)
            {
                foreach (var property in properties)
                {
                    var propertyType = dest.GetPropertyType(property.PropertyTypeId);
                    if (propertyType != null)
                        dest.AddProperty(propertyType, property.StringValue);
                }
            }
        }

        private void DuplicateThreatActors([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var actors = ThreatActors?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (actors?.Any() ?? false)
            {
                foreach (var actor in actors)
                {
                    actor.Clone(dest);
                }
            }
        }

        private void DuplicateMitigations([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var mitigations = Mitigations?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (mitigations?.Any() ?? false)
            {
                foreach (var mitigation in mitigations)
                {
                    mitigation.Clone(dest);
                }
            }
        }

        private void DuplicateThreatTypes([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var threatTypes = ThreatTypes?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (threatTypes?.Any() ?? false)
            {
                foreach (var threatType in threatTypes)
                {
                    threatType.Clone(dest);
                }
            }
        }

        private void DuplicateGroups([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var groups = Groups?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (groups?.Any() ?? false)
            {
                foreach (var group in groups)
                {
                    group.Clone(dest);
                }
            }
        }

        private void DuplicateEntityTemplates([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var templates = EntityTemplates?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (templates?.Any() ?? false)
            {
                foreach (var template in templates)
                {
                    template.Clone(dest);
                }
            }
        }

        private void DuplicateEntities([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var entities = Entities?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (entities?.Any() ?? false)
            {
                foreach (var entity in entities)
                {
                    entity.Clone(dest);
                }
            }
        }

        private void DuplicateDataFlows([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var dataFlows = DataFlows?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (dataFlows?.Any() ?? false)
            {
                foreach (var dataFlow in dataFlows)
                {
                    dataFlow.Clone(dest);
                }
            }
        }

        private void DuplicateDiagrams([NotNull] ThreatModel dest, IEnumerable<Guid> list)
        {
            var diagrams = Diagrams?.Where(x => list?.Contains(x.Id) ?? false).ToArray();
            if (diagrams?.Any() ?? false)
            {
                foreach (var diagram in diagrams)
                {
                    diagram.Clone(dest);
                }
            }
        }

        private bool Validate([NotNull] DuplicationDefinition def, out IEnumerable<string> reasons)
        {
            bool result = true;

            List<string> r = new List<string>();

            List<Guid> known = new List<Guid>();
            var knownSeverities = _severities?
                .Where(x => def.AllSeverities || (def.Severities?.Contains(x.Id) ?? false))
                .Select(x => x.Id).ToArray();
            var knownStrengths = _strengths?
                .Where(x => def.AllStrengths || (def.Strengths?.Contains(x.Id) ?? false))
                .Select(x => x.Id).ToArray();

            AddIdentities(known, def.AllPropertySchemas, def.PropertySchemas, _schemas);

            if (!Check(known, def.AllProperties, def.Properties, _properties))
            {
                result = false;
                r.Add("One or more Threat Model Properties are associated to a Property Type which has not been selected.");
            }

            if (!Check(known, knownSeverities, knownStrengths, def.AllThreatActors, def.ThreatActors, _actors))
            {
                result = false;
                r.Add("One or more Threat Actors are associated to a Property Type which has not been selected.");
            }
            AddIdentities(known, def.AllThreatActors, def.ThreatActors, _actors);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllMitigations, def.Mitigations, _mitigations))
            {
                result = false;
                r.Add("One or more Mitigations are associated to a Property Type which has not been selected.");
            }
            AddIdentities(known, def.AllMitigations, def.Mitigations, _mitigations);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllThreatTypes, def.ThreatTypes, _threatTypes))
            {
                result = false;
                r.Add("One or more Threat Types are associated to an object which has not been selected.");
            }
            AddIdentities(known, def.AllThreatTypes, def.ThreatTypes, _threatTypes);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllGroups, def.Groups, _groups))
            {
                result = false;
                r.Add("One or more Groups are associated to an object which has not been selected.");
            }
            AddIdentities(known, def.AllGroups, def.Groups, _groups);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllEntityTemplates, def.EntityTemplates, _entityTemplates))
            {
                result = false;
                r.Add("One or more Entity Templates are associated to an object which has not been selected.");
            }
            AddIdentities(known, def.AllEntityTemplates, def.EntityTemplates, _entityTemplates);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllEntities, def.Entities, _entities))
            {
                result = false;
                r.Add("One or more Entities are associated to an object which has not been selected.");
            }
            AddIdentities(known, def.AllEntities, def.Entities, _entities);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllDataFlows, def.DataFlows, _dataFlows))
            {
                result = false;
                r.Add("One or more Flows are associated to an object which has not been selected.");
            }
            AddIdentities(known, def.AllDataFlows, def.DataFlows, _dataFlows);              

            if (!Check(known, knownSeverities, knownStrengths, def.AllDiagrams, def.Diagrams, _diagrams))
            {
                result = false;
                r.Add("One or more Diagrams are associated to an object which has not been selected.");
            }

            reasons = r;

            return result;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private bool Check([NotNull] List<Guid> known, IEnumerable<int> knownSeverities, 
            IEnumerable<int> knownStrengths, bool all, IEnumerable<Guid> selected, 
            IEnumerable<IIdentity> identities)
        {
            bool result = true;

            var list = identities?.Where(x => all || (selected?.Contains(x.Id) ?? false)).ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (item is IPropertiesContainer container && !Check(known, true, null, container.Properties))
                    {
                        result = false;
                        break;
                    }

                    if (item is IEntitiesContainer eContainer && 
                        !Check(known, knownSeverities, eContainer.Entities))
                    {
                        result = false;
                        break;
                    }

                    if (item is IDataFlowsContainer dfContainer && 
                        !Check(known, knownSeverities, dfContainer.DataFlows))
                    {
                        result = false;
                        break;
                    }

                    if (item is IThreatType threatType && threatType.Severity is ISeverity severity &&
                        !(knownSeverities?.Contains(severity.Id) ?? false))
                    {
                        result = false;
                        break;
                    }

                    if (item is IThreatTypeMitigationsContainer ttmContainer &&
                        !Check(known, knownStrengths, ttmContainer.Mitigations))
                    {
                        result = false;
                        break;
                    }

                    if (item is IThreatEventsContainer teContainer &&
                        !Check(known, knownStrengths, teContainer.ThreatEvents))
                    {
                        result = false;
                        break;
                    }

                    if (item is IThreatEventMitigationsContainer tteContainer &&
                        !Check(known, knownStrengths, tteContainer.Mitigations))
                    {
                        result = false;
                        break;
                    }

                    if (item is IEntityShapesContainer esContainer && !Check(known, esContainer.Entities))
                    {
                        result = false;
                        break;
                    }

                    if (item is IGroupShapesContainer gsContainer && !Check(known, gsContainer.Groups))
                    {
                        result = false;
                        break;
                    }

                    if (item is ILinksContainer lContainer && !Check(known, lContainer.Links))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
        
        private bool Check([NotNull] List<Guid> known, IEnumerable<int> knownStrengths, 
            IEnumerable<IThreatTypeMitigation> mitigations)
        {
            bool result = true;

            var list = mitigations?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.MitigationId) || 
                        (item.Strength is IStrength strength &&
                          !(knownStrengths?.Contains(strength.Id) ?? false)))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, IEnumerable<int> knownSeverities, 
            IEnumerable<IEntity> entities)
        {
            bool result = true;

            var list = entities?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.ParentId) || !Check(known, knownSeverities, item.ThreatEvents))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
        
        private bool Check([NotNull] List<Guid> known, IEnumerable<int> knownSeverities, 
            IEnumerable<IDataFlow> dataFlows)
        {
            bool result = true;

            var list = dataFlows?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.SourceId) || !known.Contains(item.TargetId) || 
                        !Check(known, knownSeverities, item.ThreatEvents))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, IEnumerable<int> knownSeverities, 
            IEnumerable<IThreatEvent> threatEvents)
        {
            bool result = true;

            var list = threatEvents?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.ThreatTypeId) || 
                        (item.Severity is ISeverity severity && 
                          !(knownSeverities?.Contains(severity.Id) ?? false)))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, IEnumerable<int> knownStrengths,
            IEnumerable<IThreatEventMitigation> mitigations)
        {
            bool result = true;

            var list = mitigations?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.MitigationId) ||
                          !(knownStrengths?.Contains(item.StrengthId) ?? false))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, IEnumerable<IEntityShape> container)
        {
            bool result = true;

            var list = container?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.AssociatedId))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, IEnumerable<IGroupShape> container)
        {
            bool result = true;

            var list = container?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.AssociatedId))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, IEnumerable<ILink> container)
        {
            bool result = true;

            var list = container?.ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.AssociatedId))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private bool Check([NotNull] List<Guid> known, bool all, IEnumerable<Guid> selected, IEnumerable<IProperty> properties)
        {
            bool result = true;

            var list = properties?.Where(x => (all || (selected?.Contains(x.Id) ?? false)) && this.GetPropertyType(x.PropertyTypeId) != null).ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var property in list)
                {
                    if (!known.Contains(property.PropertyTypeId))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private void AddIdentities([NotNull] List<Guid> known, 
            bool all, IEnumerable<Guid> selected, IEnumerable<IIdentity> identities)
        {
            var list = identities?.Where(x => all || (selected?.Contains(x.Id) ?? false)).ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var item in list)
                {
                    if (!known.Contains(item.Id))
                    {
                        known.Add(item.Id);
                        if (item is IPropertySchema schema)
                        {
                            AddIdentities(known, true, null, schema.PropertyTypes);
                        }
                    }
                }
            }
        }
        #endregion

        #region Implementation of Threat Model Merge.
        public bool Merge([NotNull] IThreatModel source, [NotNull] DuplicationDefinition def)
        {
            bool result = false;

            if (source is ThreatModel sourceThreatModel)
            {
                if (sourceThreatModel.Validate(def, out var reasons))
                {
                    result = true;
                    MergeSchemas(source, def.AllPropertySchemas, def.PropertySchemas);
                    MergeEntityTemplates(source, def.AllEntityTemplates, def.EntityTemplates);
                    MergeSeverities(source, def.AllSeverities, def.Severities);
                    MergeStrengths(source, def.AllSeverities, def.Severities);
                    MergeThreatActors(source, def.AllThreatActors, def.ThreatActors);
                    MergeMitigations(source, def.AllMitigations, def.Mitigations);
                    MergeThreatTypes(source, def.AllThreatTypes, def.ThreatTypes);
                }
            }

            return result;
        }

        private void MergeSchemas([NotNull] IThreatModel source, bool all, IEnumerable<Guid> ids)
        {
            var selected = source.Schemas?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var schema in selected)
                {
                    MergeSchema(schema);
                }
            }
        }

        private void MergeSchema([NotNull] IPropertySchema schema)
        {
            var existing = GetSchema(schema.Name, schema.Namespace);
            if (existing == null)
            {
                schema.Clone(this);
            }
            else
            {
                existing.MergePropertyTypes(schema);
            }

            if (schema.AutoApply)
            {
                ApplySchema(schema.Id);
            }
        }

        private void MergeEntityTemplates([NotNull] IThreatModel source, bool all, IEnumerable<Guid> ids)
        {
            var selected = source.EntityTemplates?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var entityTemplate in selected)
                {
                    MergeEntityTemplate(entityTemplate);
                }
            }
        }

        private void MergeEntityTemplate([NotNull] IEntityTemplate entityTemplate)
        {
            var existing = EntityTemplates?.FirstOrDefault(x => string.CompareOrdinal(x.Name, entityTemplate.Name) == 0);
            if (existing == null)
            {
                entityTemplate.Clone(this);
            }
            else
            {
                existing.MergeProperties(entityTemplate);
            }
        }

        private void MergeSeverities([NotNull] IThreatModel source, bool all, IEnumerable<int> ids)
        {
            var selected = source.Severities?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var severity in selected)
                {
                    MergeSeverity(severity);
                }
            }
        }

        private void MergeSeverity([NotNull] ISeverity severity)
        {
            var existing = Severities?.FirstOrDefault(x => x.Id == severity.Id);
            if (existing == null)
            {
                severity.Clone(this);
            }
            else
            {
                existing.MergeProperties(severity);
            }
        }

        private void MergeStrengths([NotNull] IThreatModel source, bool all, IEnumerable<int> ids)
        {
            var selected = source.Strengths?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var strength in selected)
                {
                    MergeStrength(strength);
                }
            }
        }

        private void MergeStrength([NotNull] IStrength strength)
        {
            var existing = Strengths?.FirstOrDefault(x => x.Id == strength.Id);
            if (existing == null)
            {
                strength.Clone(this);
            }
            else
            {
                existing.MergeProperties(strength);
            }
        }

        private void MergeThreatActors([NotNull] IThreatModel source, bool all, IEnumerable<Guid> ids)
        {
            var selected = source.ThreatActors?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var threatActor in selected)
                {
                    MergeThreatActor(threatActor);
                }
            }
        }

        private void MergeThreatActor([NotNull] IThreatActor threatActor)
        {
            var existing = ThreatActors?.FirstOrDefault(x => string.CompareOrdinal(x.Name, threatActor.Name) == 0);
            if (existing == null)
            {
                threatActor.Clone(this);
            }
            else
            {
                existing.MergeProperties(threatActor);
            }
        }

        private void MergeMitigations([NotNull] IThreatModel source, bool all, IEnumerable<Guid> ids)
        {
            var selected = source.Mitigations?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var mitigation in selected)
                {
                    MergeMitigation(mitigation);
                }
            }
        }

        private void MergeMitigation([NotNull] IMitigation mitigation)
        {
            var existing = Mitigations?.FirstOrDefault(x => string.CompareOrdinal(x.Name, mitigation.Name) == 0);
            if (existing == null)
            {
                mitigation.Clone(this);
            }
            else
            {
                existing.MergeProperties(mitigation);
            }
        }

        private void MergeThreatTypes([NotNull] IThreatModel source, bool all, IEnumerable<Guid> ids)
        {
            var selected = source.ThreatTypes?.Where(x => all || (ids?.Contains(x.Id) ?? false)).ToArray();
            if (selected?.Any() ?? false)
            {
                foreach (var threatType in selected)
                {
                    MergeThreatType(threatType);
                }
            }
        }

        private void MergeThreatType([NotNull] IThreatType threatType)
        {
            var existing = ThreatTypes?.FirstOrDefault(x => string.CompareOrdinal(x.Name, threatType.Name) == 0);
            if (existing == null)
            {
                threatType.Clone(this);
            }
            else
            {
                existing.MergeProperties(threatType);

                var mitigations = threatType.Mitigations?.ToArray();
                if (mitigations?.Any() ?? false)
                {
                    foreach (var mitigation in mitigations)
                    {
                        var m = GetMitigation(mitigation.MitigationId) ??
                                Mitigations?.FirstOrDefault(x =>
                                    string.CompareOrdinal(x.Name, mitigation.Mitigation.Name) == 0);
                        var s = GetStrength(mitigation.StrengthId);
                        if (m != null && s != null)
                            existing.AddMitigation(m, s);
                    }
                }
            }
        }
        #endregion

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

        public void Add(IThreatEvent threatEvent)
        {
        }

        public IThreatEvent AddThreatEvent(IThreatType threatType)
        {
            return null;
        }

        public bool RemoveThreatEvent(Guid id)
        {
            return false;
        }
        #endregion

        #region Additional placeholders required.
        protected Guid _id { get; set; }
        private IThreatModel Model => this;
        private IPropertiesContainer PropertiesContainer => this;
        private List<IProperty> _properties { get; set; }
        private IThreatEventsContainer ThreatEventsContainer => this;
        private List<IThreatEvent> _threatEvents { get; set; }
        #endregion

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}
