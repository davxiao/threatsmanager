﻿using System;

namespace ThreatsManager.Interfaces
{
    /// <summary>
    /// Enumeration of the potential scopes for the Context Aware action.
    /// </summary>
    [Flags]
    public enum Scope
    {
        /// <summary>
        /// Undefined scope.
        /// </summary>
        [UiHidden]
        Undefined = 0,
        /// <summary>
        /// External Interactor.
        /// </summary>
        [EnumLabel("External Interactor")]
        ExternalInteractor = 1,
        /// <summary>
        /// Process.
        /// </summary>
        Process = 2,
        /// <summary>
        /// Data Store.
        /// </summary>
        [EnumLabel("Data Store")]
        DataStore = 4,
        /// <summary>
        /// Scope is any Entity (External Interactor, Process or Data Store).
        /// </summary>
        [UiHidden]
        Entity = ExternalInteractor | Process | DataStore,
        /// <summary>
        /// Entity Template.
        /// </summary>
        [EnumLabel("Entity Template")]
        EntityTemplate = 8,
        /// <summary>
        /// Data Flow.
        /// </summary>
        [EnumLabel("Flow")]
        DataFlow = 16,
        /// <summary>
        /// Trust Boundary.
        /// </summary>
        [EnumLabel("Trust Boundary")]
        TrustBoundary = 32,
        /// <summary>
        /// Logical Group.
        /// </summary>
        /// <remarks>Not implemented so far.</remarks>
        [UiHidden]
        [EnumLabel("Logical Group")]
        LogicalGroup = 64,
        /// <summary>
        /// Any group, including Trust Boundaries.
        /// </summary>
        [UiHidden]
        Group = LogicalGroup | TrustBoundary,
        /// <summary>
        /// Threat Type.
        /// </summary>
        [EnumLabel("Threat Type")]
        ThreatType = 128,
        /// <summary>
        /// Threat Event.
        /// </summary>
        [EnumLabel("Threat Event")]
        ThreatEvent = 256,
        /// <summary>
        /// Threat Event Scenario.
        /// </summary>
        [EnumLabel("Threat Event Scenario")]
        ThreatEventScenario = 512,
        /// <summary>
        /// Threat Event Mitigation.
        /// </summary>
        [EnumLabel("Threat Event Mitigation")]
        [UiHidden]
        ThreatEventMitigation = 1024,
        /// <summary>
        /// Everything related to Threats.
        /// </summary>
        [UiHidden]
        Threats = ThreatType | ThreatEvent | ThreatEventScenario,
        /// <summary>
        /// Standard Mitigation.
        /// </summary>
        [EnumLabel("Standard Mitigation")]
        Mitigation = 2048,
        /// <summary>
        /// Threat Type Mitigation.
        /// </summary>
        [EnumLabel("Threat Type Mitigation")]
        [UiHidden]
        ThreatTypeMitigation = 4096,
        /// <summary>
        /// Threat Actor.
        /// </summary>
        [EnumLabel("Threat Actor")]
        ThreatActor = 8192,
        /// <summary>
        /// Severity.
        /// </summary>
        [UiHidden]
        Severity = 16384,
        /// <summary>
        /// Property Type.
        /// </summary>
        [UiHidden]
        PropertyType = 32768,
        /// <summary>
        /// Property Schema.
        /// </summary>
        [UiHidden]
        PropertySchema = 65536,
        /// <summary>
        /// Standard Diagram.
        /// </summary>
        Diagram = 262144,
        /// <summary>
        /// Entity Shape.
        /// </summary>
        [EnumLabel("Entity Shape")]
        EntityShape = 524288,
        /// <summary>
        /// Group Shape.
        /// </summary>
        [EnumLabel("Group Shape")]
        GroupShape = 1048576,
        /// <summary>
        /// Link.
        /// </summary>
        Link = 2097152,
        /// <summary>
        /// Threat Model.
        /// </summary>
        [EnumLabel("Threat Model")]
        ThreatModel = 16777216,
        /// <summary>
        /// Everything.
        /// </summary>
        [UiHidden]
        All = Entity | EntityTemplate | DataFlow | Group | Threats | Mitigation | ThreatActor | 
              Severity | PropertyType | PropertySchema | Diagram | 
              EntityShape | GroupShape | Link | ThreatModel
    }
}