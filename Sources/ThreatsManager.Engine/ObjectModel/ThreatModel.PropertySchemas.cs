﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using PostSharp.Patterns.Contracts;
using ThreatsManager.Engine.ObjectModel.Properties;
using ThreatsManager.Interfaces;
using ThreatsManager.Interfaces.ObjectModel.Diagrams;
using ThreatsManager.Interfaces.ObjectModel.Entities;
using ThreatsManager.Interfaces.ObjectModel.Properties;
using ThreatsManager.Interfaces.ObjectModel.ThreatsMitigations;
using ThreatsManager.Utilities;
using ThreatsManager.Utilities.Aspects;

namespace ThreatsManager.Engine.ObjectModel
{
    public partial class ThreatModel
    {
        [JsonProperty("schemas")]
        private List<IPropertySchema> _schemas;

        public IEnumerable<IPropertySchema> Schemas => _schemas?.OrderBy(x => x.Priority);

        [InitializationRequired]
        public IPropertySchema GetSchema([Required] string name, [Required] string nspace)
        {
            return _schemas?.FirstOrDefault(x => name.IsEqual(x.Name) && nspace.IsEqual(x.Namespace));
        }

        [InitializationRequired]
        public IPropertySchema GetSchema(Guid schemaId)
        {
            return _schemas?.FirstOrDefault(x => x.Id == schemaId);
        }

        [InitializationRequired]
        public void ApplySchema(Guid schemaId)
        {
            var schema = GetSchema(schemaId);
            if (schema != null)
            {
                if (schema.AppliesTo.HasFlag(Scope.ExternalInteractor))
                {
                    ApplySchema<IExternalInteractor>(schema);
                }

                if (schema.AppliesTo.HasFlag(Scope.Process))
                {
                    ApplySchema<IProcess>(schema);
                }

                if (schema.AppliesTo.HasFlag(Scope.DataStore))
                {
                    ApplySchema<IDataStore>(schema);
                }
 
                if (schema.AppliesTo.HasFlag(Scope.DataFlow))
                {
                    var list = _dataFlows?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }
 
                if (schema.AppliesTo.HasFlag(Scope.TrustBoundary))
                {
                    var list = _groups?.OfType<ITrustBoundary>().ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }
 
                if (schema.AppliesTo.HasFlag(Scope.ThreatType))
                {
                    var list = _threatTypes?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }
 
                if (schema.AppliesTo.HasFlag(Scope.ThreatEvent))
                {
                    var listEntities = _entities?.Select(x => x.ThreatEvents).ToArray();
                    if (listEntities?.Any() ?? false)
                    {
                        foreach (var currentEntityThreats in listEntities)
                        {
                            if (currentEntityThreats?.Any() ?? false)
                            {
                                foreach (var et in currentEntityThreats)
                                {
                                    Apply(schema, et);
                                }
                            }
                        }
                    }

                    var listFlows = _dataFlows?.Select(x => x.ThreatEvents).ToArray();
                    if (listFlows?.Any() ?? false)
                    {
                        foreach (var currentFlowThreats in listFlows)
                        {
                            if (currentFlowThreats?.Any() ?? false)
                            {
                                foreach (var ft in currentFlowThreats)
                                {
                                    Apply(schema, ft);
                                }
                            }
                        }
                    }
                }
  
                if (schema.AppliesTo.HasFlag(Scope.ThreatEventScenario))
                {
                    var listEntities = _entities?.Select(x => x.ThreatEvents).ToArray();
                    if (listEntities?.Any() ?? false)
                    {
                        foreach (var currentEntityThreats in listEntities)
                        {
                            if (currentEntityThreats?.Any() ?? false)
                            {
                                foreach (var et in currentEntityThreats)
                                {
                                    var ets = et.Scenarios?.ToArray();
                                    if (ets?.Any() ?? false)
                                    {
                                        foreach (var currEts in ets)
                                        {
                                            Apply(schema, currEts);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var listFlows = _dataFlows?.Select(x => x.ThreatEvents).ToArray();
                    if (listFlows?.Any() ?? false)
                    {
                        foreach (var currentFlowThreats in listFlows)
                        {
                            if (currentFlowThreats?.Any() ?? false)
                            {
                                foreach (var ft in currentFlowThreats)
                                {
                                    var fts = ft.Scenarios?.ToArray();
                                    if (fts?.Any() ?? false)
                                    {
                                        foreach (var currFts in fts)
                                        {
                                            Apply(schema, currFts);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
 
                if (schema.AppliesTo.HasFlag(Scope.Mitigation))
                {
                    var list = _mitigations?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }

                if (schema.AppliesTo.HasFlag(Scope.Diagram))
                {
                    var list = _diagrams?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }

                if (schema.AppliesTo.HasFlag(Scope.ThreatModel))
                {
                    Apply(schema, this);
                }

                if (schema.AppliesTo.HasFlag(Scope.ThreatActor))
                {
                    var list = _actors?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }

                if (schema.AppliesTo.HasFlag(Scope.EntityTemplate))
                {
                    var list = _entityTemplates?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }
 
                if (schema.AppliesTo.HasFlag(Scope.LogicalGroup))
                {
                    // TODO: Expand when the concept of Logical Group will be introduced.
                }

                if (schema.AppliesTo.HasFlag(Scope.ThreatTypeMitigation))
                {
                    var list = _threatTypes?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            var mitigations = current.Mitigations?.ToArray();
                            if (mitigations?.Any() ?? false)
                            {
                                foreach (var mitigation in mitigations)
                                    Apply(schema, mitigation);
                            }
                        }
                    }
                }

                if (schema.AppliesTo.HasFlag(Scope.ThreatEventMitigation))
                {
                    var listEntities = _entities?.Select(x => x.ThreatEvents).ToArray();
                    if (listEntities?.Any() ?? false)
                    {
                        foreach (var currentEntityThreats in listEntities)
                        {
                            if (currentEntityThreats?.Any() ?? false)
                            {
                                foreach (var et in currentEntityThreats)
                                {
                                    var etms = et.Mitigations?.ToArray();
                                    if (etms?.Any() ?? false)
                                    {
                                        foreach (var etm in etms)
                                            Apply(schema, etm);
                                    }
                                }
                            }
                        }
                    }

                    var listFlows = _dataFlows?.Select(x => x.ThreatEvents).ToArray();
                    if (listFlows?.Any() ?? false)
                    {
                        foreach (var currentFlowThreats in listFlows)
                        {
                            if (currentFlowThreats?.Any() ?? false)
                            {
                                foreach (var ft in currentFlowThreats)
                                {
                                    var ftms = ft.Mitigations?.ToArray();
                                    if (ftms?.Any() ?? false)
                                    {
                                        foreach (var ftm in ftms)
                                            Apply(schema, ftm);
                                    }
                                }
                            }
                        }
                    }
                }

                if (schema.AppliesTo.HasFlag(Scope.Severity))
                {
                    var list = _severities?.ToArray();
                    if (list?.Any() ?? false)
                    {
                        foreach (var current in list)
                        {
                            Apply(schema, current);
                        }
                    }
                }
            }
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void ApplySchema<T>([NotNull] IPropertySchema schema) where T : IEntity
        {
            var list = _entities?.Where(x => x is T).ToArray();
            if (list?.Any() ?? false)
            {
                foreach (var current in list)
                {
                    Apply(schema, current);
                }
            }

            IEnumerable<IEntityTemplate> templates = null;
            if (typeof(T) == typeof(IExternalInteractor)) 
                templates = _entityTemplates?.Where(x => x.EntityType == EntityType.ExternalInteractor).ToArray();
            else if (typeof(T) == typeof(IProcess)) 
                templates = _entityTemplates?.Where(x => x.EntityType == EntityType.Process).ToArray();
            else if (typeof(T) == typeof(IDataStore)) 
                templates = _entityTemplates?.Where(x => x.EntityType == EntityType.DataStore).ToArray();
            if (templates?.Any() ?? false)
            {
                foreach (var current in templates)
                {
                    Apply(schema, current);
                }
            }
        }

        private void Apply([NotNull] IPropertySchema schema, [NotNull] IPropertiesContainer container)
        {
            var existingProp = container.Properties?.ToArray();
            var schemaProp = schema.PropertyTypes?.ToArray();
            var missing = existingProp == null ? schemaProp : schemaProp?.Except(existingProp.Select(x => x.PropertyType)).ToArray();
            var inExcess = existingProp?.Where(x => x.PropertyType != null &&
                x.PropertyType.SchemaId == schema.Id && !(schemaProp?.Any(y => y.Id == x.PropertyTypeId) ?? false)).Select(x => x.PropertyType).ToArray();

            if (missing?.Any() ?? false)
            {
                foreach (var item in missing)
                {
                    container.AddProperty(item, null);
                }
            }

            if (inExcess?.Any() ?? false)
            {
                foreach (var item in inExcess)
                {
                    container.RemoveProperty(item);
                }
            }
        }

        [InitializationRequired(false)]
        public bool AutoApplySchemas(IPropertiesContainer container)
        {
            bool result = false;
            Scope scope = Scope.Undefined;
            if (container is IExternalInteractor)
                scope = Scope.ExternalInteractor;
            else if (container is IProcess)
                scope = Scope.Process;
            else if (container is IDataStore)
                scope = Scope.DataStore;
            else if (container is IDataFlow)
                scope = Scope.DataFlow;
            else if (container is ITrustBoundary)
                scope = Scope.TrustBoundary;
            else if (container is IThreatType)
                scope = Scope.ThreatType;
            else if (container is IThreatEvent)
                scope = Scope.ThreatEvent;
            else if (container is IThreatEventScenario)
                scope = Scope.ThreatEventScenario;
            else if (container is IMitigation)
                scope = Scope.Mitigation;
            else if (container is IDiagram)
                scope = Scope.Diagram;
            else if (container is IEntityTemplate template)
            {
                switch (template.EntityType)
                {
                    case EntityType.ExternalInteractor:
                        scope = Scope.ExternalInteractor;
                        break;
                    case EntityType.Process:
                        scope = Scope.Process;
                        break;
                    case EntityType.DataStore:
                        scope = Scope.DataStore;
                        break;
                }
            }

            if (scope != Scope.Undefined)
            {
                var schemas = _schemas?.Where(x => x.AutoApply && x.AppliesTo.HasFlag(scope)).OrderBy(x => x.Priority).ToArray();

                if (schemas?.Any() ?? false)
                {
                    foreach (var schema in schemas)
                    {
                        Apply(schema, container);
                    }

                    result = true;
                }
            }

            return result;
        }

        [InitializationRequired]
        public void Add([NotNull] IPropertySchema propertySchema)
        {
            if (_schemas == null)
                _schemas = new List<IPropertySchema>();

            _schemas.Add(propertySchema);
        }

        [InitializationRequired]
        public IPropertySchema AddSchema([Required] string name, [Required] string nspace)
        {
            IPropertySchema result = null;

            if (GetSchema(name, nspace) == null)
            {
                if (_schemas == null)
                    _schemas = new List<IPropertySchema>();
                result = new PropertySchema(name, nspace);
                _schemas.Add(result);
                Dirty.IsDirty = true;
                RegisterEvents(result);
                ChildCreated?.Invoke(result);
            }

            return result;
        }

        [InitializationRequired]
        public bool RemoveSchema([Required] string name, [Required] string nspace, bool force = false)
        {
            bool result = false;

            var schema = GetSchema(name, nspace);
            if (schema != null)
            {
                result = RemoveSchema(schema, force);
            }

            return result;
        }

        [InitializationRequired]
        public bool RemoveSchema(Guid schemaId, bool force = false)
        {
            bool result = false;

            var schema = GetSchema(schemaId);
            if (schema != null)
            {
                result = RemoveSchema(schema, force);
            }

            return result;
        }

        private bool RemoveSchema([NotNull] IPropertySchema schema, bool force)
        {
            bool result = false;

            if (force || !IsUsed(schema))
            {
                RemoveRelated(schema);

                result = _schemas.Remove(schema);
                if (result)
                {
                    UnregisterEvents(schema);
                    Dirty.IsDirty = true;
                    ChildRemoved?.Invoke(schema);
                }
            }

            return result;
        }

        private bool IsUsed([NotNull] IPropertySchema propertySchema)
        {
            return (_entities?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (_entities?.Any(x => x.ThreatEvents?.Any(y => y.Properties?
                        .Any(z => (z.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ?? false) ||
                   (_entities?.Any(x => x.ThreatEvents?.Any(y => y.Scenarios?.Any(z => z.Properties?
                        .Any(t => (t.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ?? false) ?? false) ||
                   (_dataFlows?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (_dataFlows?.Any(x => x.ThreatEvents?.Any(y => y.Properties?
                        .Any(z => (z.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ?? false) ||
                   (_dataFlows?.Any(x => x.ThreatEvents?.Any(y => y.Scenarios?.Any(z => z.Properties?
                        .Any(t => (t.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ?? false) ?? false) ||
                   (_diagrams?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (_groups?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ||
                   (_severities?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (_mitigations?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (_actors?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false) ||
                   (_threatTypes?.Any(x => x.Properties?.Any(y => (y.PropertyType?.SchemaId ?? Guid.Empty) == propertySchema.Id) ?? false) ?? false);
        }

        private void RemoveRelated([NotNull] IPropertySchema propertySchema)
        {
            RemoveRelatedForEntities(propertySchema);
            RemoveRelatedForDataFlows(propertySchema);
            RemoveRelated(propertySchema, _diagrams);
            RemoveRelated(propertySchema, _groups);
            RemoveRelated(propertySchema, this);
            RemoveRelated(propertySchema, _severities);
            RemoveRelatedForMitigations(propertySchema);
            RemoveRelated(propertySchema, _actors);
            RemoveRelated(propertySchema, _threatTypes);
        }

        private void RemoveRelatedForEntities([NotNull] IPropertySchema propertySchema)
        {
            var entities = _entities?.ToArray();
            if (entities?.Any() ?? false)
            {
                foreach (var entity in entities)
                {
                    RemoveRelated(propertySchema, entity);

                    var events = entity.ThreatEvents?.ToArray();
                    if (events?.Any() ?? false)
                    {
                        foreach (var threatEvent in events)
                        {
                            RemoveRelated(propertySchema, threatEvent);

                            var threatEventScenarios = threatEvent.Scenarios?.ToArray();
                            if (threatEventScenarios?.Any() ?? false)
                            {
                                foreach (var scenario in threatEventScenarios)
                                {
                                    RemoveRelated(propertySchema, scenario);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveRelatedForDataFlows([NotNull] IPropertySchema propertySchema)
        {
            var dataFlows = _dataFlows?.ToArray();
            if (dataFlows?.Any() ?? false)
            {
                foreach (var dataFlow in dataFlows)
                {
                    RemoveRelated(propertySchema, dataFlow);

                    var events = dataFlow.ThreatEvents?.ToArray();
                    if (events?.Any() ?? false)
                    {
                        foreach (var threatEvent in events)
                        {
                            RemoveRelated(propertySchema, threatEvent);

                            var threatEventScenarios = threatEvent.Scenarios?.ToArray();
                            if (threatEventScenarios?.Any() ?? false)
                            {
                                foreach (var scenario in threatEventScenarios)
                                {
                                    RemoveRelated(propertySchema, scenario);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveRelatedForMitigations([NotNull] IPropertySchema propertySchema)
        {
            var mitigations = _mitigations?.ToArray();
            if (mitigations?.Any() ?? false)
            {
                foreach (var mitigation in mitigations)
                {
                    RemoveRelated(propertySchema, mitigation);
                }
            }
        }

        private void RemoveRelated([NotNull] IPropertySchema schema, IEnumerable<IPropertiesContainer> containers)
        {
            var array = containers?.ToArray();
            if (array?.Any() ?? false)
            {
                foreach (var item in array)
                {
                    RemoveRelated(schema, item);
                }
            }
        }

        private void RemoveRelated([NotNull] IPropertySchema schema, IPropertiesContainer container)
        {
            if (container != null)
            {
                var properties = container.Properties?.Where(x => (x.PropertyType?.SchemaId ?? Guid.Empty) == schema.Id).ToArray();
                if (properties?.Any() ?? false)
                {
                    foreach (var property in properties)
                    {
                        container.RemoveProperty(property.PropertyType);
                    }
                }
            }
        }

        [InitializationRequired]
        public IPropertyType GetPropertyType(Guid propertyTypeId)
        {
            return _schemas?
                .FirstOrDefault(x => x.PropertyTypes?.Any(y => y.Id == propertyTypeId) ?? false)?.PropertyTypes
                .FirstOrDefault(x => x.Id == propertyTypeId);
        }
    }
}