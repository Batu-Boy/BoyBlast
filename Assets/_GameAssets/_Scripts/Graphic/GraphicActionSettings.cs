using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GraphicActionSettings
{
    [Header("Action Durations")]
    [SerializeField] [Range(0,1)] public float fallDuration;
    [SerializeField] [Range(0,1)] public float instantiateDuration = .3f;
    [SerializeField] [Range(0,1)] public float iconChangeDuration;
    [SerializeField] [Range(0,1)] public float shuffleDuration = .5f;
    [SerializeField] [Range(0,1)] public float bombCreateDuration = .5f;
    [SerializeField] [Range(0,1)] public float bombExplodeDuration = .5f;
    [Space]
    [SerializeField] [Range(0,1)] public float destroyDefaultDuration;
    [SerializeField] [Range(0,1)] public float combo1Duration = .3f;
    [FormerlySerializedAs("destroyCombo2Duration")] [SerializeField] [Range(0,1)] public float combo2Duration = .3f;
    [FormerlySerializedAs("destroyCombo3Duration")] [SerializeField] [Range(0,1)] public float combo3Duration = .3f;

    [Header("BlockGraphic Options")]
    [SerializeField] [Range(0,1)] public float blockFallDuration = .3f;
    [SerializeField] [Range(0,1)] public float instantiatedBlockFallDuration = .35f;
}
